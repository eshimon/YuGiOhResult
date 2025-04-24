using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuGiOhResult.ViewModels
{
    partial class ResultListViewModel : ViewModelBase
    {
        // 画面描画時のコマンド
        public override void Appearing()
        {
            JsonLoad(FileType.Matches);
            JsonLoad(FileType.Decks);
        }
    }


}
