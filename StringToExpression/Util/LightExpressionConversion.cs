using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using FastExpressionCompiler.LightExpression;
using JetBrains.Annotations;
using Sys = System.Linq.Expressions;
using Light = FastExpressionCompiler.LightExpression;

namespace StringToExpression;

/// <summary>
/// This class provides extension methods for converting <see cref="System.Linq.Expression"/>s
/// to their <see cref="FastExpressionCompiler.LightExpression"/> counterparts.
/// </summary>
[PublicAPI]
public static class LightExpressionConversion {

  /// <summary>
  /// Converts a System.Linq.Expressions.LambdaExpression to a 
  /// FastExpressionCompiler.LightExpression.LambdaExpression.
  /// </summary>
  /// <param name="expr">The System.Linq.Expressions.LambdaExpression to convert.</param>
  /// <returns>The converted FastExpressionCompiler.LightExpression.LambdaExpression.</returns>
  public static Light.LambdaExpression ToLightExpressions(this Sys.LambdaExpression expr) {
    var body = expr.Body.ToLightExpression();
    var parameters = expr.Parameters.ToLightExpressions();
    var retType = expr.ReturnType;
    return Light.Expression.Lambda(
      body,
      parameters,
      retType);
  }

  /// <summary>
  /// Converts an IEnumerable of System.Linq.Expressions.Expression to an IEnumerable of 
  /// FastExpressionCompiler.LightExpression.Expression.
  /// </summary>
  /// <param name="expressions">The IEnumerable of System.Linq.Expressions.Expression to convert.</param>
  /// <returns>The IEnumerable of converted FastExpressionCompiler.LightExpression.Expression.</returns>
  public static IEnumerable<Light.Expression?> ToLightExpressions(this IEnumerable<Sys.Expression?> expressions) {
    foreach (var expr in expressions)
      yield return expr.ToLightExpression();
  }

  /// <summary>
  /// Converts <see cref="System.Linq.Expressions.ParameterExpression"/>s to <see cref="FastExpressionCompiler.LightExpression.ParameterExpression"/>s.
  /// </summary>
  /// <param name="expressions">The <see cref="System.Linq.Expressions.ParameterExpression"/>s to convert.</param>
  /// <returns>The converted <see cref="FastExpressionCompiler.LightExpression.ParameterExpression"/>s.</returns>
  public static IEnumerable<Light.ParameterExpression> ToLightExpressions(this IEnumerable<Sys.ParameterExpression> expressions) {
    foreach (var expr in expressions)
      yield return expr.ToLightExpression();
  }

  /// <summary>
  /// Converts <see cref="Sys.BinaryExpression"/> to <see cref="Light.BinaryExpression"/>.
  /// </summary>
  /// <param name="expr">The <see cref="Sys.BinaryExpression"/> to be converted.</param>
  /// <returns>The converted <see cref="Light.BinaryExpression"/>.</returns>
  public static Light.BinaryExpression ToLightExpression(this Sys.BinaryExpression expr)
    => Light.Expression.MakeBinary(
      expr.NodeType,
      expr.Left.ToLightExpression(),
      expr.Right.ToLightExpression(),
      expr.IsLiftedToNull,
      expr.Method,
      expr.Conversion?.ToLightExpressions()
    );

  /// <summary>
  /// Converts <see cref="Sys.BlockExpression"/> to <see cref="Light.BlockExpression"/>.
  /// </summary>
  /// <param name="expr">The <see cref="Sys.BlockExpression"/> to be converted.</param>
  /// <returns>The converted <see cref="Light.BlockExpression"/>.</returns>
  public static Light.BlockExpression ToLightExpression(this Sys.BlockExpression expr)
    => Light.Expression.MakeBlock(
      expr.Type,
      expr.Variables.ToLightExpressions(),
      expr.Expressions.ToLightExpressions()
    );

  /// <summary>
  /// Converts a <see cref="System.Linq.Expressions.ConditionalExpression"/> to a 
  /// <see cref="FastExpressionCompiler.LightExpression.ConditionalExpression"/>.
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.ConditionalExpression"/> to convert.</param>
  /// <returns>A <see cref="FastExpressionCompiler.LightExpression.ConditionalExpression"/> that is equivalent to 
  /// <paramref name="expr"/>.</returns>
  public static Light.ConditionalExpression ToLightExpression(this Sys.ConditionalExpression expr)
    => Light.Expression.Condition(
      expr.Test.ToLightExpression(),
      expr.IfTrue.ToLightExpression(),
      expr.IfFalse.ToLightExpression()
    );

