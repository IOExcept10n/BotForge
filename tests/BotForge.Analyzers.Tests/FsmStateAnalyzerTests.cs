using BotForge.Analyzers.Tests.Helpers;
using BotForge.Analyzers.CodeFixes;

namespace BotForge.Analyzers.Tests;

[TestClass]
public sealed class FsmStateAnalyzerTests
{
    private const string BaseBotForgeUsings = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using BotForge.Fsm;
using BotForge.Messaging;
using BotForge.Modules;
using BotForge.Modules.Attributes;
using BotForge.Modules.Contexts;
";

    #region FSM001 - Menu method signature tests

    [TestMethod]
    public async Task WhenMenuMethodHasInvalidSignature_Diagnostics()
    {
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [Menu("TestMenu")]
        public void {|#0:IncorrectMenuSignature|}(SelectionStateContext ctx)
        {
            // This should generate FSM001
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.All;

        var expected = AnalyzerVerifier<FsmStateAnalyzer>
            .Diagnostic("FSM001")
            .WithLocation(0)
            .WithArguments("IncorrectMenuSignature");

        await AnalyzerVerifier<FsmStateAnalyzer>
            .VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task WhenMenuMethodHasValidSignature_NoDiagnostics()
    {
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [Menu("TestMenu")]
        public StateResult CorrectMenuSignature(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Hello");
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.All;

        await AnalyzerVerifier<FsmStateAnalyzer>
            .VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task WhenAsyncMenuMethodHasValidSignature_NoDiagnostics()
    {
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [Menu("TestMenu")]
        public async Task<StateResult> CorrectAsyncMenuSignature(SelectionStateContext ctx)
        {
            await Task.Delay(100);
            return RetryWithMessage(ctx, "Hello");
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.All;

        await AnalyzerVerifier<FsmStateAnalyzer>
            .VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task WhenMenuMethodHasValidSignatureWithCancellationToken_NoDiagnostics()
    {
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [Menu("TestMenu")]
        public async Task<StateResult> CorrectAsyncMenuSignature(SelectionStateContext ctx, CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken);
            return RetryWithMessage(ctx, "Hello");
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.All;

        await AnalyzerVerifier<FsmStateAnalyzer>
            .VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    #endregion

    #region FSM002 - Prompt method signature tests

    [TestMethod]
    public async Task WhenPromptMethodHasInvalidSignature_Diagnostics()
    {
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [Prompt<string>("TestPrompt")]
        public void {|#0:IncorrectPromptSignature|}(PromptStateContext<string> ctx)
        {
            // This should generate FSM002
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.All;

        var expected = AnalyzerVerifier<FsmStateAnalyzer>
            .Diagnostic("FSM002")
            .WithLocation(0)
            .WithArguments("IncorrectPromptSignature");

        await AnalyzerVerifier<FsmStateAnalyzer>
            .VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task WhenPromptMethodHasValidSignature_NoDiagnostics()
    {
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [Prompt<string>("TestPrompt")]
        public StateResult CorrectPromptSignature(PromptStateContext<string> ctx)
        {
            return RetryWithMessage(ctx, "Hello");
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.All;

        await AnalyzerVerifier<FsmStateAnalyzer>
            .VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    #endregion

    #region FSM003 - Prompt generic type mismatch tests

    [TestMethod]
    public async Task WhenPromptGenericTypeDoesNotMatchParameter_Diagnostics()
    {
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [Prompt<int>("TestPrompt")]
        public StateResult {|#0:GenericTypeMismatch|}(PromptStateContext<string> ctx)
        {
            return RetryWithMessage(ctx, "Hello");
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.All;

        var expected = AnalyzerVerifier<FsmStateAnalyzer>
            .Diagnostic("FSM003")
            .WithLocation(0)
            .WithArguments("int", "string");

        await AnalyzerVerifier<FsmStateAnalyzer>
            .VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    #endregion

    #region FSM004 - MenuItem without Menu tests

    [TestMethod]
    public async Task WhenMenuItemUsedWithoutMenu_Diagnostics()
    {
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [MenuItem(nameof(ButtonLabels.TestButton))]
        public StateResult {|#0:MenuItemWithoutMenuState|}(ModuleStateContext ctx)
        {
            return RetryWithMessage(ctx, "Hello");
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.All;

        var expected = AnalyzerVerifier<FsmStateAnalyzer>
            .Diagnostic("FSM004")
            .WithLocation(0)
            .WithArguments("MenuItemWithoutMenuState");

        await AnalyzerVerifier<FsmStateAnalyzer>
            .VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task WhenMenuItemUsedWithMenu_NoDiagnostics()
    {
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [Menu(nameof(Resources.TestMenu))]
        [MenuItem(nameof(ButtonLabels.TestButton))]
        public StateResult ValidMenuItemUsage(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Hello");
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.All;

        await AnalyzerVerifier<FsmStateAnalyzer>
            .VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    #endregion

    #region FSM014 - Multiple state attributes tests

    [TestMethod]
    public async Task WhenMultipleStateAttributesUsed_Diagnostics()
    {
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
// Suppress warnings not related to the problem
#pragma warning disable FSM001, FSM002
        [Menu(nameof(Resources.TestMenu))]
        [Prompt<string>(nameof(Resources.TestPrompt))]
        public StateResult {|#0:MultipleStateAttributes|}(ModuleStateContext ctx)
        {
            return RetryWithMessage(ctx, "Hello");
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.All;

        var expected = AnalyzerVerifier<FsmStateAnalyzer>
            .Diagnostic("FSM014")
            .WithLocation(0)
            .WithArguments("MultipleStateAttributes");

        await AnalyzerVerifier<FsmStateAnalyzer>
            .VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    #endregion

    #region FSM008 - Keyboard instruction without Menu tests

    [TestMethod]
    public async Task WhenKeyboardInstructionUsedWithoutMenu_Diagnostics()
    {
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [InheritKeyboard]
        public StateResult {|#0:KeyboardWithoutMenu|}(ModuleStateContext ctx)
        {
            return RetryWithMessage(ctx, "Hello");
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.All;

        var expected = AnalyzerVerifier<FsmStateAnalyzer>
            .Diagnostic("FSM008")
            .WithLocation(0)
            .WithArguments("InheritKeyboardAttribute", "KeyboardWithoutMenu");

        await AnalyzerVerifier<FsmStateAnalyzer>
            .VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    #endregion

    #region FSM009 - ModelPrompt method signature tests

    [TestMethod]
    public async Task WhenModelPromptMethodHasInvalidSignature_Diagnostics()
    {
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class UserModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class TestModule : ModuleBase
    {
        [ModelPrompt<UserModel>("TestPrompt")]
        public void {|#0:IncorrectModelPromptSignature|}(ModelPromptContext<UserModel> ctx)
        {
            // This should generate FSM009
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.All;

        var expected = AnalyzerVerifier<FsmStateAnalyzer>
            .Diagnostic("FSM009")
            .WithLocation(0)
            .WithArguments("IncorrectModelPromptSignature");

        await AnalyzerVerifier<FsmStateAnalyzer>
            .VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    #endregion

    #region FSM010 - CustomState method signature tests

    [TestMethod]
    public async Task WhenCustomStateMethodHasInvalidSignature_Diagnostics()
    {
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [CustomState(nameof(Resources.TestMenu))]
        public void {|#0:IncorrectCustomStateSignature|}(ModuleStateContext ctx)
        {
            // This should generate FSM010
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.All;

        var expected = AnalyzerVerifier<FsmStateAnalyzer>
            .Diagnostic("FSM010")
            .WithLocation(0)
            .WithArguments("IncorrectCustomStateSignature");

        await AnalyzerVerifier<FsmStateAnalyzer>
            .VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    #endregion

    #region Code Fix Tests

    [TestMethod]
    public async Task FixMenuMethodSignature_CodeFix()
    {
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [Menu("TestMenu")]
        public void IncorrectMenuSignature(SelectionStateContext ctx)
        {
            // This should be fixed
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.All;

        var fixedTest = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [Menu("TestMenu")]
        public StateResult IncorrectMenuSignature(SelectionStateContext ctx)
        {
            // This should be fixed
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.All;

        var expected = AnalyzerVerifier<FsmStateAnalyzer, FsmStateCodeFixProvider>
            .Diagnostic("FSM001")
            .WithSpan(11, 16, 11, 37)
            .WithArguments("IncorrectMenuSignature");

        await AnalyzerVerifier<FsmStateAnalyzer, FsmStateCodeFixProvider>
            .VerifyCodeFixAsync(test, fixedTest, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task FixPromptGenericTypeMismatch_CodeFix()
    {
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [Prompt<int>("TestPrompt")]
        public StateResult GenericTypeMismatch(PromptStateContext<string> ctx)
        {
            return RetryWithMessage(ctx, "Hello");
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.All;

        var fixedTest = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [Prompt<string>("TestPrompt")]
        public StateResult GenericTypeMismatch(PromptStateContext<string> ctx)
        {
            return RetryWithMessage(ctx, "Hello");
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.All;

        var expected = AnalyzerVerifier<FsmStateAnalyzer, FsmStateCodeFixProvider>
            .Diagnostic("FSM003")
            .WithSpan(11, 16, 11, 34)
            .WithArguments("int", "string");

        await AnalyzerVerifier<FsmStateAnalyzer, FsmStateCodeFixProvider>
            .VerifyCodeFixAsync(test, fixedTest, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task FixMenuItemWithoutMenu_AddMenuAttribute_CodeFix()
    {
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [MenuItem("TestButton")]
        public StateResult MenuItemWithoutMenuState(ModuleStateContext ctx)
        {
            return RetryWithMessage(ctx, "Hello");
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.All;

        var fixedTest = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [BotForge.Modules.Attributes.Menu("PromptText")]
        [MenuItem("TestButton")]
        public StateResult MenuItemWithoutMenuState(ModuleStateContext ctx)
        {
            return RetryWithMessage(ctx, "Hello");
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.All;

        var expected = AnalyzerVerifier<FsmStateAnalyzer, MenuItemCodeFixProvider>
            .Diagnostic("FSM004")
            .WithSpan(11, 16, 11, 39)
            .WithArguments("MenuItemWithoutMenuState");

        await AnalyzerVerifier<FsmStateAnalyzer, MenuItemCodeFixProvider>
            .VerifyCodeFixAsync(test, fixedTest, 0, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task FixMultipleStateAttributes_KeepOnlyMenu_CodeFix()
    {
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [Menu("TestMenu")]
        [Prompt<string>("TestPrompt")]
        public StateResult MultipleStateAttributes(ModuleStateContext ctx)
        {
            return RetryWithMessage(ctx, "Hello");
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.All;

        var fixedTest = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [Menu("TestMenu")]
        public StateResult MultipleStateAttributes(ModuleStateContext ctx)
        {
            return RetryWithMessage(ctx, "Hello");
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.All;

        var expected = AnalyzerVerifier<FsmStateAnalyzer, MenuItemCodeFixProvider>
            .Diagnostic("FSM014")
            .WithSpan(12, 16, 12, 38)
            .WithArguments("MultipleStateAttributes");

        await AnalyzerVerifier<FsmStateAnalyzer, MenuItemCodeFixProvider>
            .VerifyCodeFixAsync(test, fixedTest, 0, expected).ConfigureAwait(false);
    }

    #endregion
}
