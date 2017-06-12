using System.Collections.Generic;

namespace LendingClubDotNet.Models.Responses
{
	public sealed class CreatePortfolioResponse
	{
		public int PortfolioId { get; set; }

		public string PortfolioName { get; set; }

		public string PortfolioDescription { get; set; }

		public List<CreatePortfolioResponseError> Errors { get; set; }
}

	public sealed class CreatePortfolioResponseError
	{
		public string Field { get; set; }

		public string Code { get; set; }

		public string Message { get; set; }
	}
}
