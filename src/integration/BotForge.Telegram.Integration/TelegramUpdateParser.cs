using System.Globalization;
using BotForge.Messaging;
using Telegram.Bot.Types;
using BotForge.Telegram.Integration.Models;

namespace BotForge.Telegram.Integration;

/// <summary>
/// Parses Telegram <see cref="Update"/> instances into <see cref="TelegramUpdate"/> DTOs.
/// </summary>
public static class TelegramUpdateParser
{
    /// <summary>
    /// Converts a Telegram <see cref="Update"/> to a BotForge <see cref="IUpdate"/>.
    /// </summary>
    public static TelegramUpdate Parse(Update raw)
    {
        DateTimeOffset Timestamp;
        UpdateType Type;
        UserIdentity Sender;
        IMessage? Message;
        IInteraction? Interaction;

        static UserIdentity FromTelegramUser(User? u)
        {
            if (u is null)
                return new UserIdentity(0);

            var display = string.IsNullOrEmpty(u.FirstName) ? u.Username : (u.FirstName + (string.IsNullOrEmpty(u.LastName) ? string.Empty : " " + u.LastName));
            CultureInfo? locale = null;
            try { if (!string.IsNullOrEmpty(u.LanguageCode)) locale = new CultureInfo(u.LanguageCode); } catch (CultureNotFoundException) { locale = null; }
            return new UserIdentity(u.Id, u.Username, display, 0, locale);
        }

        static MessageContent MapContent(Message m)
        {
            if (!string.IsNullOrEmpty(m.Text))
                return new TextMessageContent(m.Text);

            if (m.Photo is not null && m.Photo.Length > 0)
            {
                var fileId = m.Photo[^1].FileId;
                return new FileMessageContent(fileId ?? string.Empty);
            }

            if (m.Document is not null)
                return new FileMessageContent(m.Document.FileId ?? string.Empty);

            if (m.Video is not null)
                return new FileMessageContent(m.Video.FileId ?? string.Empty);

            if (m.Voice is not null)
                return new FileMessageContent(m.Voice.FileId ?? string.Empty);

            return MessageContent.Unknown;
        }

        // Message
        if (raw.Message is not null)
        {
            var m = raw.Message;
            Timestamp = new(m.Date);
            Sender = FromTelegramUser(m.From);
            Message = new TelegramMessage(Sender, new Messaging.ChatId(m.Chat?.Id ?? 0), MapContent(m));

            // Command parsing: keep Message, but mark update as Command and provide Interaction context.
            if (!string.IsNullOrEmpty(m.Text) && m.Text.TrimStart().StartsWith('/'))
            {
                var text = m.Text.Trim();
                var parts = text.Split([' '], 2, StringSplitOptions.RemoveEmptyEntries);
                var cmd = parts.Length > 0 ? parts[0] : string.Empty; // e.g. "/start@Bot"
                if (cmd.StartsWith('/'))
                    cmd = cmd.Substring(1);
                // strip optional @botusername
                int atIdx = cmd.IndexOf('@', StringComparison.Ordinal);
                if (atIdx >= 0)
                    cmd = cmd[..atIdx];

                var query = parts.Length > 1 ? parts[1].Trim() : null;
                Type = UpdateType.Command;
                Interaction = new TelegramInteraction(Sender, InteractionType.Command, cmd, options: null, query: query, raw: m);
                return new TelegramUpdate(Timestamp, Type, Sender, Message, Interaction, raw);
            }

            Type = UpdateType.MessageCreated;
            return new TelegramUpdate(Timestamp, Type, Sender, Message, null, raw);
        }

        // Edited message
        if (raw.EditedMessage is not null)
        {
            var m = raw.EditedMessage;
            Timestamp = new(m.EditDate ?? m.Date);
            Sender = FromTelegramUser(m.From);
            Message = new TelegramMessage(Sender, new Messaging.ChatId(m.Chat?.Id ?? 0), MapContent(m));
            Type = UpdateType.MessageEdited;
            return new TelegramUpdate(Timestamp, Type, Sender, Message, null, raw);
        }

        // Channel posts
        if (raw.ChannelPost is not null)
        {
            var m = raw.ChannelPost;
            Timestamp = new(m.Date);
            Sender = FromTelegramUser(m.From);
            Message = new TelegramMessage(Sender, new Messaging.ChatId(m.Chat?.Id ?? 0), MapContent(m));
            Type = UpdateType.MessageCreated;
            return new TelegramUpdate(Timestamp, Type, Sender, Message, null, raw);
        }

        if (raw.EditedChannelPost is not null)
        {
            var m = raw.EditedChannelPost;
            Timestamp = new(m.EditDate ?? m.Date);
            Sender = FromTelegramUser(m.From);
            Message = new TelegramMessage(Sender, new Messaging.ChatId(m.Chat?.Id ?? 0), MapContent(m));
            Type = UpdateType.MessageEdited;
            return new TelegramUpdate(Timestamp, Type, Sender, Message, null, raw);
        }

        // Callback query => button press
        if (raw.CallbackQuery is not null)
        {
            var cb = raw.CallbackQuery;
            Timestamp = cb.Message is not null ? new(cb.Message.Date) : DateTimeOffset.UtcNow;
            Sender = FromTelegramUser(cb.From);
            Type = UpdateType.CallbackQuery;
            Interaction = new TelegramInteraction(Sender, InteractionType.CallbackQuery, commandName: null, options: null, query: cb.Data, raw: cb);
            return new TelegramUpdate(Timestamp, Type, Sender, null, Interaction, raw);
        }

        // Inline query
        if (raw.InlineQuery is not null)
        {
            var iq = raw.InlineQuery;
            Timestamp = DateTimeOffset.UtcNow;
            Sender = FromTelegramUser(iq.From);
            Type = UpdateType.Interaction;
            Interaction = new TelegramInteraction(Sender, InteractionType.Command, commandName: null, options: null, query: iq.Query, raw: iq);
            return new TelegramUpdate(Timestamp, Type, Sender, null, Interaction, raw);
        }

        // Chosen inline result
        if (raw.ChosenInlineResult is not null)
        {
            var cir = raw.ChosenInlineResult;
            Timestamp = DateTimeOffset.UtcNow;
            Sender = FromTelegramUser(cir.From);
            Type = UpdateType.Interaction;
            Interaction = new TelegramInteraction(Sender, InteractionType.Command, commandName: null, options: null, query: cir.ResultId, raw: cir);
            return new TelegramUpdate(Timestamp, Type, Sender, null, Interaction, raw);
        }

        // MyChatMember / ChatMember / Poll / PollAnswer -> system events
        if (raw.MyChatMember is not null || raw.ChatMember is not null || raw.Poll is not null || raw.PollAnswer is not null)
        {
            Timestamp = DateTimeOffset.UtcNow;
            Sender = raw.MyChatMember?.From is not null ? FromTelegramUser(raw.MyChatMember.From) : new UserIdentity(0);
            Type = UpdateType.System;
            return new TelegramUpdate(Timestamp, Type, Sender, null, null, raw);
        }

        // Default
        return new TelegramUpdate(DateTimeOffset.UtcNow, UpdateType.None, new UserIdentity(0), null, null, raw);
    }
}
