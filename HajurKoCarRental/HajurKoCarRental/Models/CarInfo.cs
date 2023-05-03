using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using HajurKoCarRental.Models;


namespace HajurKoCarRental.Models
{
    public class CarInfo

    {
      
    
        [Key]
        [Display(Name = "Car ID")]
        [Required(ErrorMessage = "Car ID is must required")]
        public int id { get; set; }

       
        [Display(Name = "Car Image")]
        public string CarImage { get; set; }


        [Display(Name = "Car Name")]
        [Required(ErrorMessage = "Car Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Car name must be in between 3 and 50 characters")]
        public string CarName { get; set; }

        [Display(Name = "Car Brand")]
        [Required(ErrorMessage = "Car Brand is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Car Brand must be in between 3 and 50 characters")]
        public string CarBrand { get; set; }


        [Display(Name = "Car Model")]
        [Required(ErrorMessage = "Car Model is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Car Model must be in between 3 and 50 characters")]
        public string CarModel { get; set; }

        [Display(Name = "Car Description")]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "Car description must be entered")]
        public string CarDescription { get; set; }

        [Display(Name = "Car Number")]
        public string CarNumber { get; set; }

        [Display(Name = "Rent price")]
        public decimal RentPrice { get; set; }


        [Display(Name = "Is Available")]

        public bool is_available { get; set; }
        public bool is_rented { get; set; } 






    }
}
