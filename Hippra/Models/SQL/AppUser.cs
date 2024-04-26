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

        public string FullName { get { return this.FirstName + " " + this.LastName; } }

        [Required]
        [PersonalData]
        [Display(Name = "National Provider Identifier Number")]
        public int NPIN { get; set; }

        [Required]
        [PersonalData]
        [Display(Name = "Medical Specialty")]
        public MedicalSpecialtyType MedicalSpecialty { get; set; }

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

        [PersonalData]
        [Display(Name = "Country")]
        public string Country { get; set; }

        public UserAccountType AccountType { get; set; } = UserAccountType.Nurse;

        // - personal info 
        // autogenerated
        public int PublicId { get; set; }
        public DateTime DateJoined { get; set; }
        public UserOnlineStatus Status { get; set; }
        // admin
        public bool isApproved { get; set; }
        public string ProfileUrl { get; set; }
        public string BackgroundUrl { get; set; }
        public string Bio { get; set; }

        public string Topic { get; set; }
        public string Position { get; set; }
        public string Timeline { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }

    }
}
