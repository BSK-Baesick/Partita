// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.


namespace Naninovel
{
    /// <summary>
    /// Provides extension methods for <see cref="ILocalizationManager"/>.
    /// </summary>
    public static class LocalizationManagerExtensions
    {
        /// <summary>
        /// Whether <see cref="LocalizationConfiguration.SourceLocale"/> is currently selected.
        /// </summary>
        public static bool IsSourceLocaleSelected (this ILocalizationManager manager) => manager.SelectedLocale == manager.Configuration.SourceLocale;
    }
}
