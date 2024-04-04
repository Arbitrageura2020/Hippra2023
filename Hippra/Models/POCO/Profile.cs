using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Hippra.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace Hippra.Models.POCO
{
    public class Profile
    {
        // + personal info 

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Display(Name = "National Provider Identifier Number")]
        public int NPIN { get; set; }


        public string UserName { get; set; }
        public string Email { get; set; }

        [Display(Name = "Medical Specialty")]
        public MedicalSpecialtyType MedicalSpecialty { get; set; }

        [Display(Name = "American Board Certified")]
        public bool AmericanBoardCertified { get; set; }

        [Display(Name = "Residency Hospital")]
        public string ResidencyHospital { get; set; }

        [Display(Name = "Medical School Attended")]
        public string MedicalSchoolAttended { get; set; }

        [Display(Name = "Education/Degree")]
        public string EducationDegree { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Zipcode")]
        public string Zipcode { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

       [Display(Name = "Contact Number")]
       [Phone]
        public string PhoneNumber { get; set; }


        // - personal info 
        public int PublicId { get; set; }
        public string DateJoined { get; set; }
        public UserOnlineStatus Status { get; set; }
        public string ProfileUrl { get; set; }
        public string BackgroundUrl { get; set; }
        public string Bio { get; set; }
    }
}
