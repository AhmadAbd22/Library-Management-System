using System.Security.Claims;
using Library_Management.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Library_Management.Models.Dtos;
using Library_Management.Models.Context;

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
            }

            return loggedInUser;
        }

        public async Task SetUserClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.email),
                new Claim(ClaimTypes.Role, ((enumRole)user.role).ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(60)
            };

            await hcontext.SignInAsync( 
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
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