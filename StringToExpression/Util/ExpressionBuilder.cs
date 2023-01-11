using FastExpressionCompiler.LightExpression;

namespace StringToExpression;

public delegate Expression ExpressionBuilder(
  IReadOnlyList<Expression> localParameters,
  IReadOnlyList<ParameterExpression> globalParameters,
  IEnumerable<Substring> tokens
);