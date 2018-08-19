using System;
namespace RobotInvest.Model
{
    public class UpdateInfoEventArgs : EventArgs
    {
        public UpdateInfoEventArgs()
        {
        }

        public ResultStatusEnum Result { get; set; }
        public string FileName { set; get; }
    }
}
