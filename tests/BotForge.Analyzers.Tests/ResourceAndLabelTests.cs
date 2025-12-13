using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using BotForge.Analyzers.Tests.Helpers;
using System.Threading.Tasks;

namespace BotForge.Analyzers.Tests;

[TestClass]
public class ResourceAndLabelTests
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

    #region FSM005 - MenuItem label not found tests

    [TestMethod]
    public async Task WhenMenuItemLabelDoesNotExist_InProjectWithLabelStorage_Diagnostics()
    {
        // Include a class with [LabelStorage] to test label validation
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [Menu("TestMenu")]
        [MenuItem({|#0:"InvalidButton"|})]  // This label doesn't exist
        public StateResult InvalidLabelUsage(SelectionStateContext ctx)
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
            .Diagnostic("FSM005")
            .WithLocation(0)
            .WithArguments("InvalidButton");

        await AnalyzerVerifier<FsmStateAnalyzer>
            .VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task WhenMenuItemLabelDoesNotExist_InProjectWithoutLabelStorage_SuggestLabelStorage()
    {
        // No LabelStorage class defined in the project
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [Menu("TestMenu")]
        [MenuItem({|#0:"TestButton"|})]  // No label storage exists at all
        public StateResult InvalidLabelUsage(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Hello");
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.Stubs + Stub.Resource;

        var expected = AnalyzerVerifier<FsmStateAnalyzer>
            .Diagnostic("FSM013")  // Should suggest adding label storage
            .WithLocation(0);

        await AnalyzerVerifier<FsmStateAnalyzer>
            .VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task WhenMenuItemLabelExists_NoDiagnostics()
    {
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [Menu("TestMenu")]
        [MenuItem(nameof(ButtonLabels.ValidLabel))]  // This is correct
        public StateResult ValidLabelUsage(SelectionStateContext ctx)
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

    #region FSM006/FSM012 - Localization key tests

    [TestMethod]
    public async Task WhenLocalizationKeyDoesNotExist_InProjectWithResources_Diagnostics()
    {
        // Include a Resources class to test key validation
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [Menu({|#0:"InvalidKey"|})]  // This key doesn't exist
        public StateResult InvalidKeyUsage(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Hello");
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.Stubs + Stub.Resource;

        var expected = AnalyzerVerifier<FsmStateAnalyzer>
            .Diagnostic("FSM006")
            .WithLocation(0)
            .WithArguments("InvalidKey");

        await AnalyzerVerifier<FsmStateAnalyzer>
            .VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task WhenLocalizationKeyDoesNotExist_InProjectWithoutResources_SuggestResources()
    {
        // No Resources class defined in the project
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [Menu({|#0:"TestMenu"|})]  // No resources exist at all
        public StateResult InvalidKeyUsage(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Hello");
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.Stubs;

        var expected = AnalyzerVerifier<FsmStateAnalyzer>
            .Diagnostic("FSM012")  // Should suggest adding resources
            .WithLocation(0)
            .WithArguments("TestMenu");

        await AnalyzerVerifier<FsmStateAnalyzer>
            .VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task WhenLocalizationKeyExists_NoDiagnostics()
    {
        var test = BaseBotForgeUsings + """
namespace TestNamespace
{
    public class TestModule : ModuleBase
    {
        [Menu(nameof(Resources.TestMenu))]  // This is correct
        public StateResult ValidKeyUsage(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Hello");
        }

        public override StateResult OnModuleRoot(SelectionStateContext ctx)
        {
            return RetryWithMessage(ctx, "Root");
        }
    }
}
""" + Stub.Stubs + Stub.Resource;

        await AnalyzerVerifier<FsmStateAnalyzer>
            .VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    #endregion
}
