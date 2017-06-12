using LendingClubDotNet.Models.Responses;

namespace LendingClubDotNet.Client.v1.Areas
{
	public sealed class LoanResource
	{
		public LoanResource(string baseUrl, string investorId, string authorizationToken)
		{
			m_baseUrl = baseUrl;
			m_investorId = investorId;
			m_authorizationToken = authorizationToken;
		}

		public ListedLoansResponse GetListedLoans(bool showAll = true)
		{
			return RequestUtility.ExecuteGetRequest<ListedLoansResponse>(string.Format("{0}loans/listing?showAll={1}", m_baseUrl, showAll), m_authorizationToken);
		}

		private readonly string m_baseUrl;

		private readonly string m_authorizationToken;

		private readonly string m_investorId;
	}
}
