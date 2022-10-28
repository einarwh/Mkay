using System;

namespace Mkay
{
    public class ConsCellImpl : ConsCell
    {
        private SExpression _car;
        private SExpression _cdr;
        private Type _type;

        public ConsCellImpl(ConsCell cell)
        {
            _car = cell.Car;
            _cdr = cell.Cdr;
            _type = typeof(object);
        }

        public ConsCellImpl(SExpression car, SExpression cdr)
        {
            _car = car;
            _cdr = cdr;
        }

        public override SExpression Car
        {
            get { return _car; }
            set { _car = value; }
        }

        public override SExpression Cdr
        {
            get { return _cdr; }
            set { _cdr = value; }
        }

        public override bool IsNil()
        {
            return false;
        }

        public override void Accept(IExpVisitor visitor)
        {
            var symbol = (Symbol)Car;
            visitor.VisitEnter(symbol);

            foreach (ConsCell exp in Next)
            {
                exp.Car.Accept(visitor);
            }

            visitor.VisitLeave();
		}

        public override string Print()
        {
            // TODO: Depends on whether or not the cdr is a cons cell, right? What happens if it ends in something else?
            // TODO: There are three cases: cdr is nil, cdr is a non-nil cons cell, cdr is something else.
            string s = "";
            ConsCell cdrCell = this;
            bool isFirst = true;
            do
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    s += " ";
                }
                s += cdrCell.Car.Print();
                var tempCdr = cdrCell.Cdr;
                if (tempCdr is ConsCell)
                {
                    cdrCell = (ConsCell)tempCdr;
                }
                else
                {
                    s += " . " + tempCdr.Print();
                    break;
                }
            } while (!cdrCell.IsNil());

            return "(" + s + ")";
        }
    }
}