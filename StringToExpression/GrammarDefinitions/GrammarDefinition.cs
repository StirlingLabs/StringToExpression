namespace StringToExpression.GrammarDefinitions;

/// <inheritdoc />
public sealed class GrammarDefinition : BaseGrammarDefinition {
    
    /// <inheritdoc />
    public GrammarDefinition(string name, string regex, bool ignore = false)
    : base(name, regex, ignore) { }

}