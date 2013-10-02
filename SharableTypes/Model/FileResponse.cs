using System.Runtime.Serialization;

namespace SharableTypes.Model
{
    [DataContract]
    public class FileResponse
    {
        [DataMember]
        public bool IsValid = true;
        [DataMember]
        public string description = string.Empty;
        [DataMember]
        public string error = string.Empty;
        [DataMember]
        public string extension = string.Empty;
        [DataMember]
        public string file = string.Empty;
        [DataMember]
        public string name = string.Empty;
    }
}