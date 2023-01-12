using JetBrains.Annotations;

namespace StringToExpression;

/// <summary>
/// Exception when a string can not be parsed as an enumeration
/// </summary>
[PublicAPI]
public class EnumParseException : Exception {

  /// <summary>
  /// The string that was attempted to be parsed.
  /// </summary>
  public readonly Substring StringValue;

  /// <summary>
  /// The enumeration that the string was attempted to be parsed as.
  /// </summary>
  public readonly Type EnumType;

  /// <summary>
  /// Exception thrown when an enum value cannot be parsed from a string.
  /// </summary>
  /// <param name="stringValue">The string value that failed to parse</param>
  /// <param name="enumType">The Enum type that the string value was being parsed as</param>
  /// <param name="ex">An inner exception that may have caused this exception.</param>
  public EnumParseException(Substring stringValue, Type enumType, Exception ex)
    : base($"'{stringValue}' is not a valid value for enum type '{enumType}'", ex) {
    StringValue = stringValue;
    EnumType = enumType;
  }

}