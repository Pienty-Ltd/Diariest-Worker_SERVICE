using System.ComponentModel.DataAnnotations;

namespace Pienty.Diariest.Core.Models.Database
{
    public class ErrorLog
    {
        [Key]
        public long Id { get; set; }
        public string? Code { get; set; }
        public string? Message { get; set; }
        public bool Fixed { get; set; }
        public DateTime LoggedAt { get; set; }
    }   
}