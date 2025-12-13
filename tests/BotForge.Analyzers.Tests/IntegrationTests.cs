using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using BotForge.Analyzers.Tests.Helpers;
using BotForge.Analyzers.CodeFixes;

namespace BotForge.Analyzers.Tests;

[TestClass]
public class IntegrationTests
{
    private const string BaseBotForgeUsings = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using BotForge.Fsm;
using BotForge.Modules;
using BotForge.Modules.Attributes;
using BotForge.Modules.Contexts;
using BotForge.Messaging;
";

    [TestMethod]
    public async Task ComplexModuleWithMultipleIssues_AllDiagnosticsReported()
    {
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class ComplexTestModule : ModuleBase
    {
        // Issue 1: Invalid signature (void instead of StateResult)
        [Menu("MainMenu")]
        public void {|#0:InvalidMenuMethod|}(SelectionStateContext ctx)
        {
            // Should report FSM001
        }

        // Issue 2: Generic type mismatch
        [Prompt<int>("EnterAge")]
        public StateResult {|#1:GenericMismatch|}(PromptStateContext<string> ctx)
        {
            return RetryWithMessage(ctx, "Enter your age");
        }

        // Issue 3: MenuItem without Menu
        [MenuItem({|#5:"SubmitButton"|})]
        public StateResult {|#2:MenuItemWithoutMenu|}(ModuleStateContext ctx)
        {
            return RetryWithMessage(ctx, "Submit");
        }

        // Issue 4: Multiple state attributes (without warnings for invalid signature)
#pragma warning disable FSM001, FSM002
        [Menu("ProfileMenu")]
        [Prompt<string>("EnterName")]
        public StateResult {|#3:MultipleStateAttributes|}(ModuleStateContext ctx)
        {
            return RetryWithMessage(ctx, "Enter your name");
        }

        // Issue 5: Keyboard attribute without Menu
        [InheritKeyboard]
        public StateResult {|#4:KeyboardWithoutMenu|}(ModuleStateContext ctx)
        {
            return RetryWithMessage(ctx, "Hello");
        }

        // No issues - this is correct
        [Menu("ConfirmMenu")]
        public StateResult CorrectMenuMethod(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Please confirm");
        }

        // Required override - no issues
        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Welcome to the module");
        }
    }
}
""" + Stub.All;

        var expected = new DiagnosticResult[]
        {
            // Issue 1: Invalid menu signature
            AnalyzerVerifier<FsmStateAnalyzer>
                .Diagnostic("FSM001")
                .WithLocation(0)
                .WithArguments("InvalidMenuMethod"),

            // Issue 2: Generic type mismatch
            AnalyzerVerifier<FsmStateAnalyzer>
                .Diagnostic("FSM003")
                .WithLocation(1)
                .WithArguments("int", "string"),

            // Also there would be a FSM013 for MenuItem label not in a LabelStorage
            AnalyzerVerifier<FsmStateAnalyzer>
                .Diagnostic("FSM005")
                .WithLocation(5)
                .WithArguments("SubmitButton"),

            // Issue 3: MenuItem without Menu
            AnalyzerVerifier<FsmStateAnalyzer>
                .Diagnostic("FSM004")
                .WithLocation(2)
                .WithArguments("MenuItemWithoutMenu"),

            // Issue 4: Multiple state attributes
            AnalyzerVerifier<FsmStateAnalyzer>
                .Diagnostic("FSM014")
                .WithLocation(3)
                .WithArguments("MultipleStateAttributes"),

            // Issue 5: Keyboard attribute without Menu
            AnalyzerVerifier<FsmStateAnalyzer>
                .Diagnostic("FSM008")
                .WithLocation(4)
                .WithArguments("InheritKeyboardAttribute", "KeyboardWithoutMenu"),
        };

        await AnalyzerVerifier<FsmStateAnalyzer>
            .VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task FixAllIssuesInModule_MultipleFixes()
    {
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class SimpleModule : ModuleBase
    {
        // Issue 1: Invalid signature
        [Menu("MainMenu")]
        public void InvalidMenuMethod(SelectionStateContext ctx)
        {
            // Code here
        }

        // Issue 2: MenuItem without Menu
        [MenuItem("SubmitButton")]
        public StateResult MenuItemWithoutMenu(ModuleStateContext ctx)
        {
            return RetryWithMessage(ctx, "Submit");
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Welcome");
        }
    }
}
""" + Stub.All;

        var fixedTest = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class SimpleModule : ModuleBase
    {
        // Issue 1: Invalid signature
        [Menu("MainMenu")]
        public StateResult InvalidMenuMethod(SelectionStateContext ctx)
        {
            // Code here
        }

        // Issue 2: MenuItem without Menu
        [BotForge.Modules.Attributes.Menu("PromptText")]
        [MenuItem("SubmitButton")]
        public StateResult MenuItemWithoutMenu(ModuleStateContext ctx)
        {
            return RetryWithMessage(ctx, "Submit");
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Welcome");
        }
    }
}
""" + Stub.All;

        // To simplify test, fix only FSM001
        var expected = AnalyzerVerifier<FsmStateAnalyzer, FsmStateCodeFixProvider>
            .Diagnostic("FSM001")
            .WithSpan(11, 16, 11, 33);

        await AnalyzerVerifier<FsmStateAnalyzer, FsmStateCodeFixProvider>
            .VerifyCodeFixAsync(test, fixedTest, 0, expected).ConfigureAwait(false);
    }
}
