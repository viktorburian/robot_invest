using System;
using Foundation;
using System.ComponentModel;

namespace RobotInvest.Model
{
    [Register("IndicatorData")]
    public sealed class IndicatorData : NSObject, INotifyPropertyChanged
    {
        #region Private Fields
        private static readonly IndicatorData _instance = new IndicatorData();

        private double _stocks;
        private double _inflation;
        private double _yieldSpread;
        private double _profits;
        private double _risk;
        private double _loansMajor;
        private double _loansMinor;
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        [Export("Stocks")]
        public double Stocks
        {
            get { return _stocks; }
            set
            {
                WillChangeValue("Stocks");
                _stocks = value;
                DidChangeValue("Stocks");
                OnPropertyChanged("Stocks");
            }
        }

        [Export("Inflation")]
        public double Inflation
        {
            get { return _inflation; }
            set
            {
                WillChangeValue("Inflation");
                _inflation = value;
                DidChangeValue("Inflation");
                OnPropertyChanged("Inflation");
            }
        }

        [Export("Risk")]
        public double Risk
        {
            get { return _risk; }
            set
            {
                WillChangeValue("Risk");
                _risk = value;
                DidChangeValue("Risk");
                OnPropertyChanged("Risk");
            }
        }

        [Export("YieldSpread")]
        public double YieldSpread
        {
            get { return _yieldSpread; }
            set
            {
                WillChangeValue("YieldSpread");
                _yieldSpread = value;
                DidChangeValue("YieldSpread");
                OnPropertyChanged("YieldSpread");
            }
        }

        [Export("Profits")]
        public double Profits
        {
            get { return _profits; }
            set
            {
                WillChangeValue("Profits");
                _profits = value;
                DidChangeValue("Profits");
                OnPropertyChanged("Profits");
            }
        }

        [Export("LoansMajor")]
        public double LoansMajor
        {
            get { return _loansMajor; }
            set
            {
                WillChangeValue("LoansMajor");
                _loansMajor = value;
                DidChangeValue("LoansMajor");
                OnPropertyChanged("LoansMajor");
            }
        }

        [Export("LoansMinor")]
        public double LoansMinor
        {
            get { return _loansMinor; }
            set
            {
                WillChangeValue("LoansMinor");
                _loansMinor = value;
                DidChangeValue("LoansMinor");
                OnPropertyChanged("LoansMinor");
            }
        }

        static IndicatorData()
        {
        }

        private IndicatorData()
        {
        }

        public static IndicatorData Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
