﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingClubDotNet.Models.Requests
{
    public sealed class CancelTransferFundsRequest
    {
        public int[] transferId { get; set; }
    }
}
