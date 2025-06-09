using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using YuGiOhResult.Models;
using Newtonsoft.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace YuGiOhResult.ViewModels
{
    partial class ViewModelBase : ObservableObject
    {

        // 宣言
        protected string matchesDataPath;
        protected string decksDataPath;
        protected string ociBucketUrl = "https://objectstorage.ap-northeast-1.oraclecloud.com/n/nrcfexeh4wiw/b/YuGiOhCounter_Backet/o/"; // OCIのバケットURL
        protected string authToken = "ed5#(iCvj_SQg<5NU8ZO"; // OCIの認証トークン（セキュリティ上の理由で実際のトークンはここに書かないこと）
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
        public async Task UploadJsonToOCI(string jsonContent)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{ociBucketUrl}/yourfile.json", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Upload successful!");
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
