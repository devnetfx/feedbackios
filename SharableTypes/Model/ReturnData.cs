using System.Runtime.Serialization;

namespace SharableTypes.Model
{
    [DataContract]
    public class ReturnData
    {
        [DataMember] public object CommentList = null;
        [DataMember] public string ErrorMessage = string.Empty;
        [DataMember] public bool HasError = false;
        [DataMember] public object ItemList = null;
        [DataMember] public string ListData = string.Empty;
        [DataMember] public object PageData = null;
        [DataMember] public object PostList = null;
        [DataMember] public string ReloadTo = string.Empty;
        [DataMember] public string TableContent = string.Empty;
        [DataMember] public object TagList = null;
    }
}