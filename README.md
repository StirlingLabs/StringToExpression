![StringToExpression](https://raw.githubusercontent.com/StirlingLabs/StringToExpression/main/StringToExpression.jpg)

[![Integration](https://github.com/StirlingLabs/StringToExpression/actions/workflows/integrate.yaml/badge.svg)](https://github.com/StirlingLabs/StringToExpression/actions/workflows/integrate.yaml)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/8f83d54adab74d1ba646887cdfdf9b64)](https://www.codacy.com/gh/StirlingLabs/StringToExpression/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=StirlingLabs/StringToExpression&amp;utm_campaign=Badge_Grade)
![GitHub release (latest SemVer)](https://img.shields.io/github/v/release/StirlingLabs/StringToExpression?sort=semver)

This is a fork of [Alex Davies' StringToExpression](https://github.com/codecutout/StringToExpression/) library.

StringToExpression allows you to create methods that take strings and outputs .NET expressions. It is highly
configurable allowing you to define your own language with your own syntax.

Available via NuGet as [`StirlingLabs.StringToExpression`](https://www.nuget.org/packages/StirlingLabs.StringToExpression).

## Arithmetic

A basic arithmetic language `ArithmeticLanguage` is provided for performing algebra. It can be used as is, or extended as desired.

```csharp
var language = new ArithmeticLanguage();
Expression<Func<decimal>> expressionFunction = language.Parse("(4 - 2) * 5 + 9 / 3");
Func<decimal> function = expressionFunction.compile();

Assert.Equal(13, function());
```

## OData filter

`ODataFilterLanguage` is provided as a lightweight way to parse [OData filter expressions](http://www.odata.org/documentation/odata-version-2-0/uri-conventions/#FilterSystemQueryOption) which are a nice way to pass generic filtering requirements into a WebAPI.

```csharp
public async Task<IHttpActionResult> GetDoohickies([FromUri(Name = "$filter")] string filter = "name eq 'discount' and rating gt 18")
{
  var language = new ODataFilterLanguage()
  Expression<Func<Doohicky, bool>> predicate = language.Parse<Doohickey>(filter);
  
  //can either pass this expression into either IQueryable or IEnumerable where clauses
  return await DataContext.Doohickies.Where(predicte).ToListAsync();
}

```

`StringToExpression` has the advantage of being configurable; if the OData parser doesnt support methods you want, (or
it supports methods you dont want) it is very easy to extend `ODataFilterLanguage` and modify the configuration

## Custom languages

Languages are defined by a set of `GrammarDefintions`. These define both how the string is broken up into tokens as well
as the behaviour of each token. There are many subclasses of `GrammarDefinition` that makes implementing standard
language features very easy.

An example of a very simple arithmetic language is as follows

```csharp
ListDelimiterDefinition delimeter;
BracketOpenDefinition openBracket, sqrt;
language = new Language(new [] {
    new OperandDefinition(
        name:"DECIMAL",
        regex: @"\-?\d+(\.\d+)?",
        expressionBuilder: x => Expression.Constant(decimal.Parse(x))),
    new BinaryOperatorDefinition(
        name:"ADD",
        regex: @"\+",
        orderOfPrecedence: 2,
        expressionBuilder: (left,right) => Expression.Add(left, right)),
    new BinaryOperatorDefinition(
        name:"SUB",
        regex: @"\-",
        orderOfPrecedence: 2,
        expressionBuilder: (left,right) => Expression.Subtract(left, right)),
    new BinaryOperatorDefinition(
        name:"MUL",
        regex: @"\*",
        orderOfPrecedence: 1, //multiply should be done before add/subtract
        expressionBuilder: (left,right) => Expression.Multiply(left, right)),
    new BinaryOperatorDefinition(
        name:"DIV",
        regex: @"\/",
        orderOfPrecedence: 1, //division should be done before add/subtract
        expressionBuilder: (left,right) => Expression.Divide(left, right)),
    sqrt = new FunctionCallDefinition(
        name:"FN_SQRT",
        regex: @"sqrt\(",
        argumentTypes: new[] {typeof(double) },
        expressionBuilder: (parameters) => {
            return Expression.Call(
                null,
                method:typeof(Math).GetMethod("Sqrt"),
                arguments: new [] { parameters[0] });
        }),
    openBracket = new BracketOpenDefinition(
        name: "OPEN_BRACKET",
        regex: @"\("),
    delimeter = new ListDelimiterDefinition(
        name: "COMMA",
        regex: ","),
    new BracketCloseDefinition(
        name: "CLOSE_BRACKET",
        regex: @"\)",
        bracketOpenDefinitions: new[] { openBracket, sqrt },
        listDelimeterDefinition: delimeter)
    new GrammarDefinition(name: "WHITESPACE", regex: @"\s+", ignore: true) //we dont want to process whitespace
  });
```

Some of the out of the box grammar definitions are detailed below

| Name  | Description | Properties |
|-------|-------------|------------|
| `GrammarDefintion` | Base class for all definitions. Does not perform any functionality during the parsing. | **name** - A name for this rule.<br />**regex** - the regular expression that will match for this token. |
| `OperandDefinition` | Defines the smallest atomic piece in your language, used to represent items like numbers or strings. | **expressionBuilder** - a function that when given the string matched from the regex it produces a .NET expression (usually an `ConstantExpression`). |
| `BinaryOperatorDefintion` | An operation that takes parameters from the left and right of it. Often represents arithmetic operaitons (`+`, `-`, `*`, `/`) or equality checks (`==`, `!=`, `<` `>`) or boolean logic (`and`, `or`). | **orderOfPrecedence** - determines when this function should run, lower numbers get run before higher numbers (allowing defining BEDMAS rules).<br />**expressionBuilder** - function that takes in two Expression (the left and right of the operator), and outputs a new expression combining them. |
| `UnaryOperator` | An operation that takes a single parameter, used for operations such as `not`. | **orderOfPrecedence** - determines when this function should run, lower numbers get run before higher numbers.<br />**parameterPosition** - whether the operand is to the left or right of the operator. |
| `BracketOpenDefinition` | Defines an open bracket, functionally does not do much unless paired with a `BracketCloseDefinition`. | |
| `ListDelimeterDefinition` | The separator to use to denote lists within brackets (a `,` in most languages) functionally does not do much unless paired with a `BracketCloseDefinition`. | |
| `BracketCloseDefinition`  | The expression between the brackets is evaluated first. | **bracketOpenDefinitions** - list of definitions that would be treated as a start to the bracketing.<br />**listDelimiterDefinition** - definition of the `ListDelimeterDefinition`. |                                                                                                                   
| `FunctionCallDefinition`  | Defines a function that takes in a list of operands. Also acts as a bracket `BracketOpenDefinition` definition. | **argumentTypes** - list of types that define the types expected and the number of arguments expecte.<br />**expressionBuilder** - takes an array of expressions and output a single expression. |

If your language is more complicated than the provided `GrammarDefinitions` you are able to define your own by
extending `GrammarDefintion`. You best read the [Nuts and bolts](#Nuts-and-bolts) section to determine the best way to
implement your definition.

## Error handling

All parsing exceptions extend `ParseException`. A `ParseException` will contain both a readable message and
a `StringSegment` that represents what token(s) in the original input string caused the error.

The `StringSegment` allows pinpointing of issues such as where operands are missing, which function has too many
parameters or where the unexpected character is. It provides a useful feedback for wherever you are getting your
original strings from.

## Nuts and bolts

Under the hood `StringToExpression` implements a [shunting-yard algorithm](https://en.wikipedia.org/wiki/Shunting-yard_algorithm).

The internal parsing state contains a `Stack<Operand>` and a `Stack<Operator>` that are built up during the parsing

* `Operand` - Represent a .NET expression. This may be a simple `ConstantExpression`, or the root of a complicated Tree
  of `BinaryExpressions`
* `Operator` - Is a function that can be run. Generally these functions when run will consume one or more operands and
  produce one operand. Such that run operators reduces the number of operands on the stack.

The parsing is done in roughly three steps

1. *Tokenize* - The string is parsed through a tokenizer which uses the regular expressions defined in
   the `GrammarDefinitions` to break the strings into `Tokens`. A `Token` knows the `GrammarDefinition` that created it
   and the string value it represents.

2. *Apply `GrammarDefinitions`* - All `GrammarDefinition` has an `void Apply(Token token, ParseState state)` method. We
   first read all the `Tokens` in sequentially and run each `GrammarDefinition` `Apply` method. The apply method can
   make any modifications to the state it wants, this can range from something simple like pushing an Operand on to the
   stack, to something more complicated like executing operands.

3. *Execute Operators* -Once all the tokens are Applied we will start poping Operators off the stack and executing them.
   When an `Operator` executes its generally expected that it will consume one or more `Operands` and create
   one `Operand`. This way by the time we apply all the operators we should only have a single `Operand` on the stack,
   that is our result.

To customize you can make your own `GrammarDefinition` and implment the `Apply` method to meet your purposes.