  /// <summary>
  /// Converts a <see cref="System.Linq.Expressions.ConstantExpression"/> to a 
  /// <see cref="FastExpressionCompiler.LightExpression.ConstantExpression"/>.
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.ConstantExpression"/> to convert.</param>
  /// <returns>A <see cref="FastExpressionCompiler.LightExpression.ConstantExpression"/> that is equivalent to 
  /// <paramref name="expr"/>.</returns>
  public static Light.ConstantExpression ToLightExpression(this Sys.ConstantExpression expr)
    => Light.Expression.Constant(
      expr.Value,
      expr.Type
    );

  /// <summary>
  /// Converts a System.Linq.Expressions.DebugInfoExpression object to a FastExpressionCompiler.LightExpression.DebugInfoExpression object
  /// </summary>
  /// <param name="expr">The System.Linq.Expressions.DebugInfoExpression object to convert</param>
  /// <returns>A FastExpressionCompiler.LightExpression.DebugInfoExpression object</returns>
  public static Light.DebugInfoExpression ToLightExpression(this Sys.DebugInfoExpression expr)
    => (Light.DebugInfoExpression)
      Light.Expression.DebugInfo(
        expr.Document.ToLightExpression(),
        expr.StartLine,
        expr.StartColumn,
        expr.EndLine,
        expr.EndColumn
      );

  /// <summary>
  /// Converts a System.Linq.Expressions.SymbolDocumentInfo object to a FastExpressionCompiler.LightExpression.SymbolDocumentInfo object
  /// </summary>
  /// <param name="doc">The System.Linq.Expressions.SymbolDocumentInfo object to convert</param>
  /// <returns>A FastExpressionCompiler.LightExpression.SymbolDocumentInfo object</returns>
  public static Light.SymbolDocumentInfo ToLightExpression(this Sys.SymbolDocumentInfo doc)
    => Light.Expression.SymbolDocument(doc.FileName);

  /// <summary>
  /// Converts a System.Linq.Expressions.DefaultExpression object to a FastExpressionCompiler.LightExpression.DefaultExpression object
  /// </summary>
  /// <param name="expr">The System.Linq.Expressions.DefaultExpression object to convert</param>
  /// <returns>A FastExpressionCompiler.LightExpression.DefaultExpression object</returns>
  public static Light.DefaultExpression ToLightExpression(this Sys.DefaultExpression expr)
    => Light.Expression.Default(expr.Type);

  /// <summary>
  /// Converts the given <see cref="System.Linq.Expressions.DynamicExpression"/> to a <see cref="FastExpressionCompiler.LightExpression.DynamicExpression"/>.
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.DynamicExpression"/> to convert.</param>
  /// <returns>A new <see cref="FastExpressionCompiler.LightExpression.DynamicExpression"/> with the same properties as the given <see cref="System.Linq.Expressions.DynamicExpression"/>.</returns>
  public static Light.DynamicExpression ToLightExpression(this Sys.DynamicExpression expr)
    => new(
      expr.DelegateType,
      expr.Binder,
      expr.Arguments.ToLightExpressions().ToList()
    );

  /// <summary>
  /// A <see cref="System.Runtime.CompilerServices.ConditionalWeakTable{TKey, TValue}"/> used to map <see cref="System.Linq.Expressions.LabelTarget"/> objects to <see cref="FastExpressionCompiler.LightExpression.LabelTarget"/> objects.
  /// </summary>
  public static readonly ConditionalWeakTable<Sys.LabelTarget, Light.LabelTarget> LabelResolver = new();

  /// <summary>
  /// Converts the given <see cref="System.Linq.Expressions.GotoExpression"/> to a <see cref="FastExpressionCompiler.LightExpression.GotoExpression"/>.
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.GotoExpression"/> to convert.</param>
  /// <returns>A new <see cref="FastExpressionCompiler.LightExpression.GotoExpression"/> with the same properties as the given <see cref="System.Linq.Expressions.GotoExpression"/>.</returns>
  public static Light.GotoExpression ToLightExpression(this Sys.GotoExpression expr)
    => Light.Expression.MakeGoto(
      expr.Kind,
      expr.Target.ToLightExpression(),
      expr.Value.ToLightExpression(),
      expr.Type
    );

  /// <summary>
  /// Converts the given <see cref="System.Linq.Expressions.LabelTarget"/> to a <see cref="FastExpressionCompiler.LightExpression.LabelTarget"/>.
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.LabelTarget"/> to convert.</param>
  /// <returns>A new <see cref="FastExpressionCompiler.LightExpression.LabelTarget"/> with the same properties as the given <see cref="System.Linq.Expressions.LabelTarget"/>.</returns>
  public static Light.LabelTarget ToLightExpression(this Sys.LabelTarget expr)
    => LabelResolver.GetValue(expr, LabelTargetFactory);

