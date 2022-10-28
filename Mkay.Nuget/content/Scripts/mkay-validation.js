/// <reference path="jquery-1.7.1-vsdoc.js" />
/// <reference path="jquery.validate-vsdoc.js" />
/// <reference path="jquery.validate.unobtrusive.js" />

var MKAY = MKAY || {};

(function (namespace) {
    "use strict";

    var rules = { };

    var doc = document;

    function parseDate(val) {
        var dp = /^(\d\d)\S(\d\d)\S(\d\d\d\d)(\s(\d\d)\S(\d\d)\S(\d\d))?$/g;
        var match = dp.exec(val);
        if (match !== null) {
            var day = +match[1];
            var month = +match[2];
            var year = +match[3];
            return new Date(year, month - 1, day);
        }
        return null;
    }

    var calls = function () {

        function logical(fun, breaker) {
            return function (args) {
                var copy = args.slice(0);
                while (copy.length > 0) {
                    var val = copy.pop()();
                    if (val === breaker) {
                        return breaker;
                    }
                }

                return !breaker;
            };
        }

        function compare(fun) {
            return function (args) {
                var a = args[0]();
                var b = args[1]();
                if (typeof a === "string" || typeof b === "string") {
                    if (a instanceof Date) {
                        b = parseDate(b);
                    } else if (b instanceof Date) {
                        a = parseDate(a);
                    } else {
                        var aDate = parseDate(a);
                        var bDate = parseDate(b);
                        if (aDate && bDate) {
                            a = aDate;
                            b = bDate;
                        }
                    }
                }
                
                return fun(a, b);
            };
        }
        
        function justArith(fun, base) {
            var result = function (vals) {
                var acc, val;
                acc = base;
                while (vals.length > 0) {
                    val = vals.pop();
                    acc = fun(acc, val);
                }
                return acc;
            };
            return result;
        }

        function arith(fun, base) {
            return function (args) {
                var acc, copy, val;
                acc = base;
                copy = args.slice(0);
                while (copy.length > 0) {
                    val = copy.pop()();
                    acc = fun(acc, val);
                }
                return acc;
            };
        }

        var concat = function(args) {
            var len = args.length;
            var res = "";
            for (var i = 0; i < len; i++) {
                res += args[i]();
            }
            return res;
        };

        var and = logical(function (a, b) { return a && b; }, false);
        var or = logical(function (a, b) { return a || b; }, true);

        var isNumber = function(o) {
            return typeof o === 'number' && isFinite(o);
        };

        var plus = function(args) {
            var vals = [];
            var len = args.length;
            var numbersOnly = true;
            for (var i = 0; i < len; i++) {
                var val = args[i]();
                numbersOnly = numbersOnly && isNumber(val);
                vals.push(val);
            }
            if (numbersOnly) {
                var arithResult = justArith(
                    function(a, b) { 
                        return a + b; 
                    }, 
                    0);
                return arithResult(vals);
            }
            return vals.join("");
        };
        
        var multiply = arith(function (a, b) { return a * b; }, 1);

        return {
            "&&": and,
            "and": and,
            "all": and,
            "||": or,
            "or": or,
            "any": or,
            "len": function (args) {
                var first = args[0];
                var s = first();
                return s.length;
            },
            "max": function (args) {
                var a = args[0]();
                var b = args[1]();
                return Math.max(a, b);
            },
            "avg": function (args) {
                var acc = 0;
                for (var i = 0; i < args.length; i++) {
                    acc += args[i]();
                }
                return acc / args.length;
            },
            "sum": plus,
            "+": plus,
            "*": multiply,
            ">": compare(function(a, b) { return a > b; }),
            "<": compare(function(a, b) { return a < b; }),
            ">=": compare(function(a, b) { return a >= b; }),
            "<=": compare(function(a, b) { return a <= b; }),
            "==": compare(function(a, b) { return a === b; }),
            "!=": compare(function(a, b) { return a !== b; }),
            "now": function() {
                return new Date();
            },
            "~" : function (args) {
                var input = args[0]();
                var pattern = args[1]();
                var rex = new RegExp(pattern);
                return rex.test(input);
            },
            "rev": function(args) {
                var s = args[0]();
                return s.split("").reverse().join("");
            },
            "cut": function(args) {
                var vals = [];
                var argslen = args.length;
                for (var i = 0; i < argslen; i++) {
                    var v = args[i]();
                    vals.push(v);
                }
                var s = vals.shift();
                var result = String.prototype.slice.apply(s, vals);
                return result;
            }   
        };
    }();

    var types = {
        "call" : function (jo) {
            var fun = findCall(jo.value);
            var args = jo.operands; 
            var funcs = [];
            for (var i = 0; i < args.length; i++) {
                funcs.push(create(args[i]));
            }

            return function() {
                return fun(funcs);
            };
        },
        
        "property" : function (jo) {
            var id = jo.value;
            return function() {
                var elem = doc.getElementById(id);
                if (elem === null || elem === undefined) {
                    return undefined;
                }
                var val = elem.value;
                if (val.length === 0) {
                    return val;
                }
                var number = +val;
                if (!isNaN(number)) {
                    return number;
                }
                var d = parseDate(val);
                if (d !== null) {
                    return d;
                }
                return val;
            };
        },
        
        "integer" : function (jo) {
            var ival = parseInt(jo.value, 10);

            return function() {
                return ival;
            };
        },
        
        "float" : function (jo) {
            var dval = parseFloat(jo.value);

            return function() {
                return dval;
            };
        },

        "boolean" : function (jo) {
            var bval = jo.value;

            return function() {
                return bval;
            };
        },
        
        "string" : function (jo) {
            var sval = jo.value;

            return function() {
                return sval;
            };
        }
    };
        
    function getValidator(id, rule, ast) {
        var key = id + ":" + rule;
        if (!rules[key]) {
            rules[key] = create(ast);
        }
        
        return rules[key];
    }

    function create(jo) {
        var func = types[jo.type];
        if (func) {
            return func(jo);
        }

        return function() { return false; };
    }

    function findCall(name) {
        console.log("lookup call " + name);
        var m = calls[name];
        if (m) {
            return m;
        }
		
        return function() { return false; };
    }
    
    function addMethod(name, fun) {
        calls[name] = fun;
    }

    var errors = { };
    
    function setError(element, message) {
        errors["MKAY-error" + element.id] = message;
    }
    
    function getError(element) {
        var key = "MKAY-error" + element.id;
        var msg = errors[key];
        if (msg !== undefined) {
            delete errors[key];
        }
        return msg;
    }

    function setDocument(replacement) {
        doc = replacement;
    }

    namespace.setDocument = setDocument;

    namespace.addMethod = addMethod;

    namespace.getValidator = getValidator;

    namespace.setError = setError;

    namespace.getError = getError;

})(MKAY);
