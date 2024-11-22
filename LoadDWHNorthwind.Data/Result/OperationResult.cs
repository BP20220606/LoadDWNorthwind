

namespace LoadDWHNorthwind.Data.Result
{
    public class OperationResult
    {
        public OperationResult()
        {
            this.Success = true;
        }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
