using System;
using AppKit;
using Foundation;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

using RobotInvest.Model;

namespace RobotInvest
{
    public partial class ViewController : NSViewController, IDisposable
    {
        private MainModel mainModel = new MainModel();
        private IndicatorData _indicatorData;
        private Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();

        [Export("Indicators")]
        public IndicatorData Indicators
        {
            get { return _indicatorData; }
            set
            {
                WillChangeValue("Indicators");
                _indicatorData = value;
                DidChangeValue("Indicators");
            }
        }

        partial void UpdateButton(NSObject sender)
        // Action performed after the Update button is clicked
        {
            // Disabling the button to prevent reentring the operation
            UpdateButtonOutlet.Enabled = false;

            progress.ProgressChanged += ReportProgress;
            Task task = mainModel.UpdateIndicators(progress);
            // Enabling update buttomn after the Update method finished
            task.ContinueWith((t) => UpdateButtonOutlet.Enabled = true, TaskScheduler.FromCurrentSynchronizationContext());
            // Displaying unhandled exceptions
            task.ContinueWith((t) =>
            {
                if (t.Exception.Flatten().InnerExceptions != null)
                {
                    AppInfoLabel.StringValue = "Application ran into an error";
                    ResetDashboard();
                }
                foreach (var exception in t.Exception.Flatten().InnerExceptions)
                {
                    var alert = new NSAlert
                    {
                        AlertStyle = NSAlertStyle.Warning,
                        MessageText = "Unhandled Exception occured",
                        InformativeText = exception.Message
                    };
                    alert.RunModal();
                }
            }, CancellationToken.None,
               TaskContinuationOptions.OnlyOnFaulted,
               TaskScheduler.FromCurrentSynchronizationContext());
        }

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Do any additional setup after loading the view.

            ResetDashboard();
            AppInfoLabel.StringValue = "Application Ready";

            Indicators = IndicatorData.Instance;
            Indicators.PropertyChanged += UpdateUI;
            mainModel.UpdateFinishedEvent += MainModel_UpdateFinishedEvent;
        }

        void UpdateUI(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Stocks":
                    StocksArrow.FrameCenterRotation = (nfloat)(-2.2 * Indicators.Stocks);
                    break;
                case "Inflation":
                    InflationArrow.FrameCenterRotation = (nfloat)(-8 * Indicators.Inflation);
                    break;
                case "Risk":
                    RiskArrow.FrameCenterRotation = (nfloat)(-2.5 * Indicators.Risk + 125);
                    break;
                case "YieldSpread":
                    YieldSpreadArrow.FrameCenterRotation = (nfloat)(-20 * Indicators.YieldSpread);
                    break;
                case "Profits":
                    ProfitabilityArrow.FrameCenterRotation = (nfloat)(-3 * Indicators.Profits);
                    break;
                case "LoansMajor":
                    LoansMajorArrow.FrameCenterRotation = (nfloat)(-1.2 * Indicators.LoansMajor);
                    break;
                case "LoansMinor":
                    LoansMinorArrow.FrameCenterRotation = (nfloat)(-1.2 * Indicators.LoansMinor);
                    break;
                default:
                    break;
            }
        }

        void MainModel_UpdateFinishedEvent(object sender, UpdateInfoEventArgs e)
        {
            // Successful finish
            if (e.Result == ResultStatusEnum.Success)
            {
                AppInfoLabel.StringValue = "FRED resources downloaded";
            }
            // Unsuccessful finish
            else
            {
                ResetDashboard();
                AppInfoLabel.StringValue = "Application ran into an error";
                NSAlert alert = new NSAlert
                {
                    AlertStyle = NSAlertStyle.Warning
                };
                switch (e.Result)
                {
                    case ResultStatusEnum.HomeDirectoryError:
                        alert.MessageText = "The home directory doesn't exist";
                        alert.RunModal();
                        break;
                    case ResultStatusEnum.DirectoryNotFoundError:
                        alert.MessageText = "The directory not found";
                        alert.InformativeText = e.ExceptionMessage;
                        alert.RunModal();
                        break;
                    case ResultStatusEnum.FileAccessError:
                        alert.MessageText = "File access error";
                        alert.InformativeText = e.ExceptionMessage;
                        alert.RunModal();
                        break;
                    case ResultStatusEnum.DownloadError:
                        alert.MessageText = "File download error";
                        if (e.ExceptionMessage != null)
                        {
                            if (e.FileName != null)
                            {
                                alert.InformativeText = $"{e.FileName}\n{e.ExceptionMessage}";
                            }
                            else
                            {
                                alert.InformativeText = e.ExceptionMessage;
                            }
                        }
                        else
                        {
                            alert.InformativeText = "Check your internet connection";
                        }
                        alert.RunModal();
                        break;
                    default:
                        alert.MessageText = "Unknown error";
                        alert.RunModal();
                        break;
                }
            }
        }

        private void ReportProgress(object sender, ProgressReportModel e)
        {
            AppInfoLabel.StringValue = $"Downloaded {e.FileDownloaded}";
        }

        public override void ViewDidDisappear()
        {
            base.ViewDidDisappear();
            // Releasing the resources
            Indicators.PropertyChanged -= UpdateUI;
            mainModel.UpdateFinishedEvent -= MainModel_UpdateFinishedEvent;
        }

        void ResetDashboard()
        {
            // Resetting the indicators displayed values
            StocksLabel.StringValue = "+0.0%";
            InflationLabel.StringValue = "+0.0%";
            YieldSpreadLabel.StringValue = "+0.0%";
            RiskLabel.StringValue = "0.0";
            ProfitabilityLabel.StringValue = "+0.0%";
            BankMeterCurrentLabel.StringValue = "+0.0";
            BankMeterYTDLabel.StringValue = "+0.0";
            // resetting the arrows displayed values
            StocksArrow.FrameCenterRotation = 0;
            InflationArrow.FrameCenterRotation = 0;
            RiskArrow.FrameCenterRotation = 0;
            YieldSpreadArrow.FrameCenterRotation = 0;
            ProfitabilityArrow.FrameCenterRotation = 0;
            LoansMajorArrow.FrameCenterRotation = 0;
            LoansMinorArrow.FrameCenterRotation = 0;
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }
    }
}
