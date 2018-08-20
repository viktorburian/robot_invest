using System;
namespace RobotInvest.Model
{
    public enum ResultStatusEnum
    {
        Success = 0,
        DownloadError,
        FileAccessError,
        HomeDirectoryError,
        DirectoryNotFoundError,
        InternetConnectionError
    }
}
