using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace StringToExpression.GrammarDefinitions;

/// <summary>
/// Represents a single piece of grammar and defines how it behaves within the system.
/// </summary>
public abstract class BaseGrammarDefinition : IEquatable<BaseGrammarDefinition> {

  private static readonly Regex NameValidation = new("^[a-zA-Z0-9_]+$");

  /// <summary>
  /// Name of the definition.
  /// </summary>
  public readonly string Name;

  /// <summary>
  /// Regex to match tokens.
  /// </summary>
  public readonly string Regex;

  /// <summary>
  /// Indicates whether this grammar should be ignored during tokenization.
  /// </summary>
  public readonly bool Ignore;

  /// <summary>
  /// Initializes a new instance of the <see cref="BaseGrammarDefinition" /> class.
  /// </summary>
  /// <param name="name">The name of the definition.</param>
  /// <param name="regex">The regex to match tokens.</param>
  /// <param name="ignore">if set to <c>true</c> will ignore grammar during tokenization.</param>
  /// <exception cref="System.ArgumentNullException">
  /// name
  /// or
  /// regex
  /// </exception>
  /// <exception cref="StringToExpression.GrammarDefinitionInvalidNameException">When the name contains characters other than [a-zA-Z0-9_]</exception>
  public BaseGrammarDefinition(string name, [RegexPattern] string regex, bool ignore = false) {
    if (name is null) throw new ArgumentNullException(nameof(name));
    if (!NameValidation.IsMatch(name))
      throw new GrammarDefinitionInvalidNameException(name);

    Name = name;
    Regex = regex ?? throw new ArgumentNullException(nameof(regex));
    Ignore = ignore;
  }

  /// <summary>
  /// Applies the token to the parsing state.
  /// </summary>
  /// <param name="token">The token to apply.</param>
  /// <param name="state">The state to apply the token to.</param>
  public virtual void Apply(Token token, ParseState state) {
  }

  /// <summary>
  /// Determines whether the specified base grammar definition is equal to the current object
  /// </summary>
  /// <param name="other">The BaseGrammarDefinition to compare with the current object.</param>
  /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
  public bool Equals(BaseGrammarDefinition? other)
    => !ReferenceEquals(null, other)
      && (ReferenceEquals(this, other)
        || Name == other.Name);

  /// <summary>
  /// Determines whether the specified object is equal to the current object
  /// </summary>
  /// <param name="obj">The object to compare with the current object.</param>
  /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
  public override bool Equals(object? obj)
    => !ReferenceEquals(null, obj)
      && (ReferenceEquals(this, obj)
        || obj.GetType() == GetType()
        && Equals((BaseGrammarDefinition)obj));

  /// <summary>
  /// Serves as the default hash function
  /// </summary>
  /// <returns>A hash code for the current object</returns>
  public override int GetHashCode()
    => Name.GetHashCode();

  /// <summary>
  /// Equality operator
  /// </summary>
  /// <param name="left">The left side of the operator</param>
  /// <param name="right">The right side of the operator</param>
  /// <returns>True if the objects are equal, false otherwise</returns>
  public static bool operator ==(BaseGrammarDefinition? left, BaseGrammarDefinition? right)
    => Equals(left, right);

  /// <summary>
  /// Inequality operator
  /// </summary>
  /// <param name="left">The left side of the operator</param>
  /// <param name="right">The right side of the operator</param>
  /// <returns>True if the objects are not equal, false otherwise</returns>
  public static bool operator !=(BaseGrammarDefinition? left, BaseGrammarDefinition? right)
    => !Equals(left, right);

}