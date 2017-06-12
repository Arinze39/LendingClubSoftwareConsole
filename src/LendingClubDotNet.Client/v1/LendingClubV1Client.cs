using LendingClubDotNet.Client.v1.Areas;

namespace LendingClubDotNet.Client.v1
{
	public class LendingClubV1Client
	{
		public LendingClubV1Client(string baseUrl, string authorizationToken, string investorId)
		{
			InitializeAreas(baseUrl, investorId, authorizationToken);
		}

		public LendingClubV1Client(string authorizationToken, string investorId)
		{
			InitializeAreas(c_defaultBaseUrl, investorId, authorizationToken);
		}

		private void InitializeAreas(string baseUrl, string investorId, string authorizationToken)
		{
			m_accountResource = new AccountResource(baseUrl, investorId, authorizationToken);
			m_loanResource = new LoanResource(baseUrl, investorId, authorizationToken);
		}

		public AccountResource AccountResource { get { return m_accountResource; } }

		public LoanResource LoanResource { get { return m_loanResource; }}


		private const string c_defaultBaseUrl = "https://api.lendingclub.com/api/investor/v1/";
		private AccountResource m_accountResource;
		private LoanResource m_loanResource;
	}
}
