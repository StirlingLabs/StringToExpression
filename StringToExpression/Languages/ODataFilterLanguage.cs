using System.Diagnostics.CodeAnalysis;
using StringToExpression.GrammarDefinitions;
using System.Globalization;
using FastExpressionCompiler.LightExpression;
using System.Reflection;
using JetBrains.Annotations;

namespace StringToExpression.LanguageDefinitions;

/// <summary>
/// Provides the base class for parsing OData filter parameters.
/// </summary>
[PublicAPI]
public class ODataFilterLanguage {

  /// <summary>
  /// Access to common String Members
  /// </summary>
  protected static class StringMembers {

    /// <summary>
    /// The MethodInfo for the StartsWith method
    /// </summary>
    public static readonly MethodInfo StartsWith = ReflectionUtil<string>.Method(x => x.StartsWith(""));

    /// <summary>
    /// The MethodInfo for the EndsWith method
    /// </summary>
    public static readonly MethodInfo EndsWith = ReflectionUtil<string>.Method(x => x.EndsWith(""));

    /// <summary>
    /// The MethodInfo for the Contains method
    /// </summary>
    public static readonly MethodInfo Contains = ReflectionUtil<string>.Method(x => x.Contains(""));

    /// <summary>
    /// The MethodInfo for the ToLower method
    /// </summary>
    public static readonly MethodInfo ToLower = ReflectionUtil<string>.Method(x => x.ToLower());

    /// <summary>
    /// The MethodInfo for the ToUpper method
    /// </summary>
    public static readonly MethodInfo ToUpper = ReflectionUtil<string>.Method(x => x.ToUpper());

  }

  /// <summary>
  /// Access to common DateTime Members
  /// </summary>
  protected static class DateTimeMembers {

    /// <summary>
    /// The MemberInfo for the Year property
    /// </summary>
    public static readonly MemberInfo Year = ReflectionUtil<DateTime>.Member(x => x.Year);

    /// <summary>
    /// The MemberInfo for the Month property
    /// </summary>
    public static readonly MemberInfo Month = ReflectionUtil<DateTime>.Member(x => x.Month);

    /// <summary>
    /// The MemberInfo for the Day property
    /// </summary>
    public static readonly MemberInfo Day = ReflectionUtil<DateTime>.Member(x => x.Day);

    /// <summary>
    /// The MemberInfo for the Hour property
    /// </summary>
    public static readonly MemberInfo Hour = ReflectionUtil<DateTime>.Member(x => x.Hour);

    /// <summary>
    /// The MemberInfo for the Minute property
    /// </summary>
    public static readonly MemberInfo Minute = ReflectionUtil<DateTime>.Member(x => x.Minute);

    /// <summary>
    /// The MemberInfo for the Second property
    /// </summary>
    public static readonly MemberInfo Second = ReflectionUtil<DateTime>.Member(x => x.Second);

  }

  private readonly Language _language;