  /// <summary>
  /// A private factory method that creates a <see cref="FastExpressionCompiler.LightExpression.LabelTarget"/> from a
  /// <see cref="System.Linq.Expressions.LabelTarget"/>.
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.LabelTarget"/> to be converted.</param>
  /// <returns>A new <see cref="FastExpressionCompiler.LightExpression.LabelTarget"/> instance.</returns>
  private static Light.LabelTarget LabelTargetFactory(Sys.LabelTarget expr)
    => expr.Name is not null
      ? new TypedNamedLabelTarget(expr.Type, expr.Name)
      : new TypedLabelTarget(expr.Type);
  /*
  private static Light.LabelTarget LabelTargetFactory(Sys.LabelTarget expr)
    => expr.Name is not null
      ? expr.Type is not null
        ? new TypedNamedLabelTarget(expr.Type, expr.Name)
        : new NamedLabelTarget(expr.Name)
      : expr.Type is not null
        ? new TypedLabelTarget(expr.Type)
        : new Light.LabelTarget();
   */

  /// <summary>
  /// Converts a <see cref="System.Linq.Expressions.IndexExpression"/> to a
  /// <see cref="FastExpressionCompiler.LightExpression.IndexExpression"/>.
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.IndexExpression"/> to be converted.</param>
  /// <returns>A new <see cref="FastExpressionCompiler.LightExpression.IndexExpression"/> instance.</returns>
  public static Light.IndexExpression ToLightExpression(this Sys.IndexExpression expr)
    => Light.Expression.MakeIndex(
      expr.Object.ToLightExpression(),
      expr.Indexer,
      expr.Arguments.ToLightExpressions()
    );

  /// <summary>
  /// Converts a <see cref="System.Linq.Expressions.InvocationExpression"/> to a
  /// <see cref="FastExpressionCompiler.LightExpression.InvocationExpression"/>.
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.InvocationExpression"/> to be converted.</param>
  /// <returns>A new <see cref="FastExpressionCompiler.LightExpression.InvocationExpression"/> instance.</returns>
  public static Light.InvocationExpression ToLightExpression(this Sys.InvocationExpression expr)
    => Light.Expression.Invoke(
      expr.Expression.ToLightExpression(),
      expr.Arguments.ToLightExpressions()
    );

  /// <summary>
  /// Converts a <see cref="System.Linq.Expressions.LabelExpression"/> to a <see cref="FastExpressionCompiler.LightExpression.LabelExpression"/>.
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.LabelExpression"/> to convert.</param>
  /// <returns>A <see cref="FastExpressionCompiler.LightExpression.LabelExpression"/> that represents the same expression as the input <see cref="System.Linq.Expressions.LabelExpression"/>.</returns>
  public static Light.LabelExpression ToLightExpression(this Sys.LabelExpression expr)
    => Light.Expression.Label(
      expr.Target.ToLightExpression(),
      expr.DefaultValue.ToLightExpression()
    );

  /// <summary>
  /// Converts a <see cref="System.Linq.Expressions.ListInitExpression"/> to a <see cref="FastExpressionCompiler.LightExpression.ListInitExpression"/>.
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.ListInitExpression"/> to convert.</param>
  /// <returns>A <see cref="FastExpressionCompiler.LightExpression.ListInitExpression"/> that represents the same expression as the input <see cref="System.Linq.Expressions.ListInitExpression"/>.</returns>
  public static Light.ListInitExpression ToLightExpression(this Sys.ListInitExpression expr)
    => Light.Expression.ListInit(
      expr.NewExpression.ToLightExpression(),
      expr.Initializers.ToLightExpressions()
    );

  /// <summary>
  /// Converts a <see cref="System.Linq.Expressions.ElementInit"/> to a <see cref="FastExpressionCompiler.LightExpression.ElementInit"/>.
  /// </summary>
  /// <param name="init">The <see cref="System.Linq.Expressions.ElementInit"/> to convert.</param>
  /// <returns>A <see cref="FastExpressionCompiler.LightExpression.ElementInit"/> that represents the same expression as the input <see cref="System.Linq.Expressions.ElementInit"/>.</returns>
  public static Light.ElementInit ToLightExpression(this Sys.ElementInit init)
    => init.Arguments.Count == 1
      ? Light.Expression.ElementInit(init.AddMethod, init.Arguments[0].ToLightExpression())
      : Light.Expression.ElementInit(init.AddMethod, init.Arguments.ToLightExpressions());

