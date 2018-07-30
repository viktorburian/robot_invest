using AppKit;
using System;
using System.IO;
using Foundation;
using RobotInvest.Model;

namespace RobotInvest
{
    [Register("AppDelegate")]
    public class AppDelegate : NSApplicationDelegate
    {
        public AppDelegate()
        {
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            // Insert code here to initialize your application
            // Checking if RobotInvest folder exists in Library directory. If not, the folder is created
            try
            {
                DirectoryInfo directoryInfo = Directory.CreateDirectory(HelperClass.homeDirectoryPath);
            }
            catch (Exception ex)
            {
                var alert = new NSAlert
                {
                    AlertStyle = NSAlertStyle.Warning,
                    MessageText = "Exception has been raised",
                    InformativeText = ex.Message
                };
                alert.RunModal();
            }
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }
    }
}
