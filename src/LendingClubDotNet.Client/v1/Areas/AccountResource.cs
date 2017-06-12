using LendingClubDotNet.Models.Requests;
using LendingClubDotNet.Models.Responses;

namespace LendingClubDotNet.Client.v1.Areas
{
	public sealed class AccountResource
	{
		public AccountResource(string baseUrl, string investorId, string authorizationToken)
		{
			m_baseUrl = baseUrl;
			m_investorId = investorId;
			m_authorizationToken = authorizationToken;
		}

		// Get operations
		public AvailableCashResponse GetAvailableCash()
		{
			return RequestUtility.ExecuteGetRequest<AvailableCashResponse>(string.Format("{0}accounts/{1}/availablecash", m_baseUrl, m_investorId), m_authorizationToken);
		}

		public SummaryResponse GetSummary()
		{
			return RequestUtility.ExecuteGetRequest<SummaryResponse>(string.Format("{0}accounts/{1}/summary", m_baseUrl, m_investorId), m_authorizationToken);
		}

		public NotesOwnedResponse GetNotesOwned()
		{
			return RequestUtility.ExecuteGetRequest<NotesOwnedResponse>(string.Format("{0}accounts/{1}/notes", m_baseUrl, m_investorId), m_authorizationToken);
		}

		public DetailedNotesOwnedResponse GetDetailedNotesOwnedResponse()
		{
			return RequestUtility.ExecuteGetRequest<DetailedNotesOwnedResponse>(string.Format("{0}accounts/{1}/detailednotes", m_baseUrl, m_investorId), m_authorizationToken);
		}

		public PortfoliosOwnedResponse GetPortfoliosOwned()
		{
			return RequestUtility.ExecuteGetRequest<PortfoliosOwnedResponse>(string.Format("{0}accounts/{1}/portfolios", m_baseUrl, m_investorId), m_authorizationToken);
		}

		// Post operations
		public CreatePortfolioResponse CreatePortfolio(CreatePortfolioRequest createPortfolioRequest)
		{
			return RequestUtility.ExecutePostRequest<CreatePortfolioRequest, CreatePortfolioResponse>(createPortfolioRequest, string.Format("{0}accounts/{1}/portfolios", m_baseUrl, m_investorId), m_authorizationToken);
		}

		public SubmitOrdersResponse SubmitOrders(SubmitOrdersRequest submitOrdersRequest)
		{
			return RequestUtility.ExecutePostRequest<SubmitOrdersRequest, SubmitOrdersResponse>(submitOrdersRequest, string.Format("{0}accounts/{1}/orders", m_baseUrl, m_investorId), m_authorizationToken);
		}

		private readonly string m_baseUrl;

		private readonly string m_authorizationToken;

		private readonly string m_investorId;
	}
}