  /// <summary>
  /// Extension method that converts a IEnumerable of <see cref="System.Linq.Expressions.ElementInit"/> to a IEnumerable of <see cref="FastExpressionCompiler.LightExpression.ElementInit"/>
  /// </summary>
  /// <param name="inits">The IEnumerable of <see cref="System.Linq.Expressions.ElementInit"/> to convert</param>
  /// <returns>An IEnumerable of <see cref="FastExpressionCompiler.LightExpression.ElementInit"/></returns>
  public static IEnumerable<Light.ElementInit> ToLightExpressions(this IEnumerable<Sys.ElementInit> inits) {
    foreach (var init in inits)
      yield return init.ToLightExpression();
  }

  /// <summary>
  /// Extension method that converts a <see cref="System.Linq.Expressions.NewExpression"/> to a <see cref="FastExpressionCompiler.LightExpression.NewExpression"/>
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.NewExpression"/> to convert</param>
  /// <returns>A <see cref="FastExpressionCompiler.LightExpression.NewExpression"/></returns>
  public static Light.NewExpression ToLightExpression(this Sys.NewExpression expr) {
    var args = expr.Arguments;
    return args.Count switch {
      0 => Light.Expression.New(expr.Constructor),
      1 => Light.Expression.New(expr.Constructor,
        args[0].ToLightExpression()),
      2 => Light.Expression.New(expr.Constructor,
        args[0].ToLightExpression(),
        args[1].ToLightExpression()),
      3 => Light.Expression.New(expr.Constructor,
        args[0].ToLightExpression(),
        args[1].ToLightExpression(),
        args[2].ToLightExpression()),
      4 => Light.Expression.New(expr.Constructor,
        args[0].ToLightExpression(),
        args[1].ToLightExpression(),
        args[2].ToLightExpression(),
        args[3].ToLightExpression()),
      5 => Light.Expression.New(expr.Constructor,
        args[0].ToLightExpression(),
        args[1].ToLightExpression(),
        args[2].ToLightExpression(),
        args[3].ToLightExpression(),
        args[4].ToLightExpression()),
      6 => Light.Expression.New(expr.Constructor,
        args[0].ToLightExpression(),
        args[1].ToLightExpression(),
        args[2].ToLightExpression(),
        args[3].ToLightExpression(),
        args[4].ToLightExpression(),
        args[5].ToLightExpression()),
      7 => Light.Expression.New(expr.Constructor,
        args[0].ToLightExpression(),
        args[1].ToLightExpression(),
        args[2].ToLightExpression(),
        args[3].ToLightExpression(),
        args[4].ToLightExpression(),
        args[5].ToLightExpression(),
        args[6].ToLightExpression()),
      _ => Light.Expression.New(expr.Constructor, args.ToLightExpressions())
    };
  }

  /// <summary>
  /// Extension method that converts a <see cref="System.Linq.Expressions.LoopExpression"/> to a <see cref="FastExpressionCompiler.LightExpression.LoopExpression"/>
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.LoopExpression"/> to convert</param>
  /// <returns>A <see cref="FastExpressionCompiler.LightExpression.LoopExpression"/></returns>
  public static Light.LoopExpression ToLightExpression(this Sys.LoopExpression expr)
    => Light.Expression.Loop(
      expr.Body.ToLightExpression(),
      expr.BreakLabel?.ToLightExpression(),
      expr.ContinueLabel?.ToLightExpression()
    );

  /// <summary>
  /// Extension method that converts a <see cref="System.Linq.Expressions.MemberExpression"/> to a <see cref="FastExpressionCompiler.LightExpression.MemberExpression"/>
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.MemberExpression"/> to convert</param>
  /// <returns>A <see cref="FastExpressionCompiler.LightExpression.MemberExpression"/></returns>
  public static Light.MemberExpression ToLightExpression(this Sys.MemberExpression expr)
    => Light.Expression.MakeMemberAccess(
      expr.Expression.ToLightExpression(),
      expr.Member
    );

