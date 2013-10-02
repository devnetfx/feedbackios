using System.Runtime.Serialization;
using SharableTypes.Interface;

namespace SharableTypes.Model
{
    [DataContract]
    public class QuestionEventM : IModel
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
        public long QuestionSetId { get; set; }
        [DataMember]
        public string QuestionSetGlobalId { get; set; }
        [DataMember]
        public long QuestionId { get; set; }
        [DataMember]
        public string QuestionGlobalId { get; set; }
        [DataMember]
        public long EntityId { get; set; }
        [DataMember]
        public string EntityGlobalId { get; set; }
        [DataMember]
        public int EntityTypeId { get; set; }
        [DataMember]
        public string EventType { get; set; }
        [DataMember]
        public string ActionType { get; set; }
        [DataMember]
        public long TargetEntityId { get; set; }
        [DataMember]
        public string TargetEntityGlobalId { get; set; }
        [DataMember]
        public string TargetEntityName { get; set; }
        [DataMember]
        public int TargetEntityTypeId { get; set; }
        [DataMember]
        public string TargetUrl { get; set; }

    }
}