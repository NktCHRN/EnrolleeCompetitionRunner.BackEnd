using System.Reflection;
using System.Text.Json.Serialization;

namespace EnrolleeCompetitionRunner.Infrastructure.DataProviders.Common.Utilities;
public static class FormDataHelperMethods
{
    public static Dictionary<string, string> ToFormDictionary(object request)
    {
        return request
            .GetType()
            .GetProperties()
                .Select(p => (p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? p.Name, p.GetValue(request)))
                .ToDictionary(p => p.Item1, p => p.Item2?.ToString() ?? string.Empty);
    }
}
