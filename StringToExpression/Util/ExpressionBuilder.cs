using FastExpressionCompiler.LightExpression;

namespace StringToExpression;

/// <summary>
/// A delegate representing a method that can build an expression from a list of input tokens.
/// </summary>
/// <param name="localParameters">A list of local parameters that can be used in the generated expression.</param>
/// <param name="globalParameters">A list of global parameters that can be used in the generated expression.</param>
/// <param name="tokens">A list of input tokens that will be used to generate the expression.</param>
/// <returns>A generated expression based on the input tokens.</returns>
public delegate Expression ExpressionBuilder(
  IReadOnlyList<Expression> localParameters,
  IReadOnlyList<ParameterExpression> globalParameters,
  IEnumerable<Substring> tokens
);