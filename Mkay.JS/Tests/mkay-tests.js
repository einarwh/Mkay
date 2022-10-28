function createFauxDocument(elems) {
    return function() {
        var self = this;
        self.getElementById = function(eid) {
            return elems[eid];
        };
        return self;
    }();
}

var comparePropertyToSumOfValues = {
    "type": "call",
    "value": ">",
    "operands": [
        {
        "type": "property",
        "value": "X"
        },
        {
        "type": "call",
        "value": "+",
        "operands": [
            {
            "type": "property",
            "value": "A"
            },
            {
            "type": "property",
            "value": "B"
            },
            {
            "type": "property",
            "value": "C"
            }
        ]}
    ]
};

var compareTwoHardcodedDateStrings = {
    "type": "call",
    "value": "<",
    "operands": [
        {
            "type": "string",
            "value": "31.01.2013"
        },
        {
            "type": "string",
            "value": "01.02.2013"
        }
    ]
};

var compareDateStringToSomeOtherString = {
    "type": "call",
    "value": "<",
    "operands": [
        {
            "type": "string",
            "value": "31.01.2013"
        },
        {
            "type": "string",
            "value": "Foobar"
        }
    ]
};

var compareDateStringToTooLongDateString = {
    "type": "call",
    "value": "==",
    "operands": [
        {
            "type": "string",
            "value": "31.01.2013"
        },
        {
            "type": "string",
            "value": "31.01.20130"
        }
    ]
};


test("another test", function() {
    var rule = "(< \"31.01.2013\" \"01.02.2013\")";
    var fd = createFauxDocument({});
    MKAY.setDocument(fd);
    var validator = MKAY.getValidator("", rule, compareTwoHardcodedDateStrings);
    var result = validator();
    ok(result, rule);
});

test("yet another test", function () {
    var rule = "(> \"31.01.2013\" \"Foobar\")";
    var fd = createFauxDocument({});
    MKAY.setDocument(fd);
    var validator = MKAY.getValidator("", rule, compareDateStringToSomeOtherString);
    var result = validator();
    ok(result, rule);
});

test("and another test", function () {
    var rule = "(< \"31.01.2013\" (now))";
    var fd = createFauxDocument({});
    MKAY.setDocument(fd);
    var validator = MKAY.getValidator("", rule, compareDateStringToSomeOtherString);
    var result = validator();
    ok(result, rule);
});

test("too long date", function () {
    var rule = "(== \"31.01.2013\" \"31.01.20130\")";
    var fd = createFauxDocument({});
    MKAY.setDocument(fd);
    var validator = MKAY.getValidator("", rule, compareDateStringToTooLongDateString);
    var result = validator();
    ok(!result, rule);
});

test("too long date with property", function () {
    var rule = "(== X \"31.01.20130\")";
    var es = {
        X: { value: "31.01.2013" }
    };
    var fd = createFauxDocument(es);
    MKAY.setDocument(fd);
    var data = {
        "type": "call",
        "value": "==",
        "operands": [
            {
                "type": "property",
                "value": "X"
            },
            {
                "type": "string",
                "value": "31.01.20130"
            }
        ]
    };
    var validator = MKAY.getValidator("", rule, data);
    var result = validator();
    ok(!result, rule);
});

test("too long date with property 2", function () {
    var rule = "(== X Y)";
    var es = {
        X: { value: "31.01.2013" },
        Y: { value: "" }
    };
    var fd = createFauxDocument(es);
    MKAY.setDocument(fd);
    var data = {
        "type": "call",
        "value": "==",
        "operands": [
            {
                "type": "property",
                "value": "X"
            },
            {
                "type": "property",
                "value": "Y"
            }
        ]
    };
    var validator = MKAY.getValidator("", rule, data);
    var result = validator();
    ok(!result, rule);
});

test("first mkay test", function() {
    var rule = "(> X (+ A B C))";
    
    var es = {
       X: { value: "120" },
       A: { value: "10" },
       B: { value: "20" },
       C: { value: "30" }
    };
    var fd = createFauxDocument(es);
    MKAY.setDocument(fd);
    var validator = MKAY.getValidator("X", rule, comparePropertyToSumOfValues);
    var result = validator();
    ok(result, rule);
});

