using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System.Linq;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FsmStateAnalyzer : DiagnosticAnalyzer
{
    private const string Category = "Usage";
    
    private readonly static DiagnosticDescriptor MenuSig = new DiagnosticDescriptor(
        id: "FSM001",
        title: "Menu state method signature",
        messageFormat: "Method '{0}' has incorrect signature for [MenuState]. Expected: public instance Task<StateResult> Method(ModuleStateContext).",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private readonly static DiagnosticDescriptor PromptSig = new DiagnosticDescriptor(
        id: "FSM002",
        title: "Prompt state method signature",
        messageFormat: "Method '{0}' has incorrect signature for [PromptState<T>]. Expected: public instance Task<StateResult> Method(PromptStateContext<T>).",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private readonly static DiagnosticDescriptor PromptGenericMismatch = new DiagnosticDescriptor(
        id: "FSM003",
        title: "Prompt state generic mismatch",
        messageFormat: "Generic argument of [PromptState<{0}>] does not match method parameter PromptStateContext<{1}>",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private readonly static DiagnosticDescriptor MenuItemWithoutMenu = new DiagnosticDescriptor(
        id: "FSM004",
        title: "MenuItem or MenuRow without MenuState",
        messageFormat: "[MenuItem] or [MenuRow] used on method '{0}' without [MenuState]",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private readonly static DiagnosticDescriptor MenuItemLabelNotFound = new DiagnosticDescriptor(
        id: "FSM005",
        title: "MenuItem or MenuRow label not found",
        messageFormat: "MenuItem or MenuRow label '{0}' is not a public static readonly ButtonLabel member of a class marked with [LabelsStorage]",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private readonly static DiagnosticDescriptor LocalizationKeyNotFound = new DiagnosticDescriptor(
        id: "FSM006",
        title: "Localization key not found",
        messageFormat: "Localization key '{0}' not found in project resources or string members",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private readonly static DiagnosticDescriptor NullabilityMismatch = new DiagnosticDescriptor(
        id: "FSM007",
        title: "Nullability mismatch",
        messageFormat: "Nullability mismatch between attribute generic argument and method parameter: '{0}' vs '{1}'",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private readonly static DiagnosticDescriptor KeyboardInstructionWithoutMenu = new DiagnosticDescriptor(
        id: "FSM008",
        title: "Keyboard instruction without MenuState",
        messageFormat: "Keyboard instruction [{0}] is used on method {1} without [MenuState]",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(MenuSig, PromptSig, PromptGenericMismatch, MenuItemWithoutMenu, MenuItemLabelNotFound, LocalizationKeyNotFound, NullabilityMismatch, KeyboardInstructionWithoutMenu);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
        context.RegisterSyntaxNodeAction(AnalyzeAttributeSyntax, Microsoft.CodeAnalysis.CSharp.SyntaxKind.Attribute);
    }

    private void AnalyzeMethod(SymbolAnalysisContext context)
    {
        var method = (IMethodSymbol)context.Symbol;

        var attrs = method.GetAttributes();
        if (attrs.Length == 0) return;

        var menuAttr = attrs.FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == "MyBots.Modules.Common.MenuStateAttribute");
        var promptAttr = attrs.FirstOrDefault(a => a.AttributeClass?.OriginalDefinition?.ToDisplayString() == "MyBots.Modules.Common.PromptStateAttribute<T>");
        var menuItemAttrs = attrs.Where(a => a.AttributeClass?.ToDisplayString() == "MyBots.Modules.Common.MenuItemAttribute").ToImmutableArray();
        var menuRowAttrs = attrs.Where(a => a.AttributeClass?.ToDisplayString() == "MyBots.Modules.Common.MenuRowAttribute").ToImmutableArray();
        var keyboardAttrs = attrs.Where(a => a.AttributeClass?.ToDisplayString() == "MyBots.Modules.Common.InheritKeyboardAttribute" ||
                                             a.AttributeClass?.ToDisplayString() == "MyBots.Modules.Common.RemoveKeyboardAttribute").ToImmutableArray();

        // Check access and instance requirement if any of the relevant attributes present
        if (menuAttr != null || promptAttr != null || menuItemAttrs.Length > 0 || menuRowAttrs.Length > 0 || keyboardAttrs.Length > 0)
        {
            if (method.IsStatic || method.DeclaredAccessibility != Accessibility.Public)
            {
                if (menuAttr != null)
                    context.ReportDiagnostic(Diagnostic.Create(MenuSig, method.Locations.FirstOrDefault() ?? method.Locations[0], method.Name));
                if (promptAttr != null)
                    context.ReportDiagnostic(Diagnostic.Create(PromptSig, method.Locations.FirstOrDefault() ?? method.Locations[0], method.Name));
            }
        }

        // MenuState signature checks
        if (menuAttr != null)
        {
            // Return type must be Task<StateResult>
            if (!IsTaskOfStateResult(method.ReturnType, context.Compilation))
            {
                context.ReportDiagnostic(Diagnostic.Create(MenuSig, method.Locations.FirstOrDefault() ?? method.Locations[0], method.Name));
            }

            // Parameters: single ModuleStateContext
            if (method.Parameters.Length != 1 || !SymbolEqualityComparer.Default.Equals(method.Parameters[0].Type, context.Compilation.GetTypeByMetadataName("MyBots.Modules.Common.ModuleStateContext")))
            {
                context.ReportDiagnostic(Diagnostic.Create(MenuSig, method.Locations.FirstOrDefault() ?? method.Locations[0], method.Name));
            }

            // MenuItem existence is OK here; individual MenuItem validation done in syntax-phase
        }
        else
        {
            // If method has MenuItem but no MenuState -> warn (if this is not root method for module).
            if ((menuItemAttrs.Length > 0 || menuRowAttrs.Length > 0) && method.Name != "OnModuleRootAsync")
            {
                context.ReportDiagnostic(Diagnostic.Create(MenuItemWithoutMenu, method.Locations.FirstOrDefault() ?? method.Locations[0], method.Name));
            }

            // If method has keyboard control but no MenuState -> warn (if this is not root method for module).
            if (keyboardAttrs.Length > 0 && method.Name != "OnModuleRootAsync")
            {
                context.ReportDiagnostic(Diagnostic.Create(KeyboardInstructionWithoutMenu, method.Locations.FirstOrDefault() ?? method.Locations[0], promptAttr.AttributeClass.Name, method.Name));
            }
        }

        // PromptState checks
        if (promptAttr != null)
        {
            // Return Task<StateResult>
            if (!IsTaskOfStateResult(method.ReturnType, context.Compilation))
            {
                context.ReportDiagnostic(Diagnostic.Create(PromptSig, method.Locations.FirstOrDefault() ?? method.Locations[0], method.Name));
            }

            // Params: single PromptStateContext<T>
            if (method.Parameters.Length != 1)
            {
                context.ReportDiagnostic(Diagnostic.Create(PromptSig, method.Locations.FirstOrDefault() ?? method.Locations[0], method.Name));
            }
            else
            {
                var promptCtxDef = context.Compilation.GetTypeByMetadataName("MyBots.Modules.Common.PromptStateContext`1");
                if (!(method.Parameters[0].Type is INamedTypeSymbol paramType) || promptCtxDef is null || !SymbolEqualityComparer.Default.Equals(paramType.ConstructedFrom, promptCtxDef))
                {
                    context.ReportDiagnostic(Diagnostic.Create(PromptSig, method.Locations.FirstOrDefault() ?? method.Locations[0], method.Name));
                }
                else
                {
                    var paramT = paramType.TypeArguments[0];
                    // Get attribute generic arg
                    var genericArg = promptAttr.ConstructorArguments.FirstOrDefault();
                    // promptAttr is a generic attribute instance; its AttributeClass is constructed
                    var attrNamed = promptAttr.AttributeClass;
                    ITypeSymbol attrGenericArg = null;
                    if (promptAttr.AttributeClass is INamedTypeSymbol nat && nat.IsGenericType && nat.TypeArguments.Length == 1)
                    {
                        attrGenericArg = nat.TypeArguments[0];
                    }
                    else if (promptAttr.ConstructorArguments.Length > 0)
                    {
                        // If attribute stored type via typeof(T) -- but in your usage you used PromptState<string>(nameof(...)) so generic is in attribute type, not ctor args.
                    }

                    if (attrGenericArg != null)
                    {
                        if (!SymbolEqualityComparer.Default.Equals(attrGenericArg, paramT))
                        {
                            // Check nullability difference
                            if (SymbolEqualityComparer.Default.Equals(attrGenericArg.OriginalDefinition, paramT.OriginalDefinition)
                                && attrGenericArg.NullableAnnotation != paramT.NullableAnnotation)
                            {
                                context.ReportDiagnostic(Diagnostic.Create(NullabilityMismatch, method.Locations.FirstOrDefault() ?? method.Locations[0], attrGenericArg.ToDisplayString(), paramT.ToDisplayString()));
                            }
                            else
                            {
                                context.ReportDiagnostic(Diagnostic.Create(PromptGenericMismatch, method.Locations.FirstOrDefault() ?? method.Locations[0], attrGenericArg.ToDisplayString(), paramT.ToDisplayString()));
                            }
                        }
                    }
                }
            }
        }
    }

    private static bool IsTaskOfStateResult(ITypeSymbol returnType, Compilation compilation)
    {
        var taskT = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");
        var stateResult = compilation.GetTypeByMetadataName("MyBots.Core.Fsm.States.StateResult");
        if (taskT is null || stateResult is null) return false;
        if (returnType is INamedTypeSymbol nt && SymbolEqualityComparer.Default.Equals(nt.OriginalDefinition, taskT))
        {
            return nt.TypeArguments.Length == 1 && SymbolEqualityComparer.Default.Equals(nt.TypeArguments[0], stateResult);
        }
        return false;
    }

    // Syntax-level checks for nameof usage and MenuItem label resolution
    private void AnalyzeAttributeSyntax(SyntaxNodeAnalysisContext context)
    {
        var attrSyntax = (AttributeSyntax)context.Node;
        var model = context.SemanticModel;
        if (!(model.GetSymbolInfo(attrSyntax).Symbol is IMethodSymbol attrSymbol)) return;

        var attrClass = attrSymbol.ContainingType;
        var attrFullName = attrClass.ToDisplayString();

        if (attrFullName == "MyBots.Modules.Common.MenuItemAttribute")
        {
            // Expect single argument that is nameof(...)
            var arg = attrSyntax.ArgumentList?.Arguments.FirstOrDefault();
            if (arg is null)
                return;
            bool flowControl = CheckAttributeArgumentLabelReference(context, model, arg);
            if (!flowControl)
            {
                return;
            }

            // OK — label exists and looks valid.
        }

        if (attrFullName == "MyBots.Modules.Common.MenuRowAttribute")
        {
            var arguments = attrSyntax.ArgumentList?.Arguments ?? new SeparatedSyntaxList<AttributeArgumentSyntax>();

            foreach (var arg in arguments)
            {
                bool flowControl = CheckAttributeArgumentLabelReference(context, model, arg);
                if (!flowControl)
                {
                    return;
                }
            }
        }

        // Check resource keys for MenuState/PromptState messageResourceKey and also for MenuItem labelKey (if they are nameof(...))
        var targetAttrs = new[] { "MyBots.Modules.Common.MenuStateAttribute", "MyBots.Modules.Common.PromptStateAttribute`1" };
        if (targetAttrs.Contains(attrFullName))
        {
            var arg = attrSyntax.ArgumentList?.Arguments.FirstOrDefault();
            if (arg is null) return;

            var op = model.GetOperation(arg.Expression);
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
                    context.ReportDiagnostic(Diagnostic.Create(LocalizationKeyNotFound, arg.GetLocation(), arg.ToString()));
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
                    context.ReportDiagnostic(Diagnostic.Create(LocalizationKeyNotFound, arg.GetLocation(), arg.ToString()));
                    return;
                }

                // Optionally check that the member comes from a Resources.Designer class (resx auto-gen)
                // We'll accept any string member declared in project.
            }
            else
            {
                // Not nameof — require nameof only for keys per your requirement
                context.ReportDiagnostic(Diagnostic.Create(LocalizationKeyNotFound, arg.GetLocation(), arg.ToString()));
            }
        }
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

            if (namedSymbol is null)
            {
                context.ReportDiagnostic(Diagnostic.Create(MenuItemLabelNotFound, arg.GetLocation(), arg.ToString()));
                return false;
            }

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

            if (memberType is null || memberType.ToDisplayString() != "MyBots.Modules.Common.Interactivity.ButtonLabel")
            {
                context.ReportDiagnostic(Diagnostic.Create(MenuItemLabelNotFound, arg.GetLocation(), arg.ToString()));
                return false;
            }

            // check modifiers and containing type attribute LabelsStorage
            bool isStatic = (namedSymbol is IFieldSymbol ff && ff.IsStatic) || (namedSymbol is IPropertySymbol pp && pp.IsStatic);
            if (!isStatic)
            {
                context.ReportDiagnostic(Diagnostic.Create(MenuItemLabelNotFound, arg.GetLocation(), arg.ToString()));
                return false;
            }

            // For field: check readonly
            if (namedSymbol is IFieldSymbol fieldSym && !fieldSym.IsReadOnly)
            {
                context.ReportDiagnostic(Diagnostic.Create(MenuItemLabelNotFound, arg.GetLocation(), arg.ToString()));
                return false;
            }

            var containing = namedSymbol.ContainingType;
            var hasLabelsAttr = containing.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == "MyBots.Modules.Common.LabelsStorageAttribute");
            if (!hasLabelsAttr)
            {
                context.ReportDiagnostic(Diagnostic.Create(MenuItemLabelNotFound, arg.GetLocation(), arg.ToString()));
                return false;
            }
        }
        else
        {
            // not nameof — warn
            context.ReportDiagnostic(Diagnostic.Create(MenuItemLabelNotFound, arg.GetLocation(), arg.ToString()));
            return false;
        }

        return true;
    }
}
