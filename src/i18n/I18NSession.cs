﻿using System.Web;

namespace i18n
{
    /// <summary>
    /// A convenience class for localization operations
    /// </summary>
    public class I18NSession
    {
        protected const string SessionKey = "po:language";

        public virtual void Set(HttpContextBase context, string language)
        {
            if(context.Session != null)
            {
                context.Session[SessionKey] = language;
            }
        }

        public static string GetLanguageFromSession(HttpContext context)
        {
            object val;
            return context.Session != null && (val = context.Session[SessionKey]) != null
                       ? val.ToString()
                       : null;
        }

        public static string GetLanguageFromSession(HttpContextBase context)
        {
            object val;
            return context.Session != null && (val = context.Session[SessionKey]) != null
                       ? val.ToString()
                       : null;
        }

        public virtual string GetLanguageFromSessionOrService(HttpContextBase context)
        {
            var language = GetLanguageFromSession(context);
            if(language == null)
            {
                var languages = context.Request.UserLanguages;
                language = DefaultSettings.LocalizingService.GetBestAvailableLanguageFrom(languages);
                if (context.Session != null)
                {
                    context.Session.Add(SessionKey, language);
                }
            }
            return language;
        }

        public virtual string GetText(HttpContext context, string text)
        {
            string[] languages;
    
            // Prefer a stored value to browser-supplied preferences
            var stored = GetLanguageFromSession(context);
            if (stored != null)
            {
                languages = new[] { stored };
            }
            else
            {
                // Use the client's browser settings to find a match
                languages = context.Request.UserLanguages;
            }

            switch (DefaultSettings.DefaultLanguageMatchingAlgorithm)
            {
                case DefaultSettings.LanguageMatching.Basic:
                {
                    text = DefaultSettings.LocalizingService.GetText(text, languages);
                    break;
                }
                case DefaultSettings.LanguageMatching.Enhanced:
                {
                    LanguageItem[] lis = LanguageItem.ParseHttpLanguageHeader(context.Request.Headers["Accept-Language"]);
                    text = DefaultSettings.LocalizingServiceEnhanced.GetText(text, lis);
                    break;
                }
                default:
                    throw new System.ApplicationException();
            }

            return HttpUtility.HtmlDecode(text);
        }

        public virtual string GetText(HttpContextBase context, string text)
        {
            string[] languages;
    
            // Prefer a stored value to browser-supplied preferences
            var stored = GetLanguageFromSession(context);
            if (stored != null)
            {
                languages = new[] { stored };
            }
            else
            {
                // Use the client's browser settings to find a match
                languages = context.Request.UserLanguages;
            }

            switch (DefaultSettings.DefaultLanguageMatchingAlgorithm)
            {
                case DefaultSettings.LanguageMatching.Basic:
                {
                    text = DefaultSettings.LocalizingService.GetText(text, languages);
                    break;
                }
                case DefaultSettings.LanguageMatching.Enhanced:
                {
                    LanguageItem[] lis = LanguageItem.ParseHttpLanguageHeader(context.Request.Headers["Accept-Language"]);
                    text = DefaultSettings.LocalizingServiceEnhanced.GetText(text, lis);
                    break;
                }
                default:
                    throw new System.ApplicationException();
            }

            return HttpUtility.HtmlDecode(text);
        }

        public virtual string GetUrlFromRequest(HttpRequestBase context)
        {
            var url = context.RawUrl;
            if (url.EndsWith("/") && url.Length > 1)
            {
                // Support trailing slashes
                url = url.Substring(0, url.Length - 1);
            }
            return url;
        }
    }
}