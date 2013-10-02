using System.Runtime.Serialization;

namespace SharableTypes.Model
{
    [DataContract]
    public class QuestionContainerM
    {
        [DataMember]
        public QuestionSetM QuestionSet { get; set; }
        [DataMember]
        public QuestionM[] QuestionList { get; set; }
    }
}
