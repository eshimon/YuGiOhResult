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
        public string? matchesDataPath;
        public string? decksDataPath;

        [ObservableProperty]
        private List<Deck> _decks;

        // コンストラクタ
        public ViewModelBase()
        {
            decksDataPath = Path.Combine(FileSystem.AppDataDirectory, "decks.json");
            matchesDataPath = Path.Combine(FileSystem.AppDataDirectory, "matches.json");
        }

        protected string JsonLoad(string path)
        {
            if (File.Exists(path)) return File.ReadAllText(path);
            else return string.Empty;
        }

        [RelayCommand]
        public void Appearing()
        {
            // デッキリストの初期化
            Decks = JsonConvert.DeserializeObject<List<Deck>>(JsonLoad(decksDataPath)) ?? new List<Deck>();
        }
    }
}