  /// <summary>
  /// Extension method that converts a <see cref="System.Linq.Expressions.MemberInitExpression"/> to a <see cref="FastExpressionCompiler.LightExpression.MemberInitExpression"/>
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.MemberInitExpression"/> to convert</param>
  /// <returns>A <see cref="FastExpressionCompiler.LightExpression.MemberInitExpression"/></returns>
  public static Light.MemberInitExpression ToLightExpression(this Sys.MemberInitExpression expr) {
    var binds = expr.Bindings;
    return binds.Count switch {
      0 => Light.Expression.MemberInit(expr.NewExpression.ToLightExpression()),
      1 => Light.Expression.MemberInit(expr.NewExpression.ToLightExpression(),
        binds[0].ToLightExpression()),
      2 => Light.Expression.MemberInit(expr.NewExpression.ToLightExpression(),
        binds[0].ToLightExpression(),
        binds[1].ToLightExpression()),
      3 => Light.Expression.MemberInit(expr.NewExpression.ToLightExpression(),
        binds[0].ToLightExpression(),
        binds[1].ToLightExpression(),
        binds[2].ToLightExpression()),
      4 => Light.Expression.MemberInit(expr.NewExpression.ToLightExpression(),
        binds[0].ToLightExpression(),
        binds[1].ToLightExpression(),
        binds[2].ToLightExpression(),
        binds[3].ToLightExpression()),
      5 => Light.Expression.MemberInit(expr.NewExpression.ToLightExpression(),
        binds[0].ToLightExpression(),
        binds[1].ToLightExpression(),
        binds[2].ToLightExpression(),
        binds[3].ToLightExpression(),
        binds[4].ToLightExpression()),
      6 => Light.Expression.MemberInit(expr.NewExpression.ToLightExpression(),
        binds[0].ToLightExpression(),
        binds[1].ToLightExpression(),
        binds[2].ToLightExpression(),
        binds[3].ToLightExpression(),
        binds[4].ToLightExpression(),
        binds[6].ToLightExpression()),
      _ => Light.Expression.MemberInit(expr.NewExpression.ToLightExpression(),
        binds.ToLightExpressions())
    };
  }

  /// <summary>
  /// Extension method that converts a <see cref="System.Linq.Expressions.MemberBinding"/> to a <see cref="FastExpressionCompiler.LightExpression.MemberBinding"/>
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.MemberBinding"/> to convert</param>
  /// <returns>A <see cref="FastExpressionCompiler.LightExpression.MemberBinding"/></returns>
  public static Light.MemberBinding ToLightExpression(this Sys.MemberBinding expr) {
    switch (expr) {
      case Sys.MemberMemberBinding m:
        return Light.Expression.MemberBind(m.Member, m.Bindings.ToLightExpressions());
      case Sys.MemberListBinding m:
        return Light.Expression.ListBind(m.Member, m.Initializers.ToLightExpressions());
      case Sys.MemberAssignment m:
        return Light.Expression.Bind(m.Member, m.Expression.ToLightExpression());
      default: throw new NotImplementedException(expr.GetType().AssemblyQualifiedName);
    }
  }

  /// <summary>
  /// Extension method that converts a IEnumerable of <see cref="System.Linq.Expressions.MemberBinding"/> to a IEnumerable of <see cref="FastExpressionCompiler.LightExpression.MemberBinding"/>
  /// </summary>
  /// <param name="binds">The IEnumerable of <see cref="System.Linq.Expressions.MemberBinding"/> to convert</param>
  /// <returns>An IEnumerable of <see cref="FastExpressionCompiler.LightExpression.MemberBinding"/></returns>
  public static IEnumerable<Light.MemberBinding> ToLightExpressions(this IEnumerable<Sys.MemberBinding> binds) {
    foreach (var bind in binds)
      yield return bind.ToLightExpression();
  }

