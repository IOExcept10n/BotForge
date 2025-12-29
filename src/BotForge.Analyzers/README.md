# BotForge.Analyzers

Roslyn analyzers for the [BotForge framework](https://github.com/IOExcept10n/BotForge) to help enforce best practices and avoid common mistakes when developing chatbots using BotForge.

## Features

The BotForge Analyzers package provides the following features:

### FSM State Method Signature Validation

- **FSM001**: Validates that methods with `[Menu]` attribute have the correct signature: `StateResult Method(SelectionStateContext)` or `Task<StateResult> Method(SelectionStateContext [, CancellationToken])`.
- **FSM002**: Validates that methods with `[Prompt<T>]` attribute have the correct signature: `StateResult Method(PromptStateContext<T>)` or `Task<StateResult> Method(PromptStateContext<T> [, CancellationToken])`.
- **FSM009**: Validates that methods with `[ModelPrompt<T>]` attribute have the correct signature: `StateResult Method(ModelPromptContext<T>)` or `Task<StateResult> Method(ModelPromptContext<T> [, CancellationToken])`.
- **FSM010**: Validates that methods with `[CustomState]` attribute have the correct signature: `StateResult Method(ModuleStateContext)` or `Task<StateResult> Method(ModuleStateContext [, CancellationToken])`.

### Type Checking

- **FSM003**: Ensures that the generic type parameter in `[Prompt<T>]` matches the `PromptStateContext<T>` parameter type.
- **FSM011**: Ensures that the generic type parameter in `[ModelPrompt<T>]` matches the `ModelPromptContext<T>` parameter type.
- **FSM007**: Validates nullability consistency between attribute generic arguments and method parameters.

### Menu and Button Validation

- **FSM004**: Warns when `[MenuItem]` or `[MenuRow]` attributes are used on methods without `[Menu]` or `[CustomState]` attributes.
- **FSM005**: Verifies that menu item labels refer to valid `ButtonLabel` fields/properties in classes marked with `[LabelStorage]`.
- **FSM008**: Warns when keyboard instruction attributes (`[InheritKeyboard]`, `[RemoveKeyboard]`) are used on methods without `[Menu]` or `[CustomState]` attributes.

### Localization Validation

- **FSM006**: Checks if localization keys used in state attributes exist in resource files or as string properties.
- **FSM012**: Suggests adding resource files for localization when none exist but localization keys are used.
- **FSM013**: Suggests adding a class with `[LabelStorage]` when button labels are used but no storage class exists.

### Attribute Usage Validation

- **FSM014**: Ensures that methods don't have multiple state attributes (only one of `[Menu]`, `[Prompt<T>]`, `[ModelPrompt<T>]`, or `[CustomState]` should be used).

## Code Fixes

The package also includes code fixes for many of the diagnostics:

- Fix method signatures for state methods
- Add missing state attributes to methods with menu items
- Remove redundant menu item attributes
- Fix generic type mismatches
- Keep only one state attribute when multiple are present

## Installation

The analyzer is automatically included when you reference the BotForge NuGet package. No additional installation is required.

## Usage

Once installed, the analyzers will run automatically as part of the compiler pipeline, providing warnings and suggestions in your IDE and during build.
