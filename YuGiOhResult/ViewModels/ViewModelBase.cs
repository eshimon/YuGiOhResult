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
using System.Net.Mime;
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

            if (fileType == FileType.Decks) 
            {
                // デッキリストのJSONデータを保存
                var json = SerializeToJson(Decks);
                File.WriteAllText(decksDataPath, json);
            }
            else
            {
                // マッチリストのJSONデータを保存
                var json = SerializeToJson(Matches);
                File.WriteAllText(matchesDataPath, json);
            }
        }

        // JSONシリアル化
        protected string SerializeToJson(object data)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
            };
            return System.Text.Json.JsonSerializer.Serialize(data, options);
        }

        // OCIにJSONデータをアップロードする
        protected async Task UploadJsonToOCIAsync(FileType fileType)
        {
            try
            {
                // アップロード対象のファイルパスを取得
                var jsonPath = fileType == FileType.Decks ? decksDataPath : matchesDataPath;

                // ペイロード作成
                using var multiPartContent = new MultipartFormDataContent();
                using var fileStream = File.OpenRead(jsonPath);
                using var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                multiPartContent.Add(streamContent, "file", Path.GetFileName(jsonPath));

                // OCIのアップロード先情報を設定
                var namespaceName = "nrcfexeh4wiw";
                var bucketName = "YuGiOhCounter_Backet";
                var objectName = fileType == FileType.Decks ? "decks.json" : "matches.json";
                var uploadUri = $"http://161.33.135.51:8000/upload-object/{namespaceName}/{bucketName}/{objectName}";

                // HttpClientを使用してアップロード
                using HttpClient client = new HttpClient();
                using HttpResponseMessage response = await client.PostAsync(uploadUri, multiPartContent);

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
