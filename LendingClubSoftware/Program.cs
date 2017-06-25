using System;
using System.Collections.Generic;
using LendingClubDotNet.Client.v1;
using LendingClubDotNet.Models.Responses;
using LendingClubDotNet.Models.Requests;
using Microsoft.Win32.TaskScheduler;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using TaskScheduler.Config;
using Newtonsoft.Json;


namespace LendingClubSoftware
{
    class Program
    {

        [DllImport("kernel32.dll", ExactSpelling = true)]

        private static extern IntPtr GetConsoleWindow();
        private static IntPtr ThisConsole = GetConsoleWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);        
        private const int MAXIMIZE = 3;
       


        //This is the entry point of the software.
        //This is the first code that will run when you start the software.
        static void Main(string[] args)
        {

            string myPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string myDir = Path.GetDirectoryName(myPath);
            string realpath = Path.Combine(myDir, "Config.json");

            //This shows the console window maximized.
            ShowWindow(ThisConsole, MAXIMIZE);

            //This makes sure the console window doesn't cut off outputs
            Console.BufferHeight = short.MaxValue - 1;

            //This initializes the title of the software
            Console.Title = "Lending Club Software v1.0.0";

            //this changes the color of the text.
            Console.ForegroundColor = ConsoleColor.White;

            string text = "";
            using (StreamReader str = new StreamReader(realpath))
            {
               text = str.ReadToEnd();
            }                                                     

           Configuration config = JsonConvert.DeserializeObject<Configuration>(text); 
                        
            foreach(var item in config.Account)
            {
                Console.WriteLine(item.ActorId);
                Console.WriteLine(item.AuthorisationToken);
                Console.WriteLine(item.PortfolioId);
                Console.WriteLine(item.InvestorId);
            }
            Console.ReadKey();
             
            

            //Register the TaskScheduler to automatically invest into LC
            // 
            //TaskScheduler.startTaskBy6AM();
            //TaskScheduler.startTaskBy10AM();
            //TaskScheduler.startTaskBy2PM();
            //TaskScheduler.startTaskBy6PM();


            //Add another instance of the class 'Account' with AuthToken And ID, to create and manage another account
            Account _1stAcct = new Account("","");
            //Account _2ndAcct = new Account("YOUR_AUTHORISATION_TOKEN_GOES_HERE", "YOUR_INVESTOR_ID_GOES_HERE");

            //This kickstarts the first account
            _1stAcct.start("Ist Account");

           // _2ndAcct.start("2nd Account");

            //_1stAcct.createPortfolios(11111, "PORTFOLIOID", "PORTFOLIODESCRIPTION");

            //_2ndAcct.createPortfolios(22222, "PORTFOLIOID", "PORTFOLIODESCRIPTION");
                                                         
            Console.ReadKey();
        }
    }
     class Account
    {
        //public string  { private get; set; }
        //public string  { private get; set; }

        private LendingClubV1Client Client;
        public Account(string AuthorizationToken, string InvestorId)
        {
           Client = new LendingClubV1Client(AuthorizationToken, InvestorId);
        }

        public void start(string AcctName)
        {                     
            getAvailableCash(AcctName);
            getSummary();
            getNotesOwned();
            getDetailedNotesOwned();
            getPorfolios();
            getFilters();
            getPendingTransfers();


            //You will need to set the transferFrequency inside the code.
            //Here you supply args which are startDate,endate,amount and then the transferFrequency inside the method
            //AddFunds(DateTime.Now, DateTime.Now, 24);

            //Here you put an array of transferids
            //CancelTransferFunds(new int[] { 12, 34, 23 });

            //Here you put the portfolioid,portfolioname,portfolioDescrition
            //createPortfolios(1111, "NAME", "PortfolioDescription");

            //Here you enter the amount to withdraw
            //WithdrawFunds(1111);


        }

        //This function gets the current time of the PC
        private string time()
        {
            return DateTime.Now.ToString();
        }
        
        //This function gets the available cash of the account logged in
        private void getAvailableCash(string AcctName)
        {
            Console.WriteLine("Get AvailableCash {0}",AcctName);
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
        private void getSummary()
        {           
            SummaryResponse summary = new SummaryResponse();

            try
            {
                Console.WriteLine("\nSummaryResponses \n");

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

        //This function gets the detailed notes owned.
        private void getDetailedNotesOwned()
        {           
            DetailedNotesOwnedResponse getDetailedNotes = new DetailedNotesOwnedResponse();
            DetailedNote mDetailedNote = new DetailedNote();
            try
            {
                Console.WriteLine("\nDetailedNotes Response\n");

                getDetailedNotes.MyNotes = new List<DetailedNote>();
                getDetailedNotes.MyNotes.Add(mDetailedNote);

                getDetailedNotes = Client.AccountResource.GetDetailedNotesOwnedResponse();              

                foreach (var item in getDetailedNotes.MyNotes)
                {
                    Console.WriteLine("LoanStatus: {0}\t{1}", item.LoanStatus, time());
                    Console.WriteLine("LoanID: {0}\t{1}", item.LoanId, time());
                    Console.WriteLine("PortfolioName: {0}\t{1}", item.PortfolioName, time());
                    Console.WriteLine("NoteID: {0}\t{1}", item.NoteId, time());
                    Console.WriteLine("Grade: {0}\t{1}", item.Grade, time());
                    Console.WriteLine("LoanAmount: {0}\t{1}", item.LoanAmount, time());
                    Console.WriteLine("AccruedInterest: {0}\t{1}", item.AccruedInterest, time());
                    Console.WriteLine("NoteAmount: {0}\t{1}", item.NoteAmount, time());
                    Console.WriteLine("Purpose: {0}\t{1}", item.Purpose, time());
                    Console.WriteLine("InterestRate: {0}\t{1}", item.InterestRate, time());
                    Console.WriteLine("PortfolioID: {0}\t{1}", item.PortfolioId, time());
                    Console.WriteLine("OrderID: {0}\t{1}", item.OrderId, time());
                    Console.WriteLine("LoanLength: {0}\t{1}", item.LoanLength, time());
                    Console.WriteLine("IssueDate: {0}\t{1}", item.IssueDate, time());
                    Console.WriteLine("OrderDate: {0}\t{1}", item.OrderDate, time());
                    Console.WriteLine("LoanStatusDate: {0}\t{1}", item.LoanStatusDate, time());
                    Console.WriteLine("CreditTrend: {0}\t{1}", item.CreditTrend, time());
                    Console.WriteLine("CurrentPaymentStatus: {0}\t{1}", item.CurrentPaymentStatus, time());
                    Console.WriteLine("CanBeTraded: {0}\t{1}", item.CanBeTraded, time());
                    Console.WriteLine("PaymentReceived: {0}\t{1}", item.PaymentsReceived.ToString("C"), time());
                    Console.WriteLine("NextPaymentDate: {0}\t{1}", item.NextPaymentDate, time());
                    Console.WriteLine("PrincipalPending: {0}\t{1}", item.PrincipalPending, time());
                    Console.WriteLine("InterestPending: {0}\t{1}", item.InterestPending, time());
                    Console.WriteLine("InterestReceived: {0}\t{1}", item.InterestReceived.ToString("C"), time());
                    Console.WriteLine("PrincipalReceived: {0}\t{1}", item.PrincipalReceived, time());
                }               
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error{0}\nStackTrace{1}", ex.Message, ex.StackTrace);
            }
        }

        //This function gets the Notes Owned by the Account Logged in
        private void getNotesOwned()
        {
            NotesOwnedResponse getNotes = new NotesOwnedResponse(); 
            Note mNote = new Note();
            try
            {
                Console.WriteLine("\nNotesOwned Responses\n");

                getNotes.MyNotes = new List<Note>();
                getNotes.MyNotes.Add(mNote);

                getNotes = Client.AccountResource.GetNotesOwned();               

                foreach (var items in getNotes.MyNotes)
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

        //This function gets the portfolios owned by the Account Logged in.
        private void getPorfolios()
        {
            PortfoliosOwnedResponse Portfolios = new PortfoliosOwnedResponse();
            Portfolio mPortfolio = new Portfolio();
            try
            {
                Console.WriteLine("\nPortfoliosOwned Response\n");

                Portfolios.MyPortfolios = new List<Portfolio>();
                Portfolios.MyPortfolios.Add(mPortfolio);

                Portfolios = Client.AccountResource.GetPortfoliosOwned();

                foreach(var item in Portfolios.MyPortfolios)
                {
                    Console.WriteLine("PortfolioID: {0}\t{1}", item.PortfolioId, time());
                    Console.WriteLine("PorfolioName: {0}\t{1}", item.PortfolioName, time());
                    Console.WriteLine("PortfolioDescription: {0}\t{1}", item.PortfolioDescription, time());
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error {0}\nStackTrace {1}", ex.Message, ex.StackTrace);
            }
        }

        //This function gets the filters set by the Account Logged in.
        private void getFilters()
        {
            FilterResponse getFilters = new FilterResponse();

            Console.WriteLine("\nFilters\n");

            try
            {
                getFilters = Client.AccountResource.GetFilters();
                Console.WriteLine("FilterId: {0}\t{1}", getFilters.FilterId, time());
                Console.WriteLine("FilterName: {0}\t{1}", getFilters.FilterName, time());
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error {0}\nStackTrace {1]", ex.Message, ex.StackTrace);
            }
        }
        
        //This function gets the pending Transfers in the account
        private void getPendingTransfers()
        {
            PendingTransferResponse pendingResponse = new PendingTransferResponse();
            pendingTransfer _pendingTransfer = new pendingTransfer();
            try
            {
                pendingResponse.transfers = new List<pendingTransfer>();
                pendingResponse.transfers.Add(_pendingTransfer);

                pendingResponse = Client.AccountResource.GetPendingTransfers();

                foreach(var item in pendingResponse.transfers)
                {
                    Console.WriteLine("TransferId: {0}", item.transferId);
                    Console.WriteLine("TransferDate: {0}", item.transferDate);
                    Console.WriteLine("Amount: {0}", item.amount);
                    Console.WriteLine("SourceAmount: {0}", item.sourceAccount);
                    Console.WriteLine("Status: {0}", item.status);
                    Console.WriteLine("Frequency: {0}", item.frequency);
                    Console.WriteLine("EndDate: {0}", item.endDate);
                    Console.WriteLine("Operation: {0}", item.operation);
                    Console.WriteLine("Cancellable: {0}", item.cancellable.ToString());
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //This function creates a new Portfolio in the Account Logged in.
        private void createPortfolios(int actorId, string portfolioName, string portfolioDescription)
        {
            CreatePortfolioRequest _createPortfolioRequest = new CreatePortfolioRequest { ActorId = actorId, PortfolioName = portfolioName, PortfolioDescription = portfolioDescription };
            CreatePortfolioResponse _createPortfolioResponse = new CreatePortfolioResponse();
            CreatePortfolioResponseError mErrors = new CreatePortfolioResponseError();
            _createPortfolioResponse.Errors = new List<CreatePortfolioResponseError>();
            _createPortfolioResponse.Errors.Add(mErrors);            

            try
            {
                _createPortfolioResponse = Client.AccountResource.CreatePortfolio(_createPortfolioRequest);

                Console.WriteLine("PortfolioID: {0}\t{1}", _createPortfolioResponse.PortfolioId, time());
                Console.WriteLine("PortfolioName: {0}\t{1}", _createPortfolioResponse.PortfolioName, time());
                Console.WriteLine("PortfolioDescription: {0}\t{1}", _createPortfolioResponse.PortfolioDescription, time());

                if(_createPortfolioResponse.Errors != null)
                {
                    foreach(var item in _createPortfolioResponse.Errors)
                    {
                        Console.WriteLine("PortfolioErrorField: {0}\t{1}", item.Field, time());
                        Console.WriteLine("PortfolioErrorCode: {0}\t{1}", item.Code, time());
                        Console.WriteLine("PortfolioErrorMessage: {0}\t{1}", item.Message, time());
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error {0}\nStackTrace {1}", ex.Message, ex.StackTrace);
            }

        }

        //This adds funds in the account logged in
        private void AddFunds(DateTime _startDate,DateTime _endDate,double _amount)
        {
            AddFundsResponse addFundsResponse = new AddFundsResponse();
            Frequency freq = new Frequency();
            AddFundsRequest addFundsRequest = new AddFundsRequest
            {
                amount = _amount,
                startDate = _startDate,
                endDate = _endDate,
                transferFrequency = freq.LOAD_NOW
            };

            try
            {
                addFundsResponse = Client.AccountResource.AddFunds(addFundsRequest);

                Console.WriteLine("InvestorId: {0}", addFundsResponse.investorId);
                Console.WriteLine("Amount: {0}", addFundsResponse.amount);
                Console.WriteLine("TransferFrequency: {0}", addFundsResponse.transferFrequency);
                Console.WriteLine("StartDate: {0}", addFundsResponse.startDate);
                Console.WriteLine("EndDate {0}", addFundsResponse.endDate);
                Console.WriteLine("EstimatedFundsTransferRate: {0}", addFundsResponse.estimatedFundsTransferDate);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //This cancels transfers
        private void CancelTransferFunds(int[] _transferId)
        {
            CancelTransferFundsResponse cancelResponse = new CancelTransferFundsResponse();
            cancellationResults result = new cancellationResults();
            CancelTransferFundsRequest cancelRequest = new CancelTransferFundsRequest
            {
                transferId = _transferId
            };

            try
            {
                
                cancelResponse.CancelResult = new List<cancellationResults>();
                cancelResponse.CancelResult.Add(result);

                cancelResponse = Client.AccountResource.CancelTransfers(cancelRequest);

                Console.WriteLine(cancelResponse.investorId);
                foreach(var item in cancelResponse.CancelResult)
                {
                    Console.WriteLine("TransferId: {0}", item.transferId);
                    Console.WriteLine("Status: {1}", item.status);
                    Console.WriteLine("Message: {0}", item.message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }



        }

        private void WithdrawFunds(int _amount)
        {
            WithdrawFundsResponse withdrawResponse = new WithdrawFundsResponse();
            WithdrawFundsRequest withdrawRequest = new WithdrawFundsRequest
            {
                amount = _amount
            };

            try
            {
                withdrawResponse = Client.AccountResource.WithdrawFunds(withdrawRequest);

                Console.WriteLine("Withdrawal InvestorId: {0}", withdrawResponse.investorId);
                Console.WriteLine("Amount Withdrawn: {0}", withdrawResponse.amount);
                Console.WriteLine("Withdrawal estimatedTransferDate: {0}", withdrawResponse.estimatedFundsTransferDate);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

    }

    static class TaskScheduler
    {
        public static void startTaskBy6AM()
        {
            string myPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string myDir = Path.GetDirectoryName(myPath);
            string realpath = Path.Combine(myDir, "TaskScheduler.exe");

            using (TaskService ts = new TaskService())
            {
                // Create a new task definition and assign properties
                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = "Automatic Investing into Lending Club";

                // Create a trigger that will fire the task at this time every specific time of the day(This 15 seconds before the main time)                
                td.Triggers.Add(new DailyTrigger { StartBoundary = DateTime.ParseExact("05:59:45 AM", "hh:mm:ss tt", CultureInfo.InvariantCulture) });

                // Create an action that will launch TaskScheduler whenever the trigger fires
                td.Actions.Add(new ExecAction(realpath, null, null));

                // Register the task in the root folder
                ts.RootFolder.RegisterTaskDefinition("TaskScheduler6AM", td);

                // Remove the task we just created
                //if (td.Actions == null)
                //    ts.RootFolder.DeleteTask("TaskScheduler6AM");
            }
        }

        public static void startTaskBy10AM()
        {
            string myPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string myDir = Path.GetDirectoryName(myPath);
            string realpath = Path.Combine(myDir, "TaskScheduler.exe");

            using (TaskService ts = new TaskService())
            {
                // Create a new task definition and assign properties
                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = "Automatic Investing into Lending Club";

                // Create a trigger that will fire the task at this time every specific time of the day(This 15 seconds before the main time)               
                td.Triggers.Add(new DailyTrigger { StartBoundary = DateTime.ParseExact("09:59:45 AM", "hh:mm:ss tt", CultureInfo.InvariantCulture) });

                // Create an action that will launch TaskScheduler whenever the trigger fires
                td.Actions.Add(new ExecAction(realpath, null, null));


                // Register the task in the root folder
                ts.RootFolder.RegisterTaskDefinition("TaskScheduler10AM", td);

                // Remove the task we just created
                //if (td.Actions == null)
                //    ts.RootFolder.DeleteTask("TaskScheduler10AM");
            }
        }

        public static void startTaskBy2PM()
        {
            string myPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string myDir = Path.GetDirectoryName(myPath);
            string realpath = Path.Combine(myDir, "TaskScheduler.exe");

            using (TaskService ts = new TaskService())
            {
                // Create a new task definition and assign properties
                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = "Automatic Investing into Lending Club";

                // Create a trigger that will fire the task at this time every specific time of the day(This 15 seconds before the main time)               
                td.Triggers.Add(new DailyTrigger { StartBoundary = DateTime.ParseExact("01:59:45 PM", "hh:mm:ss tt", CultureInfo.InvariantCulture) });

                // Create an action that will launch TaskScheduler whenever the trigger fires
                td.Actions.Add(new ExecAction(realpath, null, null));


                // Register the task in the root folder
                ts.RootFolder.RegisterTaskDefinition("TaskScheduler2PM", td);

                // Remove the task we just created
                //if (td.Actions == null)
                //    ts.RootFolder.DeleteTask("TaskScheduler2PM");
            }
        }

        public static void startTaskBy6PM()
        {
            string myPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string myDir = Path.GetDirectoryName(myPath);
            string realpath = Path.Combine(myDir, "TaskScheduler.exe");

            using (TaskService ts = new TaskService())
            {
                // Create a new task definition and assign properties
                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = "Automatic Investing into Lending Club";

                // Create a trigger that will fire the task at this time every specific time of the day(This 15 seconds before the main time)                         
                td.Triggers.Add(new DailyTrigger { StartBoundary = DateTime.ParseExact("05:59:45 PM", "hh:mm:ss tt", CultureInfo.InvariantCulture) });

                // Create an action that will launch TaskScheduler whenever the trigger fires
                td.Actions.Add(new ExecAction(realpath, null, null));


                // Register the task in the root folder
                ts.RootFolder.RegisterTaskDefinition("TaskScheduler6PM", td);

                // Remove the task we just created
                //if (td.Actions == null)
                //    ts.RootFolder.DeleteTask("TaskScheduler6PM");
            }
        }
    }
}
