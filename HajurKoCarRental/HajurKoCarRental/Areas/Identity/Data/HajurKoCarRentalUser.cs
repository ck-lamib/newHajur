using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;

namespace HajurKoCarRental.Areas.Identity.Data
{

    // Add profile data for application users by adding properties to the HajurKoCarRentalUser class
    public class HajurKoCarRentalUser : IdentityUser
    {
        [Required]
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string? FullName { get; set; }

        [Required]
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string Address { get; set; }

        //[Required]
        //[PersonalData]
        //[Column(TypeName = "nvarchar(100)")]
        //public string PhoneNumber { get; set; }


        [PersonalData]
        [Column(TypeName = "varbinary(max)")]
        public byte[]? Citizenship { get; set; }

        [PersonalData]
        public string? CitizenshipFileName { get; set; }


        [Column(TypeName = "varbinary(max)")]
        public byte[]? DrivingLicense { get; set; }

        [PersonalData]
        public string? DrivingLicenseFileName { get; set; }





        public bool is_RegularCustomer { get; set; }

    }
}



