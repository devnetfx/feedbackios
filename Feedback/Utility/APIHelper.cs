using System;
using System.Globalization;
using System.IO;
using System.Net;
using SharableTypes.Model;

namespace FeedbackIOS.Utility
{
    public class APIHelper
    {
        private const string JSONContentType = "application/json";
        private const string PostType = "post";

        private static string ServiceEndPoint
        {
            get
            {
                return "https://betaapi.architected.net/PublicAPI.svc/";
            }
        }

        public static string ConstructEndPoint(string operationName)
        {
            return String.Format("{0}{1}", ServiceEndPoint, operationName);
        }

        public static HttpWebRequest InitializeRequest(string operationName)
        {
            var operationUrl = ConstructEndPoint(operationName);

            var request = (HttpWebRequest)WebRequest.Create(operationUrl);

            request.ContentType = JSONContentType;
            request.Method = PostType;

            return request;
        }

        private static string CreateAuthHeader(UserSessionM userSession)
        {
            return String.Format("{0}:{1}", userSession.LoginId, GenerateHash(userSession));
        }

        private static string GenerateHash(UserSessionM userSession)
        {
            return SecurityHelper.EncryptString(userSession.LoginId.ToString(CultureInfo.InvariantCulture), userSession.UserPassword, userSession.UserEmailAddress.ToLower());
        }

        public static HttpWebRequest InitializeRequest(string operationName, UserSessionM userSession)
        {
            var operationUrl = ConstructEndPoint(operationName);

            var request = (HttpWebRequest)WebRequest.Create(operationUrl);

            request.ContentType = JSONContentType;
            request.Method = PostType;

            if (userSession.LoginId > 0)
            {
                var loginParameters = StorageHelper.LoadFromIsolatedStorage<LgnPrmtrsM>(StorageHelper.LGN_PTMTRS);
                userSession.UserPassword = loginParameters.Password;

                request.Headers[HttpRequestHeader.Authorization] = CreateAuthHeader(userSession);
            }

            return request;
        }

        public static void LogError(string requestContent)
        {
            var userSession = StorageHelper.LoadFromIsolatedStorage<UserSessionM>(StorageHelper.USER_SESSION);
            var request = InitializeRequest(UIConstants.LOG_ERROR_API, userSession);

            request.BeginGetRequestStream(result =>
            {
                var req = ((HttpWebRequest)result.AsyncState).EndGetRequestStream(result);

                using (var streamWriter = new StreamWriter(req))
                {
                    streamWriter.Write(requestContent);
                }

                request.BeginGetResponse(responseResult =>
                {
                    
                }, request);
            }, request);
        }


    }
}
