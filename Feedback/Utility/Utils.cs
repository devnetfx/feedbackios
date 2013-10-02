namespace FeedbackIOS.Utility
{
    public static class Utils
    {
        public const bool DebugMode =
                #if DEBUG
                    true;
                #else
                    false;
                #endif

        public const int MaxAudioRecordingDuration = 120;
        public const int MaxVideoRecordingDuration = 60;

        public static string BytesToBase64(byte[] data)
        {
            return System.Convert.ToBase64String(data, 0, data.Length);
        }

        public static byte[] Base64ToBytes(string data)
        {
            return System.Convert.FromBase64String(data);
        }
    }
}