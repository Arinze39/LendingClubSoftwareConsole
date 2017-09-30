using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingClubDotNet.Models.Requests
{
    public sealed class ReturnOrderBook
    {
        public List<Currency> ReturnCurrency { get; set; }
    }

    public class Currency
    {
        public String[] asks { get; set; }
        public string[] bids { get; set; }
        public int isFrozen { get; set; }
        public long seq { get; set; }
    }
}
