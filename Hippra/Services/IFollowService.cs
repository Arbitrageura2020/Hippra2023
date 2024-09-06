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
using FTEmailService;
using Microsoft.EntityFrameworkCore;
using Hippra.Models.Enums;
using Hippra.Code;
using Hippra.API;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Hippra.Models.ViewModel;
using Hippra.Components;

namespace Hippra.Services
{
    public interface IFollowService
    {
        //Follow
        Task<bool> AddFollower(string followingId, string followerId);
        Task<bool> RemoveFollower(string followingId, string followerId);
        Task<List<Follow>> GetAllFollowers(string userId);
        Task<bool> CheckFollower(string myId, string followingId);

        Task<int> GetNrOfFollowers(string userId);

        Task<int> GetNrOfFollowing(string userId);
        Task<IList<ProfileViewModel>> GetExpertsToFollow(string currentUserId);
    }
}
