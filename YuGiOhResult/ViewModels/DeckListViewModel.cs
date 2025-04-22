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
    partial class DeckListViewModel : ViewModelBase
    {
        // 宣言
        [ObservableProperty]
        private Deck _selectedDeck;

        // コンストラクタ
        public DeckListViewModel() { }
        
        // デッキ削除コマンド
        [RelayCommand]
        public async Task DeleteDeck()
        {
            Decks.Remove(SelectedDeck);
            // JSON書き込み
            JsonWrite(FileType.Decks);

            // 終了メッセージ
            //Announcement = "登録完了";
            await Task.Delay(1500);
            //Announcement = "";

        }

    }
}
