using System.Collections.Generic;
using System;
namespace LendingClubDotNet.Models.Responses
{
    public sealed class PendingTransferResponse
    {
       public List<pendingTransfer> transfers { get; set; }
    }

    public sealed class pendingTransfer
    {
        public int transferId { get; set; }
        public DateTime transferDate { get; set; }
        public double amount { get; set; }
        public string sourceAccount { get; set; }
        public string status { get; set; }
        public string frequency { get; set; }
        public DateTime endDate { get; set; }
        public string operation { get; set; }
        public bool cancellable { get; set; }

    }

}
