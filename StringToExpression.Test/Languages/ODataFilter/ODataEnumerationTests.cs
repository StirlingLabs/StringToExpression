﻿using System.Diagnostics.CodeAnalysis;
using StringToExpression.LanguageDefinitions;
using System.Linq;
using NUnit.Framework;

namespace StringToExpression.Test.Languages.ODataFilter;

public class ODataEnumerationTests {

  public enum Numbers {

    One = 1,

    Two = 2,

    Three = 3,

    Five = 5

  }

  [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
  public class EnumHolder {

    public Numbers Number { get; init; }

    public Numbers? NullableNumber { get; init; }

    public string? NumberString { get; set; }

    public bool Order { get; init; } = true;

  }

  [Theory]
  [TestCase("Number eq 2", new[] { Numbers.Two })]
  [TestCase("Number ne 2", new[] { Numbers.One, Numbers.Three, Numbers.Five })]
  [TestCase("Number ne 4", new[] { Numbers.One, Numbers.Two, Numbers.Three, Numbers.Five })]
  [TestCase("Number eq 'Two'", new[] { Numbers.Two })]
  [TestCase("Number eq 'two'", new[] { Numbers.Two })]
  [TestCase("Number eq 'tWo'", new[] { Numbers.Two })]
  [TestCase("Number eq toupper('five')", new[] { Numbers.Five })]
  [TestCase("Number eq NullableNumber", new[] { Numbers.One, Numbers.Two, Numbers.Three, Numbers.Five })]
  [TestCase("Number ne NullableNumber", new Numbers[0])]
  [TestCase("2 eq Number", new[] { Numbers.Two })]
  [TestCase("2 ne Number", new[] { Numbers.One, Numbers.Three, Numbers.Five })]
  [TestCase("4 ne Number", new[] { Numbers.One, Numbers.Two, Numbers.Three, Numbers.Five })]
  [TestCase("'Two' eq Number", new[] { Numbers.Two })]
  [TestCase("'two' eq Number", new[] { Numbers.Two })]
  [TestCase("'tWo' eq Number", new[] { Numbers.Two })]
  [TestCase("toupper('five') eq Number", new[] { Numbers.Five })]
  [TestCase("NullableNumber eq Number", new[] { Numbers.One, Numbers.Two, Numbers.Three, Numbers.Five })]
  [TestCase("NullableNumber ne Number", new Numbers[0])]
  [TestCase("order eq false", new[] { Numbers.Five })]
  [SuppressMessage("ReSharper", "StringLiteralTypo")]
  public void When_filtering_enumeration_should_parse(string query, Numbers[] expectedNumbers) {
    var data = new[] {
      new EnumHolder
        { Number = Numbers.One, NullableNumber = Numbers.One },
      new EnumHolder
        { Number = Numbers.Two, NullableNumber = Numbers.Two },
      new EnumHolder
        { Number = Numbers.Three, NullableNumber = Numbers.Three },
      new EnumHolder
        { Number = Numbers.Five, NullableNumber = Numbers.Five, Order = false },
    }.AsQueryable();

    var filter = new ODataFilterLanguage().Parse<EnumHolder>(query);
    var filtered = data.Where(filter.ToLambdaExpression()).ToList();

    var distinct = filtered.Select(x => x.Number).Distinct().ToArray();
    Assert.AreEqual(expectedNumbers, distinct);
  }

  [Theory]
  [TestCase("NullableNumber eq 2", new[] { Numbers.Two, })]
  [TestCase("NullableNumber ne 2", new[] { Numbers.One, Numbers.Three, Numbers.Five, (Numbers)0 })]
  [TestCase("NullableNumber ne 4", new[] { Numbers.One, Numbers.Two, Numbers.Three, Numbers.Five, (Numbers)0 })]
  [TestCase("NullableNumber eq null", new[] { (Numbers)0 })]
  [TestCase("NullableNumber ne null", new[] { Numbers.One, Numbers.Two, Numbers.Three, Numbers.Five })]
  [TestCase("NullableNumber eq 'Two'", new[] { Numbers.Two, })]
  [TestCase("NullableNumber ne 'tWo'", new[] { Numbers.One, Numbers.Three, Numbers.Five, (Numbers)0 })]
  [TestCase("2 eq NullableNumber", new[] { Numbers.Two, })]
  [TestCase("2 ne NullableNumber", new[] { Numbers.One, Numbers.Three, Numbers.Five, (Numbers)0 })]
  [TestCase("4 ne NullableNumber", new[] { Numbers.One, Numbers.Two, Numbers.Three, Numbers.Five, (Numbers)0 })]
  [TestCase("null eq NullableNumber", new[] { (Numbers)0 })]
  [TestCase("null ne NullableNumber", new[] { Numbers.One, Numbers.Two, Numbers.Three, Numbers.Five })]
  [TestCase("'Two' eq NullableNumber", new[] { Numbers.Two, })]
  [TestCase("'tWo' ne NullableNumber", new[] { Numbers.One, Numbers.Three, Numbers.Five, (Numbers)0 })]
  public void When_filtering_nullable_enumeration_should_parse(string query, Numbers[] expectedNumbers) {
    var data = new[] {
      new EnumHolder
        { NullableNumber = Numbers.One },
      new EnumHolder
        { NullableNumber = Numbers.Two },
      new EnumHolder
        { NullableNumber = Numbers.Three },
      new EnumHolder
        { NullableNumber = Numbers.Five },
      new EnumHolder
        { NullableNumber = null },
    }.AsQueryable();

    var filter = new ODataFilterLanguage().Parse<EnumHolder>(query);
    var filtered = data.Where(filter.ToLambdaExpression()).ToList();

    //we will treat 0 as null (limit put on us by attributes)
    var expectedNumbersWithNull = expectedNumbers.Select(x => x == 0 ? null : (Numbers?)x);

    var distinct = filtered.Select(x => x.NullableNumber).Distinct().ToArray();
    Assert.AreEqual(expectedNumbersWithNull, distinct);
  }

  [Theory]
  [TestCase("Number eq 'Four'")] //not a valid enum value
  [TestCase("Number eq 2.8")] //double cant be a valid enum
  [TestCase("Number eq null")] //number is not nullable cant be null
  [TestCase("Number eq NumberString")] //we do not support non-constant strings
  public void When_filtering_bad_enum_const_should_error_on_parse(string query) {
    Assert.Throws<OperationInvalidException>(() => new ODataFilterLanguage().Parse<EnumHolder>(query));
  }

}