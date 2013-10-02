using Feedback.Utility;
using SharableTypes.iOS.Enum;

namespace FeedbackIOS.Utility
{
    public class ConnectionHelper
    {
        public static bool IsConnected()
        {
            var internetStatus = Reachability.InternetConnectionStatus();

            return internetStatus != NetworkStatus.NotReachable;
        }
    }
}