  /// <summary>
  /// Extension method that converts a <see cref="System.Linq.Expressions.MethodCallExpression"/> to a <see cref="FastExpressionCompiler.LightExpression.MethodCallExpression"/>
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.MethodCallExpression"/> to convert</param>
  /// <returns>A <see cref="FastExpressionCompiler.LightExpression.MethodCallExpression"/></returns>
  public static Light.MethodCallExpression ToLightExpression(this Sys.MethodCallExpression expr) {
    var args = expr.Arguments;
    if (expr.Method.IsStatic) {
      return args.Count switch {
        0 => Light.Expression.Call(expr.Method),
        1 => Light.Expression.Call(expr.Method,
          args[0].ToLightExpression()),
        2 => Light.Expression.Call(expr.Method,
          args[0].ToLightExpression(),
          args[1].ToLightExpression()),
        3 => Light.Expression.Call(expr.Method,
          args[0].ToLightExpression(),
          args[1].ToLightExpression(),
          args[2].ToLightExpression()),
        4 => Light.Expression.Call(expr.Method,
          args[0].ToLightExpression(),
          args[1].ToLightExpression(),
          args[2].ToLightExpression(),
          args[3].ToLightExpression()),
        5 => Light.Expression.Call(expr.Method,
          args[0].ToLightExpression(),
          args[1].ToLightExpression(),
          args[2].ToLightExpression(),
          args[3].ToLightExpression(),
          args[4].ToLightExpression()),
        6 => Light.Expression.Call(expr.Method,
          args[0].ToLightExpression(),
          args[1].ToLightExpression(),
          args[2].ToLightExpression(),
          args[3].ToLightExpression(),
          args[4].ToLightExpression(),
          args[5].ToLightExpression()),
        _ => Light.Expression.Call(expr.Method,
          args.ToLightExpressions())
      };
    }
    else {
      return args.Count switch {
        0 => Light.Expression.Call(
          expr.Object.ToLightExpression(),
          expr.Method),
        1 => Light.Expression.Call(
          expr.Object.ToLightExpression(),
          expr.Method,
          args[0].ToLightExpression()),
        2 => Light.Expression.Call(
          expr.Object.ToLightExpression(),
          expr.Method,
          args[0].ToLightExpression(),
          args[1].ToLightExpression()),
        3 => Light.Expression.Call(
          expr.Object.ToLightExpression(),
          expr.Method,
          args[0].ToLightExpression(),
          args[1].ToLightExpression(),
          args[2].ToLightExpression()),
        4 => Light.Expression.Call(
          expr.Object.ToLightExpression(),
          expr.Method,
          args[0].ToLightExpression(),
          args[1].ToLightExpression(),
          args[2].ToLightExpression(),
          args[3].ToLightExpression()),
        5 => Light.Expression.Call(
          expr.Object.ToLightExpression(),
          expr.Method,
          args[0].ToLightExpression(),
          args[1].ToLightExpression(),
          args[2].ToLightExpression(),
          args[3].ToLightExpression(),
          args[4].ToLightExpression()),
        6 => Light.Expression.Call(
          expr.Object.ToLightExpression(),
          expr.Method,
          args[0].ToLightExpression(),
          args[1].ToLightExpression(),
          args[2].ToLightExpression(),
          args[3].ToLightExpression(),
          args[4].ToLightExpression(),
          args[5].ToLightExpression()),
        _ => Light.Expression.Call(
          expr.Object.ToLightExpression(),
          expr.Method,
          args.ToLightExpressions())
      };
    }
  }

  /// <summary>
  /// Extension method for converting a <see cref="System.Linq.Expressions.NewArrayExpression"/> to a <see cref="FastExpressionCompiler.LightExpression.NewArrayExpression"/>.
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.NewArrayExpression"/> to convert.</param>
  /// <returns>A <see cref="FastExpressionCompiler.LightExpression.NewArrayExpression"/> representing the same expression as the input.</returns>
  public static Light.NewArrayExpression ToLightExpression(this Sys.NewArrayExpression expr) {
    var expressions = expr.Expressions;
    return expressions.Count switch {
      0 => Light.Expression.NewArrayInit(expr.Type),
      1 => Light.Expression.NewArrayInit(expr.Type,
        expressions[0].ToLightExpression()),
      2 => Light.Expression.NewArrayInit(expr.Type,
        expressions[0].ToLightExpression(),
        expressions[1].ToLightExpression()),
      3 => Light.Expression.NewArrayInit(expr.Type,
        expressions[0].ToLightExpression(),
        expressions[1].ToLightExpression(),
        expressions[2].ToLightExpression()),
      4 => Light.Expression.NewArrayInit(expr.Type,
        expressions[0].ToLightExpression(),
        expressions[1].ToLightExpression(),
        expressions[2].ToLightExpression(),
        expressions[3].ToLightExpression()),
      5 => Light.Expression.NewArrayInit(expr.Type,
        expressions[0].ToLightExpression(),
        expressions[1].ToLightExpression(),
        expressions[2].ToLightExpression(),
        expressions[3].ToLightExpression(),
        expressions[4].ToLightExpression()),
      6 => Light.Expression.NewArrayInit(expr.Type,
        expressions[0].ToLightExpression(),
        expressions[1].ToLightExpression(),
        expressions[2].ToLightExpression(),
        expressions[3].ToLightExpression(),
        expressions[4].ToLightExpression(),
        expressions[5].ToLightExpression()),
      _ => Light.Expression.NewArrayInit(expr.Type,
        expressions.ToLightExpressions())
    };
  }

  /// <summary>
  /// Table for resolving <see cref="System.Linq.Expressions.ParameterExpression"/> to <see cref="FastExpressionCompiler.LightExpression.ParameterExpression"/>.
  /// </summary>
  public static readonly ConditionalWeakTable<Sys.ParameterExpression, Light.ParameterExpression> ParameterResolver = new();

  /// <summary>
  /// Extension method for converting a <see cref="System.Linq.Expressions.ParameterExpression"/> to a <see cref="FastExpressionCompiler.LightExpression.ParameterExpression"/>.
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.ParameterExpression"/> to convert.</param>
  /// <returns>A <see cref="FastExpressionCompiler.LightExpression.ParameterExpression"/> representing the same expression as the input.</returns>
  public static Light.ParameterExpression ToLightExpression(this Sys.ParameterExpression expr)
    => ParameterResolver.GetValue(expr, ParameterFactory);

