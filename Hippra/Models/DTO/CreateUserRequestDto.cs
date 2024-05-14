using Hippra.Models.Enums;
using Hippra.Models.ViewModel;
using System.ComponentModel.DataAnnotations;

namespace Hippra.Models.DTO
{
    public class CreateUserRequestDto
    {


        public string Email { get; set; }


        public string Password { get; set; }


        public string ConfirmPassword { get; set; }


        public UserAccountType AccountType { get; set; }
        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }


        public MedicalSpecialtyType MedicalSpecialty { get; set; }


        public int IdNumber { get; set; }


        public int IdMe { get; set; }



        public bool AmericanBoardCertified { get; set; }


        public string ResidencyHospital { get; set; }



        public string MedicalSchoolAttended { get; set; }



        public string EducationDegree { get; set; }



        public string Address { get; set; }



        public string Zipcode { get; set; }



        public string State { get; set; }



        public string City { get; set; }


        public string Country { get; set; }


        public string PhoneNumber { get; set; }


        public bool AgreedTerm { get; set; }
    }
}
