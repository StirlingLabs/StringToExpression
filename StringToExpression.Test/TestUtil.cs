using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace StringToExpression.Test;

public static class TestUtil {

  public static string Highlight(this Substring segment, string startHighlight = "[", string endHighlight = "]")
    => segment.Source
      .Insert(segment.End, endHighlight)
      .Insert(segment.Start, startHighlight);

  [DebuggerHidden]
  [DebuggerStepThrough]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void EnumerateOnly<T>(this IEnumerable<T> enumerable) {
    using var e = enumerable.GetEnumerator();
    while (e.MoveNext()) { }
  }

  [DebuggerHidden]
  [DebuggerStepThrough]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Discard<T>(Func<T> func) {
    func();
  }

}