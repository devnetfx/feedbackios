using System.Runtime.Serialization;
using SharableTypes.Interface;

namespace SharableTypes.Model
{
    [DataContract]
    public class QuestionM : IModel
    {
        [DataMember]
        public string GlobalId { get; set; }

        [DataMember]
        public bool SavedOffline { get; set; }

        [DataMember]
        public string CompanyGlobalId { get; set; }

        [DataMember]
        public int SyncAction { get; set; }

        [DataMember]
        public int ResponseCode { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public bool Active { get; set; }
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public int Number { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string SubText { get; set; }
        [DataMember]
        public long SectionId { get; set; }
        [DataMember]
        public string SectionName { get; set; }
        [DataMember]
        public long QuestionSetId { get; set; }
        [DataMember]
        public string QuestionSetGlobalId { get; set; }
        [DataMember]
        public long QuestionDefinitionId { get; set; }
        [DataMember]
        public string InputType { get; set; }
        [DataMember]
        public string InputFormat { get; set; }
        [DataMember]
        public string DisplayDirection { get; set; }
        [DataMember]
        public string Multiline { get; set; }
        [DataMember]
        public bool HasAdditionalResponse { get; set; }
        [DataMember]
        public string AdditionalResponseTitle { get; set; }
        [DataMember]
        public string AdditionalResponseType { get; set; }
        [DataMember]
        public bool Mandatory { get; set; }
        [DataMember]
        public string MandatoryMessage { get; set; }
        [DataMember]
        public bool VisibleByDefault { get; set; }
        [DataMember]
        public string SurveyCode { get; set; }
        [DataMember]
        public string SurveyName { get; set; }
        [DataMember]
        public long LanguageId { get; set; }

    }
}
