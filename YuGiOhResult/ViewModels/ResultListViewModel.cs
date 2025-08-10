using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuGiOhResult.Models;

namespace YuGiOhResult.ViewModels
{
    partial class ResultListViewModel : ViewModelBase
    {
        // 画面描画時のコマンド
        public override void Appearing()
        {
            JsonLoad(FileType.Matches);
            JsonLoad(FileType.Decks);
            UploadJsonToOCIAsync(FileType.Decks);
            UploadJsonToOCIAsync(FileType.Matches);
        }

        // 削除コマンド
        [RelayCommand]
        public async Task DeleteMatch(MatchResult match)
        {
            if (match == null) return;

            // 現在のウィンドウのページを取得
            var currentPage = Application.Current?.Windows.FirstOrDefault()?.Page;

            if (currentPage == null)
                // ページが取得できない場合は処理を中断
                return;
            
            // 確認ダイアログを表示
            bool answer = await currentPage.DisplayAlert("確認", $"{match.FormattedDateTime}の対戦履歴を削除しますか？", "Yes", "No");
            if (!answer) return;

            // マッチリストから削除
            Matches.Remove(match);

            // JSONデータ書き込み
            JsonWrite(FileType.Matches);
        }

        public ResultListViewModel()
        {
            //UploadJsonToOCIAsync(FileType.Decks);
            //UploadJsonToOCIAsync(FileType.Matches);
        }
    }



}
