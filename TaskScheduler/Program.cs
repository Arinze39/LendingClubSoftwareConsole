using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendingClubDotNet.Client.v1;
using LendingClubDotNet.Models.Requests;
using LendingClubDotNet.Models.Responses;


namespace TaskScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            //This initializes the title of the software
            Console.Title = "Lending Club Software v1.0.0 AutoInvestor";

            //this changes the color of the text.
            Console.ForegroundColor = ConsoleColor.DarkGreen;


            //Console.WriteLine(DateTime.Now.ToLongTimeString());
            //System.Threading.Thread.Sleep(5000);
            //Console.WriteLine(DateTime.Now.ToLongTimeString());
            Console.ReadKey();
        }
    }

    class AutoInvest
    {
        LendingClubV1Client Client;
        StringBuilder save_to_file = new StringBuilder();
        public AutoInvest(string AuthorisationToken, string InvestorId)
        {
            Client = new LendingClubV1Client(AuthorisationToken, InvestorId);
            save_to_file.AppendLine(string.Format("Account logged in. {0}", time()));
        }

        public string time()
        {
            return DateTime.Now.ToString();
        }

        //This function gets the balance of the account logged in.
        public int Calc_no_Loans()
        {
            decimal cashBalance = 0.0M;
            int noOfLoans = 0;
            AvailableCashResponse Avcash = new AvailableCashResponse();
            try
            {

                Avcash = Client.AccountResource.GetAvailableCash();

                cashBalance = Avcash.AvailableCash;
                save_to_file.AppendLine(string.Format("AvailableCash got = {0}. {1}", cashBalance, time()));

                noOfLoans = (int)cashBalance / 25;
                save_to_file.AppendLine(string.Format("No_of_Loans_calculated = {0}. {1}", noOfLoans, time()));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error {0}\nStackTrace {1}", ex.Message, ex.StackTrace);
                save_to_file.AppendLine(string.Format("Error {0}\nError occured in {1}\n{2}", ex.Message, ex.StackTrace, time()));

            }
            return noOfLoans;
        }

        //This function pulls old Loans from LC
        public void Pull_Old_Loans()
        {
            ListedLoansResponse getloans = new ListedLoansResponse();

            try
            {
                getloans.Loans = new List<Loan>();

                getloans = Client.LoanResource.GetListedLoans();
                save_to_file.AppendLine(string.Format("Old Loans got. {}", time()));

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error {0}\nStackTrace {1}", ex.Message, ex.StackTrace);
                save_to_file.AppendLine(string.Format("Error {0}\nError occured in {1}\n{2}", ex.Message, ex.StackTrace, time()));

            }
        }

        public void Pull_New_Loans()
        {
            ListedLoansResponse getloans = new ListedLoansResponse();

            try
            {
                getloans.Loans = new List<Loan>();

                getloans = Client.LoanResource.GetListedLoans();
                save_to_file.AppendLine(string.Format("New Loans got. {}", time()));

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error {0}\nStackTrace {1}", ex.Message, ex.StackTrace);
                save_to_file.AppendLine(string.Format("Error {0}\nError occured in {1}\n{2}", ex.Message, ex.StackTrace, time()));

            }
        }
    }
}
