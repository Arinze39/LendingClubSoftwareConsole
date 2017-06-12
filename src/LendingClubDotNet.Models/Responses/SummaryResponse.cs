namespace LendingClubDotNet.Models.Responses
{
	public sealed class SummaryResponse
	{
		public decimal AvailableCash { get; set; }

		public long InvestorId { get; set; }

		public decimal AccruedInterest { get; set; }

		public decimal OutstandingPrincipal { get; set; }

		public decimal AccountTotal { get; set; }

		public int TotalNotes { get; set; }

		public int TotalPortfolios { get; set; }

		public decimal InFundingBalance { get; set; }

		public decimal ReceivedInterest { get; set; }

		public decimal ReceivedPrincipal { get; set; }

		public decimal? ReceivedLateFees { get; set; }
	}
}
