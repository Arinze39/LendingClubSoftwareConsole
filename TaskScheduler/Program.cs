using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendingClubDotNet.Client.v1;
using LendingClubDotNet.Models.Requests;
using LendingClubDotNet.Models.Responses;
using System.Diagnostics;


namespace TaskScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            //This initializes the title of the software
            Console.Title = "Lending Club Software v1.0.0 AutoInvestor";

            //this changes the color of the text.
            Console.ForegroundColor = ConsoleColor.White;
            
            Console.ReadKey();
        }
    }

    class AutoInvest
    {
        Stopwatch stopwatch = new Stopwatch();
        LendingClubV1Client Client;
        StringBuilder save_to_file = new StringBuilder();
        StringBuilder timeElapsedfile = new StringBuilder();

        long _portfolioId = 00000;
        int maxloans_to_Buy = 0;
        int ActorId = 0;
        List<int> loanId_in_NotesOwned = new List<int>();
        List<Loan> filteredLoans = new List<Loan>();

        public AutoInvest(string AuthorisationToken, string InvestorId)
        {
            Client = new LendingClubV1Client(AuthorisationToken, InvestorId);
            save_to_file.AppendLine(string.Format("Account logged in. {0}", time()));
        }

        public string time()
        {
            return DateTime.Now.ToString();
        }

        //This function calculates max loans to buy.
        public void MaxLoans_to_Buy()
        {
            decimal cashBalance = 0.0M;
            
            AvailableCashResponse Avcash = new AvailableCashResponse();
            try
            {

                Avcash = Client.AccountResource.GetAvailableCash();

                cashBalance = Avcash.AvailableCash;
                save_to_file.AppendLine(string.Format("AvailableCash got = {0}. {1}", cashBalance, time()));

                maxloans_to_Buy = (int)cashBalance / 25;
                save_to_file.AppendLine(string.Format("No_of_Loans_calculated = {0}. {1}", maxloans_to_Buy, time()));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error {0}\nStackTrace {1}", ex.Message, ex.StackTrace);
                save_to_file.AppendLine(string.Format("Error {0}\nError occured in {1}\n{2}", ex.Message, ex.StackTrace, time()));

            }
            
        }

        //This function pulls loanId's of already owned notes
        public void Pull_NotesOwned()
        {
            NotesOwnedResponse getNotes = new NotesOwnedResponse();
            Note mNote = new Note();

            try
            {
                getNotes.MyNotes = new List<Note>();
                getNotes.MyNotes.Add(mNote);

                getNotes = Client.AccountResource.GetNotesOwned();
                save_to_file.AppendLine(string.Format("Notes Owned got. {0}", time()));

                stopwatch.Reset();
                stopwatch.Start();
                foreach(var item in getNotes.MyNotes)
                {
                    loanId_in_NotesOwned.Add((short)item.LoanId);
                }
                stopwatch.Stop();
                timeElapsedfile.AppendLine(string.Format("TimeElapsed(milliseconds) to get loanId of NotesOwned{0:F3}", (stopwatch.ElapsedMilliseconds / 1000.0).ToString()));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error {0}\nStackTrace {1}", ex.Message, ex.StackTrace);
                save_to_file.AppendLine(string.Format("Error {0}\nError occured in {1}\n{2}", ex.Message, ex.StackTrace, time()));

            }
        }

        //This function pulls new listed loans from LC and removes already invested loans
        public void Pull_New_Loans()
        {
            ListedLoansResponse getloans = new ListedLoansResponse();
            Loan mLoans = new Loan();

            try
            {
                getloans.Loans = new List<Loan>();
                getloans.Loans.Add(mLoans);

                getloans = Client.LoanResource.GetListedLoans();
                save_to_file.AppendLine(string.Format("New Loans got. {}", time()));

                foreach(var item in getloans.Loans)
                {
                    //get id of new loans
                    int id = item.Id;
                    //add new loans to list
                    filteredLoans.Add(item);
                    for (int i = 0; i <= loanId_in_NotesOwned.Count; i++) 
                    {
                        //check if id of new loans match old loans and remove
                        if(id == loanId_in_NotesOwned[i])
                        {
                            filteredLoans.Remove(item); break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error {0}\nStackTrace {1}", ex.Message, ex.StackTrace);
                save_to_file.AppendLine(string.Format("Error {0}\nError occured in {1}\n{2}", ex.Message, ex.StackTrace, time()));

            }
        }

        //This function runs filter on the new loans
        public void FilterLoans()
        {
            stopwatch.Reset();
            stopwatch.Start();
            //This runs the filters on the listed loans
            foreach (var item in filteredLoans)
            {
                decimal? deliq_rate = (decimal?)item.NumAcctsEver120Ppd / item.TotalAcc;
                if (item.Dti > 25 || item.Term != 36 || item.SubGrade.ToUpper() == "E4" || item.SubGrade.ToUpper() == "E5"
                    || item.Grade.ToUpper() == "A" || item.Grade.ToUpper() == "F" || item.Grade.ToUpper() == "G" 
                    || item.Grade.ToUpper() == "H" || item.HomeOwnership.ToLower() != "mortgage" || item.AnnualInc <= 4500
                    || item.NumTl120dpd2m != null || item.NumTl120dpd2m != 0 || deliq_rate >= 0.45M)
                {
                    filteredLoans.Remove(item);
                }
            }
            stopwatch.Stop();
            timeElapsedfile.AppendLine(string.Format("TimeElapsed(milliseconds) to filter loans \t{0:F3}", stopwatch.ElapsedMilliseconds / 1000.0));


            List<int> loanId = new List<int>();
            List<decimal>_percentFunded = new List<decimal>();
            foreach(var item in filteredLoans)
            {
                _percentFunded.Add((item.FundedAmount / item.LoanAmount));
                loanId.Add(item.Id);
            }

            //Sort the filteredloan in decreasing _percentFunded 
            List<int> SortedLoanId = new List<int>();
            int N = _percentFunded.Count;
            int[] index = Enumerable.Range(0, N).ToArray();           
            Array.Sort(index, (a, b) => _percentFunded[b].CompareTo(_percentFunded[a]));           
            foreach (int i in index)
            {
                //Add the sorted loan to a new list
                SortedLoanId.Add(loanId[i]);
                save_to_file.AppendLine(string.Format("LoanID: {0}\t percentFunded: {1}", loanId[i], _percentFunded[i]));
            }



            List<Order> myOrders = new List<Order>();
            if(loanId.Count <= maxloans_to_Buy)
            {
                foreach (var item in SortedLoanId)
                {
                    Order newOrder = new Order
                    {
                        loanId = item,
                        requestedAmount = 25,
                        portfolioId = _portfolioId
                    };
                    myOrders.Add(newOrder);
                }              
            }
            else
            {
                for (int i = 0; i <= maxloans_to_Buy; i++)
                {
                    Order newOrder = new Order
                    {
                        loanId = SortedLoanId[i],
                        requestedAmount = 25,
                        portfolioId = _portfolioId
                    };
                }
            }
            
        }
    }
}
