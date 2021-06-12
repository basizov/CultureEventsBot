using CultureEventsBot.Domain.Enums;

namespace CultureEventsBot.Core.Core
{
    public static class LanguageHandler
    {
        public static string	ChooseLanguage(ELanguage language, string english, string russian) => language == ELanguage.English ? english : russian;
    }
}