using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuGiOhResult.Models
{
    public class MatchResult
    {
        public string? PlayedDeckName { get; set; }
        public string? OpponentsDeckName { get; set; }
        public string Coin { get; set; }
        public string TurnOrder { get; set; }
        public string Result { get; set; }
        public string? Memo { get; set; }
        public DateTime DateTime { get; set; }

        // フォーマット済みの日時プロパティ
        public string FormattedDateTime => DateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
