using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuGiOhResult.Models
{
    class MatchResult
    {
        // 使用デッキ
        public string? PlayedDeck { get; set; }
        // 対戦相手のデッキ
        public string? OpponentsDeck { get; set; }
        // コインの表裏
        public string Coin { get; set; }
        // 勝敗
        public string Result { get; set; }
        // 備考
        public string? Memo { get; set; }
        // 登録日時
        public DateTime DateTime { get; set; }
    }
}
