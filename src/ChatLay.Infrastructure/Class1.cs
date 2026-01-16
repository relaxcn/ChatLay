using System;
using Windows.System;

namespace ChatLay.Infrastructure.System
{
    /// <summary>
    /// 示例：调用 Windows API（WinRT）
    /// 这个类只能存在于 Infrastructure
    /// </summary>
    public static class WindowsEnvironmentInfo
    {
        public static string GetDeviceFamily()
        {
            // Windows.System.Profile 是 WinRT API
            return Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
        }
    }
}
