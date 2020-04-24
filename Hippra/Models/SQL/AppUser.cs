﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Hippra.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace Hippra.Models.SQL
{
    public class AppUser : IdentityUser
    {
        // + personal info 
        [Required]
        [PersonalData]
        public string FirstName { get; set; }
        [Required]
        [PersonalData]
        public string LastName { get; set; }

        [Required]
        [PersonalData]
        [Display(Name = "National Provider Identifier Number")]
        public int NPIN { get; set; }

        // inherited
        // UserName
        // Email
        // Password

        [Required]
        [PersonalData]
        [Display(Name = "Medical Specialty")]
        public int MedicalSpecialty { get; set; }

        [Required]
        [PersonalData]
        [Display(Name = "American Board Certified")]
        public bool AmericanBoardCertified { get; set; }

        //[Required]
        [PersonalData]
        [Display(Name = "Residency Hospital")]
        public string ResidencyHospital { get; set; }

        //[Required]
        [PersonalData]
        [Display(Name = "Medical School Attended")]
        public string MedicalSchoolAttended { get; set; }

        //[Required]
        [PersonalData]
        [Display(Name = "Education/Degree")]
        public string EducationDegree { get; set; }

        //[Required]
        [PersonalData]
        [Display(Name = "Address")]
        public string Address { get; set; }

        //[Required]
        [PersonalData]
        [Display(Name = "Zipcode")]
        public string Zipcode { get; set; }

        //[Required]
        [PersonalData]
        [Display(Name = "State")]
        public string State { get; set; }

        //[Required]
        [PersonalData]
        [Display(Name = "City")]
        public string City { get; set; }

        // inherited
        //[Required]
        //[PersonalData]
        //[Display(Name = "Contact Number")]
        //[Phone]
        //public string PhoneNumber { get; set; }


        // - personal info 
        // autogenerated
        public int PublicId { get; set; }
        public DateTime DateJoined { get; set; }
        public UserOnlineStatus Status { get; set; }
        // admin
        public bool isApproved { get; set; }

    }
}
