using System;
using System.Collections.Generic;

namespace LendingClubDotNet.Models.Responses
{
	public sealed class NotesOwnedResponse
	{
		public List<Note> MyNotes { get; set; }
	}

	public sealed class Note
	{
		public string LoanStatus { get; set; }

		public decimal LoanId { get; set; }

		public decimal NoteId { get; set; }

		public string Grade { get; set; }

		public decimal LoanAmount { get; set; }

		public decimal NoteAmount { get; set; }

		public decimal InterestRate { get; set; }

		public decimal OrderId { get; set; }

		public int LoanLength { get; set; }

		public DateTime? IssueDate { get; set; }

		public DateTime OrderDate { get; set; }

		public DateTime? LoanStatusDate { get; set; }

		public decimal PaymentsReceived { get; set; }
	}
}
