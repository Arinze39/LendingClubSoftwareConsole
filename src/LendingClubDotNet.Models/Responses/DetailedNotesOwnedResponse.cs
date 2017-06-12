using System;
using System.Collections.Generic;

namespace LendingClubDotNet.Models.Responses
{
	public sealed class DetailedNotesOwnedResponse
	{
		public List<DetailedNote> MyNotes { get; set; }
	}

	public sealed class DetailedNote
	{
		public string LoanStatus { get; set; }

		public int LoanId { get; set; }

		public string PortfolioName { get; set; }

		public int NoteId { get; set; }

		public string Grade { get; set; }

		public decimal LoanAmount { get; set; }

		public decimal AccruedInterest { get; set; }

		public decimal NoteAmount { get; set; }

		public string Purpose { get; set; }

		public decimal InterestRate { get; set; }

		public int? PortfolioId { get; set; }

		public int OrderId { get; set; }

		public int LoanLength { get; set; }

		public DateTime? IssueDate { get; set; }

		public DateTime OrderDate { get; set; }

		public DateTime LoanStatusDate { get; set; }

		public string CreditTrend { get; set; }

		public string CurrentPaymentStatus { get; set; }

		public bool CanBeTraded { get; set; }

		public decimal PaymentsReceived { get; set; }

		public DateTime? NextPaymentDate { get; set; }

		public decimal PrincipalPending { get; set; }

		public decimal InterestPending { get; set; }

		public decimal InterestReceived { get; set; }

		public decimal PrincipalReceived { get; set; }
	}
}
