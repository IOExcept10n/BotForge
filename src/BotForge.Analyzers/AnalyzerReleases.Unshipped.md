; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

---

### Diagnostic Summaries

| Rule ID | Category | Severity | Description |
|---------|----------|----------|-------------|
| **FSM001** | Usage | Warning | Method with `[MenuState]` must be `public instance Task<StateResult>(ModuleStateContext)`. This ensures correct method signature for the menu state behavior. |
| **FSM002** | Usage | Warning | Method with `[PromptState<T>]` must be `public instance Task<StateResult>(PromptStateContext<T>)`. This enforces the expected signature for prompt state methods. |
| **FSM003** | Usage | Warning | Generic `T` in `[PromptState<T>]` does not match the parameter in `PromptStateContext<T>`. This ensures type consistency in generic methods. |
| **FSM004** | Usage | Warning | `[MenuItem]` or `[MenuRow]` is used without an appropriate `[MenuState]`. This diagnostic checks that menu items are only used with the correct state attributes. |
| **FSM005** | Usage | Warning | The `nameof` in `[MenuItem]` or `[MenuRow]` must point to a `public static readonly ButtonLabel` in a class with `[LabelsStorage]`. This verifies proper label assignment. |
| **FSM006** | Usage | Warning | The `nameof` in the message key must point to a string resource (either a `.resx` file or a string member). This ensures that localization keys are correctly referenced. |
| **FSM007** | Usage | Warning | Nullability mismatch between the attribute generic argument and the method parameter. This diagnostic checks for consistent nullability definitions. |
| **FSM008** | Usage | Warning | Keyboard instruction (`[InheritKeyboard]` or `[DisableKeyboard]`) is used without an appropriate `[MenuState]`. This ensures keyboard instructions are contextually correct. |
| **FSM009** | Usage | Warning | Related to the state method signature. It checks for correct implementation as specified in the guidelines. |
| **FSM010** | Usage | Warning | Related to the custom state method signature. Confirms that the method signatures adhere to the expected format. |
| **FSM011** | Usage | Warning | Similar to FSM003, it checks for generic argument mismatches with `ModelPrompt` context. |
| **FSM012** | Usage | Info | Suggests adding resource files for localization to support missing keys. Promotes better localization practices. |
| **FSM013** | Usage | Info | Recommends adding a class with the `[LabelsStorage]` attribute to define button labels. Encourages best practices for label storage. |
| **FSM014** | Usage | Warning | Alerts for multiple state attributes being applied to a method when only one should be used. Ensures adherence to design rules for state management. |

---

