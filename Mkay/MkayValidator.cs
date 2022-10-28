using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Mkay
{
    public class MkayValidator : DataAnnotationsModelValidator<MkayAttribute>
    {
        public MkayValidator(ModelMetadata metadata, ControllerContext context, MkayAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        public override IEnumerable<ModelValidationResult> Validate(object container)
        {
            var context = new ValidationContext(container ?? Metadata.Model, null, null)
                { 
                    DisplayName = Metadata.GetDisplayName(),
                    MemberName = Metadata.PropertyName
                };

            var result = Attribute.GetValidationResult(Metadata.Model, context);
            if (result != ValidationResult.Success)
            {
                yield return new ModelValidationResult
                    {
                        Message = result.ErrorMessage
                    };
            }
        }
    }
}