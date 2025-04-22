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
    partial class DeckRegistrationViewModel : ViewModelBase
    {
        // 宣言
        private string filepath;
        //private List<Deck> decks;
        [ObservableProperty]
        private string _deckName;
        [ObservableProperty]
        private string _announcement;

        public DeckRegistrationViewModel()
        {
            //// セーブデータ呼び出し
            //filepath = Path.Combine(FileSystem.AppDataDirectory, "decks.json");
            //decks = JsonConvert.DeserializeObject<List<Deck>>(JsonLoad(filepath)) ?? new List<Deck>();  // JsonLoadメソッドでJSONデータを読み取り後、デシリアライズする
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

            if (Decks.Exists(x => x.Name == DeckName))
            {
                Announcement = "登録済みのデッキです";
                return;
            }

            Deck newDeck = new Deck
            {
                Name = DeckName
            };
            Decks.Add(newDeck);

            // デッキリストのJSONデータを保存
            JsonWrite(FileType.Decks);

            // 終了メッセージ
            Announcement = "登録完了";
            await Task.Delay(1500);
            Announcement = "";
        } 

    }
}
