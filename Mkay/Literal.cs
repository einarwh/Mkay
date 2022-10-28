using System;

namespace Mkay
{
    public class Literal<T> : Atom<T>
    {
        public Literal(T literal)
            : base(literal)
        {
            if (literal is Literal<T>)
            {
                throw new Exception("Nesting literals, man! " + literal);
            }
        }

        public override void Accept(IExpVisitor visitor)
        {
            visitor.VisitOperand(this);
        }

        public override string Print()
        {
            return Value.ToString();
        }
    }
}