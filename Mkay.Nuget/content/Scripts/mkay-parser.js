var MKAY = MKAY || {};

(function(namespace) {
    "use strict";

    function mkayParser(property, rule) {

        var input = rule;
        if (rule[0] !== "(") {
            input = "(" + input + ")";
        }

        var index = 0;

        var skipWhitespace = function() {
            while (true) {
                if (input[index] !== ' ') {
                    return;
                }
                ++index;
            }
        };

        var parseAtomExpression = function(atomText) {
            var c = atomText[0];
            if (c <= "9" && (c >= "0" || ((c === "+" || c === "-") && atomText.length > 1))) {
                var ival = parseInt(atomText, 10);
                if (!isNaN(ival)) {
                    return {
                        type: "integer",
                        value: atomText
                    };
                }
                var dval = parseFloat(atomText);
                if (!isNaN(dval)) {
                    return {
                        type: "float",
                        value: atomText
                    };
                }
            }
            if (c !== "\"") {
                if (atomText === "true") {
                    return {
                        type: "boolean",
                        value: true
                    };
                }
                if (atomText === "false") {
                    return {
                        type: "boolean",
                        value: false
                    };
                }
                var propertyName = atomText;
                if (propertyName === ".") {
                    propertyName = property;
                }
                return {
                    type: "property",
                    value: propertyName
                };
            }
            return {
                type: "string",
                value: atomText
            };
        };

        var parseSimpleExpression = function() {
            skipWhitespace();
            var start = index;
            var insideQuotes = false;
            var seenQuotes = false;
            while (index < input.length) {
                var c = input[index];
                if (c === "\"") {
                    if (seenQuotes && !insideQuotes) {
                        throw "EVIL EVAL " + input.slice(start, index);
                    }
                    insideQuotes = !insideQuotes;
                    seenQuotes = true;
                }
                if (c === ")" || (c === " " && !insideQuotes)) {
                    var s = input.slice(start, index);
                    var atomExp = parseAtomExpression(s);
                    return atomExp;
                }
                ++index;
            }
            throw "EVIL EVAL " + input.slice(start, index);
        };

        var parseSingleExpression = function() {
            skipWhitespace();
            if (input[index] === '(') {
                return parseCompoundExpression();
            }
            return parseSimpleExpression();
        };

        var parseExpressionList = function() {
            skipWhitespace();
            var result = [];
            while (index < input.length) {
                var exp = parseSingleExpression();
                result.push(exp);
                if (input[index] === ')') {
                    break;
                }
                ++index;
            }
            return result;
        };

        var readChar = function(c) {
            if (input[index] !== c) {
                throw "Bad input at position " + index + " in string " + input + " (excepted '" + c + "')";
            }
            ++index;
        };

        var comparisons = [ "==", "!=", "<=", ">=", "<", ">" ];

        var parseCompoundExpression = function() {
            readChar("(");
            var exps = parseExpressionList();
            readChar(")");
            var first = exps.shift();
            var op = first.value;
            if (exps.length == 1 && comparisons.indexOf(op) !== -1) {
                exps.unshift({ type: "property", value: property });
            }
            return {
                type: "call",
                value: first.value,
                operands: exps
            };
        };

        this.parse = function() {
            console.log("calling parse function.");
            console.log("input: " + input);
            console.log("index: " + index);

            var result = parseCompoundExpression();
            for (var i = 0; i < result.operands.length; i++) {
                var op = result.operands[i];
                console.log("   " + op.type + " " + op.value);
            }
            return result;
        };
    }

    function getMkayParser(property) {
        console.log("CREATE PARSER function.");
        var cached = [];

        return function(mtext) {
            console.log("CALLING PARSER with " + mtext);
            for (var i = 0; i < cached.length; i++) {
                var it = cached[i];
                if (it.source === mtext) {
                    console.log("returning CACHED result for " + mtext);
                    return it.ast;
                }
            }
            var p = new mkayParser(property, mtext);
            var result = p.parse();
            while (cached.length > 10) {
                cached.shift();
            }
            var keep = { source: mtext, ast: result };
            cached.push(keep);
            return result;
        };
    }

    namespace.getMkayParser = getMkayParser;
    
})(MKAY);