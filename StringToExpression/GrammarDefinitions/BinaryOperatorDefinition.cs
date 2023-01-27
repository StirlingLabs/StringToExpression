using FastExpressionCompiler.LightExpression;
using JetBrains.Annotations;
using SysExpression = System.Linq.Expressions.Expression;

namespace StringToExpression.GrammarDefinitions;

/// <summary>
/// Represents an operator that has two parameters, one to the left and one to the right.
/// </summary>
/// <seealso cref="BaseOperatorDefinition" />
public sealed class BinaryOperatorDefinition : BaseOperatorDefinition {

  /// <summary>
  /// Initializes a new instance of the <see cref="BinaryOperatorDefinition"/> class.
  /// </summary>
  /// <param name="name">The name of the definition.</param>
  /// <param name="regex">The regex to match tokens.</param>
  /// <param name="orderOfPrecedence">The relative order this operator should be applied. Lower orders are applied first.</param>
  /// <param name="expressionBuilder">The function given the single operand expressions, outputs a new operand.</param>
  public BinaryOperatorDefinition(string name,
    [RegexPattern] string regex,
    int orderOfPrecedence,
    ExpressionBuilder expressionBuilder)
    : base(
      name,
      regex,
      orderOfPrecedence,
      new[] { RelativePosition.Left, RelativePosition.Right },
      expressionBuilder) {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="BinaryOperatorDefinition"/> class.
  /// </summary>
  /// <param name="name">The name of the definition.</param>
  /// <param name="regex">The regex to match tokens.</param>
  /// <param name="orderOfPrecedence">The relative order this operator should be applied. Lower orders are applied first.</param>
  /// <param name="expressionBuilder">The function given the single operand expressions, outputs a new operand.</param>
  /// <param name="implicitTypeConversion">Whether or not to attempt to implicitly convert to the same type.</param>
  public BinaryOperatorDefinition(string name,
    [RegexPattern] string regex,
    int orderOfPrecedence,
    Func<Expression,Expression,Expression> expressionBuilder,
    bool implicitTypeConversion = true)
    : base(
      name,
      regex,
      orderOfPrecedence,
      new[] { RelativePosition.Left, RelativePosition.Right },
      expressionBuilder is not null
      ? (param,_,_) => {
        var left = param[0];
        var right = param[1];
        if (implicitTypeConversion)
          ExpressionConversions.TryImplicitlyConvert(ref left, ref right);
        return expressionBuilder(left, right);
      }
      : throw new ArgumentNullException(nameof(expressionBuilder))) {
  }

}