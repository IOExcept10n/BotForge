using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Rename;
using BotForge.Analyzers;

namespace BotForge.Analyzers.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FsmStateCodeFixProvider)), Shared]
    public class FsmStateCodeFixProvider : CodeFixProvider
    {
        // List diagnostic IDs that this provider can fix
        public sealed override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(
                "FSM001", // MenuSig
                "FSM002", // PromptSig
                "FSM003", // PromptGenericMismatch
                "FSM009", // ModelPromptSig
                "FSM010", // CustomStateSig
                "FSM011"  // ModelPromptGenericMismatch
            );

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // Not providing a batch fixer to keep fixes focused and predictable
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

            if (root == null || semanticModel == null) return;

            // Get the node representing the diagnostic
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the method declaration identified by the diagnostic
            var methodDecl = root.FindToken(diagnosticSpan.Start)
                .Parent?.AncestorsAndSelf()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault();

            if (methodDecl == null) return;

            switch (diagnostic.Id)
            {
                case "FSM001": // MenuSig
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Fix Menu method signature",
                            createChangedDocument: c => FixMenuSignatureAsync(context.Document, methodDecl, semanticModel, c),
                            equivalenceKey: "FixMenuSignature"),
                        diagnostic);
                    break;

                case "FSM002": // PromptSig
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Fix Prompt method signature",
                            createChangedDocument: c => FixPromptSignatureAsync(context.Document, methodDecl, semanticModel, c),
                            equivalenceKey: "FixPromptSignature"),
                        diagnostic);
                    break;

                case "FSM009": // ModelPromptSig
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Fix ModelPrompt method signature",
                            createChangedDocument: c => FixModelPromptSignatureAsync(context.Document, methodDecl, semanticModel, c),
                            equivalenceKey: "FixModelPromptSignature"),
                        diagnostic);
                    break;

                case "FSM010": // CustomStateSig
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Fix CustomState method signature",
                            createChangedDocument: c => FixCustomStateSignatureAsync(context.Document, methodDecl, semanticModel, c),
                            equivalenceKey: "FixCustomStateSignature"),
                        diagnostic);
                    break;

                case "FSM003": // PromptGenericMismatch
                case "FSM011": // ModelPromptGenericMismatch
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Fix generic type mismatch",
                            createChangedDocument: c => FixGenericMismatchAsync(context.Document, methodDecl, semanticModel, diagnostic, c),
                            equivalenceKey: "FixGenericMismatch"),
                        diagnostic);
                    break;
            }
        }

        private async Task<Document> FixMenuSignatureAsync(Document document, MethodDeclarationSyntax methodDecl,
                                                     SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null) return document;

            // Determine if the method already returns StateResult or Task<StateResult>
            bool returnsStateResultOrTask = IsStateResultOrTaskOfStateResult(methodDecl.ReturnType, semanticModel);

            // Create the correct return type and parameter list
            var newReturnType = SyntaxFactory.ParseTypeName("StateResult");

            var paramList = SyntaxFactory.ParameterList(
                SyntaxFactory.SeparatedList(new[] {
                SyntaxFactory.Parameter(
                    SyntaxFactory.List<AttributeListSyntax>(),
                    SyntaxFactory.TokenList(),
                    SyntaxFactory.ParseTypeName("BotForge.Modules.Contexts.SelectionStateContext"),
                    SyntaxFactory.Identifier("ctx"),
                    null)
                }));

            // Create a new method with the correct signature
            var newMethod = methodDecl
                .WithReturnType(newReturnType)
                .WithParameterList(paramList);

            // If the body contains await calls, we need to make it a Task<StateResult> method
            var awaitExpressions = methodDecl.DescendantNodes().OfType<AwaitExpressionSyntax>();
            if (awaitExpressions.Any())
            {
                newMethod = newMethod
                    .WithReturnType(SyntaxFactory.ParseTypeName("Task<StateResult>"))
                    .WithModifiers(methodDecl.Modifiers.Add(SyntaxFactory.Token(SyntaxKind.AsyncKeyword)));

                // Add CancellationToken if it had one before
                if (methodDecl.ParameterList.Parameters.Count > 1 &&
                    IsCancellationToken(methodDecl.ParameterList.Parameters[1].Type, semanticModel))
                {
                    newMethod = newMethod.WithParameterList(SyntaxFactory.ParameterList(
                        SyntaxFactory.SeparatedList(new[] {
                        SyntaxFactory.Parameter(
                            SyntaxFactory.List<AttributeListSyntax>(),
                            SyntaxFactory.TokenList(),
                            SyntaxFactory.ParseTypeName("BotForge.Modules.Contexts.SelectionStateContext"),
                            SyntaxFactory.Identifier("ctx"),
                            null),
                        SyntaxFactory.Parameter(
                            SyntaxFactory.List<AttributeListSyntax>(),
                            SyntaxFactory.TokenList(),
                            SyntaxFactory.ParseTypeName("System.Threading.CancellationToken"),
                            SyntaxFactory.Identifier("cancellationToken"),
                            null)
                        })));
                }
            }

            // Replace the old method with the new one
            var newRoot = root.ReplaceNode(methodDecl, newMethod);
            return document.WithSyntaxRoot(newRoot);
        }

        private async Task<Document> FixPromptSignatureAsync(Document document, MethodDeclarationSyntax methodDecl,
                                                     SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null) return document;

            // Find the Prompt attribute to get the generic argument
            var attributes = methodDecl.AttributeLists
                .SelectMany(al => al.Attributes)
                .Where(a => IsPromptAttribute(a, semanticModel));

            var promptAttribute = attributes.FirstOrDefault();
            if (promptAttribute == null) return document;

            var genericType = GetGenericTypeFromAttribute(promptAttribute, semanticModel);
            if (genericType == null) return document;

            // Create the correct return type and parameter list
            var newReturnType = SyntaxFactory.ParseTypeName("StateResult");

            var paramList = SyntaxFactory.ParameterList(
                SyntaxFactory.SeparatedList(new[] {
                SyntaxFactory.Parameter(
                    SyntaxFactory.List<AttributeListSyntax>(),
                    SyntaxFactory.TokenList(),
                    SyntaxFactory.ParseTypeName($"BotForge.Modules.Contexts.PromptStateContext<{genericType}>"),
                    SyntaxFactory.Identifier("ctx"),
                    null)
                }));

            // Create a new method with the correct signature
            var newMethod = methodDecl
                .WithReturnType(newReturnType)
                .WithParameterList(paramList);

            // If the body contains await calls, we need to make it a Task<StateResult> method
            var awaitExpressions = methodDecl.DescendantNodes().OfType<AwaitExpressionSyntax>();
            if (awaitExpressions.Any())
            {
                newMethod = newMethod
                    .WithReturnType(SyntaxFactory.ParseTypeName("Task<StateResult>"))
                    .WithModifiers(methodDecl.Modifiers.Add(SyntaxFactory.Token(SyntaxKind.AsyncKeyword)));

                // Add CancellationToken if it had one before
                if (methodDecl.ParameterList.Parameters.Count > 1 &&
                    IsCancellationToken(methodDecl.ParameterList.Parameters[1].Type, semanticModel))
                {
                    newMethod = newMethod.WithParameterList(SyntaxFactory.ParameterList(
                        SyntaxFactory.SeparatedList(new[] {
                        SyntaxFactory.Parameter(
                            SyntaxFactory.List<AttributeListSyntax>(),
                            SyntaxFactory.TokenList(),
                            SyntaxFactory.ParseTypeName($"BotForge.Modules.Contexts.PromptStateContext<{genericType}>"),
                            SyntaxFactory.Identifier("ctx"),
                            null),
                        SyntaxFactory.Parameter(
                            SyntaxFactory.List<AttributeListSyntax>(),
                            SyntaxFactory.TokenList(),
                            SyntaxFactory.ParseTypeName("System.Threading.CancellationToken"),
                            SyntaxFactory.Identifier("cancellationToken"),
                            null)
                        })));
                }
            }

            // Replace the old method with the new one
            var newRoot = root.ReplaceNode(methodDecl, newMethod);
            return document.WithSyntaxRoot(newRoot);
        }

        private async Task<Document> FixModelPromptSignatureAsync(Document document, MethodDeclarationSyntax methodDecl,
                                                      SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null) return document;

            // Find the ModelPrompt attribute to get the generic argument
            var attributes = methodDecl.AttributeLists
                .SelectMany(al => al.Attributes)
                .Where(a => IsModelPromptAttribute(a, semanticModel));

            var promptAttribute = attributes.FirstOrDefault();
            if (promptAttribute == null) return document;

            var genericType = GetGenericTypeFromAttribute(promptAttribute, semanticModel);
            if (genericType == null) return document;

            // Create the correct return type and parameter list
            var newReturnType = SyntaxFactory.ParseTypeName("StateResult");

            var paramList = SyntaxFactory.ParameterList(
                SyntaxFactory.SeparatedList(new[] {
                SyntaxFactory.Parameter(
                    SyntaxFactory.List<AttributeListSyntax>(),
                    SyntaxFactory.TokenList(),
                    SyntaxFactory.ParseTypeName($"BotForge.Modules.Contexts.ModelPromptContext<{genericType}>"),
                    SyntaxFactory.Identifier("ctx"),
                    null)
                }));

            // Create a new method with the correct signature
            var newMethod = methodDecl
                .WithReturnType(newReturnType)
                .WithParameterList(paramList);

            // If the body contains await calls, we need to make it a Task<StateResult> method
            var awaitExpressions = methodDecl.DescendantNodes().OfType<AwaitExpressionSyntax>();
            if (awaitExpressions.Any())
            {
                newMethod = newMethod
                    .WithReturnType(SyntaxFactory.ParseTypeName("Task<StateResult>"))
                    .WithModifiers(methodDecl.Modifiers.Add(SyntaxFactory.Token(SyntaxKind.AsyncKeyword)));

                // Add CancellationToken if it had one before
                if (methodDecl.ParameterList.Parameters.Count > 1 &&
                    IsCancellationToken(methodDecl.ParameterList.Parameters[1].Type, semanticModel))
                {
                    newMethod = newMethod.WithParameterList(SyntaxFactory.ParameterList(
                        SyntaxFactory.SeparatedList(new[] {
                        SyntaxFactory.Parameter(
                            SyntaxFactory.List<AttributeListSyntax>(),
                            SyntaxFactory.TokenList(),
                            SyntaxFactory.ParseTypeName($"BotForge.Modules.Contexts.ModelPromptContext<{genericType}>"),
                            SyntaxFactory.Identifier("ctx"),
                            null),
                        SyntaxFactory.Parameter(
                            SyntaxFactory.List<AttributeListSyntax>(),
                            SyntaxFactory.TokenList(),
                            SyntaxFactory.ParseTypeName("System.Threading.CancellationToken"),
                            SyntaxFactory.Identifier("cancellationToken"),
                            null)
                        })));
                }
            }

            // Replace the old method with the new one
            var newRoot = root.ReplaceNode(methodDecl, newMethod);
            return document.WithSyntaxRoot(newRoot);
        }

        private async Task<Document> FixCustomStateSignatureAsync(Document document, MethodDeclarationSyntax methodDecl,
                                                      SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null) return document;

            // Create the correct return type and parameter list
            var newReturnType = SyntaxFactory.ParseTypeName("StateResult");

            var paramList = SyntaxFactory.ParameterList(
                SyntaxFactory.SeparatedList(new[] {
                SyntaxFactory.Parameter(
                    SyntaxFactory.List<AttributeListSyntax>(),
                    SyntaxFactory.TokenList(),
                    SyntaxFactory.ParseTypeName("BotForge.Modules.Contexts.ModuleStateContext"),
                    SyntaxFactory.Identifier("ctx"),
                    null)
                }));

            // Create a new method with the correct signature
            var newMethod = methodDecl
                .WithReturnType(newReturnType)
                .WithParameterList(paramList);

            // If the body contains await calls, we need to make it a Task<StateResult> method
            var awaitExpressions = methodDecl.DescendantNodes().OfType<AwaitExpressionSyntax>();
            if (awaitExpressions.Any())
            {
                newMethod = newMethod
                    .WithReturnType(SyntaxFactory.ParseTypeName("Task<StateResult>"))
                    .WithModifiers(methodDecl.Modifiers.Add(SyntaxFactory.Token(SyntaxKind.AsyncKeyword)));

                // Add CancellationToken if it had one before
                if (methodDecl.ParameterList.Parameters.Count > 1 &&
                    IsCancellationToken(methodDecl.ParameterList.Parameters[1].Type, semanticModel))
                {
                    newMethod = newMethod.WithParameterList(SyntaxFactory.ParameterList(
                        SyntaxFactory.SeparatedList(new[] {
                        SyntaxFactory.Parameter(
                            SyntaxFactory.List<AttributeListSyntax>(),
                            SyntaxFactory.TokenList(),
                            SyntaxFactory.ParseTypeName("BotForge.Modules.Contexts.ModuleStateContext"),
                            SyntaxFactory.Identifier("ctx"),
                            null),
                        SyntaxFactory.Parameter(
                            SyntaxFactory.List<AttributeListSyntax>(),
                            SyntaxFactory.TokenList(),
                            SyntaxFactory.ParseTypeName("System.Threading.CancellationToken"),
                            SyntaxFactory.Identifier("cancellationToken"),
                            null)
                        })));
                }
            }

            // Replace the old method with the new one
            var newRoot = root.ReplaceNode(methodDecl, newMethod);
            return document.WithSyntaxRoot(newRoot);
        }

        private async Task<Document> FixGenericMismatchAsync(Document document, MethodDeclarationSyntax methodDecl,
                                                   SemanticModel semanticModel, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null) return document;

            // Find the attribute and parameter to determine which one to update
            AttributeSyntax promptAttribute = null;

            // Find if this is a Prompt or ModelPrompt method
            var attributes = methodDecl.AttributeLists
                .SelectMany(al => al.Attributes);

            foreach (var attr in attributes)
            {
                if (IsPromptAttribute(attr, semanticModel) || IsModelPromptAttribute(attr, semanticModel))
                {
                    promptAttribute = attr;
                    break;
                }
            }

            if (promptAttribute == null) return document;

            // We have two options:
            // 1. Update the attribute type to match parameter
            // 2. Update the parameter type to match attribute

            // Let's update attribute to match parameter, which is generally safer

            // First, find the parameter type
            var parameter = methodDecl.ParameterList.Parameters.FirstOrDefault();
            if (parameter == null || !(parameter.Type is GenericNameSyntax genericParam)) return document;

            // Extract the generic argument from the parameter
            var paramGenericArg = genericParam.TypeArgumentList.Arguments.FirstOrDefault()?.ToString();
            if (string.IsNullOrEmpty(paramGenericArg)) return document;

            // Get the attribute name without the generic part
            string attributeName;
            if (IsPromptAttribute(promptAttribute, semanticModel))
            {
                attributeName = "Prompt";
            }
            else if (IsModelPromptAttribute(promptAttribute, semanticModel))
            {
                attributeName = "ModelPrompt";
            }
            else
            {
                return document;
            }

            // Create a new attribute with the correct generic type
            var newAttribute = SyntaxFactory.Attribute(
                SyntaxFactory.ParseName($"{attributeName}<{paramGenericArg}>"),
                promptAttribute.ArgumentList);

            // Replace the old attribute with the new one
            var newRoot = root.ReplaceNode(promptAttribute, newAttribute);
            return document.WithSyntaxRoot(newRoot);
        }

        #region Helper Methods

        private bool IsStateResultOrTaskOfStateResult(TypeSyntax typeSyntax, SemanticModel semanticModel)
        {
            var typeInfo = semanticModel.GetTypeInfo(typeSyntax);
            if (typeInfo.Type == null) return false;

            // Check if it's StateResult
            var stateResultType = semanticModel.Compilation.GetTypeByMetadataName("BotForge.Fsm.StateResult");
            if (stateResultType != null && SymbolEqualityComparer.Default.Equals(typeInfo.Type, stateResultType))
            {
                return true;
            }

            // Check if it's Task<StateResult>
            var taskType = semanticModel.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");
            if (taskType != null && typeInfo.Type is INamedTypeSymbol namedType &&
                SymbolEqualityComparer.Default.Equals(namedType.ConstructedFrom, taskType))
            {
                var typeArgs = namedType.TypeArguments;
                if (typeArgs.Length == 1 && stateResultType != null &&
                    SymbolEqualityComparer.Default.Equals(typeArgs[0], stateResultType))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsCancellationToken(TypeSyntax typeSyntax, SemanticModel semanticModel)
        {
            var typeInfo = semanticModel.GetTypeInfo(typeSyntax);
            if (typeInfo.Type == null) return false;

            var cancellationTokenType = semanticModel.Compilation.GetTypeByMetadataName("System.Threading.CancellationToken");
            return cancellationTokenType != null && SymbolEqualityComparer.Default.Equals(typeInfo.Type, cancellationTokenType);
        }

        private bool IsPromptAttribute(AttributeSyntax attributeSyntax, SemanticModel semanticModel)
        {
            var symbol = semanticModel.GetSymbolInfo(attributeSyntax).Symbol?.ContainingType;
            if (symbol == null) return false;

            return symbol.OriginalDefinition.ToDisplayString() == "BotForge.Modules.Attributes.PromptAttribute`1";
        }

        private bool IsModelPromptAttribute(AttributeSyntax attributeSyntax, SemanticModel semanticModel)
        {
            var symbol = semanticModel.GetSymbolInfo(attributeSyntax).Symbol?.ContainingType;
            if (symbol == null) return false;

            return symbol.OriginalDefinition.ToDisplayString() == "BotForge.Modules.Attributes.ModelPromptAttribute`1";
        }

        private string GetGenericTypeFromAttribute(AttributeSyntax attributeSyntax, SemanticModel semanticModel)
        {
            var attributeSymbol = semanticModel.GetSymbolInfo(attributeSyntax).Symbol?.ContainingType;
            if (attributeSymbol == null || !attributeSymbol.IsGenericType) return null;

            var typeArg = attributeSymbol.TypeArguments.FirstOrDefault();
            return typeArg?.ToDisplayString();
        }

        #endregion
    }
}
