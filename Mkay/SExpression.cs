using System;

namespace Mkay
{
    public abstract class SExpression
    {
        public abstract string Print();

        public override string ToString()
        {
            return Print();
        }

        public ConsCellImpl Cons(SExpression cdr)
        {
            return new ConsCellImpl(this, cdr);
        }

        public abstract void Accept(IExpVisitor visitor);
    }
}