  /// <summary>
  /// Initializes a new instance of the <see cref="ODataFilterLanguage"/> class.
  /// </summary>
  [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
  public ODataFilterLanguage()
    => _language = new(AllDefinitions().ToArray());

  /// <summary>
  /// Parses the specified text converting it into a predicate expression
  /// </summary>
  /// <typeparam name="T">The input type</typeparam>
  /// <param name="text">The text to parse.</param>
  /// <returns></returns>
  public Expression<Func<T, bool>> Parse<T>(string text) {
    var parameters = new[] { Expression.Parameter(typeof(T)) };
    var body = _language.Parse(text, parameters);

    ExpressionConversions.TryBoolean(ref body);

    return Expression.Lambda<Func<T, bool>>(body, parameters);
  }

  /// <summary>
  /// Returns all the definitions used by the language.
  /// </summary>
  /// <returns></returns>
  protected virtual IEnumerable<BaseGrammarDefinition> AllDefinitions() {
    IEnumerable<FunctionCallDefinition> functions;
    var definitions = new List<BaseGrammarDefinition>();
    definitions.AddRange(TypeDefinitions());
    definitions.AddRange(functions = FunctionDefinitions());
    definitions.AddRange(BracketDefinitions(functions));
    definitions.AddRange(LogicalOperatorDefinitions());
    definitions.AddRange(ArithmeticOperatorDefinitions());
    definitions.AddRange(PropertyDefinitions());
    definitions.AddRange(WhitespaceDefinitions());
    return definitions;
  }

  /// <summary>
  /// Returns the definitions for types used within the language.
  /// </summary>
  /// <returns></returns>
  [SuppressMessage("ReSharper", "StringLiteralTypo")]
  protected virtual IEnumerable<BaseGrammarDefinition> TypeDefinitions()
    => new[] {
      new OperandDefinition(
        "GUID",
        @"guid'[0-9A-Fa-f]{8}\-[0-9A-Fa-f]{4}\-[0-9A-Fa-f]{4}\-[0-9A-Fa-f]{4}\-[0-9A-Fa-f]{12}'",
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        x => Expression.Constant(Guid.Parse(x.Slice("guid".Length).Trim('\'')))),
#else
        x => Expression.Constant(Guid.Parse(x.Slice("guid".Length).Trim('\'').ToString()!))),
#endif

      new OperandDefinition(
        "STRING",
        @"'(?:\\.|[^'])*'",
        x => Expression.Constant(x.Trim('\'').ToString()!
          .Replace("\\'", "'")
          .Replace("\\r", "\r")
          .Replace("\\f", "\f")
          .Replace("\\n", "\n")
          .Replace("\\\\", "\\")
          .Replace("\\b", "\b")
          .Replace("\\t", "\t"))),
      new OperandDefinition(
        "BYTE",
        @"0x[0-9A-Fa-f]{1,2}",
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        x => Expression.Constant(byte.Parse(x.Slice("0x".Length)!, NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier))),
#else
        x => Expression.Constant(byte.Parse(x.Slice("0x".Length).ToString()!, NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier))),
#endif
      new OperandDefinition(
        "NULL",
        @"null",
        _ => Expression.Constant(null)),
      new OperandDefinition(
        "BOOL",
        @"true|false",
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        x => Expression.Constant(bool.Parse(x))),
#else
        x => Expression.Constant(bool.Parse(x.ToString()!))),
#endif
      new OperandDefinition(
        "DATETIME",
        @"[Dd][Aa][Tt][Ee][Tt][Ii][Mm][Ee]'[^']+'",
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        x => Expression.Constant(DateTime.Parse(x.Slice("datetime".Length).Trim('\'')))),
#else
        x => Expression.Constant(DateTime.Parse(x.Slice("datetime".Length).Trim('\'').ToString()!))),
#endif
      new OperandDefinition(
        "DATETIMEOFFSET",
        @"datetimeoffset'[^']+'",
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        x => Expression.Constant(DateTimeOffset.Parse(x.Slice("datetimeoffset".Length).Trim('\'')))),
#else
        x => Expression.Constant(DateTimeOffset.Parse(x.Slice("datetimeoffset".Length).Trim('\'').ToString()!))),
#endif

      new OperandDefinition(
        "FLOAT",
        @"\-?\d+?\.\d*f",
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        x => Expression.Constant(float.Parse(x.TrimEnd('f')))),
#else
        x => Expression.Constant(float.Parse(x.TrimEnd('f').ToString()!))),
#endif
      new OperandDefinition(
        "DOUBLE",
        @"\-?\d+\.?\d*d",
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        x => Expression.Constant(double.Parse(x.TrimEnd('d')))),
#else
        x => Expression.Constant(double.Parse(x.TrimEnd('d').ToString()!))),
#endif
      new OperandDefinition(
        "DECIMAL_EXPLICIT",
        @"\-?\d+\.?\d*[m|M]",
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        x => Expression.Constant(decimal.Parse(x.TrimEnd('m', 'M')))),
#else
        x => Expression.Constant(decimal.Parse(x.TrimEnd('m', 'M').ToString()!))),
#endif
      new OperandDefinition(
        "DECIMAL",
        @"\-?\d+\.\d+",
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        x => Expression.Constant(double.Parse(x))),
#else
        x => Expression.Constant(double.Parse(x.ToString()!))),
