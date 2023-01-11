namespace StringToExpression.GrammarDefinitions;

public static class RelativePositions {

  public static readonly IReadOnlyList<RelativePosition> Left =
    new[] { RelativePosition.Left };

  public static readonly IReadOnlyList<RelativePosition> Right =
    new[] { RelativePosition.Right };

  public static readonly IReadOnlyList<RelativePosition> Both =
    new[] { RelativePosition.Left, RelativePosition.Right };

  public static IReadOnlyList<RelativePosition> Lefts(int n) {
    var a = new RelativePosition[n];
    new Span<RelativePosition>(a).Fill(RelativePosition.Left);
    return a;
  }

  public static IReadOnlyList<RelativePosition> Rights(int n) {
    var a = new RelativePosition[n];
    new Span<RelativePosition>(a).Fill(RelativePosition.Right);
    return a;
  }

  public static IReadOnlyList<RelativePosition> Many(int lefts, int rights) {
    var both = lefts + rights;
    var a = new RelativePosition[both];
    var span = new Span<RelativePosition>(a);
    span.Slice(0,lefts).Fill(RelativePosition.Left);
    span.Slice(lefts).Fill(RelativePosition.Right);
    return a;
  }

}