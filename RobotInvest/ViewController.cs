using System;
using AppKit;
using Foundation;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RobotInvest.Model;

namespace RobotInvest
{
    public partial class ViewController : NSViewController
    {
        MainModel mainModel = new MainModel();
        private IndicatorData _indicatorData;

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

            Task task = mainModel.UpdateIndicators();

            //UpdateButtonOutlet.Enabled = true;
        }

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Do any additional setup after loading the view.

            StocksLabel.StringValue = "+0.0%";
            InflationLabel.StringValue = "+0.0%";
            YieldSpreadLabel.StringValue = "+0.0";
            RiskLabel.StringValue = "0.0";
            ProfitabilityLabel.StringValue = "+0.0%";
            BankMeterCurrentLabel.StringValue = "+0.0";
            BankMeterYTDLabel.StringValue = "+0.0";

            Indicators = IndicatorData.Instance();
            Indicators.PropertyChanged += UpdateUI;
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
