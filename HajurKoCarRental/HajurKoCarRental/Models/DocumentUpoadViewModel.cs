using Microsoft.AspNetCore.Http;

namespace HajurKoCarRental.Models
{
    public class DocumentUploadViewModel
    {
        public int CarId { get; set; }

        public IFormFile DrivingLicense { get; set; }

        public IFormFile CitizenPaper { get; set; }
    }
}
