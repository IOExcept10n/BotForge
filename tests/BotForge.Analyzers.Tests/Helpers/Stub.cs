using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;

namespace BotForge.Analyzers.Tests.Helpers;

internal class Stub
{
    public const string Stubs = """
namespace BotForge.Fsm
{
    /// <summary>
    /// Stub for the StateResult class
    /// </summary>
    public class StateResult
    {
        public StateResult(string stateId, string stateData, object reply) { }
    }
}

namespace BotForge.Modules
{
    /// <summary>
    /// Stub for the ModuleBase class
    /// </summary>
    public abstract class ModuleBase
    {
        /// <summary>
        /// Root method that every module must implement
        /// </summary>
        public abstract BotForge.Fsm.StateResult OnModuleRoot(BotForge.Modules.Contexts.SelectionStateContext ctx);

        /// <summary>
        /// Helper method stub for tests
        /// </summary>
        protected BotForge.Fsm.StateResult RetryWithMessage(BotForge.Modules.Contexts.ModuleStateContext ctx, string message) => new(string.Empty, string.Empty, message);
    }
}

namespace BotForge.Modules.Attributes
{
    /// <summary>
    /// Base stub for FSM state attributes
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class FsmStateAttribute : Attribute
    {
        protected FsmStateAttribute(string promptLocalizationKey) { }

        public string? StateName { get; set; }
        public string? ParentStateName { get; set; }
    }

    /// <summary>
    /// Stub for the Menu attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class MenuAttribute : FsmStateAttribute
    {
        public MenuAttribute(string promptLocalizationKey) : base(promptLocalizationKey) { }
        public bool BackButton { get; set; } = true;
    }

    /// <summary>
    /// Stub for the abstract Prompt attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class PromptAttribute : FsmStateAttribute
    {
        protected PromptAttribute(Type inputType, string promptLocalizationKey) : base(promptLocalizationKey) 
        { 
            InputType = inputType;
        }

        public bool AllowFileInput { get; set; }
        public bool AllowTextInput { get; set; } = true;
        public Type InputType { get; }
        public bool BackButton { get; set; } = true;
    }

    /// <summary>
    /// Stub for the generic Prompt attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class PromptAttribute<T> : PromptAttribute
    {
        public PromptAttribute(string promptLocalizationKey) : base(typeof(T), promptLocalizationKey) { }
    }

    /// <summary>
    /// Stub for the CustomState attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class CustomStateAttribute : FsmStateAttribute
    {
        public CustomStateAttribute(string promptLocalizationKey) : base(promptLocalizationKey) { }
    }

    /// <summary>
    /// Stub for the ModelPrompt attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ModelPromptAttribute<T> : FsmStateAttribute
    {
        public ModelPromptAttribute(string promptLocalizationKey) : base(promptLocalizationKey) { }
    }

    /// <summary>
    /// Stub for the MenuItem attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class MenuItemAttribute : MenuRowAttribute
    {
        public MenuItemAttribute(string labelKey) : base(labelKey) { }
        public string LabelKey { get; }
    }

    /// <summary>
    /// Stub for the MenuRow attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MenuRowAttribute : Attribute
    {
        public MenuRowAttribute(string labelKey) { }
    }

    /// <summary>
    /// Stub for the InheritKeyboard attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class InheritKeyboardAttribute : Attribute { }

    /// <summary>
    /// Stub for the RemoveKeyboard attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RemoveKeyboardAttribute : Attribute { }
}

namespace BotForge.Modules.Contexts
{
    /// <summary>
    /// Stub for the base module context class
    /// </summary>
    public class ModuleStateContext
    {
    }

    /// <summary>
    /// Stub for the selection context class
    /// </summary>
    public class SelectionStateContext : ModuleStateContext
    {
    }

    /// <summary>
    /// Stub for the prompt context class
    /// </summary>
    public class PromptStateContext<T> : ModuleStateContext
    {
    }

    /// <summary>
    /// Stub for the model prompt context class
    /// </summary>
    public class ModelPromptContext<T> : ModuleStateContext
    {
    }
}

namespace BotForge.Messaging
{
    /// <summary>
    /// Stub for the ButtonLabel class
    /// </summary>
    public class ButtonLabel
    {
        public ButtonLabel(string key, string text) { }
    }

    /// <summary>
    /// Stub for the LabelStorage attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class LabelStorageAttribute : Attribute { }
}
""";

    public const string Resource = """

namespace TestNamespace
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {

        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }

        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("BotForge.Analyzers.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }

        public static string TestMenu { get { return "Valid Key"; } }

        public static string TestPrompt { get { return "Valid Key"; } }

        public static string MainMenu { get { return "Valid Key"; } }

        public static string EnterAge { get { return "Valid Key"; } }

        public static string EnterName { get { return "Valid Key"; } }

        public static string ProfileMenu { get { return "Valid Key"; } }

        public static string ConfirmMenu { get { return "Valid Key"; } }
    }
}
""";

    public const string Label = """

[BotForge.Messaging.LabelStorage]
public static class ButtonLabels
{
    public static readonly ButtonLabel ValidLabel = new ButtonLabel("ValidLabel", "Valid Label");

    public static readonly ButtonLabel SubmitButton = new ButtonLabel("ValidLabel", "Valid Label");

    public static readonly ButtonLabel TestButton = new ButtonLabel("ValidLabel", "Valid Label");
}
""";

    public const string All = Stubs + Label + Resource;
}
