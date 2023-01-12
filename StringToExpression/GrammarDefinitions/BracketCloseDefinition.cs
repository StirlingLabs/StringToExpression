using JetBrains.Annotations;

namespace StringToExpression.GrammarDefinitions;

/// <summary>
/// Represents a closing bracket.
/// </summary>
/// <seealso cref="BaseGrammarDefinition" />
public sealed class BracketCloseDefinition : BaseGrammarDefinition {

  /// <summary>
  /// The definitions that can be considered as the matching opening bracket.
  /// </summary>
  public readonly IReadOnlyCollection<BracketOpenDefinition> BracketOpenDefinitions;

  /// <summary>
  /// The definition for the delimiter for a list of items.
  /// </summary>
  public readonly BaseGrammarDefinition? ListDelimiterDefinition;

  /// <summary>
  /// Initializes a new instance of the <see cref="BracketCloseDefinition"/> class.
  /// </summary>
  /// <param name="name">The name of the definition.</param>
  /// <param name="regex">The regex to match tokens.</param>
  /// <param name="bracketOpenDefinitions">The definitions that can be considered as the matching opening bracket.</param>
  /// <param name="listDelimiterDefinition">The definition for the delimiter for a list of items.</param>
  /// <exception cref="System.ArgumentNullException">bracketOpenDefinitions</exception>
  public BracketCloseDefinition(string name, [RegexPattern] string regex,
    IReadOnlyList<BracketOpenDefinition> bracketOpenDefinitions,
    BaseGrammarDefinition? listDelimiterDefinition = null)
    : base(name, regex) {
    BracketOpenDefinitions = bracketOpenDefinitions ?? throw new ArgumentNullException(nameof(bracketOpenDefinitions));
    ListDelimiterDefinition = listDelimiterDefinition;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="BracketCloseDefinition"/> class.
  /// </summary>
  /// <param name="name">The name of the definition.</param>
  /// <param name="regex">The regex to match tokens.</param>
  /// <param name="bracketOpenDefinition">The definition that can be considered as the matching opening bracket.</param>
  /// <param name="listDelimiterDefinition">The definition for the delimiter for a list of items.</param>
  public BracketCloseDefinition(string name, [RegexPattern] string regex,
    BracketOpenDefinition bracketOpenDefinition,
    BaseGrammarDefinition? listDelimiterDefinition = null)
    : this(name, regex, new[] { bracketOpenDefinition }, listDelimiterDefinition) {
  }

  /// <summary>
  /// Applies the token to the parsing state. Will pop the operator stack executing all the operators storing each of the operands
  /// When we reach an opening bracket it will pass the stored operands to the opening bracket to be processed.
  /// </summary>
  /// <param name="token">The token to apply.</param>
  /// <param name="state">The state to apply the token to.</param>
  /// <exception cref="OperandExpectedException">When there are delimiters but no operands between them.</exception>
  /// <exception cref="BracketUnmatchedException">When there was no matching closing bracket.</exception>
  public override void Apply(Token token, ParseState state) {
    var bracketOperands = new Stack<Operand>();
    var previousSeparator = token.SourceMap;
    var hasSeparators = false;

    while (state.Operators.Count > 0) {
      if (!PerOperator(token, state, bracketOperands, ref previousSeparator, ref hasSeparators))
        return;
    }

    // We have popped through all the operators and not found an open bracket
    throw new BracketUnmatchedException(token.SourceMap);
  }

  /// <summary>
  /// Executes a operator on the stack of operands.
  /// </summary>
  /// <param name="token">The current token that is being parsed.</param>
  /// <param name="state">The current state of the parse</param>
  /// <param name="bracketOperands">A stack of operands that are between a open and close bracket</param>
  /// <param name="previousSeparator">the location of the previous list delimiter</param>
  /// <param name="hasSeparators">Indicates whether the current list of operands has a list delimiter</param>
  /// <returns>True if the operator was executed, false otherwise</returns>
  private bool PerOperator(Token token, ParseState state, Stack<Operand> bracketOperands, ref Substring previousSeparator, ref bool hasSeparators) {
    var currentOperator = state.Operators.Pop();
    if (BracketOpenDefinitions.Contains(currentOperator.Definition)) {
      var operand = state.Operands.Count > 0 ? state.Operands.Peek() : null;
      var firstSegment = currentOperator.SourceMap;
      if (operand is not null && operand.SourceMap.IsBetween(firstSegment, previousSeparator))
        bracketOperands.Push(state.Operands.Pop());
      else if (hasSeparators && (operand is null || !operand.SourceMap.IsBetween(firstSegment, previousSeparator)))
        // if we have separators then we should have something between the last separator and the open bracket.
        throw new OperandExpectedException(Substring.Between(firstSegment, previousSeparator));

      // pass our all bracket operands to the open bracket method, he will know
      // what we should do.
      var closeBracketOperator = new Operator(this, token.SourceMap, () => { });
      ((BracketOpenDefinition)currentOperator.Definition).ApplyBracketOperands(
        currentOperator,
        bracketOperands,
        closeBracketOperator,
        state);
      return false;
    }

    if (ListDelimiterDefinition is not null && currentOperator.Definition == ListDelimiterDefinition) {
      hasSeparators = true;
      var operand = state.Operands.Pop();

      // if our operator is not between two delimiters, then we are missing an operator
      var firstSegment = currentOperator.SourceMap;
      var secondSegment = previousSeparator;
      if (!operand.SourceMap.IsBetween(firstSegment, secondSegment))
        throw new OperandExpectedException(Substring.Between(firstSegment, secondSegment));

      bracketOperands.Push(operand);
      previousSeparator = currentOperator.SourceMap;
    }
    else
      // regular operator, execute it
      currentOperator.Execute();

    return true;
  }

}