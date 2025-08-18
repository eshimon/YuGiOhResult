using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.IdentityModel.Tokens;
using Oci.Common.Auth;
using Oci.Common.Http.Signing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
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

        // JSONデータ読み込み（下記同名メソッドのサブルーチン）
        protected string JsonLoad(string path)
        {
            if (File.Exists(path)) return File.ReadAllText(path);
            else return string.Empty;
        }

        // JSONデータ読み込み
        protected void JsonLoad(FileType fileType) 
        {
            if (fileType == FileType.Decks)
                // デッキリストのJSONデータを読み込み
                //Decks = JsonConvert.DeserializeObject<List<Deck>>(JsonLoad(decksDataPath)) ?? new List<Deck>();
                Decks = System.Text.Json.JsonSerializer.Deserialize<List<Deck>>(JsonLoad(decksDataPath)) ?? new List<Deck>();
            else
                // マッチリストのJSONデータを読み込み
                Matches = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<MatchResult>>(JsonLoad(matchesDataPath)) ?? new ObservableCollection<MatchResult> ();
        }

        // JSONデータ書き込み
        protected void JsonWrite(FileType fileType)
        {

            if (fileType == FileType.Decks) 
            {
                // デッキリストのJSONデータを保存
                var json = SerializeToJsonString(Decks);
                File.WriteAllText(decksDataPath, json);
            }
            else
            {
                // マッチリストのJSONデータを保存
                var json = SerializeToJsonString(Matches);
                File.WriteAllText(matchesDataPath, json);
            }
        }

        // JSON文字列へシリアライズ
        private string SerializeToJsonString(object data)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
            };
            return System.Text.Json.JsonSerializer.Serialize(data, options);
        }

        // appsettings.jsonから値を取得
        private async Task<AppSettings> LoadAppSettingsAsync()
        {
            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync("appsettings.json");
            using StreamReader reader = new StreamReader(fileStream);
            string jsonContent = await reader.ReadToEndAsync();

            using var doc = JsonDocument.Parse(jsonContent);
            var appSettingsSection = doc.RootElement.GetProperty("AppSettings");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var appSettings = appSettingsSection.Deserialize<AppSettings>(options);
            return appSettings;
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
                AppSettings appSettings = await LoadAppSettingsAsync();
                var objectName = fileType == FileType.Decks ? "decks.json" : "matches.json";
                var uploadUri = $"{appSettings.ApiHttp}/upload-object/{appSettings.NamespaceName}/{appSettings.BacketName}/{objectName}";

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

        // ストリームからJSONデータをメモリへ直接ロードする
        private async Task<T> LoadJsonFromStreamAsync<T>(Stream stream) where T : new()
        {
            if (stream == null || stream.Length == 0) return new T();
            return await System.Text.Json.JsonSerializer.DeserializeAsync<T>(stream) ?? new T();
        }

        // JSONデータをOCIからダウンロードする
        protected async Task DownloadJsonAsync(FileType fileType) 
        {
            try
            {
                // OCIのダウンロード元情報を設定
                AppSettings appSettings = await LoadAppSettingsAsync();
                var objectName = fileType == FileType.Decks ? "decks.json" : "matches.json";
                var downloadUrl = $"{appSettings.ApiHttp}/download-object/{appSettings.NamespaceName}/{appSettings.BacketName}/{objectName}";

                // HttpClientを使用してダウンロード
                using HttpClient client = new HttpClient();
                using HttpResponseMessage response = await client.GetAsync(downloadUrl);

                response.EnsureSuccessStatusCode(); // リクエストが成功したか確認
                using Stream contentStream = await response.Content.ReadAsStreamAsync();
                if (FileType.Decks == fileType)
                {
                    var newDecks = new List<Deck>();
                    newDecks = LoadJsonFromStreamAsync<List<Deck>>(contentStream).Result; // Streamからオブジェクトデータへ
                    if (newDecks.IsNullOrEmpty()) return;
                    Decks = newDecks; // メモリ上のデッキリストを更新
                    JsonWrite(FileType.Decks); // デッキリストのJSONデータを上書き
                }
                else
                {
                    var newMatches  = new ObservableCollection<MatchResult>();
                    newMatches = LoadJsonFromStreamAsync<ObservableCollection<MatchResult>>(contentStream).Result; // Streamからオブジェクトデータへ
                    if (newMatches.IsNullOrEmpty()) return;
                    Matches = newMatches; // メモリ上のマッチリストを更新
                    JsonWrite(FileType.Matches); // マッチリストのJSONデータを保存
                }

            }
            catch(Exception ex)
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
