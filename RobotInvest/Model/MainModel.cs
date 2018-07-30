using System;
using AppKit;
using System.IO;
using Foundation;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RobotInvest.Model
{
    public class MainModel
    {
        IndicatorData indicatorData;

        /*
        private static string startDate = "1900-01-01";
        private static string endDate = string.Join("-", new string[] { DateTime.Today.Year.ToString(), DateTime.Today.Month.ToString(), DateTime.Today.Day.ToString() });
        private static string[] fredWebsiteParts = {"https://fred.stlouisfed.org/graph/fredgraph.csv?chart_type=line&recession_bars=on&log_scales=&bgcolor=%23e1e9f0&graph_bgcolor=%23ffffff&fo=Open+Sans&ts=12&tts=12&txtcolor=%23444444&show_legend=yes&show_axis_titles=yes&drp=0&cosd=",
                                             "&coed="
                                            };
        private static Dictionary<string, string> URLs;

        private List<string> filesToDownloadList = new List<string>();
        */

        static MainModel()
        {
            /*
            URLs = new Dictionary<string, string> { 
                {"DJIA", string.Concat(fredWebsiteParts[0], startDate, fredWebsiteParts[1], endDate,
                 "&height=450&stacking=&range=Custom&mode=fred&id=DJIA&transformation=lin&nd=2008-03-28&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Daily&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168")},
                {"MZM", string.Concat(fredWebsiteParts[0], startDate, fredWebsiteParts[1], endDate,
                 "&height=450&stacking=&range=&mode=fred&id=MZMSL&transformation=lin&nd=1959-01-01&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Monthly&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168")},
                {"MZMYTD", string.Concat(fredWebsiteParts[0], startDate, fredWebsiteParts[1], endDate,
                 "&height=450&stacking=&range=Custom&mode=fred&id=MZMSL&transformation=pc1&nd=1959-01-01&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Monthly&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168")},
                {"VIX", string.Concat(fredWebsiteParts[0], startDate, fredWebsiteParts[1], endDate,
                 "&height=450&stacking=&range=Custom&mode=fred&id=VIXCLS&transformation=lin&nd=1990-01-02&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Daily%2C+Close&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168")},
                {"LOANSYTD", string.Concat(fredWebsiteParts[0], startDate, fredWebsiteParts[1], endDate,
                 "&height=450&stacking=&range=Custom&mode=fred&id=BUSLOANS&transformation=pc1&nd=1947-01-01&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Monthly&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168")},
                {"TIGHTENING", string.Concat(fredWebsiteParts[0], startDate, fredWebsiteParts[1], endDate,
                 "&height=450&stacking=&range=&mode=fred&id=DRTSCILM&transformation=lin&nd=1990-04-01&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Quarterly%2C+End+of+Period&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168")},
                {"YIELDSPREAD", string.Concat(fredWebsiteParts[0], startDate, fredWebsiteParts[1], endDate,
                 "&height=450&stacking=&range=Custom&mode=fred&id=T10Y2Y&transformation=lin&nd=1976-06-01&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Daily&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168")},
                {"PROFITS", string.Concat(fredWebsiteParts[0], startDate, fredWebsiteParts[1], endDate,
                 "&height=450&stacking=&range=Custom&mode=fred&id=A053RC1Q027SBEA&transformation=lin&nd=1947-01-01&ost=-99999&oet=99999&lsv=&lev=&mma=0&fml=a&fgst=lin&fgsnd=2009-06-01&fq=Quarterly&fam=avg&vintage_date=&revision_date=&line_color=%234572a7&line_style=solid&lw=2&scale=left&mark_type=none&mw=2&width=1168")}
                };
            */
        }

        public async Task UpdateIndicators()
        {
            // Downloading data sources
            await GetFredResources();

            // Donwloading files
            /*
            if (filesToDownloadList.Any())
            {
                //Task task = DownloadMultipleFilesAsync(filesToDownloadList);
                foreach (var fileToDownload in filesToDownloadList)
                {
                    DownloadFile(fileToDownload);
                }
            }
            */

            // Read the values from files and calculate indicators
            indicatorData = IndicatorData.Instance();

            // STOCKS indicator
            string[] linesDJIA = null;
            string[] linesMZM = null;
            int positionDJIA = 0;
            double koefDJIA = 0.6565037324782718;
            try
            {
                linesDJIA = File.ReadAllLines(Path.Combine(HelperClass.homeDirectoryPath, "DJIA.csv"));
                linesMZM = File.ReadAllLines(Path.Combine(HelperClass.homeDirectoryPath, "MZM.csv"));
                Array.Reverse(linesDJIA);
                Array.Reverse(linesMZM);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
            while (positionDJIA < linesDJIA.Count() && linesDJIA[positionDJIA].Split(',')[1].Equals('.'))
            {
                positionDJIA++;
            }
            double MZM_0 = Convert.ToDouble(linesMZM[0].Split(',')[1]);
            double MZM_1 = Convert.ToDouble(linesMZM[1].Split(',')[1]);
            double MZM_2 = Convert.ToDouble(linesMZM[2].Split(',')[1]);
            double DJIA  = Convert.ToDouble(linesDJIA[positionDJIA].Split(',')[1]);
            double deltaDays = (DateTime.ParseExact(linesDJIA[positionDJIA].Split(',')[0], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture) 
                             - DateTime.ParseExact(linesMZM[0].Split(',')[0], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)).Days;
            //indicatorData.Stocks = koefDJIA * DJIA / (Math.Pow((MZM_0 / MZM_1 + MZM_1 / MZM_2) / 2, deltaDays / 30) * MZM_0);
            indicatorData.Stocks = 100*(koefDJIA * DJIA / (Math.Pow((MZM_0 / MZM_1 + MZM_1 / MZM_2) / 2, deltaDays / 30) * MZM_0) - 1);

            // MZMYTD indicator
            string[] lines = null;
            try
            {
                lines = File.ReadAllLines(Path.Combine(HelperClass.homeDirectoryPath, "MZMYTD.csv"));
                Array.Reverse(lines);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            indicatorData.Inflation = Convert.ToDouble(lines[0].Split(',')[1]);

            // RISK indicator
            try
            {
                lines = File.ReadAllLines(Path.Combine(HelperClass.homeDirectoryPath, "VIX.csv"));
                Array.Reverse(lines);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            int positionRisk = 0;
            while (positionRisk < lines.Count() && lines[positionRisk].Split(',')[1].Equals('.'))
            {
                positionRisk++;
            }
            indicatorData.Risk = Convert.ToDouble(lines[positionRisk].Split(',')[1]);

            // YIELD SPREAD indicator
            try
            {
                lines = File.ReadAllLines(Path.Combine(HelperClass.homeDirectoryPath, "YIELDSPREAD.csv"));
                Array.Reverse(lines);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            int positionYieldSpread = 0;
            while (lines[positionYieldSpread].Split(',')[1].Equals('.') && positionYieldSpread < lines.Count())
            {
                positionYieldSpread++;
            }
            indicatorData.YieldSpread = Convert.ToDouble(lines[positionYieldSpread].Split(',')[1]);

            // PROFITS indicator
            double koefProfits = 8.0;
            string[] linesProfits = null;
            try
            {
                linesProfits = File.ReadAllLines(Path.Combine(HelperClass.homeDirectoryPath, "PROFITS.csv"));
                linesMZM     = File.ReadAllLines(Path.Combine(HelperClass.homeDirectoryPath, "MZM.csv"));
                Array.Reverse(linesProfits);
                Array.Reverse(linesMZM);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            int positionMZM = 0;
            while (positionMZM < linesMZM.Count() && linesProfits[0].Split(',')[0] != linesMZM[positionMZM].Split(',')[0])
            {
                positionMZM++;
            }
            indicatorData.Profits = 100*(koefProfits * Convert.ToDouble(linesProfits[0].Split(',')[1]) / Convert.ToDouble(linesMZM[positionMZM].Split(',')[1]) - 1);

            // LOANS Major indicator
            double koefLoans = 1.628044909602855;
            string[] linesTight = null;
            string[] linesLoans = null;
            try
            {
                linesTight = File.ReadAllLines(Path.Combine(HelperClass.homeDirectoryPath, "TIGHTENING.csv"));
                linesLoans = File.ReadAllLines(Path.Combine(HelperClass.homeDirectoryPath, "LOANSYTD.csv"));
                Array.Reverse(linesTight);
                Array.Reverse(linesLoans);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            int positionLoans = 0;
            while (positionLoans < linesLoans.Count() && linesTight[0].Split(',')[0] != linesLoans[positionLoans].Split(',')[0])
            {
                positionLoans++;
            }
            indicatorData.LoansMajor = koefLoans * Convert.ToDouble(linesTight[0].Split(',')[1]) + Convert.ToDouble(linesLoans[positionLoans].Split(',')[1]);
            // LOANS Minor indicator
            indicatorData.LoansMinor = koefLoans * Convert.ToDouble(linesTight[4].Split(',')[1]) + Convert.ToDouble(linesLoans[positionLoans+12].Split(',')[1]);

            // Call event to enable download button
        }

        private async Task GetFredResources()
        {
            // Checking if home directory exists.
            if (!Directory.Exists(HelperClass.homeDirectoryPath))
            {
                var alert = new NSAlert
                {
                    AlertStyle = NSAlertStyle.Warning,
                    MessageText = "The home directory doesn't exist"
                };
                alert.RunModal();
                return;
            }

            // Looping over all data files
            TimeSpan timeSpan = new TimeSpan(1, 0, 0, 0);
            List<string> fileEntries = Directory.GetFiles(HelperClass.homeDirectoryPath, "*.csv", SearchOption.TopDirectoryOnly).ToList();
            foreach (var fileName in HelperClass.fileNames)
            {
                // Check if the fileName exists in the fileEntries
                if (fileEntries.Exists(fe => fe.Contains(fileName)))
                {
                    string filePath = fileEntries.Find(fe => fe.Contains(fileName));
                    if (DateTime.UtcNow - Directory.GetLastWriteTimeUtc(filePath) > timeSpan)
                    {
                        //filesToDownloadList.Add(fileName);
                        await DownloadFile(fileName);
                    }
                }
                else
                {
                    //filesToDownloadList.Add(fileName);
                    await DownloadFile(fileName);
                }
            }
        }

        private async Task DownloadFile(string fileName)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    // Call event with filename argument to display in the main window
                    await client.DownloadFileTaskAsync(HelperClass.GetURL(new DateTime(1900, 1, 1), DateTime.Today, fileName), Path.Combine(HelperClass.homeDirectoryPath, $"{fileName}.csv"));
                    //client.DownloadFile(HelperClass.GetURL(new DateTime(1900,1,1), DateTime.Today, fileName), Path.Combine(HelperClass.homeDirectoryPath, $"{fileName}.csv"));
                    Console.WriteLine($"{fileName} is downloaded");
                }
            }
            catch (WebException wex)
            {
                Console.WriteLine(wex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.GetType());
            }
        }

        /*
        private async Task DownloadMultipleFilesAsync(List<string> filesToDownload)
        {
            IEnumerable<Task> DownloadTasks = filesToDownload.Select(DownloadFileAsync);
            Task allTasks = Task.WhenAll(DownloadTasks.ToArray());
            try
            {
                await allTasks;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                Console.WriteLine("Tasks isFaulted: " + allTasks.IsFaulted);
                foreach (var inEx in allTasks.Exception.InnerExceptions)
                {
                    Console.WriteLine("Inner exception: " + inEx.Message);
                }
            }
        }

        private async Task DownloadFileAsync(string fileName)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(URLs[fileName], Path.Combine(HelperClass.homeDirectoryPath, $"{fileName}.csv"));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        */
    }
}
