using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingClubDotNet.Models.Responses
{
   public sealed class WithdrawFundsResponse
    {
        public int investorId { get; set; }
        public double amount { get; set; }
        public DateTime estimatedFundsTransferDate { get; set; }

    }
}
