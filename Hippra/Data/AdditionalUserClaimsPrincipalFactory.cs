using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Hippra.Models.Enums;
using Hippra.Models.SQL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Hippra.Data
{
    public class AdditionalUserClaimsPrincipalFactory
        : UserClaimsPrincipalFactory<AppUser, IdentityRole>
    {
        public AdditionalUserClaimsPrincipalFactory(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        { }

        public async override Task<ClaimsPrincipal> CreateAsync(AppUser user)
        {
            var principal = await base.CreateAsync(user);
            var identity = (ClaimsIdentity)principal.Identity;

            var claims = new List<Claim>();
            claims.Add(new Claim("PublicId", user.PublicId.ToString("G", CultureInfo.InvariantCulture)));
            claims.Add(new Claim("UserId", user.Id));
            claims.Add(new Claim("FullName", user.FullName ?? ""));
            claims.Add(new Claim("FirstName", user.FirstName ?? ""));
            claims.Add(new Claim("LastName", user.LastName ?? ""));
            claims.Add(new Claim("MedicalSpecialty", EnumsHelper.GetDisplayName(user.MedicalSpecialty)));
            claims.Add(new Claim("ProfilePicture", user.ProfileUrl ?? "/img/hippra/blank-profile.png"));
            claims.Add(new Claim("Email", user.Email ?? ""));
            claims.Add(new Claim("EmailConfirmed", user.EmailConfirmed ? "1" : "0"));
            claims.Add(new Claim("NormalizedEmail", user.NormalizedEmail ?? ""));
            claims.Add(new Claim("AccountType", EnumsHelper.GetDisplayName(user.AccountType) ?? "Nurse Practitioner"));
            claims.Add(new Claim("UserName", user.UserName ?? ""));
            claims.Add(new Claim("ProfileUrl", user.ProfileUrl ?? ""));
            claims.Add(new Claim("BackgroundUrl", user.BackgroundUrl ?? ""));
            claims.Add(new Claim("Bio", user.Bio ?? ""));

            claims.Add(new Claim("ResidencyHospital", user.ResidencyHospital ?? ""));
            claims.Add(new Claim("MedicalSchoolAttended", user.MedicalSchoolAttended ?? ""));
            claims.Add(new Claim("EducationDegree", user.EducationDegree ?? ""));
            claims.Add(new Claim("Address", user.Address ?? ""));
            claims.Add(new Claim("Zipcode", user.Zipcode ?? ""));
            claims.Add(new Claim("State", user.State ?? ""));
            claims.Add(new Claim("City", user.City ?? ""));
            claims.Add(new Claim("PhoneNumber", user.PhoneNumber ?? ""));

            claims.Add(new Claim("NPIN", user.IdNumber.ToString("G", CultureInfo.InvariantCulture)));
            claims.Add(new Claim("AmericanBoardCertified", user.AmericanBoardCertified ? "1" : "0"));

            //if (user.IsAdmin)
            //{
            //	claims.Add(new Claim(JwtClaimTypes.Role, "admin"));
            //}
            //else
            //{
            //	claims.Add(new Claim(JwtClaimTypes.Role, "user"));
            //}

            identity.AddClaims(claims);
            return principal;
        }
    }
}
