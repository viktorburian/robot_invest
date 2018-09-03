using System;
using AppKit;
using System.IO;
using Foundation;
using System.Net;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RobotInvest.Model
{
    public class MainModel
    {
        IndicatorData indicatorData;
        public event EventHandler<UpdateInfoEventArgs> UpdateFinishedEvent;

        static MainModel()
        {
        }

        public async Task UpdateIndicators(IProgress<ProgressReportModel> progress)
        {
            // Downloading data sources
            Task<UpdateInfoEventArgs> task = GetFredResources(progress);
            await task;
            if (task.Result.Result != ResultStatusEnum.Success)
            {
                UpdateFinishedEvent?.Invoke(this, task.Result);
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
                linesLoans = File.ReadAllLines(Path.Combine(HelperClass.homeDirectoryPath, $"{HelperClass.fileNames[4]}.csv"));
                linesTight = File.ReadAllLines(Path.Combine(HelperClass.homeDirectoryPath, $"{HelperClass.fileNames[5]}.csv"));
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
            Array.Reverse(linesTight);
            Array.Reverse(linesLoans);

            indicatorData.LoansMajor = koefLoans * Convert.ToDouble(linesTight[0].Split(',')[1]) + Convert.ToDouble(linesLoans[positionLoans].Split(',')[1]);
            indicatorData.LoansMinor = koefLoans * Convert.ToDouble(linesTight[4].Split(',')[1]) + Convert.ToDouble(linesLoans[positionLoans+12].Split(',')[1]);
#endregion

            // Raising event that the indicator update function has finished
            UpdateFinishedEvent?.Invoke(this, new UpdateInfoEventArgs{Result=ResultStatusEnum.Success});
        }

        private async Task<UpdateInfoEventArgs> GetFredResources(IProgress<ProgressReportModel> progress)
        {
            // Checking if home directory exists
            if (!Directory.Exists(HelperClass.homeDirectoryPath))
            {
                return new UpdateInfoEventArgs { Result = ResultStatusEnum.HomeDirectoryError };
            }

            // Looping over all data files
            TimeSpan timeSpan = new TimeSpan(1, 0, 0, 0);
            List<string> fileEntries = Directory.GetFiles(HelperClass.homeDirectoryPath, "*.csv", SearchOption.TopDirectoryOnly).ToList();
            List<UpdateInfoEventArgs> updateInfos = new List<UpdateInfoEventArgs>(fileEntries.Count);
            Task task = Task.Run(() =>
            {
                Parallel.ForEach<string>(HelperClass.fileNames, (fileName) =>
                {
                    // Check if the fileName exists in the fileEntries
                    if (fileEntries.Exists(fe => fe.EndsWith(fileName + ".csv", StringComparison.CurrentCulture)))
                    {
                        string filePath = fileEntries.Find(fe => fe.EndsWith(fileName + ".csv", StringComparison.CurrentCulture));
                        FileInfo fileInfo = new FileInfo(filePath);
                        if (DateTime.UtcNow - Directory.GetLastWriteTimeUtc(filePath) > timeSpan || fileInfo.Length == 0)
                        {
                            updateInfos.Add(DownloadFileSync(progress, fileName));
                        }
                    }
                    else
                    {
                        updateInfos.Add(DownloadFileSync(progress, fileName));
                    }
                });
            });
            try
            {
                await task;
            }
            catch (Exception)
            {
                return new UpdateInfoEventArgs { Result = ResultStatusEnum.DownloadError };
            }
            if (updateInfos.Any(info => info.Result != ResultStatusEnum.Success))
            {
                List<UpdateInfoEventArgs> failedDownloads = updateInfos.FindAll(info => info.Result != ResultStatusEnum.Success);
                return failedDownloads[0];
            }
            // Method exited successfully
            return new UpdateInfoEventArgs { Result = ResultStatusEnum.Success };
        }

        private UpdateInfoEventArgs DownloadFileSync(IProgress<ProgressReportModel> progress, string fileName)
        {
            ProgressReportModel report = new ProgressReportModel();
            using (WebClient client = new WebClient())
            {
                // Call event with filename argument to display in the main window
                try
                {
                    client.DownloadFile(HelperClass.GetURL(new DateTime(1900, 1, 1), DateTime.Today, fileName), Path.Combine(HelperClass.homeDirectoryPath, $"{fileName}.csv"));
                }
                catch (WebException wex)
                {
                    return new UpdateInfoEventArgs { Result = ResultStatusEnum.DownloadError, FileName=fileName, ExceptionMessage = wex.Message };
                }
            }
            report.FileDownloaded = fileName;
            progress.Report(report);
            return new UpdateInfoEventArgs { Result = ResultStatusEnum.Success };
        }
    }
}
