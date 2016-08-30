using System.Runtime.Serialization;

namespace Iata.IS.Model.Common
{
    /// <summary>
    /// The input data required for Audit Trail Package Request
    /// </summary>
    public class AuditTrailPackageRequest
    {
        public string TransactionId { get; set;}
        
        public string TransactionType { get; set; }
        
        public string FileName { get; set;}
    }
}
