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
using System.Windows.Input;
using System.Diagnostics;

namespace YuGiOhResult.ViewModels
{
    partial class MainPageViewModel : ViewModelBase
    {

        // 宣言
        private IList<MatchResult> matches;

        // バインディング用プロパティ宣言
        [ObservableProperty]
        private string[] _coinList = new string[] { "表", "裏" };
        [ObservableProperty]
        private string _coin;
        [ObservableProperty]
        private string[] _turnOrderList = new string[] { "先攻", "後攻" };
        [ObservableProperty]
        private string _turnOrder;
        [ObservableProperty]
        private string[] _resultList = new string[] { "勝ち", "負け"};
        [ObservableProperty]
        private string _result;
        [ObservableProperty]
        private string? _playedDeck;
        [ObservableProperty]
        private string? _opponentsDeck;
        [ObservableProperty]
        private string? _memo;
        [ObservableProperty]
        private string? _announcement;


        public MainPageViewModel()
        {
            // 初期化
            Coin = CoinList[0];
            TurnOrder = TurnOrderList[0];
            Result = ResultList[0];
            Announcement = string.Empty;
            string json = string.Empty;
            
            // マッチデータ呼び出し
            matches = JsonConvert.DeserializeObject<List<MatchResult>>(JsonLoad(matchesDataPath)) ?? new List<MatchResult>();

        }

        // 登録コマンド
        [RelayCommand]
        public async Task Resister()
        {
            MatchResult result = new MatchResult
            {
                PlayedDeck = this.PlayedDeck,
                OpponentsDeck = this.OpponentsDeck,
                Coin = this.Coin,
                Result = this.Result,
                Memo = this.Memo,
                DateTime = DateTime.Now
            };
            matches.Add(result);

            // JSONデータ作成
            var options = new JsonSerializerOptions();
            options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            options.WriteIndented = true;
            var json = System.Text.Json.JsonSerializer.Serialize(matches, options);

            // ファイルに書き込む
            File.WriteAllText(matchesDataPath, json);

            // 終了メッセージ
            Announcement = "登録完了";
            await Task.Delay(1500);
            Announcement = "";

        }

    }
    
}
