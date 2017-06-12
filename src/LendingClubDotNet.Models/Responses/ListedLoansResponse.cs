using System;
using System.Collections.Generic;

namespace LendingClubDotNet.Models.Responses
{
	public sealed class ListedLoansResponse
	{
		public DateTime AsOfDate { get; set; }

		public List<Loan> Loans { get; set; }
	}

	public sealed class Loan
	{
		public DateTime AsOfDate { get; set; }

		public int Id { get; set; }

		public int MemberId { get; set; }

		public int Term { get; set; }

		public decimal IntRate { get; set; }

		public decimal ExpDefaultRate { get; set; }

		public decimal ServiceFeeRate { get; set; }

		public decimal Installment { get; set; }

		public string Grade { get; set; }

		public string SubGrade { get; set; }

		public int? EmpLength { get; set; }

		public string HomeOwnership { get; set; }

		public decimal? AnnualInc { get; set; }

		public string IsIncV { get; set; }

		public string AcceptD { get; set; }

		public DateTime ExpD { get; set; }

		public string ListD { get; set; }

		public string CreditPullD { get; set; }

		public string ReviewStatusD { get; set; }

		public string ReviewStatus { get; set; }

		public string Desc { get; set; }

		public string Purpose { get; set; }

		public string AddrZip { get; set; }

		public string AddrState { get; set; }

		public int? InvestorCount { get; set; }

		public DateTime IlsExpD { get; set; }

		public string InitialListStatus { get; set; }

		public string EmpTitle { get; set; }

		public int AccNowDelinq { get; set; }

		public int AccOpenPast24Mths { get; set; }

		public int? BcOpenToBuy { get; set; }

		public decimal? PercentBcGt75 { get; set; }

		public decimal? BcUtil { get; set; }

		public decimal? Dti { get; set; }

		public int? Delinq2Yrs { get; set; }

		public decimal? DelinqAmnt { get; set; }

		public string EarliestCrLine { get; set; }

		public int? FicoRangeLow { get; set; }

		public int? FicoRangeHigh { get; set; }

		public int? InqLast6Mths { get; set; }

		public int? MthsSinceLastDelinq { get; set; }

		public int? MthsSinceLastRecord { get; set; }

		public int? MthsSinceRecentInq { get; set; }

		public int? MthsSinceRecentRevolDelinq { get; set; }

		public int? MthsSinceRecentBc { get; set; }

		public int? MortAcc { get; set; }

		public int? OpenAcc { get; set; }

		public int? PubRec { get; set; }

		public int? TotalBalExMort { get; set; }

		public decimal? RevolBal { get; set; }

		public decimal? RevolUtil { get; set; }

		public int? TotalBcLimit { get; set; }

		public int? TotalAcc { get; set; }

		public int? TotalIlHighCreditLimit { get; set; }

		public int? NumRevAccts { get; set; }

		public int? MthsSinceRecentBcDlq { get; set; }

		public int? PubRecBankruptcies { get; set; }

		public int? NumAcctsEver120Ppd { get; set; }

		public int? ChargeoffWithin12Mths { get; set; }

		public int? Collections12MthsExMed { get; set; }

		public int? TaxLiens { get; set; }

		public int? MthsSinceLastMajorDerog { get; set; }

		public int? NumSats { get; set; }

		public int? NumTlOpPast12m { get; set; }

		public int? MoSincRcntTl { get; set; }

		public int? TotHiCredLim { get; set; }

		public int? TotCurBal { get; set; }

		public int? AvgCurBal { get; set; }

		public int? NumBcTl { get; set; }

		public int? NumActvBcTl { get; set; }

		public int? NumBcSats { get; set; }

		public int? PctTlNvrDlq { get; set; }

		public int? NumTl90gDpd24m { get; set; }

		public int? NumTl30dpd { get; set; }

		public int? NumTl120dpd2m { get; set; }

		public int? NumIlTl { get; set; }

		public int? MoSinOldIlAcct { get; set; }

		public int? NumActvRevTl { get; set; }

		public int? MoSinOldRevTlOp { get; set; }

		public int? MoSinRcntRevTlOp { get; set; }

		public int? TotalRevHiLim { get; set; }

		public int? NumRevTlBalGt0 { get; set; }

		public int? NumOpRevTl { get; set; }

		public int? TotCollAmt { get; set; }

		public decimal FundedAmount { get; set; }

		public decimal LoanAmount { get; set; }
	}
}
