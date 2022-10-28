using System;
using System.Collections.Generic;

namespace Mkay
{
    public class SymbolEnvironment
    {
        private readonly Dictionary<string, Symbol> _expansions = new Dictionary<string, Symbol>();
        private readonly Dictionary<string, Symbol> _symbols = new Dictionary<string, Symbol>();
        private readonly Symbol _self;
        
        public Symbol Self { get { return _self; } }

        public SymbolEnvironment(IEnumerable<string> selfExpandingSymbols)
        {
            _self = GetSymbol(".");
            foreach (var t in selfExpandingSymbols)
            {
                AddSelfExpanding(t);
            }
        }

        public SymbolEnvironment(): this(new [] { "<", ">", "==", "<=", ">=", "!=", "~"})
        {
        }

        public Symbol GetSymbol(string name)
        {
            if (!_symbols.ContainsKey(name))
            {
                _symbols[name] = new Symbol(name);
            }

            return _symbols[name];
        }

        private void AddSelfExpanding(string symbol)
        {
            if (!_expansions.ContainsKey(symbol))
            {
                _expansions[symbol] = GetSymbol(symbol);
            }
        }

        public bool IsSelfExpanding(Symbol symbol)
        {
            var result = _expansions.ContainsValue(symbol);
            return result;
        }
    }
}