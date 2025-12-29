using System.Collections.Generic;
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

namespace BotForge.Analyzers.CodeFixes
{
    /// <summary>
    /// Provides code fixes for menu item attributes in FSM states.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MenuItemCodeFixProvider)), Shared]
    public class MenuItemCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public sealed override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(
                "FSM004", // MenuItemWithoutMenu
                "FSM008", // KeyboardInstructionWithoutMenu
                "FSM014"  // MultipleStateAttributes
            );

        /// <inheritdoc/>
        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
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
                case "FSM004": // MenuItemWithoutMenu
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Add [Menu] attribute",
                            createChangedDocument: c => AddMenuAttributeAsync(context.Document, methodDecl, c),
                            equivalenceKey: "AddMenuAttribute"),
                        diagnostic);
                    
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Add [CustomState] attribute",
                            createChangedDocument: c => AddCustomStateAttributeAsync(context.Document, methodDecl, c),
                            equivalenceKey: "AddCustomStateAttribute"),
                        diagnostic);
                    
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Remove [MenuItem]/[MenuRow] attributes",
                            createChangedDocument: c => RemoveMenuItemAttributesAsync(context.Document, methodDecl, c),
                            equivalenceKey: "RemoveMenuItemAttributes"),
                        diagnostic);
                    break;
                
                case "FSM008": // KeyboardInstructionWithoutMenu
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Add [Menu] attribute",
                            createChangedDocument: c => AddMenuAttributeAsync(context.Document, methodDecl, c),
                            equivalenceKey: "AddMenuAttribute"),
                        diagnostic);
                    
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Add [CustomState] attribute",
                            createChangedDocument: c => AddCustomStateAttributeAsync(context.Document, methodDecl, c),
                            equivalenceKey: "AddCustomStateAttribute"),
                        diagnostic);
                    
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Remove keyboard attributes",
                            createChangedDocument: c => RemoveKeyboardAttributesAsync(context.Document, methodDecl, c),
                            equivalenceKey: "RemoveKeyboardAttributes"),
                        diagnostic);
                    break;
                
                case "FSM014": // MultipleStateAttributes
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Keep only [Menu] attribute",
                            createChangedDocument: c => KeepOnlyOneStateAttributeAsync(context.Document, methodDecl, "Menu", c),
                            equivalenceKey: "KeepOnlyMenuAttribute"),
                        diagnostic);
                    
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Keep only [Prompt<T>] attribute",
                            createChangedDocument: c => KeepOnlyOneStateAttributeAsync(context.Document, methodDecl, "Prompt", c),
                            equivalenceKey: "KeepOnlyPromptAttribute"),
                        diagnostic);
                    
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Keep only [ModelPrompt<T>] attribute",
                            createChangedDocument: c => KeepOnlyOneStateAttributeAsync(context.Document, methodDecl, "ModelPrompt", c),
                            equivalenceKey: "KeepOnlyModelPromptAttribute"),
                        diagnostic);
                    
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Keep only [CustomState] attribute",
                            createChangedDocument: c => KeepOnlyOneStateAttributeAsync(context.Document, methodDecl, "CustomState", c),
                            equivalenceKey: "KeepOnlyCustomStateAttribute"),
                        diagnostic);
                    break;
            }
        }

        private async Task<Document> AddMenuAttributeAsync(Document document, MethodDeclarationSyntax methodDecl,
                                                  CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null) return document;

            // Create a [Menu("PromptText")] attribute
            var menuAttribute = SyntaxFactory.Attribute(
                SyntaxFactory.ParseName("BotForge.Modules.Attributes.Menu"),
                SyntaxFactory.AttributeArgumentList(
                    SyntaxFactory.SeparatedList<AttributeArgumentSyntax>(
                        new[] { SyntaxFactory.AttributeArgument(
                            SyntaxFactory.LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                SyntaxFactory.Literal("PromptText")
                            )
                        ) }
                    )
                )
            );

            // Create an attribute list with the Menu attribute
            var attributeList = SyntaxFactory.AttributeList(
                SyntaxFactory.SingletonSeparatedList(menuAttribute));

            // Add the attribute list to the method
            var newMethod = methodDecl.AddAttributeLists(attributeList);

            // Replace the old method with the new one
            var newRoot = root.ReplaceNode(methodDecl, newMethod);
            return document.WithSyntaxRoot(newRoot);
        }

        private async Task<Document> AddCustomStateAttributeAsync(Document document, MethodDeclarationSyntax methodDecl,
                                                       CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null) return document;

            // Create a [CustomState("PromptText")] attribute
            var customStateAttribute = SyntaxFactory.Attribute(
                SyntaxFactory.ParseName("BotForge.Modules.Attributes.CustomState"),
                SyntaxFactory.AttributeArgumentList(
                    SyntaxFactory.SeparatedList<AttributeArgumentSyntax>(
                        new[] { SyntaxFactory.AttributeArgument(
                            SyntaxFactory.LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                SyntaxFactory.Literal("PromptText")
                            )
                        ) }
                    )
                )
            );

            // Create an attribute list with the CustomState attribute
            var attributeList = SyntaxFactory.AttributeList(
                SyntaxFactory.SingletonSeparatedList(customStateAttribute));

            // Add the attribute list to the method
            var newMethod = methodDecl.AddAttributeLists(attributeList);

            // Replace the old method with the new one
            var newRoot = root.ReplaceNode(methodDecl, newMethod);
            return document.WithSyntaxRoot(newRoot);
        }

        private async Task<Document> RemoveMenuItemAttributesAsync(Document document, MethodDeclarationSyntax methodDecl,
                                                        CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null) return document;

            // Find all MenuItem and MenuRow attributes
            var attributeLists = new List<AttributeListSyntax>();
            var attributesToRemove = new List<AttributeSyntax>();

            foreach (var attrList in methodDecl.AttributeLists)
            {
                foreach (var attr in attrList.Attributes)
                {
                    var name = attr.Name.ToString();
                    if (name.Contains("MenuItem") || name.Contains("MenuRow"))
                    {
                        attributesToRemove.Add(attr);
                    }
                }

                if (attrList.Attributes.All(a => attributesToRemove.Contains(a)))
                {
                    attributeLists.Add(attrList);
                }
            }

            // Create a new method without the MenuItem/MenuRow attributes
            var newMethod = methodDecl;
            foreach (var attrList in attributeLists)
            {
                newMethod = newMethod.RemoveNode(attrList, SyntaxRemoveOptions.KeepNoTrivia);
            }

            // Replace the old method with the new one
            var newRoot = root.ReplaceNode(methodDecl, newMethod);
            return document.WithSyntaxRoot(newRoot);
        }

        private async Task<Document> RemoveKeyboardAttributesAsync(Document document, MethodDeclarationSyntax methodDecl,
                                                        CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null) return document;

            // Find all keyboard-related attributes
            var attributeLists = new List<AttributeListSyntax>();
            var attributesToRemove = new List<AttributeSyntax>();

            foreach (var attrList in methodDecl.AttributeLists)
            {
                foreach (var attr in attrList.Attributes)
                {
                    var name = attr.Name.ToString();
                    if (name.Contains("InheritKeyboard") || name.Contains("RemoveKeyboard"))
                    {
                        attributesToRemove.Add(attr);
                    }
                }

                if (attrList.Attributes.All(a => attributesToRemove.Contains(a)))
                {
                    attributeLists.Add(attrList);
                }
            }

            // Create a new method without the keyboard attributes
            var newMethod = methodDecl;
            foreach (var attrList in attributeLists)
            {
                newMethod = newMethod.RemoveNode(attrList, SyntaxRemoveOptions.KeepNoTrivia);
            }

            // Replace the old method with the new one
            var newRoot = root.ReplaceNode(methodDecl, newMethod);
            return document.WithSyntaxRoot(newRoot);
        }

        private async Task<Document> KeepOnlyOneStateAttributeAsync(Document document, MethodDeclarationSyntax methodDecl,
                                                          string attributeNameToKeep, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null) return document;

            // Find all state attributes
            var attributeLists = new List<AttributeListSyntax>();
            var attributesToRemove = new List<AttributeSyntax>();

            foreach (var attrList in methodDecl.AttributeLists)
            {
                foreach (var attr in attrList.Attributes)
                {
                    var name = attr.Name.ToString();
                
                    // Check if this is a state attribute that should be removed
                    if ((name.Contains("Menu") || name.Contains("Prompt") || name.Contains("CustomState") || name.Contains("ModelPrompt")) && 
                        !name.Contains(attributeNameToKeep))
                    {
                        attributesToRemove.Add(attr);
                    }
                }

                if (attrList.Attributes.All(a => attributesToRemove.Contains(a)))
                {
                    attributeLists.Add(attrList);
                }
            }

            // Create a new method without the unwanted state attributes
            var newMethod = methodDecl;
            foreach (var attrList in attributeLists)
            {
                newMethod = newMethod.RemoveNode(attrList, SyntaxRemoveOptions.KeepNoTrivia);
            }

            // Replace the old method with the new one
            var newRoot = root.ReplaceNode(methodDecl, newMethod);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
