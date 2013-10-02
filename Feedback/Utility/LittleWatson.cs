using System;
using System.IO;
using System.IO.IsolatedStorage;
using SharableTypes.Model;

namespace FeedbackIOS.Utility
{
    public static class LittleWatson
    {
        private const string Filename = "LittleWatson.txt";

        /// <summary>
        /// Reports an exception which has caused the application to crash.
        /// </summary>
        /// <param name="ex">The exception to report.</param>
        public static void ReportException(Exception ex)
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    SafeDeleteFile(store);

                    using (TextWriter output = new StreamWriter(store.CreateFile(Filename)))
                    {
                        var exception = new ErrorM();
                        exception.ApplicationName = "FeedbackIOSClient";
                        exception.ApplicationGuid = "DD224A62-ANDR-4D22-A684-2B03C6A6F038";

                        exception.Message = ex.GetBaseException().Message;
                        exception.Trace = ex.StackTrace.Replace("\r", "").Replace("\n", "");
                        exception.TypeName = ex.GetBaseException().GetType().ToString();

                        var message = Newtonsoft.Json.JsonConvert.SerializeObject(exception);

                        output.Write(message);
                    }

                    CheckForPreviousException();
                }
            }
            catch
            {
                // We do not want this code to cause exceptions of its own
            }
        }

        public static void CheckForPreviousException()
        {
            try
            {
                string contents = null;
                if (ConnectionHelper.IsConnected())
                {
                    using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (store.FileExists(Filename))
                        {
                            using (
                                TextReader reader =
                                    new StreamReader(store.OpenFile(Filename, FileMode.Open, FileAccess.Read,
                                                                    FileShare.None)))
                            {
                                contents = reader.ReadToEnd();
                            }

                            // We have the file contents so we may delete the crash report
                            SafeDeleteFile(store);
                        }

                        if (!string.IsNullOrEmpty(contents))
                        {
                            APIHelper.LogError(contents);
                        }
                    }
                }
            }
            catch
            {
                // We do not want this code to cause exceptions of its own
            }
            finally
            {
                // One way or another, we must make sure to delete the last crash report
                SafeDeleteFile(IsolatedStorageFile.GetUserStoreForApplication());
            }
        }

        private static void SafeDeleteFile(IsolatedStorageFile store)
        {
            try
            {
                store.DeleteFile(Filename);
            }
            catch
            {
                // We do not want this code to cause exceptions of its own
            }
        }
    }
}