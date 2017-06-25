using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendingClubDotNet.Client.v1;
using LendingClubDotNet.Models.Requests;
using LendingClubDotNet.Models.Responses;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Net;

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

            //Enter your authorisationToken and InvestorID and PortfolioId and ActorID.
            AutoInvest autoinvest = new AutoInvest("AUTHORISATIONTOKEN", "INVESTORID", 111111, 999999);
            autoinvest.Invest();

           // Console.ReadKey();
        }
    }

    class AutoInvest
    {
        Stopwatch stopwatch = new Stopwatch();
        LendingClubV1Client Client;
        StringBuilder save_to_file = new StringBuilder();
        StringBuilder timeElapsedfile = new StringBuilder();

        private readonly long _portfolioId;
        private readonly int _actorId;
        int maxloans_to_Buy = 0;      
        List<long> loanId_in_NotesOwned = new List<long>();
        List<Loan> filteredLoans = new List<Loan>();
        List<Loan> filteredLoans2 = new List<Loan>();

        public AutoInvest(string AuthorisationToken, string InvestorId, long portfolioId, int actorId)
        {
            Client = new LendingClubV1Client(AuthorisationToken, InvestorId);
           
            _portfolioId = portfolioId;
            _actorId = actorId;
        }

        public void Invest()
        {
            ListedLoansResponse getloans = new ListedLoansResponse();
            Loan mLoans = new Loan();

            try
            {
                getloans.Loans = new List<Loan>();
                getloans.Loans.Add(mLoans);

                Console.WriteLine("Getting first loans from LC for comparism");
                //The number of old listed loans
                int _1stcount = Client.LoanResource.GetListedLoans().Loans.Count;
                Console.WriteLine("Loans got: Count = {0}", _1stcount);

                do
                {
                    Console.WriteLine("Waiting 1 milliseconds to get new loans");
                    //wait for 100milliseconds
                    System.Threading.Thread.Sleep(100);

                    //the number of new listed loans
                    int _2ndcount = Client.LoanResource.GetListedLoans().Loans.Count;
                    Console.WriteLine("Second Loans got: Count = {0}", _2ndcount);
                    //if the both numbers are not same move out of loop else continue looping
                    if (_1stcount != _2ndcount) { Console.WriteLine("First loans not equal to second loans, continuing execution"); break; }
                    else { Console.WriteLine("First loans equals second loans, restarting execution..."); continue; }

                }
                while (true);
            }
            catch (Exception e) { Console.WriteLine("Error {0} \nStacktrace {1}", e.Message, e.StackTrace); }

            MaxLoans_to_Buy();
            Pull_NotesOwned();
            Pull_New_Loans();
            FilterLoans();   
                    
            Console.ReadKey();
        }        

        private void SendMail(string mailBody, string attachmentFilepath,string mailFrom,string mailPassword)
        {
            string mailTo = "ayedalsabawi@gmail.com";
            try
            {
                SmtpClient SmtpServer = new SmtpClient("smtpout.secureserver.net");
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(mailFrom);//"info@predacitycapital.com"
                mail.To.Add(mailTo);
                mail.Subject = "LC C# CODE HAS EXECUTED";
                mail.Body = mailBody;
                SmtpServer.Port = 465;
                SmtpServer.Credentials = new NetworkCredential(mailFrom, mailPassword);
                SmtpServer.EnableSsl = true;
                Attachment logFile = new Attachment(attachmentFilepath);
                mail.Attachments.Add(logFile);
                save_to_file.AppendLine(string.Format("Sending mail to {0}", mailTo));
                Console.WriteLine("Sending mail to {0}", mailTo);
                SmtpServer.Send(mail);
                save_to_file.AppendLine("Mail Sent succesfully");
                Console.WriteLine("Mail Sent succesfully");

            }
            catch (Exception ex)
            { Console.WriteLine((ex.Message == null) ? ex.InnerException.ToString() : ex.Message.ToString()); }

        }

        private string time()
        {
            return DateTime.Now.ToString();
        }
        
        //This function calculates max loans to buy.
        private void MaxLoans_to_Buy()
        {
            decimal cashBalance = 0.0M;
            
            AvailableCashResponse Avcash = new AvailableCashResponse();
            try
            {

                Avcash = Client.AccountResource.GetAvailableCash();
                Console.WriteLine("Account logged in. {0}", time());

                save_to_file.AppendLine(string.Format("Account logged in. {0}", time()));
                cashBalance = Avcash.AvailableCash;
                save_to_file.AppendLine(string.Format("AvailableCash got = {0}. {1}", cashBalance, time()));

                Console.WriteLine("AvailableCash got = {0}. {1}", cashBalance, time());
                maxloans_to_Buy = (int)cashBalance / 25;
                save_to_file.AppendLine(string.Format("Maximum loans to buy = {0}. {1}", maxloans_to_Buy, time()));
                Console.WriteLine(string.Format("Maximum loans to buy = {0}. {1}", maxloans_to_Buy, time()));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error {0}\nStackTrace {1}", ex.Message, ex.StackTrace);
                save_to_file.AppendLine(string.Format("Error {0}\nError occured in {1}\n{2}", ex.Message, ex.StackTrace, time()));

            }
            
        }

        //This function pulls loanId's of already owned notes
        private void Pull_NotesOwned()
        {
            NotesOwnedResponse getNotes = new NotesOwnedResponse();
            Note mNote = new Note();

            try
            {                
                getNotes.MyNotes = new List<Note>();
                getNotes.MyNotes.Add(mNote);

                //Pull note from LC
                getNotes = Client.AccountResource.GetNotesOwned();
                save_to_file.AppendLine(string.Format("Notes Owned got. {0}", time()));
                Console.WriteLine("Notes Owned got. {0}", time());

                Console.WriteLine("Getting loanIDs of already owned loans");
                //int count = 0;
                stopwatch.Reset();
                stopwatch.Start();
                
                foreach(var item in getNotes.MyNotes)
                {
                    loanId_in_NotesOwned.Add((long)item.LoanId);
                }
                stopwatch.Stop();
                timeElapsedfile.AppendLine(string.Format("TimeElapsed(milliseconds) to get loanId of NotesOwned{0:F3}", (stopwatch.ElapsedMilliseconds / 1000.0).ToString()));
                save_to_file.AppendLine(string.Format("LoanID's of Notes Owned got {0}", time()));
                Console.WriteLine("LoanID's of Notes Owned got {0}", time());
                Console.WriteLine("Total notes owned got: {0}", loanId_in_NotesOwned.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error {0}\nStackTrace {1}", ex.Message, ex.StackTrace);
                save_to_file.AppendLine(string.Format("Error {0}\nError occured in {1}\n{2}", ex.Message, ex.StackTrace, time()));

            }
        }

        //This function pulls new listed loans from LC and removes already invested loans
        private void Pull_New_Loans()
        {
            ListedLoansResponse getloans = new ListedLoansResponse();
            Loan mLoans = new Loan();

            try
            {
                getloans.Loans = new List<Loan>();
                getloans.Loans.Add(mLoans);

                //Pull new loans
                getloans = Client.LoanResource.GetListedLoans();
                Console.WriteLine("New Loans got. {0}",time());
                save_to_file.AppendLine(string.Format("New Loans got. {0}", time()));

                Console.WriteLine("Removing Old loans from New loans...");
                save_to_file.AppendLine("Removing Old loans from New loans...");
                int no_notes_already_owned = 0;
                foreach(var item in getloans.Loans)
                {
                    //get id of new loans
                    int id = item.Id;
                    //add new loans to list
                    filteredLoans.Add(item); filteredLoans2.Add(item);
                    for (int i = 0; i < loanId_in_NotesOwned.Count; i++) 
                    {
                        //check if id of new loans match old loans and remove
                        if(id == loanId_in_NotesOwned[i])
                        {
                            no_notes_already_owned++;
                            filteredLoans.Remove(item); filteredLoans2.Remove(item);
                            Console.WriteLine("LoanId {0} removed", id);
                            save_to_file.AppendLine(string.Format("LoanId {0} removed", id));
                            break;
                        }
                    }
                }
                save_to_file.AppendLine(string.Format("{0} loans already owned was removed from new loans {1} ", no_notes_already_owned,time()));
                Console.WriteLine("{0} loans already owned was removed from new loans {1} ", no_notes_already_owned, time());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error {0}\nStackTrace {1}", ex.Message, ex.StackTrace);
                save_to_file.AppendLine(string.Format("Error {0}\nError occured in {1}\n{2}", ex.Message, ex.StackTrace, time()));

            }
        }

        //This function runs filter on the new loans and invests the filtered loans
        private void FilterLoans()
        {
            stopwatch.Reset();
            stopwatch.Start();
            //This runs the filters on the listed loans
            Console.WriteLine("Filtering loans according to criteria", time());
            save_to_file.AppendLine(string.Format("Filtering loans according to criteria", time()));
            foreach (var item in filteredLoans2)
            {
                decimal? deliq_rate = (decimal?)item.NumAcctsEver120Ppd / item.TotalAcc;

                //if (item.Dti < 25 || item.Term == 36 || item.SubGrade.ToUpper() == "E4" || item.SubGrade.ToUpper() == "E5"
                //    || item.Grade.ToUpper() != "A" || item.Grade.ToUpper() != "F" || item.Grade.ToUpper() != "G"
                //    || item.Grade.ToUpper() != "H" || item.HomeOwnership.ToLower() != "mortgage" || item.AnnualInc >= 4500
                //    || item.NumTl120dpd2m != null || item.NumTl120dpd2m != 0 || deliq_rate <= 0.45M)
                //{
                //    filteredLoans.Remove(item);
                //}

                if(item.Dti < 25)
                {
                    Console.WriteLine("DTI = {0}: Loan removed", item.Dti);
                    filteredLoans.Remove(item);
                }
                else if(item.Term == 36)
                {
                    Console.WriteLine("Term = {0}: Loan removed", item.Term);
                    filteredLoans.Remove(item);
                }
                else if (item.SubGrade.ToUpper() == "E4")
                {
                    Console.WriteLine("SubGrade = {0}: Loan removed", item.SubGrade.ToUpper());
                    filteredLoans.Remove(item);
                }
                else if (item.SubGrade.ToUpper() == "E5")
                {
                    Console.WriteLine("Subgrade = {0}: Loan removed", item.SubGrade.ToUpper());
                    filteredLoans.Remove(item);
                }
                else if (item.Grade.ToUpper() == "A")
                {
                    Console.WriteLine("Grade = {0}: Loan removed", item.Grade.ToUpper());
                    filteredLoans.Remove(item);
                }
                else if (item.Grade.ToUpper() == "F")
                {
                    Console.WriteLine("Grade = {0}: Loan removed", item.Grade.ToUpper());
                    filteredLoans.Remove(item);
                }
                else if (item.Grade.ToUpper() == "G")
                {
                    Console.WriteLine("Grade = {0}: Loan removed", item.Grade.ToUpper());
                    filteredLoans.Remove(item);
                }
                else if (item.Grade.ToUpper() == "H")
                {
                    Console.WriteLine("Grade = {0}: Loan removed", item.Grade.ToUpper());
                    filteredLoans.Remove(item);
                }
                else if (item.HomeOwnership.ToLower() == "mortgage")
                {
                    Console.WriteLine("HomeOwnership = {0}: Loan removed", item.HomeOwnership.ToLower());
                    filteredLoans.Remove(item);
                }
                else if (item.AnnualInc >= 4500)
                {
                    Console.WriteLine("AnnuallInc = {0}: Loan removed", item.AnnualInc);
                    filteredLoans.Remove(item);
                }
                else if (item.NumTl120dpd2m != null)
                {
                    Console.WriteLine("NumTl120dpd2m = {0}: Loan removed", item.NumTl120dpd2m);
                    filteredLoans.Remove(item);
                }
                else if (item.NumTl120dpd2m != 0)
                {
                    Console.WriteLine("NumTl120dpd2m = {0}: Loan removed", item.NumTl120dpd2m);
                    filteredLoans.Remove(item);
                }
                else if (deliq_rate <= 0.45M)
                {
                    Console.WriteLine("Deliq_rate = {0}: Loan removed", deliq_rate);
                    filteredLoans.Remove(item);
                }                


            }
                       
            stopwatch.Stop();

            Console.WriteLine("Finished fitering in {0:F3} milliseconds", stopwatch.ElapsedMilliseconds / 1000.0);
            Console.WriteLine("Total Loans got: {0}", filteredLoans.Count);
            timeElapsedfile.AppendLine(string.Format("TimeElapsed(milliseconds) to filter loans \t{0:F3}", stopwatch.ElapsedMilliseconds / 1000.0));
            save_to_file.AppendLine(string.Format("loans filtered according to criteria {0}", time()));

            List<int> loanId = new List<int>();
            List<decimal> _percentFunded = new List<decimal>();
            foreach (var item in filteredLoans)
            {
                decimal itemFunded = (item.FundedAmount / item.LoanAmount);
                _percentFunded.Add(itemFunded);
                loanId.Add(item.Id);
                Console.WriteLine("Available LoanID's: {0}", item.Id);
                save_to_file.AppendLine(string.Format("Available LoanID's: {0}", item.Id));
            }

            //Sort the filteredloan in decreasing _percentFunded 

            Console.WriteLine("Sorting LoadID's according to decreasing percentFunded...");
            save_to_file.AppendLine("Sorting LoadID's according to decreasing percentFunded...");
            List<int> SortedLoanId = new List<int>();
            int N = _percentFunded.Count;
            int[] index = Enumerable.Range(0, N).ToArray();
            stopwatch.Reset();
            stopwatch.Start();

            Array.Sort(index, (a, b) => _percentFunded[b].CompareTo(_percentFunded[a]));

            stopwatch.Stop();
            Console.WriteLine("Finished sorting loans in {0:F3} milliseconds", stopwatch.ElapsedMilliseconds / 1000.0);

            save_to_file.AppendLine("\nSorted Loans\n");
            save_to_file.AppendLine("LoanID \tPercentFunded");

            Console.WriteLine("\nSorted Loans\n");
            Console.WriteLine("LoanID \tPercentFunded");
            foreach (int i in index)
            {
                //Add the sorted loan to a new list
                SortedLoanId.Add(loanId[i]);
                Console.WriteLine("{0} \t{1:F3}", loanId[i], _percentFunded[i]);
                save_to_file.AppendLine(string.Format("{0} \t{1:F3}", loanId[i], _percentFunded[i]));
            }


            //These part of the code invests into LC
            Console.WriteLine("Getting Orders to invest into LC from the sorted loans...");
            save_to_file.AppendLine("Getting Orders to invest into LC from the sorted loans...");

            List<Order> myOrders = new List<Order>();
            save_to_file.AppendLine("\nAcquired Orders\n");
            save_to_file.AppendLine("LoanId \t\tRequestedAmount  Portfolio");

            Console.WriteLine("\nAcquired Orders\n");
            Console.WriteLine("LoanId \t\tRequestedAmount  Portfolio");
            if (loanId.Count != 0 || loanId != null)
            {
                if (loanId.Count <= maxloans_to_Buy)
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
                        Console.WriteLine("{0} \t{1} \t\t{2}", item, 25, _portfolioId);
                        save_to_file.AppendLine(string.Format("{0} \t{1} \t\t{2}", item, 25, _portfolioId));
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
                        Console.WriteLine("{0} \t{1} \t\t{2}", SortedLoanId[1], 25, _portfolioId);
                        save_to_file.AppendLine(string.Format("{0} \t{1} \t\t{2}",SortedLoanId[1], 25, _portfolioId));
                    }
                }


                // Submit an Orders Request (invest in loans).

                Console.WriteLine("Submitting Orders to LC...");
                save_to_file.AppendLine("Submitting Orders to LC...");
                SubmitOrdersRequest submitOrdersRequest = new SubmitOrdersRequest
                {
                    aid = _actorId,
                    orders = myOrders
                };

                try
                {
                    SubmitOrdersResponse ordersResponse = new SubmitOrdersResponse();
                    ordersResponse = Client.AccountResource.SubmitOrders(submitOrdersRequest);
                    Console.WriteLine("Order Invester successfully {0}", time());
                    save_to_file.AppendLine(string.Format("Order Invested Successfully {0}", time()));

                    //Get response from LC server
                    OrderConfirmation orderconfirmation = new OrderConfirmation();
                    ExecutionStatus status = new ExecutionStatus();
                    orderconfirmation.ExecutionStatus.Add(status);

                    ordersResponse.OrderConfirmations.Add(orderconfirmation);
                    save_to_file.AppendLine("{");
                    Console.WriteLine("{");
                    foreach (var item in ordersResponse.OrderConfirmations)
                    {
                         Console.WriteLine(string.Format("\tOrderInstructId: {0}", item.OrderInstructId));
                         Console.WriteLine(string.Format("\tLoanId: {0}", item.LoanId));
                         Console.WriteLine(string.Format("\tRequestedAmount: {0}", item.RequestedAmount));
                         Console.WriteLine(string.Format("\tInvestedAmount: {0}", item.InvestedAmount));
                         Console.WriteLine("\t{");

                        save_to_file.AppendLine(string.Format("\tOrderInstructId: {0}", item.OrderInstructId));
                        save_to_file.AppendLine(string.Format("\tLoanId: {0}", item.LoanId));
                        save_to_file.AppendLine(string.Format("\tRequestedAmount: {0}", item.RequestedAmount));
                        save_to_file.AppendLine(string.Format("\tInvestedAmount: {0}", item.InvestedAmount));
                        save_to_file.AppendLine("\t{");
                        foreach (var items in orderconfirmation.ExecutionStatus)
                        {
                            save_to_file.AppendLine(items.ToString());
                            Console.WriteLine(item.ToString());
                        }
                        save_to_file.AppendLine("\t}");
                        Console.WriteLine("\t}");
                    }
                    save_to_file.AppendLine("}");
                    Console.WriteLine("}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error {0}\nStackTrace {1}", ex.Message, ex.StackTrace);
                    save_to_file.AppendLine(string.Format("Error {0}\nError occured in {1}\n{2}", ex.Message, ex.StackTrace, time()));

                }

            }
            else { Console.WriteLine("Nothing to buy, so did not submit an order"); }
            string[] months = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "July", "Aug", "Sept", "Oct", "Nov", "Dec" };
            int date = int.Parse( DateTime.Now.Month.ToString());

            string filepath = savefile(months[date], months[date], save_to_file);

           // SendMail("A mail from Lending club software", filepath);            
        }

        //This function saves the file to a folder in your documents directory.
        private string savefile(string _Directory, string file, StringBuilder text)
        {
            string filename = string.Format("{0}.txt", file);
            string Dirpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LendingClub Software", _Directory);
            string filepath = "";

            string newText = text.ToString();
            int count = 1; string path = string.Format("{0}{1}", Dirpath, count);
            while (Directory.Exists(path))
            {
                count++;
                path = string.Format("{0}{1}", Dirpath, count);
            }

            Directory.CreateDirectory(path);
            filepath = Path.Combine(path, filename);
            using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter tw = new StreamWriter(fs))
                {
                    tw.WriteLineAsync(newText);
                }
            }

            return filepath;
        }
    }
}
