using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Hippra.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Hippra.Models.DTO;
using Hippra.Models.SQL;
using System.Globalization;

namespace Hippra.Models.POCO
{
    public class ProfileViewModel
    {
        // + personal info 
        public string Userid { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Display(Name = "National Provider Identifier Number")]
        public int NPIN { get; set; }
        [Display(Name = "ID.Me")]
        public int IdMe { get; set; }
        public UserAccountType AccountType { get; set; }

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

        [Display(Name = "Country")]
        public string Country { get; set; }

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

        public string FullName { get { return this.FirstName + " " + this.LastName; } }

        private int NrOfFollowers { get; set; }
        private int NrOfFollowing { get; set; }
        private int NrOfPosts { get; set; }
        public static ProfileViewModel FromEntity(AppUser user)
        {

            ProfileViewModel result = new ProfileViewModel
            {
                Userid = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                NPIN = user.NPIN,
                IdMe = user.IDMe,
                MedicalSpecialty = user.MedicalSpecialty,
                AmericanBoardCertified = user.AmericanBoardCertified,
                Email = user.Email,
                Status = user.Status,
                UserName = user.UserName,
                ResidencyHospital = user.ResidencyHospital,
                MedicalSchoolAttended = user.MedicalSchoolAttended,
                EducationDegree = user.EducationDegree,
                Address = user.Address,
                Zipcode = user.Zipcode,
                State = user.State,
                City = user.City,
                PhoneNumber = user.PhoneNumber,
                DateJoined = user.DateJoined.ToString("MMMM dd, yyyy", CultureInfo.CreateSpecificCulture("en-US")),
                PublicId = user.PublicId,
                ProfileUrl = user.ProfileUrl,
                BackgroundUrl = user.BackgroundUrl,
                Bio = user.Bio,
                AccountType = user.AccountType,
            };
            return result;
        }

        public static IList<ProfileViewModel> FromEntityList(ICollection<AppUser> items)
        {
            return items.Select(x => FromEntity(x)).ToList();
        }
    }
}
