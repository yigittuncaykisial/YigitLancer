// Services/Currency/CollectApiCurrencyService.cs
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Services.Contracts;
using Entities.DTOs;

namespace Services.Currency
{
    public class CollectApiCurrencyService : ICurrencyService
    {
        private readonly HttpClient _http;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CollectApiCurrencyService> _logger;
        private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public CollectApiCurrencyService(HttpClient http, IMemoryCache cache, ILogger<CollectApiCurrencyService> logger)
        {
            _http = http;
            _cache = cache;
            _logger = logger;
        }

        public async Task<CurrencyResponse?> GetRatesAsync(string baseCode = "USD", int amount = 1)
        {
            // Her zaman 1 birime normalize edeceğiz
            const int reqAmount = 100;                         // 100 veya 1000 kullanabilirsiniz
            string key = $"fx:{baseCode}:norm1";               // cache anahtarı — hep 1 birim dönüyoruz

            if (_cache.TryGetValue(key, out CurrencyResponse cached))
                return cached;

            try
            {
                var res = await _http.GetAsync($"economy/currencyToAll?int={reqAmount}&base={baseCode}");
                res.EnsureSuccessStatusCode();

                var json = await res.Content.ReadAsStringAsync();
                var obj = JsonSerializer.Deserialize<CurrencyResponse>(json, _json);

                // Normalize: rate = calculated / reqAmount (calculated varsa öncelik ver)
                if (obj?.Result?.Data is not null)
                {
                    foreach (var row in obj.Result.Data)
                    {
                        if (row is null) continue;

                        decimal normalized = row.Rate;          // fallback
                        if (row.Calculated > 0)
                            normalized = row.Calculated / reqAmount;

                        // Bazı sağlayıcılar 10x küçük döndürebiliyor — normalize edilmiş değeri esas al
                        if (normalized > 0)
                            row.Rate = normalized;
                    }
                }

                _cache.Set(key, obj!, TimeSpan.FromMinutes(5));
                return obj;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Currency fetch failed");
                return null;
            }
        }
    }
}
