using System.Collections.Generic;

namespace LendingClubDotNet.Models.Responses
{
	public sealed class SubmitOrdersResponse
	{
		public int OrderInstructId { get; set; }

		public List<OrderConfirmation> OrderConfirmations { get; set; }
	}

	public sealed class OrderConfirmation
	{
		public int OrderInstructId { get; set; }

		public int LoanId { get; set; }

		public decimal RequestedAmount { get; set; }

		public decimal InvestedAmount { get; set; }

		public List<ExecutionStatus> ExecutionStatus { get; set; }
	}

	public enum ExecutionStatus
	{
		ORDER_FULFILLED,
		LOAN_AMNT_EXCEEDED,
		NOT_AN_IN_FUNDING_LOAN,
		REQUESTED_AMNT_LOW,
		REQUESTED_AMNT_ROUNDED,
		AUGMENTED_BY_MERGE,
		ELIM_BY_MERGE,
		INSUFFICIENT_CASH,
		MAX_AMNT_PER_LOAN_EXCEEDED,
		MAX_PERCENT_PER_LOAN_EXCEEDED,
		MAX_PERCENT_PER_LOAN_TIME_EXCEEDED,
		MAX_AMNT_PER_LOAN_TIME_EXCEEDED,
		NOT_AN_INVESTOR,
		NOT_A_VALID_INVESTMENT,
		NOTE_ADDED_TO_PORTFOLIO,
		NOT_A_VALID_PORTFOLIO,
		ERROR_ADDING_NOTE_TO_PORTFOLIO,
		SYSTEM_BUSY
	}
}
