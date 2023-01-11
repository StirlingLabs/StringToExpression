using FastExpressionCompiler.LightExpression;
using JetBrains.Annotations;

namespace StringToExpression.GrammarDefinitions;

/// <summary>
/// Represents a the grammar for a function call.
/// </summary>
/// <seealso cref="StringToExpression.GrammarDefinitions.BracketOpenDefinition" />
public sealed class FunctionCallDefinition : BracketOpenDefinition {

  /// <summary>
  /// Argument types that the function accepts.
  /// </summary>
  public readonly IReadOnlyList<Type>? ArgumentTypes;

  /// <summary>
  /// A function given the arguments, outputs a new operand.
  /// </summary>
  public readonly ExpressionBuilder ExpressionBuilder;

  /// <summary>
  /// Initializes a new instance of the <see cref="FunctionCallDefinition"/> class.
  /// </summary>
  /// <param name="name">The name of the definition.</param>
  /// <param name="regex">The regex to match tokens.</param>
  /// <param name="argumentTypes">The argument types that the function accepts.</param>
  /// <param name="expressionBuilder">The function given the single operand expressions, outputs a new operand.</param>
  public FunctionCallDefinition(
    string name,
    [RegexPattern] string regex,
    IReadOnlyList<Type>? argumentTypes,
    ExpressionBuilder expressionBuilder)
    : base(name, regex) {
    ArgumentTypes = argumentTypes;
    ExpressionBuilder = expressionBuilder ?? throw new ArgumentNullException(nameof(expressionBuilder));
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="FunctionCallDefinition"/> class.
  /// </summary>
  /// <param name="name">The name of the definition.</param>
  /// <param name="regex">The regex to match tokens.</param>
  /// <param name="argumentTypes">The argument types that the function accepts.</param>
  /// <param name="expressionBuilder">The function given the single operand expressions, outputs a new operand.</param>
  public FunctionCallDefinition(
    string name,
    [RegexPattern] string regex,
    IReadOnlyList<Type>? argumentTypes,
    Func<IReadOnlyList<Expression>,Expression> expressionBuilder)
    : base(name, regex) {
    if (expressionBuilder is null)
      throw new ArgumentNullException(nameof(expressionBuilder));
    
    ArgumentTypes = argumentTypes;
    ExpressionBuilder = (param, _, _) => expressionBuilder(param);
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="FunctionCallDefinition"/> class.
  /// </summary>
  /// <param name="name">The name of the definition.</param>
  /// <param name="regex">The regex to match tokens.</param>
  /// <param name="expressionBuilder">The function given the single operand expressions, outputs a new operand.</param>
  public FunctionCallDefinition(string name, [RegexPattern] string regex, ExpressionBuilder expressionBuilder)
    : this(name, regex, null, expressionBuilder) {
  }


  /// <summary>
  /// Initializes a new instance of the <see cref="FunctionCallDefinition"/> class.
  /// </summary>
  /// <param name="name">The name of the definition.</param>
  /// <param name="regex">The regex to match tokens.</param>
  /// <param name="expressionBuilder">The function given the single operand expressions, outputs a new operand.</param>
  public FunctionCallDefinition(string name, [RegexPattern] string regex, Func<IReadOnlyList<Expression>, Expression> expressionBuilder)
    : this(name, regex, null, expressionBuilder is not null
      ? (param,_,_) => expressionBuilder(param)
      : throw new ArgumentNullException(nameof(expressionBuilder))) {
  }

  /// <summary>
  /// Applies the bracket operands. Executes the expressionBuilder with all the operands in the brackets.
  /// </summary>
  /// <param name="bracketOpen">The operator that opened the bracket.</param>
  /// <param name="bracketOperands">The list of operands within the brackets.</param>
  /// <param name="bracketClose">The operator that closed the bracket.</param>
  /// <param name="state">The current parse state.</param>
  /// <exception cref="FunctionArgumentCountException">When the number of operands does not match the number of arguments</exception>
  /// <exception cref="FunctionArgumentTypeException">When argument Type does not match the type of the expression</exception>
  /// <exception cref="OperationInvalidException">When an error occured while executing the expressionBuilder</exception>
  public override void ApplyBracketOperands(Operator bracketOpen, Stack<Operand> bracketOperands, Operator bracketClose, ParseState state) {
    var bracketOperandSourceMaps = bracketOperands.Select(x => x.SourceMap);
    var operandSource = Substring.Encompass(bracketOperandSourceMaps);
    var functionArguments = bracketOperands.Select(x => x.Expression);
    // if we have been given specific argument types validate them
    if (ArgumentTypes is not null) {
      var expectedArgumentCount = ArgumentTypes.Count;
      if (expectedArgumentCount != bracketOperands.Count)
        throw new FunctionArgumentCountException(
          operandSource,
          expectedArgumentCount,
          bracketOperands.Count);

      functionArguments = bracketOperands.Zip(ArgumentTypes, (o, t) => {
        Expression result;
        try {
          result = ExpressionConversions.Convert(o.Expression, t);
        }
        catch (InvalidOperationException) {
          // if we cant convert to the argument type then something is wrong with the argument
          // so we will throw it up
          throw new FunctionArgumentTypeException(o.SourceMap, t, o.Expression.Type);
        }

        if (result is null) throw new FunctionArgumentTypeException(o.SourceMap, t, o.Expression.Type);

        return result;
      });
    }
    var functionArgumentsArray = functionArguments.ToArray();

    var functionSourceMap = Substring.Encompass(bracketOpen.SourceMap, operandSource);
    var tokens = bracketOperandSourceMaps.Prepend(bracketOpen.SourceMap).Append(bracketClose.SourceMap);
    Expression? output;
    try {
      output = ExpressionBuilder(functionArgumentsArray, state.Parameters, tokens);
    }
    catch (Exception ex) {
      throw new OperationInvalidException(functionSourceMap, ex);
    }

    if (output is not null)
      state.Operands.Push(new(output, functionSourceMap));
  }

}