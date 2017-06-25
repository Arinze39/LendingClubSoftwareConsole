using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingClubDotNet.Models.Responses
{
    public sealed class CancelTransferFundsResponse
    {
        public int investorId { get; set; }
        public List<cancellationResults> CancelResult { get; set; }
    }

    public sealed class cancellationResults
    {
        public int transferId { get; set; }
        public string status { get; set; }
        public string message { get; set; }
    }
}