test("second mkay test", function() {
    var rule = "(> X (+ A B C))";
    
    var es = {
       X: { value: "12" },
       A: { value: "10" },
       B: { value: "20" },
       C: { value: "30" }
    };
    var fd = createFauxDocument(es);
    MKAY.setDocument(fd);
    var validator = MKAY.getValidator("X", rule, comparePropertyToSumOfValues);
    var result = validator();
    ok(!result, rule);
});

test("third mkay test", function() {
    var rule = "(> X (+ A B C))";
    
    var es = {
       X: { value: "60" },
       A: { value: "10" },
       B: { value: "20" },
       C: { value: "30" }
    };
    var fd = createFauxDocument(es);
    MKAY.setDocument(fd);
    var validator = MKAY.getValidator("X", rule, comparePropertyToSumOfValues);
    var result = validator();
    ok(!result, rule);
});

test("plus for numbers", function () {
    var rule = "(== X (+ A B C))";
    var fd = createFauxDocument({
        X: { value: "60" },
        A: { value: "10" },
        B: { value: "20" },
        C: { value: "30" }
    });
    MKAY.setDocument(fd);
    var validator = MKAY.getValidator("X", rule, {
        "type": "call",
        "value": "==",
        "operands": [
            {
                "type": "property",
                "value": "X"
            },
            {
                "type": "call",
                "value": "+",
                "operands": [
                    {
                        "type": "property",
                        "value": "A"
                    },
                    {
                        "type": "property",
                        "value": "B"
                    },
                    {
                        "type": "property",
                        "value": "C"
                    }
                ]
            }
        ]
    });
    var result = validator();
    ok(result, rule);
});

test("plus for strings", function () {
    var rule = "(== X (+ A \" \" C))";

    var es = {
        X: { value: "Hello World" },
        A: { value: "Hello" },
        C: { value: "World" }
    };
    var fd = createFauxDocument(es);
    MKAY.setDocument(fd);
    var validator = MKAY.getValidator("X", rule, {
        "type": "call",
        "value": "==",
        "operands": [
            {
                "type": "property",
                "value": "X"
            },
            {
                "type": "call",
                "value": "+",
                "operands": [
                    {
                        "type": "property",
                        "value": "A"
                    },
                    {
                        "type": "string",
                        "value": " "
                    },
                    {
                        "type": "property",
                        "value": "C"
                    }
                ]
            }
        ]
    });
    var result = validator();
    ok(result, rule);
});

test("plus for strings and numbers", function () {
    var rule = "(== X (+ A \" \" C))";

    var es = {
        X: { value: "Hello 10" },
        A: { value: "Hello" },
        C: { value: 10 }
    };
    var fd = createFauxDocument(es);
    MKAY.setDocument(fd);
    var validator = MKAY.getValidator("X", rule, {
        "type": "call",
        "value": "==",
        "operands": [
            {
                "type": "property",
                "value": "X"
            },
            {
                "type": "call",
                "value": "+",
                "operands": [
                    {
                        "type": "property",
                        "value": "A"
                    },
                    {
                        "type": "string",
                        "value": " "
                    },
                    {
                        "type": "integer",
                        "value": "10"
                    }
                ]
            }
        ]
    });
    var result = validator();
    ok(result, rule);
});

test("len string", function () {
    var rule = "(== 4 (len X))";

    var fd = createFauxDocument({
        X: { value: "quux" }
    });
    MKAY.setDocument(fd);
    var validator = MKAY.getValidator("X", rule, {
        "type": "call",
        "value": "==",
        "operands": [
            {
                "type": "integer",
                "value": "4"
            },
            {
                "type": "call",
                "value": "len",
                "operands": [
                    {
                        "type": "property",
                        "value": "X"
                    }
                ]
            }
        ]
    });
    var result = validator();
    ok(result, rule);
});


test("len empty string", function () {
    var rule = "(== 0 (len X))";

    var fd = createFauxDocument({
        X: { value: "" }
    });
    MKAY.setDocument(fd);
    var validator = MKAY.getValidator("X", rule, {
        "type": "call",
        "value": "==",
        "operands": [
            {
                "type": "integer",
                "value": "0"
            },
            {
                "type": "call",
                "value": "len",
                "operands": [
                    {
                        "type": "property",
                        "value": "X"
                    }
                ]
            }
        ]
    });
    var result = validator();
    ok(result, rule);
});
