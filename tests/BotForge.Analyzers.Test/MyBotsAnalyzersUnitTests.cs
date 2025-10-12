using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = MyBots.Analyzers.Test.CSharpAnalyzerVerifier<FsmStateAnalyzer>;

[TestClass]
public class FsmStateAnalyzerTests
{
    private const string CommonTypes = @"
#nullable enable
using System;
using System.Threading.Tasks;
using MyBots.Modules.Common;
using MyBots.Core.Fsm.States;
using MyBots.Modules.Common.Interactivity;

namespace MyBots.Modules.Common
{
    public class ModuleBase { }
    public class ModuleStateContext { }
    public class PromptStateContext<T> { }

    [AttributeUsage(AttributeTargets.Method)]
    public abstract class FsmStateAttribute(string messageResourceKey) : Attribute
    {
        public string? StateName { get; set; }

        public string MessageResourceKey { get; } = messageResourceKey;
    }

    public sealed class MenuStateAttribute(string messageResourceKey) : FsmStateAttribute(messageResourceKey)
    {
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class MenuItemAttribute(string labelKey) : Attribute
    {
        public string LabelKey { get; } = labelKey;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class PromptStateAttribute<T>(string messageResourceKey) : FsmStateAttribute(messageResourceKey)
    {
        public bool AllowFileInput { get; set; }

        public bool AllowTextInput { get; set; } = true;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class LabelsStorageAttribute : Attribute;

}

namespace MyBots.Modules.Common.Interactivity 
{
    public class ButtonLabel { }
}

namespace MyBots.Core.Fsm.States
{
    public class StateResult { }
}
";

    [TestMethod]
    public async Task MenuState_Correct_NoDiagnostic()
    {
        var test = CommonTypes + @"
class M : ModuleBase
{
    [MyBots.Modules.Common.MenuStateAttribute(nameof(Localization.Msg))]
    [MyBots.Modules.Common.MenuItemAttribute(nameof(MyLabels.Label1))]
    [MyBots.Modules.Common.MenuItemAttribute(nameof(MyLabels.Label2))]
    public async Task<StateResult> ChooseCase(ModuleStateContext ctx)
    {
        throw null!;
    }
}

internal static class Localization { public static string Msg = ""x""; }

[MyBots.Modules.Common.LabelsStorageAttribute]
internal static class MyLabels
{
    public static readonly ButtonLabel Label1 = null!;
    public static readonly ButtonLabel Label2 = null!;
}
";
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task PromptState_Correct_NoDiagnostic()
    {
        var test = CommonTypes + @"
class M : ModuleBase
{
    [MyBots.Modules.Common.PromptStateAttribute<string>(nameof(Localization.Msg))]
    public async Task<StateResult> InputTeamName(PromptStateContext<string> ctx)
    {
        throw null!;
    }
}

internal static class Localization { public static string Msg = ""x""; }
";
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task MenuState_WrongReturnType_Diagnostic()
    {
        var test = CommonTypes + @"
class M : ModuleBase
{
    [MyBots.Modules.Common.MenuStateAttribute(nameof(Localization.Msg))]
    public async Task<int> ChooseCase(ModuleStateContext ctx) // diag here
    {
        throw null!;
    }
}

internal static class Localization { public static string Msg = ""x""; }
";
        var expected = VerifyCS.Diagnostic("FSM001").WithSpan(59, 28, 59, 38).WithArguments("ChooseCase");
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task PromptState_WrongParameterType_Diagnostic()
    {
        var test = CommonTypes + @"
class M : ModuleBase
{
    [MyBots.Modules.Common.PromptStateAttribute<int>(nameof(Localization.Msg))]
    public async Task<StateResult> InputTeamName(PromptStateContext<string> ctx) // diag here
    {
        throw null!;
    }
}

internal static class Localization { public static string Msg = ""x""; }
";
        var expected = VerifyCS.Diagnostic("FSM003").WithSpan(59, 36, 59, 49).WithArguments("int", "string");
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task MenuItem_Without_MenuState_Diagnostic()
    {
        var test = CommonTypes + @"
class M : ModuleBase
{
    [MyBots.Modules.Common.MenuItemAttribute(nameof(MyLabels.Label1))]
    public async Task<StateResult> ChooseCase(ModuleStateContext ctx) // diag here
    {
        throw null!;
    }
}

[MyBots.Modules.Common.LabelsStorageAttribute]
internal static class MyLabels
{
    public static readonly ButtonLabel Label1 = null!;
}
";
        var expected = VerifyCS.Diagnostic("FSM004").WithSpan(59, 36, 59, 46).WithArguments("ChooseCase");
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }
}