using System.Runtime.Serialization;

namespace SharableTypes.Model
{
    [DataContract]
    public class QuestionContextM
    {
        [DataMember]
        public long QuestionId { get; set; }
        [DataMember]
        public long LineId { get; set; }
        [DataMember]
        public long HeaderId { get; set; }
        [DataMember]
        public long ResponseId { get; set; }
    }
}
