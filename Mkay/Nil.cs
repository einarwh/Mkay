using System;

namespace Mkay
{
    public class Nil : ConsCell
    {
        private Nil() { }

        private static readonly Nil Nihil = new Nil();

        public static Nil Instance
        {
            get { return Nihil; }
        }

        public override string Print()
        {
            return "()";
        }

        public override SExpression Car
        {
            get { throw new Exception("Car is undefined on Nil"); }
            set { throw new Exception("Car is undefined on Nil"); }
        }

        public override SExpression Cdr
        {
            get { throw new Exception("Cdr is undefined on Nil."); }
            set { throw new Exception("Cdr is undefined on Nil."); }
        }

		public override void Accept(IExpVisitor visitor) 
		{
			throw new Exception("Nil doesn't accept visitors!");
		}

        public override bool IsNil()
        {
            return true;
        }
    }
}