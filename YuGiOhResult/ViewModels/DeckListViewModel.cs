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
        private string filepath;
        [ObservableProperty]
        private ObservableCollection<Deck> _decks;

        // コンストラクタ
        public DeckListViewModel()
        {
            // セーブデータ呼び出し
            filepath = Path.Combine(FileSystem.AppDataDirectory, "decks.json");
            //Decks = JsonConvert.DeserializeObject<ObservableCollection<Deck>>(JsonLoad(filepath)) ?? new ObservableCollection<Deck>();
        }
    }
}
