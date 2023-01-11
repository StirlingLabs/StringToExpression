using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace StringToExpression;

/// <summary>
/// Extensions methods for <see cref="StringToExpression.Substring"/>.
/// </summary>
[PublicAPI]
public static class SubstringExtensions {

  /// <summary>
  /// Retrieves a substring from this substring.
  /// The substring starts at a specified character position and has the remaining length.
  /// </summary>
  /// <param name="start">The relative starting character position of a substring.</param>
  /// <returns>
  /// A substring that is the remaining characters after
  /// <paramref name="start"/> characters into this substring.
  /// </returns>
  /// <exception cref="ArgumentOutOfRangeException">Start is out of range.</exception>
  /// <seealso cref="StringToExpression.Substring.Slice(int)"/>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Substring Substring(this in Substring s, int start)
    => s.Slice(start);

  /// <summary>
  /// Retrieves a substring from this substring.
  /// The substring starts at a specified character position and has a specified length.
  /// </summary>
  /// <param name="start">The relative starting character position of a substring.</param>
  /// <param name="length">The number of characters in the substring.</param>
  /// <returns>
  /// A substring that is <see cref="length"/> characters that begins at
  /// <paramref name="start"/> characters into this substring.
  /// </returns>
  /// <exception cref="ArgumentOutOfRangeException">Start and/or length are out of range.</exception>
  /// <seealso cref="StringToExpression.Substring.Slice(int,int)"/>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Substring Substring(this in Substring s, int start, int length)
    => s.Slice(start, length);

}