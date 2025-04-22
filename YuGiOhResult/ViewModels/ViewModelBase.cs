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

namespace YuGiOhResult.ViewModels
{
    partial class ViewModelBase : ObservableObject
    {

        // 宣言
        public string matchesDataPath;
        public string decksDataPath;
        protected enum FileType
        {
            Decks,
            Matches
        }

        [ObservableProperty]
        private List<Deck> _decks;

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

        // JSONデータ書き込み
        protected void JsonWrite(FileType fileType)
        {
            // JSONデータ作成
            var options = new JsonSerializerOptions();
            options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            options.WriteIndented = true;
            var json = System.Text.Json.JsonSerializer.Serialize(Decks, options);

            if (fileType == FileType.Decks)
                // デッキリストのJSONデータを保存
                File.WriteAllText(decksDataPath, json);
            else
                // マッチリストのJSONデータを保存
                File.WriteAllText(matchesDataPath, json);
        }

        // 画面表示イベント
        [RelayCommand]
        public void Appearing()
        {
            // デッキリストの初期化
            Decks = JsonConvert.DeserializeObject<List<Deck>>(JsonLoad(decksDataPath)) ?? new List<Deck>();
        }
    }
}