#endif

      new OperandDefinition(
        "LONG",
        @"\-?\d+L",
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        x => Expression.Constant(long.Parse(x.TrimEnd('L')))),
#else
        x => Expression.Constant(long.Parse(x.TrimEnd('L').ToString()!))),
#endif
      new OperandDefinition(
        "INTEGER",
        @"\-?\d+",
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        x => Expression.Constant(int.Parse(x))),
#else
        x => Expression.Constant(int.Parse(x.ToString()!))),
#endif
    };

  /// <summary>
  /// Returns the definitions for logic operators used within the language.
  /// </summary>
  /// <returns></returns>
  protected virtual IEnumerable<BaseGrammarDefinition> LogicalOperatorDefinitions()
    => new BaseGrammarDefinition[] {
      new BinaryOperatorDefinition(
        "EQ",
        @"\b(eq)\b",
        11,
        ConvertEnumsIfRequired((left, right) => {
          _ = System.Linq.Expressions.Expression.Equal(left.ToExpression(), right.ToExpression());
          return Expression.Equal(left, right);
        })),
      new BinaryOperatorDefinition(
        "NE",
        @"\b(ne)\b",
        12,
        ConvertEnumsIfRequired(Expression.NotEqual)),

      new BinaryOperatorDefinition(
        "GT",
        @"\b(gt)\b",
        13,
        Expression.GreaterThan),
      new BinaryOperatorDefinition(
        "GE",
        @"\b(ge)\b",
        14,
        Expression.GreaterThanOrEqual),

      new BinaryOperatorDefinition(
        "LT",
        @"\b(lt)\b",
        15,
        Expression.LessThan),
      new BinaryOperatorDefinition(
        "LE",
        @"\b(le)\b",
        16,
        Expression.LessThanOrEqual),

      new BinaryOperatorDefinition(
        "AND",
        @"\b(and)\b",
        17,
        Expression.And),
      new BinaryOperatorDefinition(
        "OR",
        @"\b(or)\b",
        18,
        Expression.Or),

      new UnaryOperatorDefinition(
        "NOT",
        @"\b(not)\b",
        19,
        RelativePosition.Right,
        arg => {
          ExpressionConversions.TryBoolean(ref arg);
          return Expression.Not(arg);
        })
    };

  /// <summary>
  /// Returns the definitions for arithmetic operators used within the language.
  /// </summary>
  /// <returns></returns>
  protected virtual IEnumerable<BaseGrammarDefinition> ArithmeticOperatorDefinitions()
    => new[] {
      new BinaryOperatorDefinition(
        "ADD",
        @"\b(add)\b",
        2,
        Expression.Add),
      new BinaryOperatorDefinition(
        "SUB",
        @"\b(sub)\b",
        2,
        Expression.Subtract),
      new BinaryOperatorDefinition(
        "MUL",
        @"\b(mul)\b",
        1,
        Expression.Multiply),
      new BinaryOperatorDefinition(
        "DIV",
        @"\b(div)\b",
        1,
        Expression.Divide),
      new BinaryOperatorDefinition(
        "MOD",
        @"\b(mod)\b",
        1,
        Expression.Modulo),
    };

  /// <summary>
  /// Returns the definitions for brackets used within the language.
  /// </summary>
  /// <param name="functionCalls">The function calls in the language. (used as opening brackets)</param>
  /// <returns></returns>
  protected virtual IEnumerable<BaseGrammarDefinition> BracketDefinitions(IEnumerable<FunctionCallDefinition> functionCalls) {
    BracketOpenDefinition openBracket;
    ListDelimiterDefinition delimiter;
    return new BaseGrammarDefinition[] {
      openBracket = new(
        "OPEN_BRACKET",
        @"\("),
      delimiter = new(
        "COMMA",
        ","),
      new BracketCloseDefinition(
        "CLOSE_BRACKET",
        @"\)",
        new[] { openBracket }.Concat(functionCalls).ToArray(),
        delimiter)
    };
  }

  /// <summary>
  /// Returns the definitions for functions used within the language.
  /// </summary>
  /// <returns></returns>
  [SuppressMessage("ReSharper", "StringLiteralTypo")]
  protected virtual IEnumerable<FunctionCallDefinition> FunctionDefinitions()
    => new[] {
      new FunctionCallDefinition(
        "FN_STARTSWITH",
        @"startswith\(",
        new[] { typeof(string), typeof(string) },
        parameters => Expression.Call(
          parameters[0],
          StringMembers.StartsWith, parameters[1])),
      new FunctionCallDefinition(
        "FN_ENDSWITH",
        @"endswith\(",
        new[] { typeof(string), typeof(string) },
        parameters => Expression.Call(
          parameters[0],
          StringMembers.EndsWith, parameters[1])),
      new FunctionCallDefinition(
        "FN_SUBSTRINGOF",
        @"substringof\(",
        new[] { typeof(string), typeof(string) },
        parameters => Expression.Call(
          parameters[1],
          StringMembers.Contains, parameters[0])),
      new FunctionCallDefinition(
        "FN_TOLOWER",
        @"tolower\(",
        new[] { typeof(string) },
        parameters => Expression.Call(
          parameters[0],
          StringMembers.ToLower)),
      new FunctionCallDefinition(
        "FN_TOUPPER",
        @"toupper\(",
        new[] { typeof(string) },
        parameters => Expression.Call(
          parameters[0],
          StringMembers.ToUpper)),

      new FunctionCallDefinition(
        "FN_DAY",
        @"day\(",
        new[] { typeof(DateTime) },
        parameters => Expression.MakeMemberAccess(
          parameters[0],
          DateTimeMembers.Day)),
      new FunctionCallDefinition(
        "FN_HOUR",
        @"hour\(",
        new[] { typeof(DateTime) },
        parameters => Expression.MakeMemberAccess(
          parameters[0],
          DateTimeMembers.Hour)),
      new FunctionCallDefinition(
        "FN_MINUTE",
        @"minute\(",
        new[] { typeof(DateTime) },
        parameters => Expression.MakeMemberAccess(
          parameters[0],
          DateTimeMembers.Minute)),
      new FunctionCallDefinition(
        "FN_MONTH",
        @"month\(",
        new[] { typeof(DateTime) },
        parameters => Expression.MakeMemberAccess(
          parameters[0],
          DateTimeMembers.Month)),
      new FunctionCallDefinition(
        "FN_YEAR",
        @"year\(",
        new[] { typeof(DateTime) },
        parameters => Expression.MakeMemberAccess(
          parameters[0],
          DateTimeMembers.Year)),
      new FunctionCallDefinition(
        "FN_SECOND",
        @"second\(",
        new[] { typeof(DateTime) },
        parameters => Expression.MakeMemberAccess(
          parameters[0],
          DateTimeMembers.Second)),
    };

  /// <summary>
  /// Returns the definitions for property names used within the language.
  /// </summary>
  /// <returns></returns>
  protected virtual IEnumerable<BaseGrammarDefinition> PropertyDefinitions()
    => new[] {
      //Properties
      new OperandDefinition(
        "PROPERTY_PATH",
        @"(?<![0-9])([A-Za-z_][A-Za-z0-9_]*/?)+",
        (value, parameters) => {
          return value.Split('/').Aggregate((Expression)parameters[0],
            (exp, prop) => Expression.MakeMemberAccess(exp, TypeShim.GetProperty(exp.Type, prop)));
        }),
    };

  /// <summary>
  /// Returns the definitions for whitespace used within the language.
  /// </summary>
  /// <returns></returns>
  protected virtual IEnumerable<BaseGrammarDefinition> WhitespaceDefinitions()
    => new[] {
      new GrammarDefinition("WHITESPACE", @"\s+", true)
    };

  /// <summary>
  /// Wraps the function to convert any constants to enums if required
  /// </summary>
  /// <param name="expFn">Function to wrap</param>
  /// <returns></returns>
  protected Func<Expression, Expression, Expression> ConvertEnumsIfRequired(Func<Expression, Expression, Expression> expFn)
    => (left, right) => {
      _ = ExpressionConversions.TryEnumNumberConvert(ref left, ref right)
        || ExpressionConversions.TryEnumStringConvert(ref left, ref right, true);

      return expFn(left, right);
    };

}