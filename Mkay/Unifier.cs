using System;
using System.Collections.Generic;

namespace Mkay
{
    public class Unifier
    {
        private readonly Dictionary<Type, TypeNode> _types = new Dictionary<Type, TypeNode>();

        public Unifier()
        {
            var lnode = new TypeNode(typeof(long));
            var dnode = new TypeNode(typeof(double));
            var inode = new TypeNode(typeof(int)) { dnode, lnode };
            var snode = new TypeNode(typeof(short)) { inode }; 
            _types[typeof(long)] = lnode;
            _types[typeof(short)] = snode;
            _types[typeof(int)] = inode;
            _types[typeof(double)] = dnode;
        }

        public Type Unify(Type t1, Type t2)
        {
            if (t1 == t2)
            {
                return t1;
            }

            if (_types.ContainsKey(t1) && _types[t1].Reach(t2))
            {
                return t2;
            }

            if (_types.ContainsKey(t2) && _types[t2].Reach(t1))
            {
                return t1;
            }

            return null;
        }
    }
}
