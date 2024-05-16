//using Hippra.Code;
using Hippra.Data;
using Hippra.Extensions;
using Hippra.Models.POCO;
using Hippra.Models.SQL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hippra.Models.FTDesign;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http.Extensions;
using System.Security.Claims;
using Newtonsoft.Json;
using Hippra.Services.Email;
using Hippra.Models.ViewModel;
using SQLitePCL;
using Hippra.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace Hippra.Services
{
    public class ProfileService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        //private readonly FriendManagerService _fmService;
        //private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailService _emailSender;
        private IDbContextFactory<ApplicationDbContext> DbFactory;

        private AppSettings AppSettings { get; set; }
        public ProfileService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
           //FriendManagerService fmService,
           IEmailService emailSender,
            ApplicationDbContext context,
            IOptions<AppSettings> settings,
            RoleManager<IdentityRole> roleManager,
            IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            //_fmService = fmService;
            _emailSender = emailSender;
            AppSettings = settings?.Value;
            _roleManager = roleManager;
            DbFactory = dbFactory;
            //Storage = new AzureStorage(settings);
            //UserDataHelper = new UserDataHelper(_context, Storage, _userManager);

        }


        public async Task<Profile> GetProfile(int id)
        {
            Profile rProfile;

            var user = await UserManagerExtensions.FindByPublicIDNoTrackAsync(_userManager, id);
            if (user != null)
            {
                rProfile = new Profile
                {
                    Userid = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    NPIN = user.NPIN,
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
                    Bio = user.Bio
                };

            }
            else
            {
                rProfile = new Profile();
            }

            return rProfile;
        }

        public async Task<Profile> GetProfileById(string userId)
        {
            Profile rProfile;

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                rProfile = Profile.FromEntity(user);

            }
            else
            {
                rProfile = new Profile();
            }

            return rProfile;
        }

        public async Task<List<AppUser>> GetProfiles()
        {
            return await UserManagerExtensions.GetNotApprovedUsers(_userManager);
        }
        public async Task ApproveAccount(int id)
        {
            await UserManagerExtensions.ApproveAccount(_userManager, id);
        }
        public async Task DeleteAccount(int id)
        {
            await UserManagerExtensions.DelAccount(_userManager, id);
        }
        public async Task BanAccount(int id)
        {
            await UserManagerExtensions.BanAccount(_userManager, id);
        }
        public async Task<int> GetUserIdByEmail(string email)
        {
            var user = await UserManagerExtensions.FindByEmail(_userManager, email);
            return user.PublicId;
        }


        public async Task<int> LoginAccount(FTLoginModel Input)
        {

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            //var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);

            var user = await UserManagerExtensions.FindByEmail(_userManager, Input.Email);

            if (user == null)
            {
                return -1;
            }
            //var result = await _signInManager.CheckPasswordSignInAsync(user, Input.Password, lockoutOnFailure: false);
            var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, Input.Password);
            if (result == PasswordVerificationResult.Success)
            {
                // NOTE: THIS WILL FAIL AS OF ASPNETCORE 3.1 blazor due to fact that cannot update cookie directly
                //await _signInManager.SignInAsync(user, isPersistent: false);
            }

            //if (result.Succeeded)
            //    {
            //        // check for validation first

            //        var validateResult = await UserManagerExtensions.ValidateAccountApproval(_userManager, Input.Email);
            //        if (!validateResult)
            //        {
            //            //if (_signInManager.IsSignedIn(User))
            //            //{
            //                await _signInManager.SignOutAsync();
            //            //}

            //            return 2;
            //        }

            //        return 1;
            //    }
            //    if (result.RequiresTwoFactor)
            //    {
            //        return 3;//RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
            //    }
            //    if (result.IsLockedOut)
            //    {
            //    //_logger.LogWarning("User account locked out.");
            //    return 4;// RedirectToPage("./Lockout");
            //    }
            //    else
            //    {
            //        return -1;
            //    }
            return 0;
        }
        public async Task<int> SendVerificationEmail(string email, string urlStr)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return 1;
            }

            //var userId = await _userManager.GetUserIdAsync(user);
            //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //var callbackUrl = urlStr + "Identity/Account/ConfirmEmail?userId=" + user.Id + "&code=" + code;

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = "https://www.hippra.com/Identity/Account/ConfirmEmail?userId=" + user.Id + "&code=" + code;

            await _emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");


            return 1;
        }
        public async Task<int> UpdateEmail(ClaimsPrincipal usr, string newEmail, string urlStr)
        {
            var user = await _userManager.GetUserAsync(usr);
            var email = user.Email;
            if (newEmail != email)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
                var callbackUrl = urlStr + "Identity/Account/ConfirmEmail?userId=" + user.Id + "&code=" + code;
                //var callbackUrl = Url.Page(
                //    "/Account/ConfirmEmailChange",
                //    pageHandler: null,
                //    values: new { userId = userId, email = newEmail, code = code },
                //    protocol: Request.Scheme);
                await _emailSender.SendEmailAsync(
                    newEmail,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return 0;
            }

            return 1;
        }

        public async Task<int> UpdatePassword(AppUser user, string oldPassword, string newPassword)
        {


            var changePasswordResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!changePasswordResult.Succeeded)
            {
                //foreach (var error in changePasswordResult.Errors)
                //{
                //    ModelState.AddModelError(string.Empty, error.Description);
                //}
                return -1;
            }
            // NOTE: THIS WILL FAIL AS OF ASPNETCORE 3.1 blazor due to fact that cannot update cookie directly
            //await _signInManager.RefreshSignInAsync(user);
            //await _signInManager.SignOutAsync();

            //StatusMessage = "Your password has been changed.";
            return 1;

        }

        public async Task<int> UpdatePassword(int userId, string oldPassword, string newPassword)
        {
            var user = await UserManagerExtensions.FindByPublicIDNoTrackAsync(_userManager, userId);

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!changePasswordResult.Succeeded)
            {
                //foreach (var error in changePasswordResult.Errors)
                //{
                //    ModelState.AddModelError(string.Empty, error.Description);
                //}
                return -1;
            }
            // NOTE: THIS WILL FAIL AS OF ASPNETCORE 3.1 blazor due to fact that cannot update cookie directly
            //await _signInManager.RefreshSignInAsync(user);
            //await _signInManager.SignOutAsync();

            //StatusMessage = "Your password has been changed.";
            return 1;

        }

        //public async Task<string> DownloadPersonalData(AppUser user)
        //{
        //    if (user == null)
        //    {
        //        return "";
        //    }

        //    // Only include personal data for download
        //    var personalData = new Dictionary<string, string>();
        //    var personalDataProps = typeof(AppUser).GetProperties().Where(
        //                    prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
        //    foreach (var p in personalDataProps)
        //    {
        //        personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
        //    }

        //    //Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
        //    return JsonConvert.SerializeObject(personalData);
        //}

        public async Task<string> DownloadPersonalData(int userId)
        {
            var user = await UserManagerExtensions.FindByPublicIDNoTrackAsync(_userManager, userId);

            if (user == null)
            {
                return "";
            }

            // Only include personal data for download
            var personalData = new Dictionary<string, string>();
            var personalDataProps = typeof(AppUser).GetProperties().Where(
                            prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            //Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
            return JsonConvert.SerializeObject(personalData);
        }

        //public async Task<int> DeleteAccount(AppUser user, string password)
        //{

        //    var RequirePassword = await _userManager.HasPasswordAsync(user);
        //    if (RequirePassword)
        //    {
        //        if (!await _userManager.CheckPasswordAsync(user, password))
        //        {
        //            return -1;
        //        }
        //    }

        //    var result = await _userManager.DeleteAsync(user);
        //    var userId = await _userManager.GetUserIdAsync(user);
        //    if (!result.Succeeded)
        //    {
        //        return -1;
        //    }

        //    return 0;

        //    //await _signInManager.SignOutAsync();
        //    //return Redirect("~/");

        //}
        public async Task<int> DeleteAccount(int userId, string password)
        {
            var user = await UserManagerExtensions.FindByPublicIDNoTrackAsync(_userManager, userId);

            var RequirePassword = await _userManager.HasPasswordAsync(user);
            if (RequirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, password))
                {
                    return -1;
                }
            }

            var result = await _userManager.DeleteAsync(user);
            var userId2 = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                return -1;
            }

            return 0;

            //await _signInManager.SignOutAsync();
            //return Redirect("~/");

        }
        public async Task<int> SendContactEmail(string name, string email, string subject, string message)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(subject) ||
                string.IsNullOrWhiteSpace(message))
            {
                return 1;
            }

            await _emailSender.FTSendAdminEmailAsync(
                email,
                email + " : " + subject,
                name + " : " + message);

            return 0;
        }

        public async Task<int> GetUserCount()
        {
            return await Hippra.Extensions.UserManagerExtensions.GetAllUsersCount(_userManager);
        }
        public async Task<List<string>> GetUserNameList()
        {
            return await Hippra.Extensions.UserManagerExtensions.GetAllUsersName(_userManager);
        }

        public async Task<List<UserReport>> GetUserReport()
        {
            return await Hippra.Extensions.UserManagerExtensions.GetUserReport(_userManager);
        }

        public async Task<bool> IsEmailUsedInTheSystem(string email)
        {
            using var _context = DbFactory.CreateDbContext();
            return await _context.Users.AnyAsync(x => x.Email == email.ToLower());
        }

        public async Task UpdateUserProfile(Profile usr)
        {
            var user = await _userManager.FindByIdAsync(usr.Userid);

            if (user != null)
            {
                user.FirstName = usr.FirstName;
                user.LastName = usr.LastName;
                user.NPIN = user.NPIN;
                user.MedicalSpecialty = usr.MedicalSpecialty;
                user.AmericanBoardCertified = usr.AmericanBoardCertified;

                user.ResidencyHospital = usr.ResidencyHospital;
                user.MedicalSchoolAttended = usr.MedicalSchoolAttended;
                user.EducationDegree = usr.EducationDegree;
                user.Address = usr.Address;
                user.Zipcode = usr.Zipcode;
                user.State = usr.State;
                user.City = usr.City;
                user.PhoneNumber = usr.PhoneNumber; // check this

                user.ProfileUrl = usr.ProfileUrl;
                user.BackgroundUrl = usr.BackgroundUrl;
                user.Bio = usr.Bio;

                await _userManager.UpdateAsync(user);
            }

        }

        public async Task<(bool, IList<string>)> RegisterUser(CreateUserRequestDto Input, string callbackUrl)
        {

            var userWithlargestPublicID = await UserManagerExtensions.GetLastPID(_userManager);

            AppUser user = new AppUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                PublicId = userWithlargestPublicID + 1,
                FirstName = Input.FirstName,
                LastName = Input.LastName,


                MedicalSpecialty = Input.MedicalSpecialty,
                AmericanBoardCertified = Input.AmericanBoardCertified,
                ResidencyHospital = Input.ResidencyHospital,
                MedicalSchoolAttended = Input.MedicalSchoolAttended,
                EducationDegree = Input.EducationDegree,
                Address = Input.Address,
                Zipcode = Input.Zipcode,
                State = Input.State,
                City = Input.City,
                PhoneNumber = Input.PhoneNumber,
                DateJoined = DateTime.Now,
                isApproved = true
            };
            // temporary approve all sign up



            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {

                if (Input.AccountType == Models.Enums.UserAccountType.Nurse)
                {
                    await _userManager.AddToRoleAsync(user, "Nurse");
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, "Physician");
                }

                //_logger.LogInformation("User created a new account with password.");

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                //   var callbackUrl = urlStr + "Identity/Account/ConfirmEmail?userId=" + user.Id + "&code=" + code;

                //var callbackUrl = Url.Page(
                //    "/Account/ConfirmEmail",
                //    pageHandler: null,
                //    values: new { area = "Identity", userId = user.Id, code = code },
                //    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                // create azure storage for contact management
                //await UserDataHelper.InitUserData(user.PublicId);

                return new(true, null);
            }
            return new(false, result.Errors.Select(x=>x.Description).ToList());
      
        }

    }
}