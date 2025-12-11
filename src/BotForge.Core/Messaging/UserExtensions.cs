using System;
using System.Collections.Generic;
using System.Globalization;

namespace BotForge.Messaging;

/// <summary>
/// Provides extension methods and properties for <see cref="UserIdentity"/> class.
/// </summary>
public static class UserExtensions
{
    /// <param name="user">A user identity to extend functionality for.</param>
    extension(UserIdentity user)
    {
        /// <summary>
        /// Gets the locale to use for the current user based on his locale preferences.
        /// </summary>
        /// <remarks>
        /// If the <see cref="UserIdentity.PreferredLocale"/> is set, it will be used.
        /// Otherwise will be used the <see cref="UserIdentity.PlatformLocale"/> if this is set.
        /// As a fallback value, the <see cref="CultureInfo.InvariantCulture"/> will be used.
        /// </remarks>
        public CultureInfo TargetLocale => user.PreferredLocale ?? user.PlatformLocale ?? CultureInfo.InvariantCulture;

        /// <summary>
        /// Gets the normalized username that can be used for users indexing.
        /// </summary>
        
        public string NormalizedName => user.Username?.ToUpperInvariant() ?? user.DisplayName?.ToUpperInvariant()?.Replace(" ", "_", StringComparison.Ordinal) ?? user.Id.ToString(CultureInfo.InvariantCulture);
    }
}
