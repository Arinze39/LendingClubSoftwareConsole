using System;

namespace LendingClubDotNet.Models.Requests
{
    public sealed class AddFundsRequest
    {
        public string transferFrequency { get; set; }
        public double amount { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }

    public class Frequency
    {
        public  string LOAD_NOW = "LOAD_NOW";
        public  string LOAD_ONCE = "LOAD_ONCE";
        public  string LOAD_WEEKLY = "LOAD_WEEKLY";
        public  string LOAD_BIWEEKLY = "LAOD_BIWEEKLY";
        public  string LOAD_MONTHLY = "LOAD_MONTHLY";
       

    }
}
