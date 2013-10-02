using System.IO;
using System.IO.IsolatedStorage;

namespace FeedbackIOS.Utility
{
    public static class StorageHelper
    {
        private const string APP_FOLDER = "Settings";
        public const string LGN_PTMTRS = "LgnPrmtrs";
        public const string USER_SESSION = "UserSession";
        public const string LOCATION_STATE = "LOCATION_STATE";
        public const string ONLINE_MODE_STATE = "ONLINE_MODE_STATE";
        public const string SURVEY_IN_PROGRESS = "SURVEY_IN_PROGRESS";
        public const string APPLICATION_IDENTIFIER = "APPLICATION_IDENTIFIER";
        public const string RESPONSES = "RESPONSES";
        public const string SURVEY_CODE = "SURVEY_CODE";
		public const string SURVEY_NAME = "SURVEY_NAME";
		public const string SUBMISSION_ID = "SUBMISSION_ID";

		public const string QUESTIONS = "QUESTIONS";
        public const string QUESTIONCONTAINER = "QUESTIONCONTAINER";
		public const string QUESTION_DEFINITION_LINES = "QUESTION_DEFINITION_LINES";
		public const string QUESTION_DEFINITION_RESPONSES = "QUESTION_DEFINITION_RESPONSES";
		public const string QUESTION_DEFINITION_HEADERS = "QUESTION_DEFINITION_HEADERS";
		public const string QUESTION_EVENTS = "QUESTION_EVENTS";
		public const string FROM_SCAN = "FROM_SCAN";

        public static void SaveToIsolatedStorage(string filename, string text)
        {
            // Obtain the virtual store for the application.
            /////////////
            // IsolatedStorageFile
            /////////////
            using (IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Create a new folder and call it "MyFolder".
                if (!myStore.DirectoryExists(APP_FOLDER))
                {
                    myStore.CreateDirectory(APP_FOLDER);
                }

                // Specify the file path and options.filename
                using (var isoFileStream = new IsolatedStorageFileStream(string.Format("{0}\\{1}.txt", APP_FOLDER, filename), FileMode.OpenOrCreate, myStore))
                {
                    //Write the data
                    using (var isoFileWriter = new StreamWriter(isoFileStream))
                    {
                        isoFileWriter.WriteLine(text);
                    }
                }
            }
        }

        public static string LoadFromIsolatedStorage(string filename)
        {
            // Obtain a virtual store for the application.
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();
            string filePath = string.Format("{0}\\{1}.txt", APP_FOLDER, filename);
            string output = string.Empty;

            try
            {
                if (myStore.DirectoryExists(APP_FOLDER))
                {
                    if (myStore.FileExists(filePath))
                    {
                        // Specify the file path and options.
                        using (var isoFileStream = new IsolatedStorageFileStream(filePath, FileMode.Open, myStore))
                        {
                            // Read the data.
                            using (var isoFileReader = new StreamReader(isoFileStream))
                            {
                                output = isoFileReader.ReadLine();
                            }
                        }
                    }
                }
            }
            catch
            {
                // Handle the case when the user attempts to click the Read button first.
                output = "Problem occured please try again.";
            }
            return output;
        }

        public static T LoadFromIsolatedStorage<T>(string filename)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(LoadFromIsolatedStorage(filename));
        }
    }
}