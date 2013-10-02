using System.Runtime.Serialization;

namespace SharableTypes.Model
{
    [DataContract]
    public class LgnPrmtrsM
    {
        [DataMember] public string ChannelType = string.Empty;
        [DataMember] public string DeviceId = string.Empty;
        [DataMember] public string DeviceModel = string.Empty;
        [DataMember] public string DeviceType = string.Empty;
        [DataMember] public string EmailAddress = string.Empty;
        [DataMember] public string HashKey = string.Empty;
        [DataMember] public string IpAddress = string.Empty;
        [DataMember] public string Password = string.Empty;
        [DataMember] public string SessionId = string.Empty;
    }
}