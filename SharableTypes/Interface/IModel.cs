
namespace SharableTypes.Interface
{
    public interface IModel
    {
        string GlobalId { get; set; }
        bool SavedOffline { get; set; }
        int SyncAction { get; set; }
        int ResponseCode { get; set; }
        string ErrorMessage { get; set; }
        bool Active { get; set; }
    }
}
