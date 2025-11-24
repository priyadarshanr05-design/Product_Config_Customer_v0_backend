using System.ComponentModel.DataAnnotations;

namespace Product_Config_Customer_v0.Models.Entity
{
    public class ParentAPI_Model_Request
    {
        [Key]
        public long Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string RequestId { get; set; } = string.Empty;
        public string DomainName { get; set; } = string.Empty;
        public string? FileType { get; set; }
        public string? PartId { get; set; }
        public string? PartNumber { get; set; }
        public int? StatusCode { get; set; }
        public string? Status { get; set; }
        public string? Message { get; set; }
        public string? RequestJson { get; set; }
        public string? ApiResponse { get; set; }
        public string? ApiStatus { get; set; }
        public int RetryCount { get; set; } = 0;
        public string? RequestHash { get; set; }
        public string? CanonicalizedJson { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
