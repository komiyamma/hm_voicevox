using System.Media;
using System.Net.Http.Headers;


public partial class HmVoiceVoxSpeak {

    public static async Task Main(String[] args)
    {
            string installLocation = @"C:\usr\voicevox\VOICEVOX.exe";
            Console.WriteLine("Voicevoxのインストール場所: " + installLocation);


        GetSpearker("春日部つむぎ");


        using (var httpClient = new HttpClient())
        {
            string query;
            int speaker = 2;
            string text = "windows11で文章を読み上げるには？";

            // 音声クエリを生成
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), $"http://localhost:50021/audio_query?text={text}&speaker={speaker}"))
            {
                request.Headers.TryAddWithoutValidation("accept", "application/json");

                request.Content = new StringContent("");
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                var response = await httpClient.SendAsync(request);

                query = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(query);
            }

            // 音声クエリから音声合成
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "http://localhost:50021/synthesis?speaker=2&enable_interrogative_upspeak=true"))
            {
                request.Headers.TryAddWithoutValidation("accept", "audio/wav");

                request.Content = new StringContent(query);
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                var response = await httpClient.SendAsync(request);

                // 音声を保存
                using (var fileStream = System.IO.File.Create("test.wav"))
                {
                    using (var httpStream = await response.Content.ReadAsStreamAsync())
                    {
                        httpStream.CopyTo(fileStream);
                        fileStream.Flush();
                    }
                }
            }
        }

        //読み込む
        var player = new SoundPlayer("test.wav");
        //再生する
        player.PlaySync();
        Console.WriteLine("再生完了");


    }
}