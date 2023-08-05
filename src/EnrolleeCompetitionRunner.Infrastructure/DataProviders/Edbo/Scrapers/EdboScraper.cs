using EnrolleeCompetitionRunner.Domain.Enums;
using EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.Abstractions;
using EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.Constants;
using EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.ScrapedModels;
using EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.ScrapedResultModels;
using HtmlAgilityPack;
using System.Text.Json;

namespace EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.Scrapers;
public sealed class EdboScraper : IEdboScraper
{
    private readonly IEnumerable<string> s_necessaryGos = new List<string>
    {
        $"{EducationalStageConstants.DesiredEducationalStage.BachelorCode}-{EducationalStageConstants.EnrollmentBasis.SecondaryEducationCode}",
        $"{EducationalStageConstants.DesiredEducationalStage.BachelorCode}-{EducationalStageConstants.EnrollmentBasis.SecondaryEducationAndNrk5Code}"
    };

    public IReadOnlyList<EdboResultSupercompetition> ScrapSupercompetitionPage(string rawHtml)
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(rawHtml);

        var scriptNode = htmlDocument.DocumentNode
            .ChildNodes.FindFirst("html")
            .ChildNodes.FindFirst("main")
            .ChildNodes.Last(n => n.Name == "script");
        var script = scriptNode.InnerText;

        var go = DeserializeFromScript<IDictionary<string, EdboGoItem>>(script, SupercompetitionConstants.GoVariableName);
        var globalOrderCodesAndEducationalInfo = s_necessaryGos.Select(g => new
        {
            EducationalInfo = g,
            Items = go[g].SpecialitiesAndSpecializationsWithGlobalOrderCodes
        })
            .SelectMany(g => g.Items.Select(s => s.Value), (g, o) => new { GlobalOrderCode = o, g.EducationalInfo })
            .DistinctBy(g => g.GlobalOrderCode);

        var globalOrders = DeserializeFromScript<IDictionary<string, EdboSupercompetition>>(script, SupercompetitionConstants.GlobalOrderVariableName);

        var supercompetitions = globalOrderCodesAndEducationalInfo
            .Select(c => new EdboResultSupercompetition
            {
                TotalPlaces = globalOrders[c.GlobalOrderCode].TotalPlaces,
                Code = c.GlobalOrderCode,
                EnrollmentBasis = GetEnrollmentBasisFromEducationalInfo(c.EducationalInfo),
                EducationalStage = GetEducationalStageFromEducationalInfo(c.EducationalInfo)
            })
            .ToList();

        var specialitiesAndSpecializationsCodes = s_necessaryGos
            .SelectMany(g => go[g].SpecialitiesAndSpecializationsWithGlobalOrderCodes.Select(kvp => new { SpecialityCodeAndSpecializationDictionaryCode = kvp.Key, SupercompetitionCode = kvp.Value }))
            .DistinctBy(s => s.SpecialityCodeAndSpecializationDictionaryCode);

        var scrapedSpecialities = DeserializeFromScript<IDictionary<string, string>>(script, SupercompetitionConstants.SpecialitiesVariableName);
        var scrapedSpecializations = DeserializeFromScript<IDictionary<string, EdboSpecialization>>(script, SupercompetitionConstants.SpecializationsVariableName);

        var specialities = specialitiesAndSpecializationsCodes
            .Select(s => new
            {
                SpecialityCode = GetSpecialityCodeFromJoinedCode(s.SpecialityCodeAndSpecializationDictionaryCode, '-'),
                SpecializationDictionaryCode = GetSpecializationCodeFromJoinedCode(s.SpecialityCodeAndSpecializationDictionaryCode, '-'),
                s.SupercompetitionCode
            })
            .Where(s => s.SpecializationDictionaryCode is null || scrapedSpecializations.ContainsKey(s.SpecializationDictionaryCode))
            .Select(s => new EdboResultSpeciality
            {
                Code = s.SpecialityCode,
                Name = scrapedSpecialities[s.SpecialityCode],
                SpecializationInternalCode = s.SpecializationDictionaryCode,         // Used in edbo universities search.
                SpecializationCode = s.SpecializationDictionaryCode is null ? null : GetSpecializationCodeFromJoinedCode(scrapedSpecializations[s.SpecializationDictionaryCode].SpecialityCodeAndSpecializationCode, '.'),
                SpecializationName = s.SpecializationDictionaryCode is null ? null : scrapedSpecializations[s.SpecializationDictionaryCode].Name,
                Supercompetition = supercompetitions.First(sc => sc.Code == s.SupercompetitionCode)
            });

        foreach (var speciality in specialities)
        {
            speciality.Supercompetition.Specialities.Add(speciality);
        }

        return supercompetitions;
    }

    private static T DeserializeFromScript<T>(string script, string variableName)
    {
        var objectString = GetScriptVariableValue(script, variableName);
        return JsonSerializer.Deserialize<T>(objectString)!;
    }

    private static string GetScriptVariableValue(string script, string variableName)
    {
        variableName = $"let {variableName}";
        var value = script[(script.IndexOf(variableName) + variableName.Length)..];
        value = value[(value.IndexOf('=') + 1)..];
        value = value[..value.IndexOf('\n')];
        value = value.Remove(value.LastIndexOf(';'));
        return value.Trim();
    }

    private static EducationalStage GetEnrollmentBasisFromEducationalInfo(string educationalInfo)
    {
        var enrollmentBasisCode = educationalInfo[(educationalInfo.IndexOf('-') + 1)..];
        return enrollmentBasisCode switch
        {
            EducationalStageConstants.EnrollmentBasis.SecondaryEducationCode => EducationalStage.SecondaryEducation,
            EducationalStageConstants.EnrollmentBasis.SecondaryEducationAndNrk5Code => EducationalStage.SecondaryEducationOrNrk5,
            _ => throw new InvalidOperationException($"Unknown enrollment basis with code {enrollmentBasisCode}")
        };
    }

    private static EducationalStage GetEducationalStageFromEducationalInfo(string educationalInfo)
    {
        var educationalStageCode = educationalInfo[..educationalInfo.IndexOf('-')];
        return educationalStageCode switch
        {
            EducationalStageConstants.DesiredEducationalStage.BachelorCode => EducationalStage.Bachelor,
            _ => throw new InvalidOperationException($"Unknown educational stage with code {educationalStageCode}")
        };
    }

    private static string GetSpecialityCodeFromJoinedCode(string code, char separator)
    {
        var delimiterPosition = code.IndexOf(separator);
        return delimiterPosition == -1 ? code : code[..delimiterPosition];
    }

    private static string? GetSpecializationCodeFromJoinedCode(string code, char separator)
    {
        var delimiterPosition = code.IndexOf(separator);
        return delimiterPosition == -1 ? null : code[(delimiterPosition + 1)..];
    }
}
