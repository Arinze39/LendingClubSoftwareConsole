using Newtonsoft.Json;

namespace LendingClubDotNet.Models.Requests
{
	public sealed class CreatePortfolioRequest
	{
		[JsonProperty("aid")]
		public int ActorId { get; set; }

		[JsonProperty("portfolioName")]
		public string PortfolioName { get; set; }

		[JsonProperty("portfolioDescription")]
		public string PortfolioDescription { get; set; }
	}
}