  /// <summary>
  /// Private method for creating a <see cref="FastExpressionCompiler.LightExpression.ParameterExpression"/> from a <see cref="System.Linq.Expressions.ParameterExpression"/>.
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.ParameterExpression"/> to convert.</param>
  /// <returns>A <see cref="FastExpressionCompiler.LightExpression.ParameterExpression"/> representing the same expression as the input.</returns>
  private static Light.ParameterExpression ParameterFactory(Sys.ParameterExpression expr)
  => expr.IsByRef
    ? throw new NotImplementedException("expr.IsByRef")
    : expr.Type.IsByRef
      // both are parameter expressions
      ? Light.Expression.Parameter(expr.Type, expr.Name)
      : Light.Expression.Variable(expr.Type, expr.Name);

  /// <summary>
  /// Extension method for converting a <see cref="System.Linq.Expressions.RuntimeVariablesExpression"/> to a <see cref="FastExpressionCompiler.LightExpression.RuntimeVariablesExpression"/>.
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.RuntimeVariablesExpression"/> to convert.</param>
  /// <returns>A <see cref="FastExpressionCompiler.LightExpression.RuntimeVariablesExpression"/> representing the same expression as the input.</returns>
  public static Light.RuntimeVariablesExpression ToLightExpression(this Sys.RuntimeVariablesExpression expr)
    => throw new NotImplementedException();

  /// <summary>
  /// Extension method for converting a <see cref="System.Linq.Expressions.SwitchExpression"/> to a <see cref="FastExpressionCompiler.LightExpression.SwitchExpression"/>.
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.SwitchExpression"/> to convert.</param>
  /// <returns>A <see cref="FastExpressionCompiler.LightExpression.SwitchExpression"/> representing the same expression as the input.</returns>
  public static Light.SwitchExpression ToLightExpression(this Sys.SwitchExpression expr)
  => Light.Expression.Switch(
    expr.Type,
    expr.SwitchValue.ToLightExpression(),
    expr.DefaultBody.ToLightExpression(),
    expr.Comparison,
    expr.Cases.ToLightExpressions());

  /// <summary>
  /// Extension method for converting a collection of <see cref="System.Linq.Expressions.SwitchCase"/> to a collection of <see cref="FastExpressionCompiler.LightExpression.SwitchCase"/>.
  /// </summary>
  /// <param name="cases">The collection of <see cref="System.Linq.Expressions.SwitchCase"/> to convert.</param>
  /// <returns>A collection of <see cref="FastExpressionCompiler.LightExpression.SwitchCase"/> representing the same expressions as the input.</returns>
  public static IEnumerable<Light.SwitchCase> ToLightExpressions(this IEnumerable<Sys.SwitchCase> cases) {
    foreach (var expr in cases)
      yield return expr.ToLightExpression();
  }

  /// <summary>
  /// Extension method for converting a <see cref="System.Linq.Expressions.SwitchCase"/> to a <see cref="FastExpressionCompiler.LightExpression.SwitchCase"/>.
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.SwitchCase"/> to convert.</param>
  /// <returns>A <see cref="FastExpressionCompiler.LightExpression.SwitchCase"/> representing the same expression as the input.</returns>
  public static Light.SwitchCase ToLightExpression(this Sys.SwitchCase expr)
    => Light.Expression.SwitchCase(expr.Body.ToLightExpression(), expr.TestValues.ToLightExpressions());

  /// <summary>
  /// Extension method for converting a <see cref="System.Linq.Expressions.TryExpression"/> to a <see cref="FastExpressionCompiler.LightExpression.TryExpression"/>.
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.TryExpression"/> to convert.</param>
  /// <returns>A <see cref="FastExpressionCompiler.LightExpression.TryExpression"/> representing the same expression as the input.</returns>
  public static Light.TryExpression ToLightExpression(this Sys.TryExpression expr)
    => Light.Expression.TryCatch(expr.Body.ToLightExpression(), expr.Handlers.ToLightExpressions().ToArray());

  /// <summary>
  /// Extension method for converting a collection of <see cref="System.Linq.Expressions.CatchBlock"/> to a collection of <see cref="FastExpressionCompiler.LightExpression.CatchBlock"/>.
  /// </summary>
  /// <param name="blocks">The collection of <see cref="System.Linq.Expressions.CatchBlock"/> to convert.</param>
  /// <returns>A collection of <see cref="FastExpressionCompiler.LightExpression.CatchBlock"/> representing the same expressions as the input.</returns>
  public static IEnumerable<Light.CatchBlock> ToLightExpressions(this IEnumerable<Sys.CatchBlock> blocks) {
    foreach (var block in blocks)
      yield return block.ToLightExpression();
  }

