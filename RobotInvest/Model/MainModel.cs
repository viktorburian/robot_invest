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
        public event EventHandler<UpdateInfoEventArgs> UpdateFinishedEvent;
        public event EventHandler<string> DownloadInfoEvent;

        static MainModel()
        {
        }

        /*
        public Task FooAsync()
        {
            try
            {
                int b = 1;
                int a = 5 / b;
            }
            catch (Exception e)
            {
                return Task.FromException(e);
                //Console.WriteLine(e.Message);
            }
            Console.WriteLine("Downloading");
            Task task;
            using (WebClient client = new WebClient())
            {
                // Call event with filename argument to display in the main window
                DownloadInfoEvent?.Invoke(this, "MZM");
                task = client.DownloadFileTaskAsync(HelperClass.GetURL(new DateTime(1900, 1, 1), DateTime.Today, "MZM"), Path.Combine(HelperClass.homeDirectoryPath, "MZM.csv"));
            }
            if(task.IsCompleted)
            {
                Console.WriteLine("Completed");
            }

            UpdateFinishedEvent?.Invoke(this, new EventArgs());
            return task;
        }
        */

        public async Task UpdateIndicators()
        {
            // Downloading data sources
            Task<ResultStatusEnum> task = GetFredResources();
            await task;
            if (task.Result != ResultStatusEnum.Success)
            {
                UpdateFinishedEvent?.Invoke(this, new UpdateInfoEventArgs{Result=task.Result});
                return;
            }

            // Read the values from files and calculate indicators
            indicatorData = IndicatorData.Instance;
            string[] linesDJIA = null;
            string[] linesMZM = null;
            string[] linesMZMYTD = null;
            string[] linesRISK = null;
            string[] linesYieldSpread = null;
            string[] linesProfits = null;
            string[] linesTight = null;
            string[] linesLoans = null;
            int positionDJIA = 0;
            int positionRisk = 0;
            int positionYieldSpread = 0;
            int positionMZM = 0;
            int positionLoans = 0;
            double koef_1_DJIA = 1.07775711;
            double koef_2_DJIA = 0.14532113;
            double koefProfits = 8.0;
            double koefLoans = 1.628044909602855;

#region STOCKS
            try
            {
                linesDJIA = File.ReadAllLines(Path.Combine(HelperClass.homeDirectoryPath, $"{HelperClass.fileNames[0]}.csv"));
                linesMZM = File.ReadAllLines(Path.Combine(HelperClass.homeDirectoryPath, $"{HelperClass.fileNames[1]}.csv"));
            }
            catch(DirectoryNotFoundException dex)
            {
                UpdateFinishedEvent?.Invoke(this, new UpdateInfoEventArgs { Result = ResultStatusEnum.DirectoryNotFoundError,
                                                                            ExceptionMessage = dex.Message});
                return;
            }
            catch(IOException ioex)
            {
                UpdateFinishedEvent?.Invoke(this, new UpdateInfoEventArgs { Result = ResultStatusEnum.FileAccessError,
                                                                            ExceptionMessage = ioex.Message});
                return;
            }
            Array.Reverse(linesDJIA);
            Array.Reverse(linesMZM);

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
            double MZM_current = Math.Pow((MZM_0 / MZM_1 + MZM_1 / MZM_2) / 2, deltaDays / 30) * MZM_0;
            // Pavel Kohout log10 linear fit
            indicatorData.Stocks = 100 * ((DJIA / Math.Pow(10, koef_1_DJIA * Math.Log10(MZM_current) - koef_2_DJIA)) - 1);
#endregion

#region INFLATION
            try
            {
                linesMZMYTD = File.ReadAllLines(Path.Combine(HelperClass.homeDirectoryPath, $"{HelperClass.fileNames[2]}.csv"));
            }
            catch (DirectoryNotFoundException dex)
            {
                UpdateFinishedEvent?.Invoke(this, new UpdateInfoEventArgs
                {
                    Result = ResultStatusEnum.DirectoryNotFoundError,
                    ExceptionMessage = dex.Message
                });
                return;
            }
            catch (IOException ioex)
            {
                UpdateFinishedEvent?.Invoke(this, new UpdateInfoEventArgs
                {
                    Result = ResultStatusEnum.FileAccessError,
                    ExceptionMessage = ioex.Message
                });
                return;
            }
            Array.Reverse(linesMZMYTD);

            indicatorData.Inflation = Convert.ToDouble(linesMZMYTD[0].Split(',')[1]);
#endregion

#region RISK
            try
            {
                linesRISK = File.ReadAllLines(Path.Combine(HelperClass.homeDirectoryPath, $"{HelperClass.fileNames[3]}.csv"));
            }
            catch (DirectoryNotFoundException dex)
            {
                UpdateFinishedEvent?.Invoke(this, new UpdateInfoEventArgs
                {
                    Result = ResultStatusEnum.DirectoryNotFoundError,
                    ExceptionMessage = dex.Message
                });
                return;
            }
            catch (IOException ioex)
            {
                UpdateFinishedEvent?.Invoke(this, new UpdateInfoEventArgs
                {
                    Result = ResultStatusEnum.FileAccessError,
                    ExceptionMessage = ioex.Message
                });
                return;
            }
            Array.Reverse(linesRISK);

            while (positionRisk < linesRISK.Count() && linesRISK[positionRisk].Split(',')[1].Equals('.'))
            {
                positionRisk++;
            }
            indicatorData.Risk = Convert.ToDouble(linesRISK[positionRisk].Split(',')[1]);
#endregion

#region YIELD SPREAD
            try
            {
                linesYieldSpread = File.ReadAllLines(Path.Combine(HelperClass.homeDirectoryPath, $"{HelperClass.fileNames[6]}.csv"));
            }
            catch (DirectoryNotFoundException dex)
            {
                UpdateFinishedEvent?.Invoke(this, new UpdateInfoEventArgs
                {
                    Result = ResultStatusEnum.DirectoryNotFoundError,
                    ExceptionMessage = dex.Message
                });
                return;
            }
            catch (IOException ioex)
            {
                UpdateFinishedEvent?.Invoke(this, new UpdateInfoEventArgs
                {
                    Result = ResultStatusEnum.FileAccessError,
                    ExceptionMessage = ioex.Message
                });
                return;
            }
            Array.Reverse(linesYieldSpread);

            while (linesYieldSpread[positionYieldSpread].Split(',')[1].Equals('.') && positionYieldSpread < linesYieldSpread.Count())
            {
                positionYieldSpread++;
            }
            indicatorData.YieldSpread = Convert.ToDouble(linesYieldSpread[positionYieldSpread].Split(',')[1]);
#endregion

#region PROFITS
            try
            {
                linesProfits = File.ReadAllLines(Path.Combine(HelperClass.homeDirectoryPath, $"{HelperClass.fileNames[7]}.csv"));
                linesMZM     = File.ReadAllLines(Path.Combine(HelperClass.homeDirectoryPath, $"{HelperClass.fileNames[1]}.csv"));

            }
            catch (DirectoryNotFoundException dex)
            {
                UpdateFinishedEvent?.Invoke(this, new UpdateInfoEventArgs
                {
                    Result = ResultStatusEnum.DirectoryNotFoundError,
                    ExceptionMessage = dex.Message
                });
                return;
            }
            catch (IOException ioex)
            {
                UpdateFinishedEvent?.Invoke(this, new UpdateInfoEventArgs
                {
                    Result = ResultStatusEnum.FileAccessError,
                    ExceptionMessage = ioex.Message
                });
                return;
            }
            Array.Reverse(linesProfits);
            Array.Reverse(linesMZM);

            while (positionMZM < linesMZM.Count() && linesProfits[0].Split(',')[0] != linesMZM[positionMZM].Split(',')[0])
            {
                positionMZM++;
            }
            indicatorData.Profits = 100*(koefProfits * Convert.ToDouble(linesProfits[0].Split(',')[1]) / Convert.ToDouble(linesMZM[positionMZM].Split(',')[1]) - 1);
#endregion

#region LOANS
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

            // Deprecated to take just the last record of the loans
            /*
            while (positionLoans < linesLoans.Count() && linesTight[0].Split(',')[0] != linesLoans[positionLoans].Split(',')[0])
            {
                positionLoans++;
            }
            */
            indicatorData.LoansMajor = koefLoans * Convert.ToDouble(linesTight[0].Split(',')[1]) + Convert.ToDouble(linesLoans[positionLoans].Split(',')[1]);
            // LOANS Minor indicator
            indicatorData.LoansMinor = koefLoans * Convert.ToDouble(linesTight[4].Split(',')[1]) + Convert.ToDouble(linesLoans[positionLoans+12].Split(',')[1]);
#endregion

            // Raising event that the indicator update function has finished
            UpdateFinishedEvent?.Invoke(this, new UpdateInfoEventArgs{Result=ResultStatusEnum.Success});
        }

        private async Task<ResultStatusEnum> GetFredResources()
        {
            // Checking if home directory exists.
            if (!Directory.Exists(HelperClass.homeDirectoryPath))
            {
                return ResultStatusEnum.HomeDirectoryError;
            }

            // Looping over all data files
            TimeSpan timeSpan = new TimeSpan(1, 0, 0, 0);
            List<string> fileEntries = Directory.GetFiles(HelperClass.homeDirectoryPath, "*.csv", SearchOption.TopDirectoryOnly).ToList();
            foreach (var fileName in HelperClass.fileNames)
            {
                // TODO: Check if the internet connection is established

                // Check if the fileName exists in the fileEntries
                if (fileEntries.Exists(fe => fe.EndsWith(fileName+".csv", StringComparison.CurrentCulture)))
                {
                    string filePath = fileEntries.Find(fe => fe.EndsWith(fileName + ".csv", StringComparison.CurrentCulture));
                    FileInfo fileInfo = new FileInfo(filePath);
                    if (DateTime.UtcNow - Directory.GetLastWriteTimeUtc(filePath) > timeSpan || fileInfo.Length == 0)
                    {
                        Task<ResultStatusEnum> task = DownloadFile(fileName);
                        await task;
                        if (!task.IsCompleted || task.IsFaulted || task.IsCanceled || task.Result != ResultStatusEnum.Success)
                        {
                            return task.Result;
                        }
                    }
                }
                else
                {
                    Task<ResultStatusEnum> task = DownloadFile(fileName);
                    await task;
                    if (!task.IsCompleted || task.IsFaulted || task.IsCanceled || task.Result != ResultStatusEnum.Success)
                    {
                        return task.Result;
                    }
                }
            }
            // Method exited successfully
            return ResultStatusEnum.Success;
        }

        private async Task<ResultStatusEnum> DownloadFile(string fileName)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    // Call event with filename argument to display in the main window
                    DownloadInfoEvent?.Invoke(this, fileName);
                    Task donwloadTask = client.DownloadFileTaskAsync(HelperClass.GetURL(new DateTime(1900, 1, 1), DateTime.Today, fileName), Path.Combine(HelperClass.homeDirectoryPath, $"{fileName}.csv"));
                    await donwloadTask;
                }
            }
            catch (WebException)
            {
                return ResultStatusEnum.DownloadError;
            }
            return ResultStatusEnum.Success;
        }

        /*
        public void DoSyncStuff()
        {
            int i = 0;
            while(i < 1000000000)
            {
                i++;
            }
            try
            {
                int b = 0;
                int a = 5 / b;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //throw e.GetBaseException();
                throw;
            }

            Console.WriteLine("Func exited successfully");
        }
        */

        /*
        public void UpdateIndicatorsSync()
        {
            // Downloading data sources
            GetFredResourcesSync();

            // Read the values from files and calculate indicators
            indicatorData = IndicatorData.Instance;

            // STOCKS indicator
            string[] linesDJIA = null;
            string[] linesMZM = null;
            int positionDJIA = 0;
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
            double DJIA = Convert.ToDouble(linesDJIA[positionDJIA].Split(',')[1]);
            double deltaDays = (DateTime.ParseExact(linesDJIA[positionDJIA].Split(',')[0], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)
                             - DateTime.ParseExact(linesMZM[0].Split(',')[0], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)).Days;
            // Pavel Kohout log10 linear fit
            double MZM_current = Math.Pow((MZM_0 / MZM_1 + MZM_1 / MZM_2) / 2, deltaDays / 30) * MZM_0;
            indicatorData.Stocks = 100 * ((DJIA / Math.Pow(10, 1.07775711 * Math.Log10(MZM_current) - 0.14532113)) - 1);

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
                linesMZM = File.ReadAllLines(Path.Combine(HelperClass.homeDirectoryPath, "MZM.csv"));
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
            indicatorData.Profits = 100 * (koefProfits * Convert.ToDouble(linesProfits[0].Split(',')[1]) / Convert.ToDouble(linesMZM[positionMZM].Split(',')[1]) - 1);

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
            indicatorData.LoansMinor = koefLoans * Convert.ToDouble(linesTight[4].Split(',')[1]) + Convert.ToDouble(linesLoans[positionLoans + 12].Split(',')[1]);

            // Raising event that the indicator update function has finished
            UpdateFinishedEvent?.Invoke(this, new EventArgs());
        }
        */

        /*
        private void GetFredResourcesSync()
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
                if (fileEntries.Exists(fe => fe.EndsWith(fileName + ".csv", StringComparison.CurrentCulture)))
                {
                    string filePath = fileEntries.Find(fe => fe.Contains(fileName));
                    if (DateTime.UtcNow - Directory.GetLastWriteTimeUtc(filePath) > timeSpan)
                    {
                        DownloadFileSync(fileName);
                    }
                }
                else
                {
                    DownloadFileSync(fileName);
                }
            }
        }
        */

        /*
        private void DownloadFileSync(string fileName)
        {
            // Call event with filename argument to display in the main window
            DownloadInfoEvent?.Invoke(this, fileName);
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(HelperClass.GetURL(new DateTime(1900, 1, 1), DateTime.Today, fileName), Path.Combine(HelperClass.homeDirectoryPath, $"{fileName}.csv"));
                }
            }
            catch (WebException wex)
            {
                Console.WriteLine("Web exception:");
                Console.WriteLine(wex.Message);
                Console.WriteLine(wex.GetBaseException().Message);
                Console.WriteLine(wex.InnerException.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception:");
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.GetType());
            }
        }
        */
    }
}
