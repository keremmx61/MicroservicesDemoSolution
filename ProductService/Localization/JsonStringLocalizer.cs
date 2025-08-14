using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Text.Json;

namespace ProductService.Localization
{
    public class JsonStringLocalizer : IStringLocalizer<SharedResources>
    {
        private static readonly Dictionary<string, Dictionary<string, string>> _cache = new();

        public LocalizedString this[string name]
        {
            get
            {
                var culture = CultureInfo.CurrentUICulture.Name;

                if (!_cache.TryGetValue(culture, out var localizations))
                {
                    var filePath = Path.Combine(AppContext.BaseDirectory, "Localization", $"{culture}.json");

                    if (!File.Exists(filePath))
                        filePath = Path.Combine(AppContext.BaseDirectory, "Localization", "en-US.json");

                    if (File.Exists(filePath))
                    {
                        var json = File.ReadAllText(filePath);
                        localizations = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                                       ?? new Dictionary<string, string>();
                    }
                    else
                    {
                        localizations = new Dictionary<string, string>();
                    }

                    _cache[culture] = localizations;
                }

                var value = localizations.TryGetValue(name, out var result) ? result : name;
                return new LocalizedString(name, value, resourceNotFound: value == name);
            }
        }

        public LocalizedString this[string name, params object[] arguments] => this[name];

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var culture = CultureInfo.CurrentUICulture.Name;
            if (_cache.TryGetValue(culture, out var localizations))
            {
                foreach (var kvp in localizations)
                    yield return new LocalizedString(kvp.Key, kvp.Value, false);
            }
        }

        public IStringLocalizer WithCulture(CultureInfo culture) => this;
    }
}