namespace StringToExpression.GrammarDefinitions;

public sealed class GrammarDefinition : BaseGrammarDefinition {

  public GrammarDefinition(string name, string regex, bool ignore = false)
    : base(name, regex, ignore) { }

}