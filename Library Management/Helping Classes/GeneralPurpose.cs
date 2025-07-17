using System.Security.Claims;
using Library_Management.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Library_Management.Models.Dtos;

namespace Library_Management.HelpingClasses
{
    public class GeneralPurpose
    {
        private readonly HttpContext hcontext;
        public GeneralPurpose(IHttpContextAccessor haccess)
        {
            hcontext = haccess.HttpContext;
        }

        public UserDto? GetUserClaims()
        {
            string? userId = hcontext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value;
            string? encId = hcontext?.User.Claims.FirstOrDefault(c => c.Type == "EncId")?.Value;
            string? name = hcontext?.User.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value;
            string? email = hcontext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            string? role = hcontext?.User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;
            string? profile = hcontext?.User.Claims.FirstOrDefault(c => c.Type == "Profile")?.Value;

            UserDto? loggedInUser = null;
            if (userId != null)
            {
                loggedInUser = new UserDto()
                {
                    Id = userId,
                    EncId = encId,
                    Name = name,
                    Email = email,
                    Role = Convert.ToInt32(role),
                };
                //var baseUri = GetFilePath(loggedInUser.Id.ToString());
                //if (string.IsNullOrEmpty(profile))
                //{
                //    loggedInUser.Profile = "";
                //}
                //else
                //{
                //    loggedInUser.Profile = baseUri + loggedInUser.Profile;
                //}
            }

            return loggedInUser;
        }

        public async Task<bool> SetUserClaims(User user)
        {
            try
            {
                //var baseUri = GetFilePath(user.Id.ToString());

                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Sid, user.Id.ToString()),
                    new Claim("EncId", user.Id.ToString()),
                    new Claim("UserName", user.firstName + " " + user.lastName),
                    new Claim(ClaimTypes.Email, user.email),
                    new Claim("Role", user.role.ToString()),
                };


                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await hcontext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties { IsPersistent = true }
                );

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static DateTime DateTimeNow()
        {
            return DateTime.UtcNow;
        }

        #region directory creation and file upload
        public static void CreateUserDirectory(Guid? uId)
        {
            var rootDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var userDir = rootDir + "/UserImages/user-" + uId.ToString() + "/";

            if (!Directory.Exists(userDir))
                Directory.CreateDirectory(userDir);
        }

        public static string GetFilePathForSave(string? userId, string? filePath = "")
        {
            var rootDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var baseUri = rootDir + $"/UserImages/user-{userId}/";

            if (string.IsNullOrEmpty(filePath))
            {
                return baseUri;
            }
            return baseUri + filePath;

        }

        // public static string GetFilePath(string? userId, string? filePath = "")
        //{
        //    var baseUri = rootDir + $"UserImages/user-{userId}/";

        //    if (string.IsNullOrEmpty(filePath))
        //    {
        //        return baseUri;
        //    }
        //    return baseUri + filePath;

        //}

        public static async Task<bool> SaveFile(IFormFile file, string filePath)
        {
            try
            {
                var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                var bytes = stream.ToArray();
                stream.Close();

                await File.WriteAllBytesAsync(filePath, bytes);

                return true;
            }
            catch
            {
                return false;
            }
        }
        public static async Task<bool> DeleteFile(string userId, string filePath)
        {
            try
            {
                var fileDir = GetFilePathForSave(userId.ToString());
                if (!string.IsNullOrEmpty(filePath))
                {
                    string[] getName = filePath.Split("/");

                    // Check if a profile picture already exists for the user
                    string existingProfilePicturePath = Path.Combine(fileDir, getName.Last());
                    if (System.IO.File.Exists(existingProfilePicturePath))
                    {
                        System.IO.File.Delete(existingProfilePicturePath);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

    }
}