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
            
            Console.ReadKey();
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
        List<int> loanId_in_NotesOwned = new List<int>();
        List<Loan> filteredLoans = new List<Loan>();

        public AutoInvest(string AuthorisationToken, string InvestorId, long portfolioId, int actorId)
        {
            Client = new LendingClubV1Client(AuthorisationToken, InvestorId);
            save_to_file.AppendLine(string.Format("Account logged in. {0}", time()));
            this._portfolioId = portfolioId;
            this._actorId = actorId;
        }
        public void Invest()
        {
            MaxLoans_to_Buy();
            Pull_NotesOwned();
            Pull_New_Loans();
            FilterLoans();
        }

        private void SendMail(string mailBody, string attachmentFilepath)
        {           
            try
            {
                SmtpClient SmtpServer = new SmtpClient("smtpout.secureserver.net");
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("info@predacitycapital.com");
                mail.To.Add("ayedalsabawi@gmail.com");
                mail.Subject = "LC C# CODE HAS EXECUTED";
                mail.Body = mailBody;
                SmtpServer.Port = 465;
                SmtpServer.Credentials = new NetworkCredential("info@predacitycapital.com", "#########");
                SmtpServer.EnableSsl = true;
                Attachment logFile = new Attachment(attachmentFilepath);
                mail.Attachments.Add(logFile);
                SmtpServer.Send(mail);
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

                cashBalance = Avcash.AvailableCash;
                save_to_file.AppendLine(string.Format("AvailableCash got = {0}. {1}", cashBalance, time()));

                maxloans_to_Buy = (int)cashBalance / 25;
                save_to_file.AppendLine(string.Format("Maximum loans to buy = {0}. {1}", maxloans_to_Buy, time()));
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

                stopwatch.Reset();
                stopwatch.Start();
                foreach(var item in getNotes.MyNotes)
                {
                    loanId_in_NotesOwned.Add((short)item.LoanId);
                }
                stopwatch.Stop();
                timeElapsedfile.AppendLine(string.Format("TimeElapsed(milliseconds) to get loanId of NotesOwned{0:F3}", (stopwatch.ElapsedMilliseconds / 1000.0).ToString()));
                save_to_file.AppendLine(String.Format("LoanID's of Notes Owned got {0}", time()));
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
                save_to_file.AppendLine(string.Format("New Loans got. {0}", time()));

                int no_notes_already_owned = 0;
                foreach(var item in getloans.Loans)
                {
                    //get id of new loans
                    int id = item.Id;
                    //add new loans to list
                    filteredLoans.Add(item);
                    for (int i = 0; i < loanId_in_NotesOwned.Count; i++) 
                    {
                        //check if id of new loans match old loans and remove
                        if(id == loanId_in_NotesOwned[i])
                        {
                            no_notes_already_owned++;
                            filteredLoans.Remove(item); break;
                        }
                    }
                }
                save_to_file.AppendLine(string.Format("{0} loans already owned was removed from new loans {1} ", no_notes_already_owned,time()));
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
            save_to_file.AppendLine(string.Format("loans filtered according to criteria {0}", time()));

            List<int> loanId = new List<int>();
            List<decimal> _percentFunded = new List<decimal>();
            foreach (var item in filteredLoans)
            {
                _percentFunded.Add((item.FundedAmount / item.LoanAmount));
                loanId.Add(item.Id);
            }

            //Sort the filteredloan in decreasing _percentFunded 

            List<int> SortedLoanId = new List<int>();
            int N = _percentFunded.Count;
            int[] index = Enumerable.Range(0, N).ToArray();
            Array.Sort(index, (a, b) => _percentFunded[b].CompareTo(_percentFunded[a]));
            save_to_file.AppendLine(string.Format("Sorted Loans. \nLoanID \tPercentFunded"));
            foreach (int i in index)
            {
                //Add the sorted loan to a new list
                SortedLoanId.Add(loanId[i]);
                save_to_file.AppendLine(string.Format("{0} \t{1}", loanId[i], _percentFunded[i]));
            }


            //These part of the code invests into LC

            List<Order> myOrders = new List<Order>();
            save_to_file.AppendLine("Acquired Orders \nLoanId \tRequestedAmount \tPortfolio");
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
                        save_to_file.AppendLine(string.Format("{0} \t{1} \t{2}", item, 25, _portfolioId));
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
                        save_to_file.AppendLine(string.Format("{0} \t{1} \t{2}",SortedLoanId[1], 25, _portfolioId));
                    }
                }

                // Submit an Orders Request (invest in loans).
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
                    save_to_file.AppendLine(string.Format("order Invested Successfully {0}", time()));

                    //Get response from LC server
                    OrderConfirmation orderconfirmation = new OrderConfirmation();
                    ExecutionStatus status = new ExecutionStatus();
                    orderconfirmation.ExecutionStatus.Add(status);

                    ordersResponse.OrderConfirmations.Add(orderconfirmation);
                    save_to_file.AppendLine("{");
                    foreach (var item in ordersResponse.OrderConfirmations)
                    {
                        save_to_file.AppendLine(string.Format("\tOrderInstructId: {0}", item.OrderInstructId));
                        save_to_file.AppendLine(string.Format("\tLoanId: {0}", item.LoanId));
                        save_to_file.AppendLine(string.Format("\tRequestedAmount: {0}", item.RequestedAmount));
                        save_to_file.AppendLine(string.Format("\tInvestedAmount: {0}", item.InvestedAmount));
                        save_to_file.AppendLine("\t{");
                        foreach (var items in orderconfirmation.ExecutionStatus)
                        {
                            save_to_file.AppendLine(items.ToString());
                        }
                        save_to_file.AppendLine("\t}");
                    }
                    save_to_file.AppendLine("}");
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error {0}\nStackTrace {1}", ex.Message, ex.StackTrace);
                    save_to_file.AppendLine(string.Format("Error {0}\nError occured in {1}\n{2}", ex.Message, ex.StackTrace, time()));

                }

            }
            else { Console.WriteLine("Nothing to buy, so did not submit an order"); }

            string filepath = savefile(DateTime.Now.ToShortDateString(), DateTime.Now.ToShortDateString(), save_to_file);

            SendMail("A mail from Lending club software", filepath);
        }

        //This function saves the file to a folder in your documents directory.
        private string savefile(string Directory, string file, StringBuilder text)
        {
            string filename = string.Format("{0}.txt",file);
            string Dirpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LendingClub Software",Directory);
            string filepath = Path.Combine(Dirpath, filename);

            string newText = text.ToString();

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