  /// <summary>
  /// Extension method for converting a <see cref="System.Linq.Expressions.CatchBlock"/> to a <see cref="FastExpressionCompiler.LightExpression.CatchBlock"/>.
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.CatchBlock"/> to convert.</param>
  /// <returns>A <see cref="FastExpressionCompiler.LightExpression.CatchBlock"/> representing the same expression as the input.</returns>
  public static Light.CatchBlock ToLightExpression(this Sys.CatchBlock expr)
    => Light.Expression.MakeCatchBlock(expr.Test,
      expr.Variable?.ToLightExpression(),
      expr.Body.ToLightExpression(),
      expr.Filter.ToLightExpression());

  /// <summary>
  /// Extension method for converting a <see cref="System.Linq.Expressions.TypeBinaryExpression"/> to a <see cref="FastExpressionCompiler.LightExpression.TypeBinaryExpression"/>.
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.TypeBinaryExpression"/> to convert.</param>
  /// <returns>A <see cref="FastExpressionCompiler.LightExpression.TypeBinaryExpression"/> representing the same expression as the input.</returns>
  public static Light.TypeBinaryExpression ToLightExpression(this Sys.TypeBinaryExpression expr) {
    var exprType = expr.NodeType;
    return exprType switch {
      ExpressionType.TypeIs => Light.Expression.TypeIs(expr.Expression.ToLightExpression(), expr.TypeOperand),
      ExpressionType.TypeEqual => Light.Expression.TypeEqual(expr.Expression.ToLightExpression(), expr.TypeOperand),
      _ => throw new NotImplementedException(exprType.ToString())
    };
  }

  /// <summary>
  /// Extension method for converting a <see cref="System.Linq.Expressions.UnaryExpression"/> to a <see cref="FastExpressionCompiler.LightExpression.UnaryExpression"/>.
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.UnaryExpression"/> to convert.</param>
  /// <returns>A <see cref="FastExpressionCompiler.LightExpression.UnaryExpression"/> representing the same expression as the input.</returns>
  public static Light.UnaryExpression ToLightExpression(this Sys.UnaryExpression expr)
    => Light.Expression.MakeUnary(expr.NodeType, expr.Operand.ToLightExpression(), expr.Type);

  /// <summary>
  /// Extension method for converting a <see cref="System.Linq.Expressions.LambdaExpression"/> to a <see cref="FastExpressionCompiler.LightExpression.LambdaExpression"/>.
  /// </summary>
  /// <param name="expr">The <see cref="System.Linq.Expressions.LambdaExpression"/> to convert.</param>
  /// <returns>A <see cref="FastExpressionCompiler.LightExpression.LambdaExpression"/> representing the same expression as the input.</returns>
  public static Light.LambdaExpression ToLightExpression(this Sys.LambdaExpression expr)
    => Light.Expression.Lambda(expr.Body.ToLightExpression(), expr.Parameters.ToLightExpressions(), expr.Type);

  /// <summary>
  /// Extension method for converting a nullable <see cref="System.Linq.Expressions.Expression"/> to a nullable <see cref="FastExpressionCompiler.LightExpression.Expression"/>.
  /// </summary>
  /// <param name="expr">The nullable <see cref="System.Linq.Expressions.Expression"/> to convert.</param>
  /// <returns>A nullable <see cref="FastExpressionCompiler.LightExpression.Expression"/> representing the same expression as the input, or <c>null</c> if the input is <c>null</c>.</returns>
  [ContractAnnotation("null => null; notnull => notnull")]
  public static Light.Expression? ToLightExpression(this Sys.Expression? expr) {
    if (expr is null) return null;

    switch (expr) {
      case Sys.BinaryExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.BlockExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.ConditionalExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.ConstantExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.DebugInfoExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.DefaultExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.DynamicExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.GotoExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.IndexExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.InvocationExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.LabelExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.LambdaExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.ListInitExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.LoopExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.MemberExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.MemberInitExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.MethodCallExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.NewArrayExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.NewExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.ParameterExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.RuntimeVariablesExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.SwitchExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.TryExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.TypeBinaryExpression typedExpr: return typedExpr.ToLightExpression();
      case Sys.UnaryExpression typedExpr: return typedExpr.ToLightExpression();
      default: throw new NotImplementedException(expr.GetType().AssemblyQualifiedName);
    }
  }

}