using System.Collections;
using System.Collections.Generic;

namespace Mkay
{
    public class ConsCellEnumerator : IEnumerator<SExpression>
    {
        private readonly ConsCell _start;
        private ConsCell _current;

        public ConsCellEnumerator(ConsCell cell)
        {
            _start = cell;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            _current = _current == null ? _start : _current.Next;
            return !_current.IsNil();
        }

        public void Reset()
        {
            _current = _start;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public SExpression Current
        {
            get { return _current; }
        }
    }
}