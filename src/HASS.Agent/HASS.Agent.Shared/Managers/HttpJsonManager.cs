using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace HASS.Agent.Shared.Managers;

/// <summary>
/// Fetches JSON documents from HTTP endpoints, caching them briefly so that multiple
/// sensors sharing an endpoint only cause a single request per interval
/// </summary>
public static class HttpJsonManager
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Maximum size of a response we're willing to read, bounding what an endpoint can make us
    /// hold in memory. A LibreHardwareMonitor document covering a few hundred sensors is ~100 KB
    /// </summary>
    private const long MaxResponseBytes = 16 * 1024 * 1024;

    private static readonly HttpClient HttpClient = new() { Timeout = RequestTimeout, MaxResponseContentBufferSize = MaxResponseBytes };
    private static readonly ConcurrentDictionary<string, CachedDocument> Cache = new();

    private sealed class CachedDocument
    {
        internal JObject Document { get; set; }
        internal string Error { get; set; }
        internal DateTime FetchedAt { get; set; } = DateTime.MinValue;
    }

    /// <summary>
    /// Fetches the JSON document at the provided url, returning a recently cached copy if one is available.
    /// Failures are cached as well, to avoid hammering an endpoint that's down.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="document"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static bool TryFetch(string url, out JObject document, out string error)
    {
        var cached = Cache.GetOrAdd(url, _ => new CachedDocument());

        // only one caller fetches a given url at a time, the rest use its result
        lock (cached)
        {
            if (cached.FetchedAt.Add(CacheDuration) < DateTime.Now)
                Fetch(url, cached);

            document = cached.Document;
            error = cached.Error;
        }

        return string.IsNullOrEmpty(error);
    }

    private static void Fetch(string url, CachedDocument cached)
    {
        cached.FetchedAt = DateTime.Now;

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            // synchronous by design: the sensors calling this expose a synchronous interface
            using var response = HttpClient.Send(request);

            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("[HTTPJSON] Endpoint '{url}' returned {code} {reason}", url, (int)response.StatusCode, response.ReasonPhrase);

                cached.Document = null;
                cached.Error = $"{(int)response.StatusCode} {response.ReasonPhrase}";

                return;
            }

            using var stream = response.Content.ReadAsStream();
            using var streamReader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(streamReader);

            cached.Document = JObject.Load(jsonReader);
            cached.Error = null;
        }
        catch (Exception ex)
        {
            Log.Warning("[HTTPJSON] Error fetching '{url}': {err}", url, ex.Message);

            cached.Document = null;
            cached.Error = ex.Message;
        }
    }
}
