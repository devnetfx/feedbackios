using System.Runtime.Serialization;

namespace SharableTypes.Model
{
    [DataContract]
    public class UserSessionM
    {
        [DataMember] public long LoginId = -1;
        [DataMember] public string Message = string.Empty;

        [DataMember] public int ReturnCode = 0;

        [DataMember] public string SessionId = string.Empty;

        [DataMember] public string UserEmailAddress = string.Empty;
        [DataMember] public string UserGlobalId = string.Empty;

        [DataMember] public string UserPassword = string.Empty;
    }
}