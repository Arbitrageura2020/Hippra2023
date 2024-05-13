using Hippra.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Models.ViewModel
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [RegularExpression("^((?=.*[a-z])(?=.*[A-Z])(?=.*\\d)).+$", ErrorMessage = "The Password must contains a Uppercase, a Lowercase, a Number, and a Special characters")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        public string UserName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required]
        public UserAccountType AccountType { get; set; }

        [Required]
        [Display(Name = "Medical Specialty")]
        public MedicalSpecialtyType MedicalSpecialty { get; set; }

        [Required]
        [Display(Name = "National Provider Identifier Number")]
        public int IdNumber { get; set; }

          [Display(Name = "ID.Me")]
        public int IdMe { get; set; }


        [Required]
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

        [Display(Name = "Country")]
        public string Country { get; set; }


        [Display(Name = "Contact Number")]
        //[Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Check")]
        public bool AgreedTerm { get; set; }
    }
}
