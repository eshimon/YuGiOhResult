using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Oci.Common.Auth;
using Oci.Common.Http.Signing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Threading.Tasks;
using YuGiOhResult.Models;


namespace YuGiOhResult.ViewModels
{
    partial class ViewModelBase : ObservableObject
    {

        // 宣言
        protected string matchesDataPath;   
        protected string decksDataPath;

        protected enum FileType
        {
            Decks,
            Matches
        }

        [ObservableProperty]
        private List<Deck> _decks;　// デッキリスト

        [ObservableProperty]
        private ObservableCollection<MatchResult> _matches;　// マッチデータ

        // コンストラクタ
        public ViewModelBase()
        {
            decksDataPath = Path.Combine(FileSystem.AppDataDirectory, "decks.json");
            matchesDataPath = Path.Combine(FileSystem.AppDataDirectory, "matches.json");
        }

        // JSONデータ読み込み
        protected string JsonLoad(string path)
        {
            if (File.Exists(path)) return File.ReadAllText(path);
            else return string.Empty;
        }

        protected void JsonLoad(FileType fileType) 
        {
            if (fileType == FileType.Decks)
                // デッキリストのJSONデータを読み込み
                Decks = JsonConvert.DeserializeObject<List<Deck>>(JsonLoad(decksDataPath)) ?? new List<Deck>();
            else
                // マッチリストのJSONデータを読み込み
                Matches = JsonConvert.DeserializeObject<ObservableCollection<MatchResult>>(JsonLoad(matchesDataPath)) ?? new ObservableCollection<MatchResult>();
        }

        // JSONデータ書き込み
        protected void JsonWrite(FileType fileType)
        {
            // JSONデータ作成
            var options = new JsonSerializerOptions();
            options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            options.WriteIndented = true;
            if (fileType == FileType.Decks) 
            {
                // デッキリストのJSONデータを保存
                var json = System.Text.Json.JsonSerializer.Serialize(Decks, options);
                File.WriteAllText(decksDataPath, json);
            }
            else
            {
                // マッチリストのJSONデータを保存
                var json = System.Text.Json.JsonSerializer.Serialize(Matches, options);
                File.WriteAllText(matchesDataPath, json);
            }

        }

        // OCIにJSONデータをアップロードする
        protected async Task UploadJsonToOCIAsync(FileType fileType)
        {
            // ファイルパスを取得
            var jsonPath = fileType == FileType.Decks ? decksDataPath : matchesDataPath;
           
            // JSONファイルをバイト配列として読み込む
            var fileBytes = File.ReadAllBytes(jsonPath);

            // OCIのアップロード先情報を設定
            var namespaceName = "nrcfexeh4wiw";
            var bucketName = "YuGiOhCounter_Backet";
            var objectName = fileType == FileType.Decks ? "decks.json" : "matches.json";
            var region = "ap-tokyo-1";
            var uploadUri = $"https://objectstorage.{region}.oraclecloud.com/n/{namespaceName}/b/{bucketName}/o/{objectName}";

            try
            {
                // OCIの署名付きHandlerを使う
                var handler = OciHttpClientHandler.FromConfigFile("C:\\Users\\User\\.oci\\config", "DEFAULT");
                using var client = new HttpClient(handler);

                var request = new HttpRequestMessage(HttpMethod.Put, new Uri(uploadUri))
                {
                    Content = new ByteArrayContent(fileBytes)
                };
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                    Console.WriteLine("✅ アップロード成功！");
                else
                    Console.WriteLine($"❌ エラー：{response.StatusCode}, 内容: {await response.Content.ReadAsStringAsync()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ エラー: {ex.Message}");
                return;
            }

        }



        // 画面表示イベント
        [RelayCommand]
        public virtual void Appearing()
        {
            // デッキリストの初期化
            JsonLoad(FileType.Decks);
        }
    }
}
