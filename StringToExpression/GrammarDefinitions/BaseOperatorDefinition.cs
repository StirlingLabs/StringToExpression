﻿using FastExpressionCompiler.LightExpression;
using JetBrains.Annotations;

namespace StringToExpression.GrammarDefinitions;

/// <summary>
///  Represents a piece of grammar that defines an operator.
/// </summary>
/// <seealso cref="BaseGrammarDefinition" />
[PublicAPI]
public abstract class BaseOperatorDefinition : BaseGrammarDefinition {

  /// <summary>
  /// A function given zero or more operands expressions, outputs a new operand.
  /// </summary>
  public readonly ExpressionBuilder ExpressionBuilder;

  /// <summary>
  /// Positions where parameters can be found.
  /// </summary>
  public readonly IReadOnlyList<RelativePosition> ParameterPositions;

  /// <summary>
  /// Relative order this operator should be applied. Lower orders are applied first.
  /// </summary>
  public readonly int? OrderOfPrecedence;

  /// <summary>
  /// Initializes a new instance of the <see cref="BaseOperatorDefinition"/> class.
  /// </summary>
  /// <param name="name">The name of the definition.</param>
  /// <param name="regex">The regex to match tokens.</param>
  /// <param name="parameterPositions">The relative positions where parameters can be found.</param>
  /// <param name="expressionBuilder">The function given zero or more operands expressions, outputs a new operand.</param>
  /// <exception cref="System.ArgumentNullException">
  /// parameterPositions
  /// or
  /// expressionBuilder
  /// </exception>
  public BaseOperatorDefinition(string name,
    [RegexPattern] string regex,
    IReadOnlyList<RelativePosition> parameterPositions,
    ExpressionBuilder expressionBuilder)
    : this(name, regex, null, parameterPositions, expressionBuilder) {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="BaseOperatorDefinition"/> class.
  /// </summary>
  /// <param name="name">The name of the definition.</param>
  /// <param name="regex">The regex to match tokens.</param>
  /// <param name="parameterPositions">The relative positions where parameters can be found.</param>
  /// <param name="expressionBuilder">The function given zero or more operands expressions, outputs a new operand.</param>
  /// <exception cref="System.ArgumentNullException">
  /// parameterPositions
  /// or
  /// expressionBuilder
  /// </exception>
  public BaseOperatorDefinition(string name,
    [RegexPattern] string regex,
    IReadOnlyList<RelativePosition> parameterPositions,
    Func<IReadOnlyList<Expression>, Expression> expressionBuilder)
    : this(name, regex, null, parameterPositions,
      expressionBuilder is not null
        ? (param, _, _) => expressionBuilder(param)
        : throw new ArgumentNullException(nameof(expressionBuilder))) {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="BaseOperatorDefinition"/> class.
  /// </summary>
  /// <param name="name">The name of the definition.</param>
  /// <param name="regex">The regex to match tokens.</param>
  /// <param name="orderOfPrecedence">The relative order this operator should be applied. Lower orders are applied first.</param>
  /// <param name="parameterPositions">The relative positions where parameters can be found.</param>
  /// <param name="expressionBuilder">The function given zero or more operands expressions, outputs a new operand.</param>
  /// <exception cref="System.ArgumentNullException">
  /// parameterPositions
  /// or
  /// expressionBuilder
  /// </exception>
  public BaseOperatorDefinition(string name,
    [RegexPattern] string regex,
    int? orderOfPrecedence,
    IReadOnlyList<RelativePosition>? parameterPositions,
    ExpressionBuilder expressionBuilder)
    : base(name, regex) {
    ParameterPositions = parameterPositions ?? throw new ArgumentNullException(nameof(parameterPositions));
    ExpressionBuilder = expressionBuilder ?? throw new ArgumentNullException(nameof(expressionBuilder));
    OrderOfPrecedence = orderOfPrecedence;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="BaseOperatorDefinition"/> class.
  /// </summary>
  /// <param name="name">The name of the definition.</param>
  /// <param name="regex">The regex to match tokens.</param>
  /// <param name="orderOfPrecedence">The relative order this operator should be applied. Lower orders are applied first.</param>
  /// <param name="parameterPositions">The relative positions where parameters can be found.</param>
  /// <param name="expressionBuilder">The function given zero or more operands expressions, outputs a new operand.</param>
  /// <exception cref="System.ArgumentNullException">
  /// parameterPositions
  /// or
  /// expressionBuilder
  /// </exception>
  public BaseOperatorDefinition(string name,
    [RegexPattern] string regex,
    int? orderOfPrecedence,
    IReadOnlyList<RelativePosition>? parameterPositions,
    Func<IReadOnlyList<Expression>, Expression> expressionBuilder)
    : this(name, regex, orderOfPrecedence, parameterPositions,
      expressionBuilder is not null
        ? (param, _, _) => expressionBuilder(param)
        : throw new ArgumentNullException(nameof(expressionBuilder))) {
  }

  /// <summary>
  /// Applies the token to the parsing state. Adds an operator to the state, when executed the operator will
  /// check it has enough operands and they are in the correct position. It will then execute the expressionBuilder
  /// placing the result in the state.
  /// </summary>
  /// <param name="token">The token to apply.</param>
  /// <param name="state">The state to apply the token to.</param>
  public override void Apply(Token token, ParseState state) {
    // Apply previous operators if they have a high precedence and they share an operand
    var anyLeftOperators = ParameterPositions.Any(x => x == RelativePosition.Left);
    while (state.Operators.Count > 0 && OrderOfPrecedence is not null && anyLeftOperators) {
      var prevOperator = state.Operators.Peek().Definition as BaseOperatorDefinition;
      var prevOperatorPrecedence = prevOperator?.OrderOfPrecedence;
      if (prevOperatorPrecedence <= OrderOfPrecedence
          && prevOperator!.ParameterPositions.Any(x => x == RelativePosition.Right))
        state.Operators.Pop().Execute();
      else
        break;
    }

    void ApplyOperator() {
      // Pop all our right arguments, and check there is the correct number and they are all to the right
      var rightArgs = new Stack<Operand>(state.Operands.PopWhile(x => x.SourceMap.IsRightOf(token.SourceMap)));
      var expectedRightArgs = ParameterPositions.Count(x => x == RelativePosition.Right);
      if (expectedRightArgs > 0 && rightArgs.Count > expectedRightArgs) {
        var spanWhereOperatorExpected = Substring.Encompass(rightArgs.Reverse()
          .Take(rightArgs.Count - expectedRightArgs)
          .Select(x => x.SourceMap));
        throw new OperandUnexpectedException(token.SourceMap, spanWhereOperatorExpected);
      }

      if (rightArgs.Count < expectedRightArgs)
        throw new OperandExpectedException(token.SourceMap, new(token.SourceMap.Source, token.SourceMap.End, 0));

      // Pop all our left arguments, and check they are not to the left of the next operator
      var nextOperatorEndIndex = state.Operators.Count == 0 ? 0 : state.Operators.Peek().SourceMap.End;
      var expectedLeftArgs = ParameterPositions.Count(x => x == RelativePosition.Left);
      var leftArgs = new Stack<Operand>(state.Operands.PopWhile((x, i) => i < expectedLeftArgs && x.SourceMap.IsRightOf(nextOperatorEndIndex)));
      if (leftArgs.Count < expectedLeftArgs)
        throw new OperandExpectedException(token.SourceMap, new(token.SourceMap.Source, token.SourceMap.Start, 0));

      // Map the operators into the correct argument positions
      var args = new List<Operand>();
      foreach (var paramPos in ParameterPositions) {
        var operand = paramPos == RelativePosition.Right
          ? rightArgs.Pop()
          : leftArgs.Pop();
        args.Add(operand);
      }

      // our new source map will encompass this operator and all its operands
      var tokens = args.Select(x => x.SourceMap).Prepend(token.SourceMap).ToArray();
      var sourceMapSpan = Substring.Encompass(tokens);

      Expression expression;
      try {
        expression = ExpressionBuilder(
          args.Select(x => x.Expression).ToArray(),
          state.Parameters,
          tokens
        );
      }
      catch (Exception ex) {
        throw new OperationInvalidException(sourceMapSpan, ex);
      }

      if (expression is null)
        throw new OperationInvalidException(sourceMapSpan);

      state.Operands.Push(new(expression, sourceMapSpan));
    }

    state.Operators.Push(new(this, token.SourceMap, ApplyOperator));
  }

}