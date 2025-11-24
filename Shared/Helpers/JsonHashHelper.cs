using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Security.Cryptography;
using System.Collections.Concurrent;

namespace Product_Config_Customer_v0.Shared.Helpers
{
    public static class JsonHashHelper
    {
        private static readonly ConcurrentDictionary<string, (string NormalizedJson, string Hash, DateTime Timestamp)> _recentHashes
            = new();

        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
        private const int MaxCacheSize = 500;

        public static string NormalizeJson(string json)
        {
            var node = JsonNode.Parse(json);
            var normalized = CleanAndSort(node);
            return normalized.ToJsonString(new JsonSerializerOptions { WriteIndented = false });
        }

        private static JsonNode CleanAndSort(JsonNode? node)
        {
            switch (node)
            {
                case JsonObject obj:
                    var cleaned = new JsonObject();
                    foreach (var prop in obj.OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase))
                    {
                        var value = CleanAndSort(prop.Value);
                        if (value == null) continue;
                        cleaned[prop.Key] = value;
                    }
                    return cleaned;
                case JsonArray arr:
                    return new JsonArray(arr.Select(CleanAndSort).Where(n => n != null).ToArray());
                case JsonValue val:
                    var strVal = val.ToString()?.Trim();
                    return string.IsNullOrEmpty(strVal) ? null! : JsonValue.Create(strVal);
                default:
                    return null!;
            }
        }

        public static string ComputeSha256(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes);
        }

        public static (string NormalizedJson, string Hash) NormalizeAndHash(string json)
        {
            var normalized = NormalizeJson(json);
            var hash = ComputeSha256(normalized);
            return (normalized, hash);
        }

        public static (string NormalizedJson, string Hash) NormalizeAndHashCached(string json)
        {
            var rawHash = ComputeSha256(json);

            if (_recentHashes.TryGetValue(rawHash, out var entry) &&
                (DateTime.UtcNow - entry.Timestamp) < CacheDuration)
                return (entry.NormalizedJson, entry.Hash);

            var result = NormalizeAndHash(json);
            _recentHashes[rawHash] = (result.NormalizedJson, result.Hash, DateTime.UtcNow);

            if (_recentHashes.Count > MaxCacheSize)
            {
                var oldKeys = _recentHashes
                    .Where(kv => (DateTime.UtcNow - kv.Value.Timestamp) > CacheDuration)
                    .Select(kv => kv.Key)
                    .ToList();
                foreach (var key in oldKeys) _recentHashes.TryRemove(key, out _);
            }

            return result;
        }
    }
}
