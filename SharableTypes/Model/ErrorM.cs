using System.Runtime.Serialization;

namespace SharableTypes.Model
{
    [DataContract]
    public class ErrorM
    {
        [DataMember]
        public string ApplicationName = string.Empty;

        [DataMember]
        public string ApplicationGuid = string.Empty;

        [DataMember]
        public string Message = string.Empty;

        [DataMember]
        public string Trace = string.Empty;

        [DataMember]
        public string TypeName = string.Empty;
    }
}