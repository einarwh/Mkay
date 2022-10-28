using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Web.Mvc;

using Newtonsoft.Json.Linq;

namespace Mkay
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MkayAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string _ruleSource;
        private readonly string _defaultErrorMessage;
        private readonly Lazy<ConsCell> _cell;
        private readonly string _guid;

        public MkayAttribute(string ruleSource)
        {
            _guid = Guid.NewGuid().ToString();
            Debug.Print(_guid + " - Ctor: " + ruleSource);

            _ruleSource = ruleSource;
            _defaultErrorMessage = "Respect '{0}', mkay?".With(ruleSource);
            _cell = new Lazy<ConsCell>(() => new ExpParser(ruleSource).Parse());
        }

        protected ConsCell Tree
        {
            get { return _cell.Value; }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(
            ModelMetadata metadata, ControllerContext context)
        {
            var propName = metadata.PropertyName;
            var builder = new JsonBuilder(propName);
            var visitor = new ExpVisitor<JObject>(builder);
            Tree.Accept(visitor);
            var ast = visitor.GetResult();

            var json = new JObject(
                new JProperty("rule", _ruleSource), 
                new JProperty("property", propName),
                new JProperty("ast", ast));

            var rule = new ModelClientValidationRule
                {
                    ErrorMessage = ErrorMessage ?? _defaultErrorMessage, 
                    ValidationType = "mkay"
                };

            rule.ValidationParameters["rule"] = json.ToString();

            yield return rule;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var subject = validationContext.ObjectInstance;
            var property = validationContext.MemberName;
            if (property == null)
            {
                throw new Exception(
                    "Property name is not set for property with display name " + validationContext.DisplayName
                    + ", you should register the MkayValidator with the MkayAttribute in global.asax.");
            }

            Debug.Print(_guid + " - IsValid: " + property);

            var seen = new HashSet<string> { _ruleSource };

            var validator = CreateValidator(subject, property, Tree, seen);

            try
            {
                return validator()
                    ? ValidationResult.Success
                    : new ValidationResult(ErrorMessage ?? _defaultErrorMessage);                
            }
            catch (InfiniteTurtleException ex)
            {
                return new ValidationResult(ex.Message);
            }
        }

        private static Func<bool> CreateValidator(object subject, string property, ConsCell ast, HashSet<string> seen)
        {
            var builder = new ExpressionTreeBuilder(subject, property, seen);
            var viz = new ExpVisitor<Expression>(builder);
            ast.Accept(viz);
            var bodyExp = viz.GetResult();
            var validator = builder.DeriveFunc(bodyExp).Compile();
            return validator;
        }

        public static bool Evalidate(object subject, string property, string ruleSource, HashSet<string> seen)
        {
            Debug.Print("Evalidate: " + property);
            var key = property + ":" + ruleSource;
            if (seen.Contains(key))
            {
                throw new InfiniteTurtleException(property, ruleSource);
            }

            seen.Add(key);

            try
            {
                var ast = new ExpParser(ruleSource).Parse();
                var validator = CreateValidator(subject, property, ast, seen);
                var result = validator();
                return result;
            }
            catch (InfiniteTurtleException ex)
            {
                Debug.Print(ex.Message);
                throw;
            }
            catch (MkayException ex)
            {
                Debug.Print(ex.Message);
                return false;
            }
        }
    }
}