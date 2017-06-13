using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendingClubDotNet.Client.v1;
using LendingClubDotNet.Models.Responses;

namespace LendingClubSoftware
{
    class Program
    {
        
        //This is the entry point of the software.
        //This is the first code that will run when you start the software.
        static void Main(string[] args)
        {
            //This initializes the title of the software
            Console.Title = "Lending Club Software v1.0.0";
           
            //this changes the color of the text.
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            
            //Add another instance of the class 'Account' with AuthToken And ID, to create and manage another account
            Account _1stAcct = new Account { AuthorizationToken = "YOUR_AUTHORISATION_TOKEN_GOES_HERE", InvestorId = "YOUR_INVESTOR_ID_GOES_HERE" };
            Account _2ndAcct = new Account { AuthorizationToken = "YOUR_AUTHORISATION_TOKEN_GOES_HERE", InvestorId = "YOUR_INVESTOR_ID_GOES_HERE" };

            Console.WriteLine("Get AvailableCash 1stAccount\n");
            _1stAcct.getAvailableCash(); Console.WriteLine("\nGet Summary\n");
            _1stAcct.getSummary(); Console.WriteLine("\nGet NotesOwned\n");
            _1stAcct.getNotesOwned();

            Console.WriteLine("\nGet AvailableCash 2ndAcct\n\n");

            _2ndAcct.getAvailableCash(); Console.WriteLine("\nGet Summary\n");
            _2ndAcct.getSummary(); Console.WriteLine("\nGet NotesOwned\n");
            _2ndAcct.getNotesOwned();
                       
            Console.ReadKey();
        }
    }
     class Account
    {
        public string AuthorizationToken { private get; set; }
        public string InvestorId { get; set; }
                
        //This function gets the current time of the PC
        public string time()
        {
            return DateTime.Now.ToLongTimeString();
        }

        //This function gets the available cash of the account logged in
        public void getAvailableCash()
        {
            LendingClubV1Client Client = new LendingClubV1Client(AuthorizationToken,InvestorId);
            AvailableCashResponse Avcash = new AvailableCashResponse();
           
            try
            {

                Avcash = Client.AccountResource.GetAvailableCash();
                Console.WriteLine("InvestorID: {0}\t{1}  ", Avcash.InvestorId, time());
                Console.WriteLine(string.Format("AvailableCash: {0}\t{1}", Avcash.AvailableCash.ToString("C"), time()));
            }
            catch(Exception ex)
            {
                Console.WriteLine(string.Format("Error {0}\nStackTrace {1}", ex.Message, ex.StackTrace));
            }
        }

        //Function to get the summary of the logged in account
        public void getSummary()
        {
            LendingClubV1Client Client = new LendingClubV1Client(AuthorizationToken, InvestorId);
            SummaryResponse summary = new SummaryResponse();

            try
            {
                summary = Client.AccountResource.GetSummary();

                Console.WriteLine("InvestorID: {0}\t{1}", summary.InvestorId, time());
                Console.WriteLine("AvailableCash: {0}\t{1}", summary.AvailableCash.ToString("C"), time());
                Console.WriteLine("AccruedInterest: {0}\t{1}", summary.AccruedInterest, time());
                Console.WriteLine("OustandingPrincipal: {0}\t{1}", summary.OutstandingPrincipal, time());
                Console.WriteLine("AccountTotal: {0}\t{1}", summary.AccountTotal, time());
                Console.WriteLine("TotalNotes: {0}\t{1}", summary.TotalNotes, time());
                Console.WriteLine("TotalPortfolios: {0}\t{1}", summary.TotalPortfolios, time());
                Console.WriteLine("InfundingBalance: {0}\t{1}", summary.InFundingBalance, time());
                Console.WriteLine("ReceivedInterest: {0}\t{1}", summary.ReceivedInterest, time());
                Console.WriteLine("ReceivedPrincipal: {0}\t{1}", summary.ReceivedPrincipal, time());
                Console.WriteLine("ReceivedLateFees: {0}\t{1}", summary.ReceivedLateFees, time());
            }
            catch(Exception ex) { Console.WriteLine("Error {0} StackTrace {1}", ex.Message, ex.StackTrace); };
        }

        //This function gets the Notes Owned by the Account Logged in
        public void getNotesOwned()
        {
            LendingClubV1Client Client = new LendingClubV1Client(AuthorizationToken, InvestorId);
            NotesOwnedResponse getNotes = new NotesOwnedResponse();
            //Note mNote = new Note();
            try
            {
                getNotes.MyNotes = new List<Note>();
                //getNotes.MyNotes.Add(mNote);

                getNotes = Client.AccountResource.GetNotesOwned();
                
                foreach(var items in getNotes.MyNotes)
                {
                    Console.WriteLine("LoanStatus: {0}\t{1}",items.LoanStatus,time());
                    Console.WriteLine("LoanID: {0}\t{1}", items.LoanId, time());
                    Console.WriteLine("NoteID: {0}\t{1}", items.NoteId, time());
                    Console.WriteLine("Grade: {0}\t{1}", items.Grade, time());
                    Console.WriteLine("LoanAmount: {0}\t{1}", items.LoanAmount, time());
                    Console.WriteLine("NoteAmount: {0}\t{1}", items.NoteAmount, time());
                    Console.WriteLine("InterestRate: {0}\t{1}", items.InterestRate, time());
                    Console.WriteLine("OrderID: {0}\t{1}", items.OrderId, time());
                    Console.WriteLine("LoanLength: {0}\t{1}", items.LoanLength, time());
                    Console.WriteLine("IssueDate: {0}\t{1}", items.IssueDate, time());
                    Console.WriteLine("OrderDate: {0}\t{1}", items.OrderDate, time());
                    Console.WriteLine("LoanStatusDate: {0}\t{1}", items.LoanStatusDate, time());
                    Console.WriteLine("PaymentReceived: {0}\t{1}", items.PaymentsReceived.ToString("C"), time());
                }
                
                      
            }
            catch(Exception ex) { Console.WriteLine("Error {0}\nStackTrace{1}", ex.Message, ex.StackTrace); }
        }
    }
}
