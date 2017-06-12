using System.Collections.Generic;

namespace LendingClubDotNet.Models.Responses
{
	public sealed class PortfoliosOwnedResponse
	{
		public List<Portfolio> MyPortfolios { get; set; }
	}

	public sealed class Portfolio
	{
		public int PortfolioId { get; set; }

		public string PortfolioName { get; set; }

		public string PortfolioDescription { get; set; }
	}
}
