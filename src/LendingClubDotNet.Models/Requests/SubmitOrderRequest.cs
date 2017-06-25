using System.Collections.Generic;

namespace LendingClubDotNet.Models.Requests
{
	public sealed class SubmitOrdersRequest
	{
		public int aid { get; set; }

		public List<Order> orders { get; set; }       

	}

	public sealed class Order
	{
		public int loanId { get; set; }

		public decimal requestedAmount { get; set; }

		public long? portfolioId { get; set; }
	}
}
