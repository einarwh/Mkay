using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Mkay
{
    public class ExpParser
    {
        private const char EndChar = ')';

        private const char StartChar = '(';

        private int _index;

        private readonly string _original;

        private readonly string _s;

        private readonly SymbolEnvironment _env;

        public ExpParser(string s, SymbolEnvironment env)
        {
            _env = env;

            if (s == null)
            {
                throw new ArgumentNullException("s");
            }

            if (s.Length == 0)
            {
                throw new ArgumentException("Empty rule.");
            }

            _original = s;

            var t = s.Trim();
            if (t[0] != StartChar)
            {
                t = StartChar + t + EndChar;
            }

            _s = t;
        }

        public ExpParser(string s) : this(s, new SymbolEnvironment())
        {
        }

        public ConsCell Parse()
        {
            var cell = ParseCompoundExpression();
            if (_index < _s.Length)
            {
                throw new ParseException(_original, "Found additional unparsed characters '{0}'.".With(_s.Substring(_index)));
            }

            return cell;
        }

        private ConsCell ParseCompoundExpression()
        {
            ReadStartCharacter();
            
            var exps = ParseListOfExpressions();
            exps.Reverse();

            ConsCell cell = Nil.Instance;
            foreach (var exp in exps)
            {
                cell = exp.Cons(cell);
            }

			// Check the environment for auto-expansion.
			var symbol = (Symbol)cell.Car;
			if (_env.IsSelfExpanding(symbol) && cell.Next.Count() == 1)
            {
				var next = _env.Self.Cons(cell.Next);
				cell = symbol.Cons(next);
            }

            ReadEndCharacter();

            return cell;
        }

        private List<SExpression> ParseListOfExpressions()
        {
            SkipWhiteSpace();

            var list = new List<SExpression>();

            while (true)
            {
                char c = _s[_index];
                
                if (c == EndChar)
                {
                    break;
                }

                var exp = ParseSingleExpression();
                list.Add(exp);
            }

            return list;
        }

        private SExpression ParseSingleExpression()
        {
            SkipWhiteSpace();

            char c = _s[_index];

            if (c == StartChar)
            {
                return ParseCompoundExpression();
            }

            return ParseSimpleExpression();
        }

        private SExpression ParseSimpleExpression()
        {
            SkipWhiteSpace();

            int startIndex = _index;
            bool insideQuotes = false;
            bool seenQuotes = false;
            while (true)
            {
                char c = _s[_index];
                if (c == '\"')
                {
                    if (seenQuotes && !insideQuotes)
                    {
                        var len = _index - startIndex;
                        var s = _s.Substring(startIndex, len);
                        throw new ParseException("Weird string! " + s);
                    }

                    insideQuotes = !insideQuotes;
                    seenQuotes = true;
                }

                if (c == EndChar || (Char.IsWhiteSpace(c) && !insideQuotes))
                {
                    var len = _index - startIndex;
                    var s = _s.Substring(startIndex, len);
                    var atomExp = CreateSuitableAtomExpression(s);
                    return atomExp;
                }

                _index++;

                if (_index > _s.Length - 1)
                {
                    throw new ParseException(_s);
                }
            }
        }

        private SExpression CreateSuitableAtomExpression(string atomText)
        {
            char c = atomText[0];
            if (c <= '9' && (c >= '0' || ((c == '+' || c == '-') && atomText.Length > 1)))
            {
                int ival;
                if (int.TryParse(atomText, out ival))
                {
                    return new Literal<int>(ival);
                }

                double dval;
                if (double.TryParse(atomText, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out dval))
                {
                    return new Literal<double>(dval);
                }
            }

            if (IsIdentifier(atomText))
            {
                return _env.GetSymbol(atomText);
            }

            return new Literal<string>(atomText);
        }

        private static bool IsIdentifier(string atomText)
        {
            // TODO: Yikes.
            return atomText[0] != '"';
        }

        private void SkipWhiteSpace()
        {
            while (Char.IsWhiteSpace(_s[_index]))
            {
                ++_index;
            }            
        }

        private void ReadCharacter(char c)
        {
            SkipWhiteSpace();

            if (_s[_index] != c)
            {
                throw new ParseException(_original, "Expected character '{0}', got '{1}' instead.".With(c, _s[_index]));
            }

            ++_index;
        }

        private void ReadStartCharacter()
        {
            ReadCharacter(StartChar);
        }

        private void ReadEndCharacter()
        {
            ReadCharacter(EndChar);
        }
    }
}