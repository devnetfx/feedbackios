using System.Runtime.Serialization;
using SharableTypes.Interface;

namespace SharableTypes.Model
{
    [DataContract]
    public class QuestionResponseM : IModel
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
        public long QuestionDefinitionLineId { get; set; }
        [DataMember]
        public long QuestionId { get; set; }
        [DataMember]
        public long QuestionDefinitionResponseId { get; set; }
        [DataMember]
        public long QuestionDefinitionHeaderId { get; set; }
        [DataMember]
        public string Response { get; set; }
        [DataMember]
        public string AdditionalResponse { get; set; }
        [DataMember]
        public long[] ResponseCollection { get; set; }

        [DataMember]
        public string Latitude { get; set; }
        [DataMember]
        public string Longitude { get; set; }
        [DataMember]
        public string SubmissionIdentifier { get; set; }
        [DataMember]
        public string Channel { get; set; }
        [DataMember]
        public string DeviceType { get; set; }

        [DataMember]
        public long LanguageId { get; set; }
		[DataMember]
		public string ReferralCode { get; set; }
		[DataMember]
		public string TrackingCode { get; set; }




    }
}
