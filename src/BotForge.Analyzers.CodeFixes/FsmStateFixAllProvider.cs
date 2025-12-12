using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BotForge.Analyzers.CodeFixes
{
    /// <summary>
    /// Implements a Fix All provider for FSM state analyzer rules. This provider enables batch fixing of multiple
    /// FSM state-related issues across a document, project, or solution.
    /// </summary>
    public class FsmStateFixAllProvider : FixAllProvider
    {
        /// <summary>
        /// Gets a singleton instance of the provider.
        /// </summary>
        public static readonly FsmStateFixAllProvider Instance = new FsmStateFixAllProvider();

        /// <summary>
        /// Prevents external instantiation of this class.
        /// </summary>
        private FsmStateFixAllProvider() { }

        /// <summary>
        /// Gets the supported fix-all scopes.
        /// </summary>
        /// <returns>The supported fix-all scopes.</returns>
        public override IEnumerable<FixAllScope> GetSupportedFixAllScopes()
        {
            return new[] 
            {
                FixAllScope.Document,
                FixAllScope.Project,
                FixAllScope.Solution
            };
        }

        /// <summary>
        /// Gets the fix for a specific diagnostic.
        /// </summary>
        /// <param name="fixAllContext">The fix all context.</param>
        /// <returns>A task that represents the asynchronous operation of applying a fix.</returns>
        public override async Task<CodeAction> GetFixAsync(FixAllContext fixAllContext)
        {
            ImmutableArray<Diagnostic> targetDiagnostics;

            if (fixAllContext.Document == null)
                targetDiagnostics = await fixAllContext.GetAllDiagnosticsAsync(fixAllContext.Project);
            else
                targetDiagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(fixAllContext.Document);

            var codeActionRegistrator = new CodeActionRegistrator();

            foreach (var diagnostic in targetDiagnostics)
            {
                // Get all the fixes available for this diagnostic
                var codeFixContext = new CodeFixContext(
                    document: fixAllContext.Document,
                    diagnostic: diagnostic,
                    registerCodeFix: (action, diagnostics) => codeActionRegistrator.RegisterCodeAction(action, diagnostics),
                    cancellationToken: fixAllContext.CancellationToken);

                // Find the appropriate code fix provider based on the diagnostic ID
                CodeFixProvider provider;
                switch (diagnostic.Id)
                {
                    case "FSM001":
                    case "FSM002":
                    case "FSM003":
                    case "FSM009":
                    case "FSM010":
                    case "FSM011":
                        provider = new FsmStateCodeFixProvider();
                        break;
                    case "FSM004":
                    case "FSM008":
                    case "FSM014":
                        provider = new MenuItemCodeFixProvider();
                        break;
                    default:
                        provider = null;
                        break;
                }

                if (provider == null) return null;

                // Apply the fix
                await provider.RegisterCodeFixesAsync(codeFixContext);
            }

            // Return the first code action (or null if none were found)
            return codeActionRegistrator.FirstCodeAction;
        }

        /// <summary>
        /// Helper class to capture registered code actions.
        /// </summary>
        private class CodeActionRegistrator
        {
            private CodeAction _firstAction;

            public CodeAction FirstCodeAction => _firstAction;

            public void RegisterCodeAction(CodeAction codeAction, ImmutableArray<Diagnostic> diagnostics)
            {
                if (_firstAction == null) _firstAction = codeAction;
            }
        }
    }
}
