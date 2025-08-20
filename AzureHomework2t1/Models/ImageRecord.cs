using System.ComponentModel.DataAnnotations;

namespace AzureHomework2t1.Models
{
    public class ImageRecord
    {
        [Key]
        public int Id { get; set; } // Primary key
        public string? FileName { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; } // Image description, received from Computer Vision Services
    }
}
