// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace RobotInvest
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSTextField AppInfoLabel { get; set; }

		[Outlet]
		AppKit.NSTextField BankMeterCurrentLabel { get; set; }

		[Outlet]
		AppKit.NSTextField BankMeterYTDLabel { get; set; }

		[Outlet]
		AppKit.NSImageView InflationArrow { get; set; }

		[Outlet]
		AppKit.NSTextField InflationLabel { get; set; }

		[Outlet]
		AppKit.NSImageView LoansMajorArrow { get; set; }

		[Outlet]
		AppKit.NSImageView LoansMinorArrow { get; set; }

		[Outlet]
		AppKit.NSImageView ProfitabilityArrow { get; set; }

		[Outlet]
		AppKit.NSTextField ProfitabilityLabel { get; set; }

		[Outlet]
		AppKit.NSImageView RiskArrow { get; set; }

		[Outlet]
		AppKit.NSTextField RiskLabel { get; set; }

		[Outlet]
		AppKit.NSImageView StocksArrow { get; set; }

		[Outlet]
		AppKit.NSTextField StocksLabel { get; set; }

		[Outlet]
		AppKit.NSButton UpdateButtonOutlet { get; set; }

		[Outlet]
		AppKit.NSImageView YieldSpreadArrow { get; set; }

		[Outlet]
		AppKit.NSTextField YieldSpreadLabel { get; set; }

		[Action ("UpdateButton:")]
		partial void UpdateButton (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (BankMeterCurrentLabel != null) {
				BankMeterCurrentLabel.Dispose ();
				BankMeterCurrentLabel = null;
			}

			if (BankMeterYTDLabel != null) {
				BankMeterYTDLabel.Dispose ();
				BankMeterYTDLabel = null;
			}

			if (InflationArrow != null) {
				InflationArrow.Dispose ();
				InflationArrow = null;
			}

			if (InflationLabel != null) {
				InflationLabel.Dispose ();
				InflationLabel = null;
			}

			if (LoansMajorArrow != null) {
				LoansMajorArrow.Dispose ();
				LoansMajorArrow = null;
			}

			if (LoansMinorArrow != null) {
				LoansMinorArrow.Dispose ();
				LoansMinorArrow = null;
			}

			if (ProfitabilityArrow != null) {
				ProfitabilityArrow.Dispose ();
				ProfitabilityArrow = null;
			}

			if (ProfitabilityLabel != null) {
				ProfitabilityLabel.Dispose ();
				ProfitabilityLabel = null;
			}

			if (RiskArrow != null) {
				RiskArrow.Dispose ();
				RiskArrow = null;
			}

			if (RiskLabel != null) {
				RiskLabel.Dispose ();
				RiskLabel = null;
			}

			if (StocksArrow != null) {
				StocksArrow.Dispose ();
				StocksArrow = null;
			}

			if (StocksLabel != null) {
				StocksLabel.Dispose ();
				StocksLabel = null;
			}

			if (UpdateButtonOutlet != null) {
				UpdateButtonOutlet.Dispose ();
				UpdateButtonOutlet = null;
			}

			if (YieldSpreadArrow != null) {
				YieldSpreadArrow.Dispose ();
				YieldSpreadArrow = null;
			}

			if (YieldSpreadLabel != null) {
				YieldSpreadLabel.Dispose ();
				YieldSpreadLabel = null;
			}

			if (AppInfoLabel != null) {
				AppInfoLabel.Dispose ();
				AppInfoLabel = null;
			}
		}
	}
}
