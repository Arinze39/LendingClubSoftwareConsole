using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingClubDotNet.Models.Responses
{
    public sealed class AddFundsResponse
    {
        public int investorId { get; set; }
        public double amount { get; set; }
        public string transferFrequency { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public DateTime estimatedFundsTransferDate { get; set; }
    }
}
