using System.Collections.Generic;
using System.Media;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;



internal class Style
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }
}

internal class SupportedFeatures
{
    [JsonPropertyName("permitted_synthesis_morphing")]
    public string PermittedSynthesisMorphing { get; set; }
}

internal class Speaker
{
    [JsonPropertyName("supported_features")]
    public SupportedFeatures SupportedFeatures { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("speaker_uuid")]
    public string SpeakerUuid { get; set; }

    [JsonPropertyName("styles")]
    public List<Style> Styles { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }
}

internal static class VoicevoxUtility
{
    const string baseUrl = "http://127.0.0.1:50021/";
    // localhostだとレスポンスが遅いのアドレス指定
    private static readonly HttpClient httpClient = new HttpClient();

    public static IEnumerable<Speaker> EnumerateSpeakers()
    {
        var jsonStr = GetSpeakersAsJson().Result;
        var deserialized = JsonSerializer.Deserialize<List<Speaker>>(jsonStr);
        if (deserialized is null)
        {
            yield break;
        }

        foreach (var speakerInfo in deserialized)
        {
            yield return speakerInfo;
        }
    }

    private static async Task<string> GetSpeakersAsJson()
    {
        using var requestMessage = new HttpRequestMessage(new HttpMethod("GET"), $"{baseUrl}speakers");
        requestMessage.Headers.TryAddWithoutValidation("accept", "application/json");

        requestMessage.Content = new StringContent("");
        requestMessage.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
        var response = await httpClient.SendAsync(requestMessage);
        return await response.Content.ReadAsStringAsync();
    }
}

public partial class HmVoiceVoxSpeak
{
    public static int GetSpearker(string speaker_name)
    {
        foreach (var speaker in VoicevoxUtility.EnumerateSpeakers())
        {
            Console.WriteLine($"{speaker.Name}");
            if (speaker_name == speaker.Name)
            {
                foreach (var style in speaker.Styles)
                {
                    Console.WriteLine($"{speaker.Name}: {style.Id}");
                }
            }
        }

        return 1;
    }
}
