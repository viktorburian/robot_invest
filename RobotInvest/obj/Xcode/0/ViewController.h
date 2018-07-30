// WARNING
// This file has been generated automatically by Visual Studio to
// mirror C# types. Changes in this file made by drag-connecting
// from the UI designer will be synchronized back to C#, but
// more complex manual changes may not transfer correctly.


#import <Foundation/Foundation.h>
#import <AppKit/AppKit.h>


@interface ViewController : NSViewController {
	NSTextField *_BankMeterCurrentLabel;
	NSTextField *_BankMeterYTDLabel;
	NSImageView *_InflationArrow;
	NSTextField *_InflationLabel;
	NSImageView *_LoansMajorArrow;
	NSImageView *_LoansMinorArrow;
	NSImageView *_ProfitabilityArrow;
	NSTextField *_ProfitabilityLabel;
	NSImageView *_RiskArrow;
	NSTextField *_RiskLabel;
	NSImageView *_StocksArrow;
	NSTextField *_StocksLabel;
	NSButton *_UpdateButtonOutlet;
	NSImageView *_YieldSpreadArrow;
	NSTextField *_YieldSpreadLabel;
}

@property (nonatomic, retain) IBOutlet NSTextField *BankMeterCurrentLabel;

@property (nonatomic, retain) IBOutlet NSTextField *BankMeterYTDLabel;

@property (nonatomic, retain) IBOutlet NSImageView *InflationArrow;

@property (nonatomic, retain) IBOutlet NSTextField *InflationLabel;

@property (nonatomic, retain) IBOutlet NSImageView *LoansMajorArrow;

@property (nonatomic, retain) IBOutlet NSImageView *LoansMinorArrow;

@property (nonatomic, retain) IBOutlet NSImageView *ProfitabilityArrow;

@property (nonatomic, retain) IBOutlet NSTextField *ProfitabilityLabel;

@property (nonatomic, retain) IBOutlet NSImageView *RiskArrow;

@property (nonatomic, retain) IBOutlet NSTextField *RiskLabel;

@property (nonatomic, retain) IBOutlet NSImageView *StocksArrow;

@property (nonatomic, retain) IBOutlet NSTextField *StocksLabel;

@property (nonatomic, retain) IBOutlet NSButton *UpdateButtonOutlet;

@property (nonatomic, retain) IBOutlet NSImageView *YieldSpreadArrow;

@property (nonatomic, retain) IBOutlet NSTextField *YieldSpreadLabel;

- (IBAction)UpdateButton:(id)sender;

@end
