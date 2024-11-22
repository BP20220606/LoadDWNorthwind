
using LoadDWHNorthwind.Data.Result;

namespace LoadDWHNorthwind.Data.Interfaces
{
    public interface IDataServiceDWHNorthwind
    {
        Task<OperationResult> LoadDHW();

    }
}
