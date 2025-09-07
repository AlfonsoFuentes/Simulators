namespace Simulator.Shared.Constants.Routes
{
    public static class AccountEndpoints
    {
        public static string Register = "api/identity/account/register";
        public static string ChangePassword = "api/identity/account/changepassword";
        public static string UpdateProfile = "api/identity/account/updateprofile";

        public static string GetProfilePicture(string userId)
        {
            return $"api/identity/account/profile-picture/{userId}";
        }

        public static string UpdateProfilePicture(string userId)
        {
            return $"api/identity/account/profile-picture/{userId}";
        }
        public static class Token
        {
            public static string Get = "identity/token";
            public static string Refresh = "identity/token/refresh";
        }
        public static class User
        {
            public static string Register = "user/register";
            public static string GetUser = "user/getuser";
        }
    }
    public static class TokenEndpoints
    {
        public static string Get = "api/identity/token";
        public static string Refresh = "api/identity/token/refresh";
    }

    public static class StorageConstants
    {
        public static class Local
        {
            public static string Preference = "clientPreference";

            public static string AuthToken = "authToken";
            public static string RefreshToken = "refreshToken";
            public static string UserImageURL = "userImageURL";
        }

        public static class Server
        {
            public static string Preference = "serverPreference";


        }
    }
    public static class RoleClaimsEndpoints
    {
        public static string Delete = "api/identity/roleClaim";
        public static string GetAll = "api/identity/roleClaim";
        public static string Save = "api/identity/roleClaim";
    }
    public static class RolesEndpoints
    {
        public static string Delete = "api/identity/role";
        public static string GetAll = "api/identity/role";
        public static string Save = "api/identity/role";
        public static string GetPermissions = "api/identity/role/permissions/";
        public static string UpdatePermissions = "RolePermisions/permissionsupdate";
    }
    public static class UserEndpoints
    {
        public static string GetAll = "api/identity/user";

        public static string Get(string userId)
        {
            return $"api/identity/user/{userId}";
        }

        public static string GetUserRoles(string userId)
        {
            return $"api/identity/user/roles/{userId}";
        }

        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }

        public static string Export = "api/identity/user/export";
        public static string Register = "api/identity/user";
        public static string ToggleUserStatus = "api/identity/user/toggle-status";
        public static string ForgotPassword = "api/identity/user/forgot-password";
        public static string ResetPassword = "api/identity/user/reset-password";
    }
}