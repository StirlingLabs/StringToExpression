using FastExpressionCompiler.LightExpression;

namespace StringToExpression.GrammarDefinitions;

public class OperatorDefinition : BaseOperatorDefinition {

  public OperatorDefinition(string name, string regex, IReadOnlyList<RelativePosition> parameterPositions, ExpressionBuilder expressionBuilder)
    : base(name, regex, parameterPositions, expressionBuilder) {
  }

  public OperatorDefinition(string name, string regex, IReadOnlyList<RelativePosition> parameterPositions, Func<IReadOnlyList<Expression>, Expression> expressionBuilder)
    : base(name, regex, parameterPositions, expressionBuilder) {
  }

  public OperatorDefinition(string name, string regex, int? orderOfPrecedence, IReadOnlyList<RelativePosition>? parameterPositions, ExpressionBuilder expressionBuilder)
    : base(name, regex, orderOfPrecedence, parameterPositions, expressionBuilder) {
  }

  public OperatorDefinition(string name, string regex, int? orderOfPrecedence, IReadOnlyList<RelativePosition>? parameterPositions, Func<IReadOnlyList<Expression>, Expression> expressionBuilder)
    : base(name, regex, orderOfPrecedence, parameterPositions, expressionBuilder) {
  }

}