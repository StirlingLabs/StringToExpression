using FastExpressionCompiler.LightExpression;

namespace StringToExpression.GrammarDefinitions;

/// <summary>
/// Represents a definition for an operator in a grammar.
/// </summary>
/// <inheritdoc cref="BaseOperatorDefinition"/>
public class OperatorDefinition : BaseOperatorDefinition {
  /// <summary>
  /// Initializes a new instance of the <see cref="OperatorDefinition"/> class with the specified name, regular expression, parameter positions, and expression builder.
  /// </summary>
  /// <param name="name">The name of the operator.</param>
  /// <param name="regex">The regular expression used to match the operator in a string.</param>
  /// <param name="parameterPositions">The positions of the operator's parameters in the matched string.</param>
  /// <param name="expressionBuilder">A <see cref="ExpressionBuilder"/> that builds an expression from the operator's parameters.</param>
  public OperatorDefinition(string name, string regex, IReadOnlyList<RelativePosition> parameterPositions, ExpressionBuilder expressionBuilder)
    : base(name, regex, parameterPositions, expressionBuilder) {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="OperatorDefinition"/> class with the specified name, regular expression, parameter positions, and expression builder.
  /// </summary>
  /// <param name="name">The name of the operator.</param>
  /// <param name="regex">The regular expression used to match the operator in a string.</param>
  /// <param name="parameterPositions">The positions of the operator's parameters in the matched string.</param>
  /// <param name="expressionBuilder">A <see cref="Func{T, TResult}"/> that builds an expression from the operator's parameters.</param>
  public OperatorDefinition(string name, string regex, IReadOnlyList<RelativePosition> parameterPositions, Func<IReadOnlyList<Expression>, Expression> expressionBuilder)
    : base(name, regex, parameterPositions, expressionBuilder) {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="OperatorDefinition"/> class with the specified name, regular expression, order of precedence, parameter positions, and expression builder.
  /// </summary>
  /// <param name="name">The name of the operator.</param>
  /// <param name="regex">The regular expression used to match the operator in a string.</param>
  /// <param name="orderOfPrecedence">The order of precedence of the operator.</param>
  /// <param name="parameterPositions">The positions of the operator's parameters in the matched string.</param>
  /// <param name="expressionBuilder">A <see cref="ExpressionBuilder"/> that builds an expression from the operator's parameters.</param>
  public OperatorDefinition(string name, string regex, int? orderOfPrecedence, IReadOnlyList<RelativePosition>? parameterPositions, ExpressionBuilder expressionBuilder)
    : base(name, regex, orderOfPrecedence, parameterPositions, expressionBuilder) {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="OperatorDefinition"/> class with the specified name, regular expression, order of precedence, parameter positions, and expression builder.
  /// </summary>
  /// <param name="name">The name of the operator.</param>
  /// <param name="regex">The regular expression used to match the operator in a string.</param>
  /// <param name="orderOfPrecedence">The order of precedence of the operator.</param>
  /// <param name="parameterPositions">The positions of the operator's parameters in the matched string.</param>
  /// <param name="expressionBuilder">A <see cref="Func{T, TResult}"/> that builds an expression from the operator's parameters.</param>
  public OperatorDefinition(string name, string regex, int? orderOfPrecedence, IReadOnlyList<RelativePosition>? parameterPositions, Func<IReadOnlyList<Expression>, Expression> expressionBuilder)
    : base(name, regex, orderOfPrecedence, parameterPositions, expressionBuilder) {
  }

}