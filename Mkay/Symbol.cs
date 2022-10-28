namespace Mkay
{
    public class Symbol : Atom<string>
    {
        public Symbol(string symbol)
            : base(symbol)
        {
        }

        public override void Accept(IExpVisitor visitor)
        {
            visitor.VisitOperand(this);
        }

        public override string Print()
        {
            return Value;
        }
    }
}