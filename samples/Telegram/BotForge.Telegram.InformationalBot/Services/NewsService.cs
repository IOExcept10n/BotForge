using System.Globalization;
using System.Net.Http.Json;
using BotForge.Localization;
using BotForge.Messaging;
using Microsoft.Extensions.Configuration;

namespace BotForge.Telegram.InformationalBot.Services;

internal sealed class NewsService(HttpClient httpClient, IConfiguration config, ILocalizationService localization) : INewsService
{
    private readonly HttpClient _http = httpClient;
    private readonly ILocalizationService _localization = localization;
    private readonly string _apiKey = config["ApiKeys:NewsApi"] ?? throw new InvalidOperationException("NewsApi API key is missing (ApiKeys:NewsApi)");

    public async Task<string> GetNewsStringAsync(CultureInfo targetLocale, CancellationToken cancellationToken)
    {
        try
        {
            var url = $"https://newsapi.org/v2/top-headlines?country={targetLocale.TwoLetterISOLanguageName}&pageSize=5&apiKey={_apiKey}";
            var resp = await _http.GetFromJsonAsync<NewsApiResponse>(url, cancellationToken).ConfigureAwait(false);
            if (resp == null || resp.Status != "ok" || resp.Articles == null || resp.Articles.Length == 0)
            {
                return Properties.Localization.NoNews;
            }

            var lines = resp.Articles
                .Where(a => !string.IsNullOrWhiteSpace(a.Title))
                .Select((a, i) => $"{i + 1}. {a.Title} {(string.IsNullOrWhiteSpace(a.Url) ? "" : $"- {a.Url}")}");

            return $"{Emoji.Newspaper.ToUnicode()} {_localization.GetString(targetLocale, nameof(Properties.Localization.LatestNews))}:\n" + string.Join("\n\n", lines);
        }
        catch (Exception ex) when (ex is not TaskCanceledException)
        {
            return _localization.GetString(targetLocale, nameof(Properties.Localization.ErrorFetchingNews), ex.Message);
        }
    }

    private sealed class NewsApiResponse
    {
        public string? Status { get; set; }
        public int TotalResults { get; set; }
        public Article[]? Articles { get; set; }
    }

    private sealed class Article
    {
        public Source? Source { get; set; }
        public string? Author { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }
        public string? UrlToImage { get; set; }
        public string? PublishedAt { get; set; }
        public string? Content { get; set; }
    }

    private sealed class Source
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }
}
