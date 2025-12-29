using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using System.Collections.Immutable;

namespace BotForge.Analyzers.Tests.Helpers;

internal static class AnalyzerVerifier<TAnalyzer, TCodeFix>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
{
    public static DiagnosticResult Diagnostic(string diagnosticId)
        => CSharpCodeFixVerifier<TAnalyzer, TCodeFix, DefaultVerifier>.Diagnostic(diagnosticId);

    public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
        => new DiagnosticResult(descriptor);

    public static Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new Test<TAnalyzer>
        {
            TestCode = source,
        };

        test.ExpectedDiagnostics.AddRange(expected);

        return test.RunAsync();
    }

    public static Task VerifyCodeFixAsync(string source, string fixedSource, int codeActionIndex, params DiagnosticResult[] expected)
    {
        var test = new Test<TAnalyzer, TCodeFix>
        {
            TestCode = source,
            FixedCode = fixedSource,
            CodeActionIndex = codeActionIndex,
        };

        test.ExpectedDiagnostics.AddRange(expected);

        return test.RunAsync();
    }

    public static Task VerifyCodeFixAsync(string source, string fixedSource, params DiagnosticResult[] expected)
    {
        var test = new Test<TAnalyzer, TCodeFix>
        {
            TestCode = source,
            FixedCode = fixedSource,
        };

        test.ExpectedDiagnostics.AddRange(expected);

        return test.RunAsync();
    }

    internal class Test<T> : CSharpAnalyzerTest<T, DefaultVerifier>
        where T : DiagnosticAnalyzer, new()
    {
        public Test()
        {
            SolutionTransforms.Add((solution, projectId) =>
            {
                var compilationOptions = solution.GetProject(projectId)?.CompilationOptions;
                if (compilationOptions == null) return solution;
                compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(
                    compilationOptions.SpecificDiagnosticOptions.SetItems(GetNullableWarningsFromCompiler()));
                solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

                return solution;
            });
        }

        private static ImmutableDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
        {
            var builder = ImmutableDictionary.CreateBuilder<string, ReportDiagnostic>();
            builder.Add("CS8600", ReportDiagnostic.Error);
            builder.Add("CS8602", ReportDiagnostic.Error);
            builder.Add("CS8603", ReportDiagnostic.Error);
            builder.Add("CS8604", ReportDiagnostic.Error);
            builder.Add("CS8605", ReportDiagnostic.Error);
            builder.Add("CS8618", ReportDiagnostic.Error);
            builder.Add("CS8625", ReportDiagnostic.Error);
            builder.Add("CS8762", ReportDiagnostic.Error);
            return builder.ToImmutable();
        }
    }

    internal class Test<TDiagnosticAnalyzer, TCodeFixProvider> : CSharpCodeFixTest<TDiagnosticAnalyzer, TCodeFixProvider, DefaultVerifier>
        where TDiagnosticAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFixProvider : CodeFixProvider, new()
    {
        public Test()
        {
            SolutionTransforms.Add((solution, projectId) =>
            {
                var compilationOptions = solution.GetProject(projectId)?.CompilationOptions;
                if (compilationOptions == null) return solution;
                compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(
                    compilationOptions.SpecificDiagnosticOptions.SetItems(GetNullableWarningsFromCompiler()));
                solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

                return solution;
            });
        }

        private static ImmutableDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
        {
            var builder = ImmutableDictionary.CreateBuilder<string, ReportDiagnostic>();
            builder.Add("CS8600", ReportDiagnostic.Error);
            builder.Add("CS8602", ReportDiagnostic.Error);
            builder.Add("CS8603", ReportDiagnostic.Error);
            builder.Add("CS8604", ReportDiagnostic.Error);
            builder.Add("CS8605", ReportDiagnostic.Error);
            builder.Add("CS8618", ReportDiagnostic.Error);
            builder.Add("CS8625", ReportDiagnostic.Error);
            builder.Add("CS8762", ReportDiagnostic.Error);
            return builder.ToImmutable();
        }

        protected override async Task RunImplAsync(CancellationToken cancellationToken)
        {
            if (CodeActionIndex.HasValue)
            {
                CodeActionValidationMode = CodeActionValidationMode.None;
            }

            await base.RunImplAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}

internal static class AnalyzerVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    public static DiagnosticResult Diagnostic(string diagnosticId)
        => CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic(diagnosticId);

    public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
        => new(descriptor);

    public static Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new AnalyzerVerifier<TAnalyzer, EmptyCodeFixProvider>.Test<TAnalyzer>
        {
            TestBehaviors = TestBehaviors.SkipGeneratedCodeCheck,
            TestCode = source,
        };

        test.ExpectedDiagnostics.AddRange(expected);

        return test.RunAsync();
    }

    private class EmptyCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray<string>.Empty;

        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context) => Task.CompletedTask;
    }
}
