using System.Collections.Generic;

namespace Mkay
{
    public class ExpVisitor<TNode> : IExpVisitor where TNode : class
    {
        private readonly ITreeBuilder<TNode> _builder;
        private readonly Stack<OpData<TNode>> _stack = new Stack<OpData<TNode>>();
        private OpData<TNode> _current;
        private TNode _result;

        public ExpVisitor(ITreeBuilder<TNode> builder)
        {
            _builder = builder;
        }

        public void VisitEnter(Symbol symbol)
        {
            if (_current != null)
            {
                _stack.Push(_current);
            }

            _current = new OpData<TNode>(symbol.Value);
        }

        public void VisitOperand<T>(Atom<T> atom)
        {
            var exp = _builder.Build(atom);
            _current.AddOperand(exp);
        }

        public void VisitLeave()
        {
            var exp = _builder.Build(_current);

            _current = _stack.Count > 0 ? _stack.Pop() : null;

            if (_current == null)
            {
                _result = exp;
            }
            else
            {
                _current.AddOperand(exp);
            }
        }

        public TNode GetResult()
        {
            if (_result == null)
            {
                throw new MkayException("No result available.");
            }

            return _result;
        }
    }
}
