using System.Runtime.Serialization;
using SharableTypes.Interface;

namespace SharableTypes.Model
{
    [DataContract]
    public class QuestionSetM : IModel
    {
        [DataMember]
        public string GlobalId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string SurveyCode { get; set; }
        [DataMember]
        public string UrlKey { get; set; }
        [DataMember]
        public bool MobileEnabled { get; set; }
        [DataMember]
        public bool WebEnabled { get; set; }
        [DataMember]
        public bool RequireConsent { get; set; }
        [DataMember]
        public string ConsentMessage { get; set; }
        [DataMember]
        public string CompletionMessage { get; set; }
        [DataMember]
        public bool ShowFooter { get; set; }
        [DataMember]
        public string FooterMessage { get; set; }
        [DataMember]
        public string GroupBy { get; set; }
        [DataMember]
        public string StartDate { get; set; }
        [DataMember]
        public string EndDate { get; set; }
        [DataMember]
        public long StatusId { get; set; }
        [DataMember]
        public long LanguageId { get; set; }
        [DataMember]
        public bool Active { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public int ResponseCode { get; set; }
        [DataMember]
        public string CreatedDate { get; set; }
        [DataMember]
        public bool SavedOffline { get; set; }
        [DataMember]
        public string CompanyGlobalId { get; set; }
        [DataMember]
        public int SyncAction { get; set; }
		[DataMember]
		public bool ShowReferralCode { get; set; }
		[DataMember]
		public string ReferralCodeLabel { get; set; }
    }
}
