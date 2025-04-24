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
using Microsoft.Maui.Controls;

namespace YuGiOhResult.ViewModels
{
    partial class DeckListViewModel : ViewModelBase
    {
        
        // デッキ削除コマンド
        [RelayCommand]
        public async Task DeleteDeck(Deck deck)
        {
            // 確認ダイアログを表示
            bool answer = await Application.Current.MainPage.DisplayAlert("確認", $"{deck.Name}を削除しますか？", "Yes", "No");
            if (!answer) return;

            // デッキリストから削除
            Decks.Remove(deck);

            // JSON書き込み
            JsonWrite(FileType.Decks);

            // デッキリストのロード
            JsonLoad(FileType.Decks);

        }

    }
}
