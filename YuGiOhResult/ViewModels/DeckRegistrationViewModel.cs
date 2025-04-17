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
    partial class DeckRegistrationViewModel : ObservableObject
    {
        // 宣言
        private string filepath;
        private List<Deck> decks;
        [ObservableProperty]
        private string _deckName;
        [ObservableProperty]
        private string _announcement;

        public DeckRegistrationViewModel()
        {
            // セーブデータ呼び出し
            filepath = Path.Combine(FileSystem.AppDataDirectory, "decks.json");
            string json;
            // セーブデータがあればロードして処理終わり
            if (File.Exists(filepath))
            {
                json = File.ReadAllText(filepath);
                decks = JsonConvert.DeserializeObject<List<Deck>>(json) ?? new List<Deck>();
            }
            else
            {
                // 空のセーブデータ(JSON)を作成
                decks = new List<Deck>();
                json = System.Text.Json.JsonSerializer.Serialize(decks, new JsonSerializerOptions { WriteIndented = true });

                // ファイルに書き込む
                File.WriteAllText(filepath, json);
            }

        }

        // デッキ登録コマンド
        [RelayCommand]
        public async Task DeckResister()
        {
            // 入力チェック
            if (string.IsNullOrWhiteSpace(DeckName))
            {
                Announcement = "デッキ名が空です";
                return;
            }

            if (decks.Exists(x => x.Name == DeckName))
            {
                Announcement = "登録済みのデッキです";
                return;
            }



            Deck newDeck = new Deck
            {
                Name = DeckName
            };
            decks.Add(newDeck);

            // JSONデータ作成
            var options = new JsonSerializerOptions();
            options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            options.WriteIndented = true;
            var json = System.Text.Json.JsonSerializer.Serialize(decks, options);

            // ファイルに書き込む
            File.WriteAllText(filepath, json);
            
            // 終了メッセージ
            Announcement = "登録完了";
            await Task.Delay(1500);
            Announcement = "";
        } 

    }
}
