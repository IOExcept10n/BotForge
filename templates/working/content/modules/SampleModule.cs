using System.ComponentModel.DataAnnotations;
using BotForge.Fsm;
using BotForge.Messaging;
using BotForge.Modules;
using BotForge.Modules.Attributes;
using BotForge.Modules.Contexts;
using BotForge.Modules.Roles;

namespace MyCoolBot.Modules;

[Module("🧩 Sample module", AllowedRoleTypes = [typeof(UnknownRole)])]
internal sealed class SampleModule : ModuleBase
{
    [MenuItem("Guess a number")]
    [MenuItem("Register")]
    public override StateResult OnModuleRoot(SelectionStateContext ctx) => ctx.Selection() switch
    {
        "Guess a number" => ToStateWith(ctx, OnGuessANumber, (Number: Random.Shared.Next(1000), Attempts: 10)),
        "Register" => ToState(ctx, OnUserRegisteredAsync),
        _ => InvalidInput(ctx),
    };

    [Prompt<string>("I thought of a number from 0 to 1000. Can you guess it in 10 tries?")]
    public override StateResult OnGuessANumber(PromptStateContext<int> ctx)
    {
        if (!ctx.TryGetData(out (int number, int attempts))
            return Fail(ctx);

        if (!ctx.Input.TryGetValue(out int guessedNumber))
            return InvalidInput(ctx);

        if (guessedNumber == number)
            return ToStateWithMessage(ctx, OnGameOver, $"{Emoji.Crown.ToUnicode()} You won the game! My congratulations!")

        if (attempts == 0)
            return ToState(ctx, OnGameOver);

        string answer = $"Your number is {(guessedNumber > number ? "more" : "less")} than I guessed. Please try again";
        return RetryWith(ctx, (Number: number, Attempts: attempts - 1), answer);
    }

    [Menu("Game over", ParentState = nameof(OnGuessANumber)] // Parent states are needed for back navigation
    [MenuRow("Try again", "Exit")]
    public StateResult OnGameOver(SelectionStateContext ctx) => ctx.Selection() switch
    {
        "Try Again" => BackWith(ctx, (Number: Random.Shared.Next(1000), Attempts: 10)),
        "Exit" => Completed("Have a nice day!"),
        _ => InvalidInput(ctx),
    };

    // Note that your state method can be either syncrhonous or asynchronous
    [ModelPrompt<UserData>]
    public async Task<StateResult> OnUserRegisteredAsync(ModelPromptContext<UserData> ctx, CancellationToken cancellationToken)
    {
        // Add your logic on custom model input here:
        return Completed($"Hello, {ctx.Model.Email}!");
    }


    // You can also customize models validation process by overriding this method.
    protected override Task<bool> ValidateModelAsync(ModelBindContext ctx, ModelBindingBuilder bindingBuilder, ICollection<ValidationResult> errors)
    {
        if (bindingBuilder.InputProperty.Property.Name is nameof(UserData.Password))
        {
            string password = bindingBuilder.GetValidatedProperty<string>();
            if (!IsCorrectPassword(password))
            {
                errors.Add("Your password should contain at least 1 digit, uppercase, lowercase and a special character.");
                return Task.FromResult(false);
            }
        }
        return Task.FromResult(true);
    }

    private static bool IsCorrectPassword(string password) =>
        !string.IsNullOrEmpty(password) && // Check if password is not null or empty
        password.Any(char.IsUpper) && // Check for at least one uppercase letter
        password.Any(char.IsLower) && // Check for at least one lowercase letter
        password.Any(char.IsDigit) && // Check for at least one digit
        password.Any(c => "!@#$%^&*()_+[]{}|;:,.<>?".Contains(c))) // Check for at least one special character

    private readonly record struct UserData(
        [property: Display(Prompt = "Input e-mail", EmailAddress] string Email,
        [property: Display(Prompt = "Input password", MinLength(8)] string Password,
        [property: Display(Prompt = "Input age", Range(18, 99)] int Age);
}
