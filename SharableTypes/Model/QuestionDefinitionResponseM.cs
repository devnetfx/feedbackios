using System.Runtime.Serialization;
using SharableTypes.Interface;

namespace SharableTypes.Model
{
    [DataContract]
    public class QuestionDefinitionResponseM : IModel
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
        public string InputType { get; set; }
        [DataMember]
        public string DisplayDirection { get; set; }
        [DataMember]
        public string Multiline { get; set; }
        [DataMember]
        public string SurveyCode { get; set; }

        [DataMember]
        public long SequenceNumber { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public string HelpText { get; set; }
        [DataMember]
        public bool HasAdditionalResponse { get; set; }
        [DataMember]
        public string AdditionalResponseTitle { get; set; }
        [DataMember]
        public string AdditionalResponseType { get; set; }

        [DataMember]
        public long QuestionDefinitionId { get; set; }
        [DataMember]
        public long QuestionId { get; set; }

        [DataMember]
        public long LanguageId { get; set; }

    }
}