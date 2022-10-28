using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mkay
{
    public class TypeNode : IEnumerable<TypeNode>
    {
        private readonly Type _type;

        private readonly List<TypeNode> _edges = new List<TypeNode>(); 

        public TypeNode(Type type)
        {
            _type = type;
        }

        public Type Type
        {
            get
            {
                return _type;
            }
        }

        public void Add(TypeNode type)
        {
            _edges.Add(type);
        }

        public bool Reach(Type type)
        {
            return _type == type || _edges.Any(tn => tn.Reach(type));
        }

        public IEnumerator<TypeNode> GetEnumerator()
        {
            return _edges.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
