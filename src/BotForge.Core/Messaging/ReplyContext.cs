namespace BotForge.Core.Messaging;

public record ReplyContext(string Message, ReplyKeyboard Keyboard);

public record ReplyKeyboard(IEnumerable<IEnumerable<ButtonLabel>> buttons);
