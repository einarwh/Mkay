namespace Mkay
{
    public abstract class Atom<T> : SExpression
    {
        private readonly T _atom;

        protected Atom(T atom)
        {
            _atom = atom;
        }

        public T Value
        {
            get { return _atom; }
        }
    }
}