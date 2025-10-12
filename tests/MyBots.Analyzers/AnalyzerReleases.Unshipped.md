; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
FSM001 | Usage | Warning | Метод с `[MenuState]` должен быть `public instance Task<StateResult>(ModuleStateContext)`
FSM002 | Usage | Warning | Метод с `[PromptState<T>]` должен быть `public instance Task<StateResult>(PromptStateContext<T>)`
FSM003 | Usage | Warning | Generic `T` в `[PromptState<T>]` не совпадает с параметром `PromptStateContext<T>`
FSM004 | Usage | Warning | `[MenuItem]` или `MenuRow]` используется без соответствующего `[MenuState]`
FSM005 | Usage | Warning | `nameof` в `[MenuItem]` или `MenuRow]` должен указывать на `public static readonly ButtonLabel` в классе с `[LabelsStorage]`
FSM006 | Usage | Warning | `nameof` в message key должен указывать на string-ресурс (.resx или string-член)
FSM007 | Usage | Warning | Несоответствие nullability между аргументом атрибута и параметром метода
FSM008 | Usage | Warning | Инструкция для клавиатуры (`[InheritKeyboard]` или `[DisableKeyboard]`) используется без соответствующего `[MenuState]`
