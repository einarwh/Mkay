(function () {
    function turtle() {
        var self = this;
        var turtles = [];
        self.add = function (input) {
            turtles.push(input);
        };
        self.seen = function (input) {
            return turtles.indexOf(input) !== -1;
        };
        return self;
    }

    function createMkayEval(mkayParser, element) {
        this.ninja = turtle();
        return function (args) {
            var lazy = args[0];
            var input = lazy();
            if (ninja.seen(input)) {
                MKAY.setError(element.id, "Don't disturb the turtles, mkay?");
                return false;
            }
            else {
                ninja.add(input);
            }
            var ast = mkayParser(input);
            var val = MKAY.getValidator(element.id, input, ast);
            var res = val();
            return res;
        };
    }

    jQuery.validator.addMethod("mkay", function (value, element, param) {
        "use strict";
        console.log(param);
        var ruledata = JSON && JSON.parse(param) || $.parseJSON(param);
        var mkayParser = MKAY.getMkayParser(element.name);
        var mkayEval = createMkayEval(mkayParser, element);
        MKAY.addMethod("eval", mkayEval);
        var validator = MKAY.getValidator(element.id, ruledata.rule, ruledata.ast);
        var res = validator();
        return res;
    });

    jQuery.validator.unobtrusive.adapters.add("mkay", ["rule"], function (options) {
        "use strict";
        options.rules.mkay = options.params.rule;
        options.messages.mkay = function (rule, element) {
            var msg = MKAY.getError(element.id) || options.message;
            return msg;
        };
    });

    jQuery.validator.addMethod(
    "date",
    function (value, element) {
        "use strict";
        var s = value.replace(/\./g, '/');

        // Chrome requires tolocaledatestring conversion, otherwise just use the slashed format
        var d = new Date();
        return this.optional(element) || !/Invalid|NaN/.test(new Date(d.toLocaleDateString(value))) || !/Invalid|NaN/.test(new Date(s));
    },
    "POW"
    );
})();

