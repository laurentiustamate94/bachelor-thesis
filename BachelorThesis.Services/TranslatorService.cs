using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using BachelorThesis.Abstractions;

namespace BachelorThesis.Services
{
    public sealed class TranslatorService : ITranslatorService
    {
        private readonly string localeLanguage = null;
        private readonly string baseUri = null;
        private readonly IAuthenticationService authenticationService = null;

        public TranslatorService(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
            this.baseUri = "https://api.microsofttranslator.com/V2/Http.svc/Translate";
            this.localeLanguage = ConfigurationManager.AppSettings["TranslatorLocaleLanguage"];
        }

        public async Task<string> TranslateToEnglish(string text)
        {
            return await this.Translate(text, this.localeLanguage, "en");
        }

        public async Task<string> TranslateToLocale(string text)
        {
            return await this.Translate(text, "en", localeLanguage);
        }

        private async Task<string> Translate(string text, string fromLanguage, string toLanguage)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {this.authenticationService.Token}");
                var url = $"{this.baseUri}?text={text}&from={fromLanguage}&to={toLanguage}";

                var response = await client.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                var document = new XmlDocument()
                {
                    InnerXml = responseContent
                };

                return document.InnerText;
            }
        }
    }
}
