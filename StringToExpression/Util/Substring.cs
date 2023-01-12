using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace StringToExpression;

/// <summary>
/// Represents a part of a string.
/// </summary>
[PublicAPI]
public readonly struct Substring
  : IComparable<string>,
    IEquatable<string>,
    IComparable<Substring>,
    IEquatable<Substring>,
    IEnumerable<char> {

  /// <summary>
  /// The source string.
  /// </summary>
  public readonly string Source;

  /// <summary>
  /// The start position of this segment within the source string.
  /// </summary>
  public readonly int Start;

  /// <summary>
  /// The length of this segment.
  /// </summary>
  public readonly int Length;

  /// <summary>
  /// The end position of this segment within the source string.
  /// </summary>
  public int End {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => Start + Length;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Substring"/> class.
  /// </summary>
  /// <param name="sourceString">The source string.</param>
  /// <exception cref="System.ArgumentNullException">sourceString</exception>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Substring(string sourceString) {
    Source = sourceString ?? throw new ArgumentNullException(nameof(sourceString));
    Start = 0;
    Length = sourceString.Length;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Substring"/> class.
  /// </summary>
  /// <param name="sourceString">The source string.</param>
  /// <param name="start">The start position of this segment within the source string.</param>
  /// <param name="length">The length of this segment.</param>
  /// <exception cref="System.ArgumentNullException">sourceString</exception>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Substring(string sourceString, int start, int length) {
    Source = sourceString ?? throw new ArgumentNullException(nameof(sourceString));
    Start = start;
    Length = length;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Substring"/> class.
  /// </summary>
  /// <param name="sourceString">The source string.</param>
  /// <param name="start">The start position of this segment within the source string.</param>
  /// <exception cref="System.ArgumentNullException">sourceString</exception>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Substring(string sourceString, int start)
    : this(sourceString, start, sourceString.Length - start) {
  }

  /// <summary>
  /// Checks if the length is equal to zero.
  /// </summary>
  public bool IsEmpty {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => Length == 0;
  }

  /// <summary>
  /// Determines if given segment is to the right of this segment.
  /// </summary>
  /// <param name="segment">segment to check.</param>
  /// <returns>
  ///   <c>true</c> if passed segment is to the right of this segment; otherwise, <c>false</c>.
  /// </returns>
  /// <exception cref="System.ArgumentNullException">segment</exception>
  /// <exception cref="System.ArgumentException">segment - when this segment and passed segment have different source strings</exception>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool IsRightOf(Substring segment) {
    if (Source != segment.Source)
      throw new ArgumentException($"{nameof(segment)} must have the same source string", nameof(segment));

    return segment.End <= Start;
  }

  /// <summary>
  /// Determines if index is to the right of this segment.
  /// </summary>
  /// <param name="index">index to check.</param>
  /// <returns>
  ///   <c>true</c> if index is to the right of this segment; otherwise, <c>false</c>.
  /// </returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool IsRightOf(int index)
    => Start >= index;

  /// <summary>
  /// Determines if segment is to the left of this segment.
  /// </summary>
  /// <param name="segment">segment to check</param>
  /// <returns>
  ///   <c>true</c> if passed segment is to the left of this segment; otherwise, <c>false</c>.
  /// </returns>
  /// <exception cref="System.ArgumentNullException">segment</exception>
  /// <exception cref="System.ArgumentException">segment -  when this segment and passed segment have different source strings</exception>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool IsLeftOf(Substring segment) {
    if (Source != segment.Source)
      throw new ArgumentException($"{nameof(segment)} must have the same source string", nameof(segment));

    return segment.Start >= End;
  }

  /// <summary>
  /// Determines if index is to the left of this segment.
  /// </summary>
  /// <param name="index">index to check.</param>
  /// <returns>
  ///   <c>true</c> if index is to the left of this segment; otherwise, <c>false</c>.
  /// </returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool IsLeftOf(int index)
    => End <= index;

  /// <summary>
  /// Create a segment that encompasses all the passed segments
  /// </summary>
  /// <param name="segments">segments to encompass</param>
  /// <returns>segment that enompasses all the passed segments</returns>
  /// <exception cref="System.ArgumentException">
  /// segments - when does not contain at least one item
  /// or
  /// segments - when all segments do not have the same source strings
  /// </exception>
  public static Substring Encompass(IEnumerable<Substring> segments) {
    using var enumerator = segments.GetEnumerator();
    if (!enumerator.MoveNext())
      throw new ArgumentException($"{nameof(segments)} must have at least one item", nameof(segments));

    var sourceString = enumerator.Current.Source;
    var minStart = enumerator.Current.Start;
    var maxEnd = enumerator.Current.End;
    while (enumerator.MoveNext()) {
      if (enumerator.Current.Source != sourceString)
        throw new ArgumentException($"{nameof(segments)} must all have the same source string", nameof(segments));

      minStart = Math.Min(enumerator.Current.Start, minStart);
      maxEnd = Math.Max(enumerator.Current.End, maxEnd);
    }

    return new(sourceString, minStart, maxEnd - minStart);
  }

  /// <summary>
  /// Create a segment that encompasses all the passed segments
  /// </summary>
  /// <param name="segments">segments to encompass</param>
  /// <returns>segment that enompasses all the passed segments</returns>
  /// <exception cref="System.ArgumentException">
  /// segments - when does not contain at least one item
  /// or
  /// segments - when all segments do not have the same source strings
  /// </exception>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Substring Encompass(params Substring[] segments)
    => Encompass((IEnumerable<Substring>)segments);

  /// <summary>
  /// Determines if this segment is between (and not within) the two passed segments.
  /// </summary>
  /// <param name="segment1">The first segment.</param>
  /// <param name="segment2">The second segment.</param>
  /// <returns>
  ///   <c>true</c> if this segment is between the two passed segments; otherwise, <c>false</c>.
  /// </returns>
  /// <exception cref="System.ArgumentException">
  /// segment1 - when segment does not have the same source string
  /// or
  /// segment2 - when segment does not have the same source string
  /// </exception>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool IsBetween(Substring segment1, Substring segment2) {
    if (Source != segment1.Source)
      throw new ArgumentException($"{nameof(segment1)} must have the same source string", nameof(segment1));
    if (Source != segment2.Source)
      throw new ArgumentException($"{nameof(segment2)} must have the same source string", nameof(segment2));

    return segment1.End <= Start && segment2.Start >= End;
  }

  /// <summary>
  /// Creates a segment which fits between (and not within) the two passed segments.
  /// </summary>
  /// <param name="segment1">The first segment.</param>
  /// <param name="segment2">The second segment.</param>
  /// <returns>A Substring which is between the two passed segments</returns>
  /// <exception cref="System.ArgumentException">when the two segments do not have the same source string</exception>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Substring Between(Substring segment1, Substring segment2) {
    if (segment1.Source != segment2.Source)
      throw new ArgumentException($"{nameof(segment1)} and {nameof(segment2)} must the same source string");

    return new(
      segment1.Source,
      segment1.End,
      segment2.Start - segment1.End);
  }

  /// <inheritdoc cref="IComparable{T}.CompareTo"/>
  public int CompareTo(string? other)
    => Source is not null
      ? other switch {
        null => 1,
        "" => Length == 0 ? 0 : 1,
        _ => string.Compare(
          Source, Start,
          other, 0,
          Math.Max(Length, other.Length))
      }
      : other is null
        ? 0
        : -1;

  /// <inheritdoc cref="IEquatable{T}.Equals(T?)"/>
  public bool Equals(string? other)
    => CompareTo(other) == 0;

  /// <inheritdoc cref="IComparable{T}.CompareTo"/>
  [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
  public int CompareTo(Substring other)
    => Source is null
      ? other.Source is null
        ? 0
        : -1
      : other.Source is null
        ? 1
        : Length == 0
          ? other.Length == 0
            ? 0
            : 1
          : string.Compare(
            Source, Start,
            other.Source, other.Start,
            Math.Max(Length, other.Length));

  /// <inheritdoc cref="IEquatable{T}.Equals(T?)"/>
  public bool Equals(Substring other)
    => CompareTo(other) == 0;

  /// <inheritdoc cref="IEnumerable{T}.GetEnumerator()"/>
  public ReadOnlySpan<char>.Enumerator GetEnumerator()
    => ((ReadOnlySpan<char>)this).GetEnumerator();

  /// <inheritdoc cref="IEnumerable{T}.GetEnumerator()"/>
  IEnumerator<char> IEnumerable<char>.GetEnumerator() {
    if (Source is null) yield break;

    for (var i = Start; i < Length; ++i)
      yield return Source[i];
  }

  /// <inheritdoc cref="IEnumerable.GetEnumerator()"/>
  IEnumerator IEnumerable.GetEnumerator()
    => ((IEnumerable<char>)this).GetEnumerator();

  /// <summary>
  /// Returns a <see cref="System.String" /> that represents this instance.
  /// </summary>
  /// <returns>
  /// A <see cref="System.String" /> that represents this instance.
  /// </returns>
  public override string ToString()
    => Source?.Substring(Start, Length) ?? null!;

  /// <summary>
  /// Invokes <see cref="ToString"/> to convert to a string.
  /// </summary>
  public static explicit operator string(Substring substring)
    => substring.ToString();

  /// <summary>
  /// Invokes the <see cref="Substring(string)"/> constructor.
  /// </summary>
  public static implicit operator Substring(string str)
    => new(str);

  /// <summary>
  /// Presents the <see cref="Substring"/> as a <see cref="ReadOnlySpan{T}"/>.
  /// </summary>
  public static implicit operator ReadOnlySpan<char>(Substring substring)
    => substring.Source.AsSpan(substring.Start, substring.Length);

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
  public Substring Slice(int start) {
    if ((uint)start > (uint)Length)
      throw new ArgumentOutOfRangeException();

    return new(Source, Start + start, End - start);
  }

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
  /// <seealso cref="SubstringExtensions" />
  public Substring Slice(int start, int length) {
    if ((uint)start > (uint)Length || (uint)length > (uint)(Length - start))
      throw new ArgumentOutOfRangeException();

    return new(Source, Start + start, length);
  }

  /// <summary>
  /// Reports the zero-based index of the first occurrence of a specified character within this substring.
  /// The method returns -1 if the character is not found in this substring.
  /// </summary>
  /// <param name="ch">A character to find the index of.</param>
  /// <returns>
  /// An index relative to the <see cref="Start"/> offset into <see cref="Source"/>.
  /// </returns>
  /// <seealso cref="SubstringExtensions" />
  public int IndexOf(char ch) {
    var index = Source.IndexOf(ch, Start, Length);
    return index < 0 ? index : index - Start;
  }

  /// <summary>
  /// Reports the zero-based index of the first occurrence of one of a set of characters
  /// within this substring. The method returns -1 if none of the characters are found in
  /// this substring.
  /// </summary>
  /// <param name="ch">A set of characters to find the index of.</param>
  /// <returns>
  /// An index relative to the <see cref="Start"/> offset into <see cref="Source"/>.
  /// </returns>
  public int IndexOfAny(params char[] ch) {
    var index = Source.IndexOfAny(ch, Start, Length);
    return index < 0 ? index : index - Start;
  }

  /// <summary>
  /// Enumerates substrings within this substring that were
  /// separated by the character <paramref name="sep"/>.
  /// </summary>
  /// <param name="sep">A separator character.</param>
  /// <param name="opts">Whether to trim substrings and include empty substrings.</param>
  /// <returns>Separated substrings.</returns>
  public IEnumerable<Substring> Split(char sep, StringSplitOptions opts = StringSplitOptions.None) {
    var source = Source;
    var remaining = this;
    for (;;) {
      var index = source.IndexOf(sep, remaining.Start, remaining.Length);
      if (index < 0) {
        if (opts != StringSplitOptions.None && remaining.Length == 0)
          yield break;

        yield return remaining;

        yield break;
      }

      var slice = new Substring(source, remaining.Start, index - remaining.Start);
      if ((opts & StringSplitOptions.RemoveEmptyEntries) != 0 && remaining.Length == 0)
        yield break;

      yield return slice;

      var nextStart = index + 1;
      remaining = new(source, nextStart, remaining.End - nextStart);
    }
  }

  /// <summary>
  /// Enumerates substrings within this substring that were
  /// separated by one of the characters in <paramref name="seps"/>.
  /// </summary>
  /// <param name="seps">The separator characters.</param>
  /// <param name="opts">Whether to trim substrings and include empty substrings.</param>
  /// <returns>
  /// An enumeration of the substrings as delimited by the separator characters.
  /// </returns>
  public IEnumerable<Substring> Split(char[] seps, StringSplitOptions opts = StringSplitOptions.None) {
    var source = Source;
    var remaining = this;
    for (;;) {
      var index = source.IndexOfAny(seps, remaining.Start, remaining.Length);
      if (index < 0) {
        if (opts != StringSplitOptions.None && remaining.Length == 0)
          yield break;

        yield return remaining;

        yield break;
      }

      var slice = new Substring(source, remaining.Start, index - remaining.Start);
      if ((opts & StringSplitOptions.RemoveEmptyEntries) != 0 && remaining.Length == 0)
        yield break;

      yield return slice;

      var nextStart = index + 1;
      remaining = new(source, nextStart, remaining.End - nextStart);
    }
  }

  /// <summary>
  /// Enumerates substrings within this substring that were
  /// separated by one of the characters in <paramref name="seps"/>.
  /// </summary>
  /// <param name="seps">The separator characters.</param>
  /// <param name="opts">Whether to trim substrings and include empty substrings.</param>
  /// <returns>
  /// An enumeration of the substrings as delimited by the separator characters.
  /// </returns>
  public unsafe IEnumerable<Substring> Split(ReadOnlySpan<char> seps, StringSplitOptions opts = StringSplitOptions.None) {
    var source = Source;
    var remaining = this;
    var sepsLength = seps.Length;
    fixed (char* pSeps = seps) {
      var npSeps = (nint)pSeps;

      static int GetIndexOfAnySeparator(in Substring source, in Substring remaining, nint pSeps, int sepsLength) {
        ReadOnlySpan<char> span = source;
        span = span.Slice(remaining.Start, remaining.Length);

        var index = span.IndexOfAny(new((char*)pSeps, sepsLength));
        if (index < 0) return index;

        return remaining.Start + index;
      }

      IEnumerable<Substring> Splitter() {
        for (;;) {
          var index = GetIndexOfAnySeparator(source, remaining, npSeps, sepsLength);
          if (index < 0) {
            if (opts != StringSplitOptions.None && remaining.Length == 0)
              yield break;

            yield return remaining;

            yield break;
          }

          var slice = new Substring(source, remaining.Start, index - remaining.Start);
          if ((opts & StringSplitOptions.RemoveEmptyEntries) != 0 && remaining.Length == 0)
            yield break;

          yield return slice;

          var nextStart = index + 1;
          remaining = new(source, nextStart, remaining.End - nextStart);
        }
      }

      return Splitter();
    }
  }

  /// <summary>
  /// Removes the leading occurrences of a specified character from the current substring.
  /// </summary>
  /// <param name="c">The character to trim.</param>
  /// <returns>
  /// The substring that remains after all occurrences of the character <paramref name="c"/>
  /// are removed from the start of the current string.
  /// </returns>
  public Substring TrimStart(char c) {
    var end = End;
    var start = Start;
    while (Source[start] == c && start < end)
      ++start;
    return new(Source, start, end - start);
  }

  /// <summary>
  /// Removes the leading occurrences of a set of characters from the current substring.
  /// </summary>
  /// <param name="c">The characters to trim.</param>
  /// <returns>
  /// The substring that remains after all occurrences of the characters <paramref name="c"/>
  /// are removed from the start of the current string.
  /// </returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Substring TrimStart(params char[] c)
    => TrimStart((IReadOnlyList<char>)c);

  /// <summary>
  /// Removes the leading occurrences of a set of characters from the current substring.
  /// </summary>
  /// <param name="c">The characters to trim.</param>
  /// <returns>
  /// The substring that remains after all occurrences of the characters <paramref name="c"/>
  /// are removed from the start of the current string.
  /// </returns>
  public Substring TrimStart(IReadOnlyList<char> c) {
    if (c is null) throw ArgumentNullException();

    var end = End;
    var start = Start;
    while (c.Contains(Source[start]) && start < end)
      ++start;
    return new(Source, start, end - start);
  }

  /// <summary>
  /// Removes the leading occurrences of a set of characters from the current substring.
  /// </summary>
  /// <param name="c">The characters to trim.</param>
  /// <returns>
  /// The substring that remains after all occurrences of the characters <paramref name="c"/>
  /// are removed from the start of the current string.
  /// </returns>
  public Substring TrimStart(ISet<char> c) {
    if (c is null) throw ArgumentNullException();

    var end = End;
    var start = Start;
    while (c.Contains(Source[start]) && start < end)
      ++start;
    return new(Source, start, end - start);
  }

  /// <summary>
  /// Removes the trailing occurrences of a specified character from the current substring.
  /// </summary>
  /// <param name="c">The character to trim.</param>
  /// <returns>
  /// The substring that remains after all occurrences of the character <paramref name="c"/>
  /// are removed from the end of the current string.
  /// </returns>
  public Substring TrimEnd(char c) {
    var end = End;
    var start = Start;
    while (Source[end - 1] == c && end >= start)
      --end;
    return new(Source, start, end - start);
  }

  /// <summary>
  /// Removes the trailing occurrences of a set of characters from the current substring.
  /// </summary>
  /// <param name="c">The characters to trim.</param>
  /// <returns>
  /// The substring that remains after all occurrences of the characters <paramref name="c"/>
  /// are removed from the end of the current string.
  /// </returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Substring TrimEnd(params char[] c)
    => TrimEnd((IReadOnlyList<char>)c);

  /// <summary>
  /// Removes the trailing occurrences of a set of characters from the current substring.
  /// </summary>
  /// <param name="c">The characters to trim.</param>
  /// <returns>
  /// The substring that remains after all occurrences of the characters <paramref name="c"/>
  /// are removed from the end of the current string.
  /// </returns>
  public Substring TrimEnd(IReadOnlyList<char> c) {
    if (c is null) throw ArgumentNullException();

    var end = End;
    var start = Start;
    while (c.Contains(Source[end - 1]) && end >= start)
      --end;
    return new(Source, start, end - start);
  }

  /// <summary>
  /// Removes the trailing occurrences of a set of characters from the current substring.
  /// </summary>
  /// <param name="c">The characters to trim.</param>
  /// <returns>
  /// The substring that remains after all occurrences of the characters <paramref name="c"/>
  /// are removed from the end of the current string.
  /// </returns>
  public Substring TrimEnd(ISet<char> c) {
    if (c is null) throw ArgumentNullException();

    var end = End;
    var start = Start;
    while (c.Contains(Source[end - 1]) && end >= start)
      --end;
    return new(Source, start, end - start);
  }

  /// <summary>
  /// Removes the starting and trailing occurrences of a specified character from the current substring.
  /// </summary>
  /// <param name="c">The character to trim.</param>
  /// <returns>
  /// The substring that remains after all occurrences of the character <paramref name="c"/>
  /// are removed from the start and end of the current string.
  /// </returns>
  public Substring Trim(char c) {
    var end = End;
    var start = Start;
    while (Source[start] == c && start < end)
      ++start;
    while (Source[end - 1] == c && end >= start)
      --end;
    return new(Source, start, end - start);
  }

  /// <summary>
  /// Removes the starting and trailing occurrences of a set of characters from the current substring.
  /// </summary>
  /// <param name="c">The characters to trim.</param>
  /// <returns>
  /// The substring that remains after all occurrences of the characters <paramref name="c"/>
  /// are removed from the start and end of the current string.
  /// </returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Substring Trim(params char[] c)
    => Trim((IReadOnlyList<char>)c);

  /// <summary>
  /// Removes the starting and trailing occurrences of a set of characters from the current substring.
  /// </summary>
  /// <param name="c">The characters to trim.</param>
  /// <returns>
  /// The substring that remains after all occurrences of the characters <paramref name="c"/>
  /// are removed from the start and end of the current string.
  /// </returns>
  public Substring Trim(IReadOnlyList<char> c) {
    if (c is null) throw ArgumentNullException();

    var end = End;
    var start = Start;
    while (c.Contains(Source[start]) && start < end)
      ++start;
    while (c.Contains(Source[end - 1]) && end >= start)
      --end;
    return new(Source, start, end - start);
  }

  /// <summary>
  /// Removes the starting and trailing occurrences of a set of characters from the current substring.
  /// </summary>
  /// <param name="c">The characters to trim.</param>
  /// <returns>
  /// The substring that remains after all occurrences of the characters <paramref name="c"/>
  /// are removed from the start and end of the current string.
  /// </returns>
  public Substring Trim(ISet<char> c) {
    if (c is null) throw ArgumentNullException();

    var end = End;
    var start = Start;
    while (c.Contains(Source[start]) && start < end)
      ++start;
    while (c.Contains(Source[end - 1]) && end >= start)
      --end;
    return new(Source, start, end - start);
  }

#if NET6_0_OR_GREATER
  /// <summary>
  /// Removes the starting occurrences of a specified character from the current substring.
  /// </summary>
  /// <param name="c">The character to trim.</param>
  /// <returns>
  /// The substring that remains after all occurrences of the character <paramref name="c"/>
  /// are removed from the start of the current string.
  /// </returns>
  public Substring TrimStart(IReadOnlySet<char> c) {
    if (c is null) throw ArgumentNullException();

    var end = End;
    var start = Start;
    while (c.Contains(Source[start]) && start < end)
      ++start;
    return new(Source, start, end - start);
  }

  /// <summary>
  /// Removes the trailing occurrences of a specified character from the current substring.
  /// </summary>
  /// <param name="c">The character to trim.</param>
  /// <returns>
  /// The substring that remains after all occurrences of the character <paramref name="c"/>
  /// are removed from the end of the current string.
  /// </returns>
  public Substring TrimEnd(IReadOnlySet<char> c) {
    if (c is null) throw ArgumentNullException();

    var end = End;
    var start = Start;
    while (c.Contains(Source[end - 1]) && end >= start)
      --end;
    return new(Source, start, end - start);
  }

  /// <summary>
  /// Removes the starting and trailing occurrences of a set of characters from the current substring.
  /// </summary>
  /// <param name="c">The characters to trim.</param>
  /// <returns>
  /// The substring that remains after all occurrences of the characters <paramref name="c"/>
  /// are removed from the start and end of the current string.
  /// </returns>
  public Substring Trim(IReadOnlySet<char> c) {
    if (c is null) throw ArgumentNullException();

    var end = End;
    var start = Start;
    while (c.Contains(Source[start]) && start < end)
      ++start;
    while (c.Contains(Source[end - 1]) && end >= start)
      --end;
    return new(Source, start, end - start);
  }
#endif

  [MethodImpl(MethodImplOptions.NoInlining)]
  private static ArgumentNullException ArgumentNullException() => new();

  /// <inheritdoc cref="object.Equals(object?)"/>
  public override bool Equals(object? obj)
    => obj is Substring ss && Equals(ss);

  /// <summary>
  /// Invokes the <see cref="Equals(Substring)"/> function to compare
  /// a pair of substrings. 
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool operator ==(Substring left, Substring right)
    => left.Equals(right);

  /// <summary>
  /// Invokes the <see cref="Equals(Substring)"/> function to compare
  /// a pair of substrings and inverts the result. 
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool operator !=(Substring left, Substring right)
    => !(left == right);

  /// <summary>
  /// Returns the hash code for this substring.
  /// </summary>
  public override int GetHashCode() {
    HashCode hc = new();
    var bytes = MemoryMarshal.AsBytes(Source.AsSpan());
#if NETSTANDARD
    ref var pos = ref MemoryMarshal.GetReference(bytes);
    ref var end = ref Unsafe.Add(ref pos, bytes.Length);

    // Add four bytes at a time until the input has fewer than four bytes remaining.
    while ((nint)Unsafe.ByteOffset(ref pos, ref end) >= sizeof(int))
    {
      hc.Add(Unsafe.ReadUnaligned<int>(ref pos));
      pos = ref Unsafe.Add(ref pos, sizeof(int));
    }

    // Add the remaining bytes a single byte at a time.
    while (Unsafe.IsAddressLessThan(ref pos, ref end))
    {
      hc.Add((int)pos);
      pos = ref Unsafe.Add(ref pos, 1);
    }
#else
    hc.AddBytes(bytes);
#endif
    return hc.ToHashCode();
  }
  
  /// <summary>
  /// Creates a new readonly span over the portion of the target string.
  /// </summary>
  public ReadOnlySpan<char> AsSpan()
    => Source.AsSpan(Start, Length);

  /// <summary>
  /// Creates a new readonly span over the portion of the target string.
  /// </summary>
  public ReadOnlySpan<char> AsSpan(int start, int length)
    => Source.AsSpan(Start + start, length);

}