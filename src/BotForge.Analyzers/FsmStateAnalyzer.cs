using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace BotForge.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class FsmStateAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "Usage";

        private ImmutableArray<INamedTypeSymbol> _resourceClasses = ImmutableArray<INamedTypeSymbol>.Empty;
        private ImmutableArray<INamedTypeSymbol> _labelClasses = ImmutableArray<INamedTypeSymbol>.Empty;

        #region Diagnostic Descriptors

        // Method signature diagnostics
        private readonly static DiagnosticDescriptor MenuSig = new DiagnosticDescriptor(
            id: "FSM001",
            title: "Menu state method signature",
            messageFormat: "Method '{0}' has incorrect signature for [Menu]. Expected: (a)sync (StateResult|Task<StateResult>) Method(SelectionStateContext [, CancellationToken]).",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "The [MenuAttribute] is needed to automatically generate chatbot FSM states based on marked method. " +
            "This method will be used as handler for created state, so the specific signature for handling is required.");

        private readonly static DiagnosticDescriptor PromptSig = new DiagnosticDescriptor(
            id: "FSM002",
            title: "Prompt state method signature",
            messageFormat: "Method '{0}' has incorrect signature for [Prompt<T>]. Expected: (a)sync (StateResult|Task<StateResult>) Method(PromptStateContext<T> [, CancellationToken]).",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "The [PromptAttribute] is needed to automatically generate chatbot FSM states based on marked method. " +
            "This method will be used as handler for created state, so the specific signature for handling is required.");

        private readonly static DiagnosticDescriptor ModelPromptSig = new DiagnosticDescriptor(
            id: "FSM009",
            title: "ModelPrompt state method signature",
            messageFormat: "Method '{0}' has incorrect signature for [ModelPrompt<T>]. Expected: (a)sync (StateResult|Task<StateResult>) Method(ModelPromptContext<T> [, CancellationToken]).",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "The [ModelPromptAttribute] is needed to automatically generate chatbot FSM states based on marked method. " +
            "This method will be used as handler for created state, so the specific signature for handling is required.");

        private readonly static DiagnosticDescriptor CustomStateSig = new DiagnosticDescriptor(
            id: "FSM010",
            title: "CustomState method signature",
            messageFormat: "Method '{0}' has incorrect signature for [CustomState]. Expected: (a)sync (StateResult|Task<StateResult>) Method(ModuleStateContext [, CancellationToken]).",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "The [CustomStateAttribute] is needed to automatically generate chatbot FSM states based on marked method. " +
            "This method will be used as handler for created state, so the specific signature for handling is required.");

        // Type mismatch diagnostics
        private readonly static DiagnosticDescriptor PromptGenericMismatch = new DiagnosticDescriptor(
            id: "FSM003",
            title: "Prompt state generic mismatch",
            messageFormat: "Generic argument of [Prompt<{0}>] does not match method parameter PromptStateContext<{1}>",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "When using [PromptAttribute], you create a method that requests user for data of specific type. " +
            "This type is inferred based on attribute, and then cased as a method parameter. " +
            "These types must match because they represent the same type requested from user.");

        private readonly static DiagnosticDescriptor ModelPromptGenericMismatch = new DiagnosticDescriptor(
            id: "FSM011",
            title: "ModelPrompt state generic mismatch",
            messageFormat: "Generic argument of [ModelPrompt<{0}>] does not match method parameter ModelPromptContext<{1}>",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "When using [PromptAttribute], you create a method that requests user for data of specific complex type. " +
            "This type is inferred based on attribute, and then cased as a method parameter. " +
            "These types must match because they represent the same type requested from user.");

        // Menu items and buttons diagnostics
        private readonly static DiagnosticDescriptor MenuItemWithoutMenu = new DiagnosticDescriptor(
            id: "FSM004",
            title: "MenuItem or MenuRow without Menu/CustomState",
            messageFormat: "[MenuItem] or [MenuRow] used on method '{0}' without [Menu] or [CustomState]",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Attributes for generating quick response buttons, [MenuItemAttribute] and [MenuRowAttribute] are used with menu states to provide selection alternatives to user. " +
            "Other state types do not implement choice but do ask user for specific data, so they basically don't need generated buttons.");

        private readonly static DiagnosticDescriptor MenuItemLabelNotFound = new DiagnosticDescriptor(
            id: "FSM005",
            title: "MenuItem or MenuRow label not found",
            messageFormat: "MenuItem or MenuRow label '{0}' is not a public static readonly ButtonLabel member of a class marked with [LabelStorage]",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "If you use a dedicated button labels helper class, you should use it to make references from button definitions to its members. " +
            "Try referencing specific button label from your label storage by adding nameof operator, e.g. MenuItem(nameof(Labels.MyLabel)). " +
            "Usage of this operator will help you separate architecture and provide concise references between your localizable texts and program logic.");

        // Resource and localization diagnostics
        private readonly static DiagnosticDescriptor LocalizationKeyNotFound = new DiagnosticDescriptor(
            id: "FSM006",
            title: "Localization key not found",
            messageFormat: "Localization key '{0}' not found in project resources or string members",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "If you use application resources in your code, it is recommended to provide keys to localized text instead of typing it directly. " +
            "This helps you separate your app logic and static resources. Also you can provide a simple way to localize your application to user language. " +
            "You can reference a static resource by adding a nameof operator, e.g. [Menu(nameof(MyResources.MyMenuLabel))].");

        private readonly static DiagnosticDescriptor SuggestAddResources = new DiagnosticDescriptor(
            id: "FSM012",
            title: "Missing resource files",
            messageFormat: "Consider adding resource files for localization to support keys like '{0}'",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: "The BotForge library supports localization using ResourceManager classes. " +
            "You can add a RESX localization file to your project and then configure it by calling builder.Services.AddLocalization(MyResources.ResourceManager). " +
            "Then, you will be able to reference your localization file by using a nameof operator, e.g. [Menu(nameof(MyResources.MyMenuLabel))].");

        private readonly static DiagnosticDescriptor SuggestAddLabelStorage = new DiagnosticDescriptor(
            id: "FSM013",
            title: "Missing label storage",
            messageFormat: "Consider adding a class with [LabelStorage] attribute to define button labels",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: "The BotForge library have support for helper classes where you can place all your static button labels data. " +
            "You can create such a class, mark it with [LabelStorage] attribute and then add to it static readonly properties or fields with button labels. " +
            "They will be located, used and localized automatically at runtime. This helps you with separating app logic and data.");

        // Type nullability diagnostics
        private readonly static DiagnosticDescriptor NullabilityMismatch = new DiagnosticDescriptor(
            id: "FSM007",
            title: "Nullability mismatch",
            messageFormat: "Nullability mismatch between attribute generic argument and method parameter: '{0}' vs '{1}'",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Your prompt types are matching, but their nullability doesn't match. Consider having them both null-disallowing. " +
            "Note that user cannot input nothing, so when your state enters, you will have either the text input of your requested type or file with specified ID.");

        // Keyboard diagnostics
        private readonly static DiagnosticDescriptor KeyboardInstructionWithoutMenu = new DiagnosticDescriptor(
            id: "FSM008",
            title: "Keyboard instruction without Menu/CustomState",
            messageFormat: "Keyboard instruction [{0}] is used on method {1} without [Menu] or [CustomState]",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Keyboard management attributes such as [RemoveKeyboardAttribute] and [InheritKeyboardAttribute] are supported only in states that provide to user helper keyboard. " +
            "If your state is a prompt, it doesn't require a keyboard at all (or this keyboard will only have 'Cancel' key to cancel prompt), so they don't need keyboard management.");

        // Multiple state attributes
        private readonly static DiagnosticDescriptor MultipleStateAttributes = new DiagnosticDescriptor(
            id: "FSM014",
            title: "Multiple state attributes",
            messageFormat: "Method '{0}' has multiple state attributes. Only one of [Menu], [Prompt<T>], [ModelPrompt<T>] or [CustomState] should be used.",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "A single method can have only one state attribute because they are used to determine handler type. " +
            "If you specify multiple attributes, only one will be selected, and their priority is not guaranteed.");

        #endregion

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(
                MenuSig,
                PromptSig,
                ModelPromptSig,
                CustomStateSig,
                PromptGenericMismatch,
                ModelPromptGenericMismatch,
                MenuItemWithoutMenu,
                MenuItemLabelNotFound,
                LocalizationKeyNotFound,
                NullabilityMismatch,
                KeyboardInstructionWithoutMenu,
                SuggestAddResources,
                SuggestAddLabelStorage,
                MultipleStateAttributes);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(compilationStartContext =>
            {
                compilationStartContext.RegisterSymbolAction(AnalyzeResourceClass, SymbolKind.NamedType);
            });
            
            context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
            context.RegisterSyntaxNodeAction(AnalyzeAttributeSyntax, Microsoft.CodeAnalysis.CSharp.SyntaxKind.Attribute);
        }

        private void AnalyzeResourceClass(SymbolAnalysisContext context)
        {
            var symbol = (INamedTypeSymbol)context.Symbol;

            var resourceAttributes = new[]
            {
                "System.CodeDom.Compiler.GeneratedCodeAttribute",
                "System.Runtime.CompilerServices.CompilerGeneratedAttribute"
            };

            if (symbol.GetAttributes().Any(attr => resourceAttributes.Contains(attr.AttributeClass.OriginalDefinition.ToString())) && IsResourceClass(symbol))
            {
                // Collect resource classes.
                _resourceClasses = _resourceClasses.Add(symbol);
                return;
            }

            if (symbol.GetAttributes().Any(a => a.AttributeClass.OriginalDefinition.ToString() == "BotForge.Messaging.LabelStorageAttribute"))
            {
                // Collect label classes.
                _labelClasses = _labelClasses.Add(symbol);
            }
        }

        private void AnalyzeMethod(SymbolAnalysisContext context)
        {
            var method = (IMethodSymbol)context.Symbol;

            var attrs = method.GetAttributes();
            if (attrs.Length == 0) return;

            var menuAttr = attrs.FirstOrDefault(a => GetFullMetadataName(a.AttributeClass) == "BotForge.Modules.Attributes.MenuAttribute");
            var promptAttr = attrs.FirstOrDefault(a => GetFullMetadataName(a.AttributeClass) == "BotForge.Modules.Attributes.PromptAttribute`1");
            var modelPromptAttr = attrs.FirstOrDefault(a => GetFullMetadataName(a.AttributeClass) == "BotForge.Modules.Attributes.ModelPromptAttribute`1");
            var customStateAttr = attrs.FirstOrDefault(a => GetFullMetadataName(a.AttributeClass) == "BotForge.Modules.Attributes.CustomStateAttribute");

            var menuItemAttrs = attrs.Where(a => a.AttributeClass?.ToDisplayString() == "BotForge.Modules.Attributes.MenuItemAttribute").ToImmutableArray();
            var menuRowAttrs = attrs.Where(a => a.AttributeClass?.ToDisplayString() == "BotForge.Modules.Attributes.MenuRowAttribute").ToImmutableArray();
            var keyboardAttrs = attrs.Where(a =>
                a.AttributeClass?.ToDisplayString() == "BotForge.Modules.Attributes.InheritKeyboardAttribute" ||
                a.AttributeClass?.ToDisplayString() == "BotForge.Modules.Attributes.RemoveKeyboardAttribute")
                .ToImmutableArray();

            // Check for multiple state attributes
            var stateAttributes = new[] { menuAttr, promptAttr, modelPromptAttr, customStateAttr }.Where(a => a != null).ToArray();
            if (stateAttributes.Length > 1)
            {
                context.ReportDiagnostic(Diagnostic.Create(MultipleStateAttributes, method.Locations.FirstOrDefault() ?? method.Locations[0], method.Name));
            }

            // Check access and instance requirement if any of the relevant attributes present
            if (menuAttr != null || promptAttr != null || modelPromptAttr != null || customStateAttr != null ||
                menuItemAttrs.Length > 0 || menuRowAttrs.Length > 0 || keyboardAttrs.Length > 0)
            {
                if (method.IsStatic || method.DeclaredAccessibility != Accessibility.Public)
                {
                    if (menuAttr != null)
                        context.ReportDiagnostic(Diagnostic.Create(MenuSig, method.Locations.FirstOrDefault() ?? method.Locations[0], method.Name));
                    if (promptAttr != null)
                        context.ReportDiagnostic(Diagnostic.Create(PromptSig, method.Locations.FirstOrDefault() ?? method.Locations[0], method.Name));
                    if (modelPromptAttr != null)
                        context.ReportDiagnostic(Diagnostic.Create(ModelPromptSig, method.Locations.FirstOrDefault() ?? method.Locations[0], method.Name));
                    if (customStateAttr != null)
                        context.ReportDiagnostic(Diagnostic.Create(CustomStateSig, method.Locations.FirstOrDefault() ?? method.Locations[0], method.Name));
                }
            }

            // MenuState signature checks
            if (menuAttr != null)
            {
                // Check signature: must be StateResult or Task<StateResult>, taking SelectionStateContext [, CancellationToken]
                CheckMethodSignature(context, method, "BotForge.Modules.Contexts.SelectionStateContext", MenuSig);
            }

            // PromptState signature checks
            if (promptAttr != null)
            {
                // Check signature: must be StateResult or Task<StateResult>, taking PromptStateContext<T> [, CancellationToken]
                CheckPromptMethodSignature(context, method, promptAttr, "BotForge.Modules.Contexts.PromptStateContext`1", PromptSig, PromptGenericMismatch);
            }

            // ModelPromptState signature checks
            if (modelPromptAttr != null)
            {
                // Check signature: must be StateResult or Task<StateResult>, taking ModelPromptContext<T> [, CancellationToken]
                CheckPromptMethodSignature(context, method, modelPromptAttr, "BotForge.Modules.Contexts.ModelPromptContext`1", ModelPromptSig, ModelPromptGenericMismatch);
            }

            // CustomState signature checks
            if (customStateAttr != null)
            {
                // Check signature: must be StateResult or Task<StateResult>, taking ModuleStateContext [, CancellationToken]
                CheckMethodSignature(context, method, "BotForge.Modules.Contexts.ModuleStateContext", CustomStateSig);
            }

            // Check MenuItems and MenuRows
            var hasMenuOrCustomState = menuAttr != null || customStateAttr != null;
            if (!hasMenuOrCustomState && (menuItemAttrs.Length > 0 || menuRowAttrs.Length > 0) && method.Name != "OnModuleRoot")
            {
                context.ReportDiagnostic(Diagnostic.Create(MenuItemWithoutMenu, method.Locations.FirstOrDefault() ?? method.Locations[0], method.Name));
            }

            // Check keyboard instructions
            if (!hasMenuOrCustomState && keyboardAttrs.Length > 0 && method.Name != "OnModuleRoot")
            {
                foreach (var attr in keyboardAttrs)
                {
                    context.ReportDiagnostic(Diagnostic.Create(KeyboardInstructionWithoutMenu, method.Locations.FirstOrDefault() ?? method.Locations[0], attr.AttributeClass?.Name ?? "Keyboard", method.Name));
                }
            }
        }

        private void CheckMethodSignature(SymbolAnalysisContext context, IMethodSymbol method, string expectedContextType, DiagnosticDescriptor diagnostic)
        {
            // Check return type: StateResult or Task<StateResult>
            bool validReturnType = IsStateResult(method.ReturnType, context.Compilation) ||
                                  IsTaskOfStateResult(method.ReturnType, context.Compilation);

            if (!validReturnType)
            {
                context.ReportDiagnostic(Diagnostic.Create(diagnostic, method.Locations.FirstOrDefault() ?? method.Locations[0], method.Name));
                return;
            }

            // Check parameters:
            // 1. Single parameter with the expected context type, or
            // 2. Two parameters with the expected context type and CancellationToken
            bool validParameters = false;

            if (method.Parameters.Length == 1)
            {
                var contextType = context.Compilation.GetTypeByMetadataName(expectedContextType);
                validParameters = contextType != null && SymbolEqualityComparer.Default.Equals(method.Parameters[0].Type, contextType);
            }
            else if (method.Parameters.Length == 2)
            {
                var contextType = context.Compilation.GetTypeByMetadataName(expectedContextType);
                var cancellationTokenType = context.Compilation.GetTypeByMetadataName("System.Threading.CancellationToken");
                validParameters = contextType != null && cancellationTokenType != null &&
                                 SymbolEqualityComparer.Default.Equals(method.Parameters[0].Type, contextType) &&
                                 SymbolEqualityComparer.Default.Equals(method.Parameters[1].Type, cancellationTokenType);
            }

            if (!validParameters)
            {
                context.ReportDiagnostic(Diagnostic.Create(diagnostic, method.Locations.FirstOrDefault() ?? method.Locations[0], method.Name));
            }
        }

        private void CheckPromptMethodSignature(SymbolAnalysisContext context, IMethodSymbol method, AttributeData attrData,
                                               string expectedContextTypeBase, DiagnosticDescriptor sigDiagnostic,
                                               DiagnosticDescriptor genericMismatchDiagnostic)
        {
            // Check return type: StateResult or Task<StateResult>
            bool validReturnType = IsStateResult(method.ReturnType, context.Compilation) ||
                                  IsTaskOfStateResult(method.ReturnType, context.Compilation);

            if (!validReturnType)
            {
                context.ReportDiagnostic(Diagnostic.Create(sigDiagnostic, method.Locations.FirstOrDefault() ?? method.Locations[0], method.Name));
                return;
            }

            // Get the generic argument from the attribute
            ITypeSymbol attrGenericArg = null;
            if (attrData.AttributeClass is INamedTypeSymbol namedType && namedType.IsGenericType && namedType.TypeArguments.Length == 1)
            {
                attrGenericArg = namedType.TypeArguments[0];
            }

            // Check parameters
            bool validParameters = false;
            ITypeSymbol paramGenericArg = null;

            var contextBaseDef = context.Compilation.GetTypeByMetadataName(expectedContextTypeBase);
            var cancellationTokenType = context.Compilation.GetTypeByMetadataName("System.Threading.CancellationToken");

            if (method.Parameters.Length >= 1 && contextBaseDef != null)
            {
                if (method.Parameters[0].Type is INamedTypeSymbol paramType &&
                    paramType.IsGenericType &&
                    SymbolEqualityComparer.Default.Equals(paramType.ConstructedFrom, contextBaseDef))
                {
                    paramGenericArg = paramType.TypeArguments[0];

                    // Valid if 1 parameter of correct type, or 2 parameters with second being CancellationToken
                    validParameters = method.Parameters.Length == 1 ||
                                     (method.Parameters.Length == 2 &&
                                      cancellationTokenType != null &&
                                      SymbolEqualityComparer.Default.Equals(method.Parameters[1].Type, cancellationTokenType));
                }
            }

            if (!validParameters)
            {
                context.ReportDiagnostic(Diagnostic.Create(sigDiagnostic, method.Locations.FirstOrDefault() ?? method.Locations[0], method.Name));
                return;
            }

            // Check if generic type arguments match
            if (attrGenericArg != null && paramGenericArg != null)
            {
                if (!SymbolEqualityComparer.Default.Equals(attrGenericArg, paramGenericArg))
                {
                    // Check nullability difference
                    if (SymbolEqualityComparer.Default.Equals(attrGenericArg.OriginalDefinition, paramGenericArg.OriginalDefinition)
                        && attrGenericArg.NullableAnnotation != paramGenericArg.NullableAnnotation)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(NullabilityMismatch,
                            method.Locations.FirstOrDefault() ?? method.Locations[0],
                            attrGenericArg.ToDisplayString(), paramGenericArg.ToDisplayString()));
                    }
                    else
                    {
                        context.ReportDiagnostic(Diagnostic.Create(genericMismatchDiagnostic,
                            method.Locations.FirstOrDefault() ?? method.Locations[0],
                            attrGenericArg.ToDisplayString(), paramGenericArg.ToDisplayString()));
                    }
                }
            }
        }

        private static bool IsStateResult(ITypeSymbol type, Compilation compilation)
        {
            var stateResult = compilation.GetTypeByMetadataName("BotForge.Fsm.StateResult");
            return stateResult != null && SymbolEqualityComparer.Default.Equals(type, stateResult);
        }

        private static bool IsTaskOfStateResult(ITypeSymbol returnType, Compilation compilation)
        {
            var taskT = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");
            var stateResult = compilation.GetTypeByMetadataName("BotForge.Fsm.StateResult");
            if (taskT is null || stateResult is null) return false;
            if (returnType is INamedTypeSymbol nt && SymbolEqualityComparer.Default.Equals(nt.OriginalDefinition, taskT))
            {
                return nt.TypeArguments.Length == 1 && SymbolEqualityComparer.Default.Equals(nt.TypeArguments[0], stateResult);
            }
            return false;
        }

        // Syntax-level checks for nameof usage, localization keys, and MenuItem label resolution
        private void AnalyzeAttributeSyntax(SyntaxNodeAnalysisContext context)
        {
            var attrSyntax = (AttributeSyntax)context.Node;
            var model = context.SemanticModel;
            if (!(model.GetSymbolInfo(attrSyntax).Symbol is IMethodSymbol attrSymbol)) return;

            var attrClass = attrSymbol.ContainingType;
            var attrFullName = GetFullMetadataName(attrClass);

            if (attrFullName == "BotForge.Modules.Attributes.MenuItemAttribute" ||
                attrFullName == "BotForge.Modules.Attributes.MenuRowAttribute")
            {
                var arguments = attrSyntax.ArgumentList?.Arguments ?? new SeparatedSyntaxList<AttributeArgumentSyntax>();
                bool hasLabelStorage = HasLabelStorageInProject();

                foreach (var arg in arguments)
                {
                    if (hasLabelStorage)
                    {
                        CheckAttributeArgumentLabelReference(context, model, arg);
                    }
                    else
                    {
                        context.ReportDiagnostic(Diagnostic.Create(SuggestAddLabelStorage, arg.GetLocation()));
                    }
                }
            }

            // Check resource keys for states
            var stateAttrs = new[] {
                "BotForge.Modules.Attributes.MenuAttribute",
                "BotForge.Modules.Attributes.PromptAttribute`1", // Note that for generic types, result is TypeName<TArg>, not TypeName`1
                "BotForge.Modules.Attributes.ModelPromptAttribute`1",
                "BotForge.Modules.Attributes.CustomStateAttribute"
            };

            if (stateAttrs.Contains(attrFullName))
            {
                var arg = attrSyntax.ArgumentList?.Arguments.FirstOrDefault();
                if (arg is null) return;

                var op = model.GetOperation(arg.Expression);

                // Check if we have resources in the project
                bool hasResources = HasResourcesInProject();

                if (op is INameOfOperation nameofOp)
                {
                    // nameof points to a member; ensure member's type is string
                    ISymbol sym = null;
                    switch (nameofOp.Argument)
                    {
                        case IFieldReferenceOperation fr:
                            sym = fr.Field;
                            break;
                        case IPropertyReferenceOperation pr:
                            sym = pr.Property;
                            break;
                    }

                    if (sym == null)
                    {
                        if (hasResources)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(LocalizationKeyNotFound, arg.GetLocation(), arg.ToString().Replace("\"", "")));
                        }
                        else
                        {
                            context.ReportDiagnostic(Diagnostic.Create(SuggestAddResources, arg.GetLocation(), arg.ToString().Replace("\"", "")));
                        }
                        return;
                    }

                    ITypeSymbol memberType = null;
                    switch (sym)
                    {
                        case IFieldSymbol f:
                            memberType = f.Type;
                            break;
                        case IPropertySymbol p:
                            memberType = p.Type;
                            break;
                    }

                    if (memberType is null || memberType.SpecialType != SpecialType.System_String)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(LocalizationKeyNotFound, arg.GetLocation(), arg.ToString().Replace("\"", "")));
                        if (!hasResources)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(SuggestAddResources, arg.GetLocation(), arg.ToString().Replace("\"", "")));
                        }
                        return;
                    }

                    // Optionally check that the member comes from a Resources class
                    // (for this simple case, we'll accept any string member)
                }
                else
                {
                    // For string literals, check if they could be found in resources
                    if (op is ILiteralOperation literal && literal.ConstantValue.Value is string strValue)
                    {
                        if (!IsStringInResources(strValue) && hasResources)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(LocalizationKeyNotFound, arg.GetLocation(), strValue.Replace("\"", "")));
                        }
                        else if (!hasResources)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(SuggestAddResources, arg.GetLocation(), strValue.Replace("\"", "")));
                        }
                    }
                    else
                    {
                        // Not nameof or string literal - require nameof only for keys
                        context.ReportDiagnostic(Diagnostic.Create(LocalizationKeyNotFound, arg.GetLocation(), arg.ToString().Replace("\"", "")));
                        if (!hasResources)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(SuggestAddResources, arg.GetLocation(), arg.ToString().Replace("\"", "")));
                        }
                    }
                }
            }
        }

        private bool HasResourcesInProject() => _resourceClasses.Any();

        private static string GetFullMetadataName(INamedTypeSymbol type)
        {
            if (type == null) return null;

            var parts = new Stack<string>();

            INamespaceOrTypeSymbol current = type;
            while (current is INamedTypeSymbol)
            {
                parts.Push(current.MetadataName);
                current = current?.ContainingType ?? (INamespaceOrTypeSymbol)current.ContainingNamespace;
            }

            var ns = type.ContainingNamespace;
            while (!ns.IsGlobalNamespace)
            {
                parts.Push(ns.Name);
                ns = ns.ContainingNamespace;
            }

            return string.Join(".", parts);
        }

        private static bool IsResourceClass(INamedTypeSymbol symbol)
        {
            // Check if the class contains generated resource manager.
            return symbol.IsValueType == false &&
                   symbol.TypeKind == TypeKind.Class &&
                   symbol.AssociatedSymbol == null && // Avoid associated symbols
                   symbol.GetMembers().Any(m => m.Name == "ResourceManager");
        }

        private bool HasLabelStorageInProject() => _labelClasses.Any();

        private bool IsStringInResources(string value)
        {
            foreach (var resourceType in _resourceClasses)
            {
                if (resourceType is ITypeSymbol typ && typ.GetMembers().OfType<IPropertySymbol>().Any(p => p.Name == value))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool CheckAttributeArgumentLabelReference(SyntaxNodeAnalysisContext context, SemanticModel model, AttributeArgumentSyntax arg)
        {
            var op = model.GetOperation(arg.Expression);
            if (op is INameOfOperation nameofOp)
            {
                var namedSymbol = nameofOp.Argument is IOperation nameOfArgOp
                    ? (nameOfArgOp is IFieldReferenceOperation fr ? fr.Field :
                       nameOfArgOp is IPropertyReferenceOperation pr ? pr.Property :
                       (ISymbol)null)
                    : null;

                // Verify symbol is field or property, public static readonly and of type ButtonLabel
                ITypeSymbol memberType = null;
                switch (namedSymbol)
                {
                    case IFieldSymbol f:
                        memberType = f.Type;
                        break;
                    case IPropertySymbol p:
                        memberType = p.Type;
                        break;
                }

                var buttonLabelType = context.Compilation.GetTypeByMetadataName("BotForge.Messaging.ButtonLabel");
                if (memberType is null || buttonLabelType is null || !SymbolEqualityComparer.Default.Equals(memberType, buttonLabelType))
                {
                    context.ReportDiagnostic(Diagnostic.Create(MenuItemLabelNotFound, arg.GetLocation(), arg.ToString().Replace("\"", "")));
                    return false;
                }

                // check modifiers and containing type attribute LabelStorage
                bool isStatic = (namedSymbol is IFieldSymbol ff && ff.IsStatic) || (namedSymbol is IPropertySymbol pp && pp.IsStatic);
                if (!isStatic)
                {
                    context.ReportDiagnostic(Diagnostic.Create(MenuItemLabelNotFound, arg.GetLocation(), arg.ToString().Replace("\"", "")));
                    return false;
                }

                // For field: check readonly
                if (namedSymbol is IFieldSymbol fieldSym && !fieldSym.IsReadOnly)
                {
                    context.ReportDiagnostic(Diagnostic.Create(MenuItemLabelNotFound, arg.GetLocation(), arg.ToString().Replace("\"", "")));
                    return false;
                }

                var containing = namedSymbol.ContainingType;
                var labelStorageAttrType = context.Compilation.GetTypeByMetadataName("BotForge.Messaging.LabelStorageAttribute");
                var hasLabelsAttr = labelStorageAttrType != null && containing.GetAttributes()
                    .Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, labelStorageAttrType));

                if (!hasLabelsAttr)
                {
                    context.ReportDiagnostic(Diagnostic.Create(MenuItemLabelNotFound, arg.GetLocation(), arg.ToString().Replace("\"", "")));
                    return false;
                }
            }
            else
            {
                // not nameof — warn
                context.ReportDiagnostic(Diagnostic.Create(MenuItemLabelNotFound, arg.GetLocation(), arg.ToString().Replace("\"", "")));
                return false;
            }

            return true;
        }
    }
}
