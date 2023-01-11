using System;
using System.Linq;
using NUnit.Framework;

namespace StringToExpression.Test.Util;

public class SubstringTests {

  [Test]
  public void Substring_split_should_separate_strings_by_a_character(
    [Values(
      "Example",
      "Wannabe",
      "Fascinating",
      "Capitulated",
      "Oh my what do we have here",
      "Excalibur is a sword or something",
      "Whiskey foxtrot november mike",
      "Delta",
      "Delta gamma",
      "Alabama alabama alabama"
    )]
    string source,
    [Values(
      'a',
      'l',
      'e',
      'W',
      'A',
      'D',
      'E'
    )]
    char separator,
    [Values(
      StringSplitOptions.None,
      StringSplitOptions.RemoveEmptyEntries,
      StringSplitOptions.TrimEntries,
      StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
    )]
    StringSplitOptions opts
  ) {
    var expected = source.Split(separator);
    var ss = new Substring(source);

    var i = 0;
    foreach (var part in ss.Split(separator))
      Assert.AreEqual(expected[i++], part.ToString());
  }

  [Test]
  public void Substring_split_should_separate_strings_by_a_multiple_characters(
    [Values(
      "Example",
      "Wannabe",
      "Fascinating",
      "Capitulated",
      "Oh my what do we have here",
      "Excalibur is a sword or something",
      "Whiskey foxtrot november mike",
      "Delta",
      "Delta gamma",
      "Alabama alabama alabama"
    )]
    string source,
    [Values(
      new[] {
        'a',
        'l',
        'e'
      },
      new[] {
        'W',
        'A',
        'D',
        'E'
      }
    )]
    char[] separator,
    [Values(
      StringSplitOptions.None,
      StringSplitOptions.RemoveEmptyEntries,
      StringSplitOptions.TrimEntries,
      StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
    )]
    StringSplitOptions opts
  ) {
    var expected = source.Split(separator);
    var ss = new Substring(source);

    var i = 0;
    foreach (var part in ss.Split(separator))
      Assert.AreEqual(expected[i++], part.ToString());
  }

  [Test]
  public void Substring_split_should_separate_strings_by_a_multiple_characters_when_they_are_spans(
    [Values(
      "Example",
      "Wannabe",
      "Fascinating",
      "Capitulated",
      "Oh my what do we have here",
      "Excalibur is a sword or something",
      "Whiskey foxtrot november mike",
      "Delta",
      "Delta gamma",
      "Alabama alabama alabama"
    )]
    string source,
    [Values(
      new[] {
        'a',
        'l',
        'e'
      },
      new[] {
        'W',
        'A',
        'D',
        'E'
      }
    )]
    char[] separator,
    [Values(
      StringSplitOptions.None,
      StringSplitOptions.RemoveEmptyEntries,
      StringSplitOptions.TrimEntries,
      StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
    )]
    StringSplitOptions opts
  ) {
    var expected = source.Split(separator);
    var ss = new Substring(source);

    var i = 0;
    foreach (var part in ss.Split((ReadOnlySpan<char>)separator))
      Assert.AreEqual(expected[i++], part.ToString());
  }


}