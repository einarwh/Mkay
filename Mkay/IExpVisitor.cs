namespace Mkay
{
    public interface IExpVisitor
    {
        void VisitEnter(Symbol symbol);

        void VisitOperand<T>(Atom<T> atom);

        void VisitLeave();
    }
}
