namespace StringToExpression.GrammarDefinitions;

/// <summary>
/// The RelativePositions class provides a collection of static methods for working with relative positions.
/// </summary>
public static class RelativePositions {

  /// <summary>
  /// A read-only list containing a single relative position of "Left".
  /// </summary>
  public static readonly IReadOnlyList<RelativePosition> Left =
    new[] { RelativePosition.Left };

  /// <summary>
  /// A read-only list containing a single relative position of "Right".
  /// </summary>
  public static readonly IReadOnlyList<RelativePosition> Right =
    new[] { RelativePosition.Right };

  /// <summary>
  /// A read-only list containing both relative positions of "Left" and "Right".
  /// </summary>
  public static readonly IReadOnlyList<RelativePosition> Both =
    new[] { RelativePosition.Left, RelativePosition.Right };

  /// <summary>
  /// Returns a read-only list containing a specified number of "Left" relative positions.
  /// </summary>
  /// <param name="n">The number of "Left" relative positions to include in the list.</param>
  /// <returns>A read-only list of "Left" relative positions.</returns>
  public static IReadOnlyList<RelativePosition> Lefts(int n) {
    var a = new RelativePosition[n];
    new Span<RelativePosition>(a).Fill(RelativePosition.Left);
    return a;
  }

  /// <summary>
  /// Returns a read-only list containing a specified number of "Right" relative positions.
  /// </summary>
  /// <param name="n">The number of "Right" relative positions to include in the list.</param>
  /// <returns>A read-only list of "Right" relative positions.</returns>
  public static IReadOnlyList<RelativePosition> Rights(int n) {
    var a = new RelativePosition[n];
    new Span<RelativePosition>(a).Fill(RelativePosition.Right);
    return a;
  }

  /// <summary>
  /// Returns a read-only list containing a specified number of "Left" and "Right" relative positions.
  /// </summary>
  /// <param name="lefts">The number of "Left" relative positions to include in the list.</param>
  /// <param name="rights">The number of "Right" relative positions to include in the list.</param>
  /// <returns>A read-only list of "Left" and "Right" relative positions.</returns>
  public static IReadOnlyList<RelativePosition> Many(int lefts, int rights) {
    var both = lefts + rights;
    var a = new RelativePosition[both];
    var span = new Span<RelativePosition>(a);
    span.Slice(0,lefts).Fill(RelativePosition.Left);
    span.Slice(lefts).Fill(RelativePosition.Right);
    return a;
  }

}