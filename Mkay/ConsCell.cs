using System.Collections;
using System.Collections.Generic;

namespace Mkay
{
    public abstract class ConsCell : SExpression, IEnumerable<SExpression>
    {
        public abstract SExpression Car { get; set; }
        public abstract SExpression Cdr { get; set; }

        public ConsCell Next
        {
            get { return (ConsCell)Cdr; }
        }

        public abstract bool IsNil();

        public IEnumerator<SExpression> GetEnumerator()
        {
            return new ConsCellEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}