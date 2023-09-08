using Microsoft.AspNetCore.Http;

namespace Domain
{
    public static class SD
    {
        public const String ClientId = "Test593a12712eba48e6b7d5b9431fa71aa0";
        public const String ClientSecret = "Testaef6ccca445e4170b1c8de10f0788e84";
        public const String DefaultNationalId = "1234567890";

        public const String AuthInfo = "auth_info";
        public const String UswToken = "usw-token";

        public static Guid AdminRoleId = Guid.Parse("08151D71-9D6C-43EF-85A6-16043ADB6B3A");
        public static Guid TopPermissionId = Guid.Parse("08151D71-9D6C-43EF-85A6-16043DCB6B3A");

        public const int ExpirationTime = 15;

        #region Permissions
        // big goal
        public const String Permission_AddBigGoal = "Permission_CreateBigGoal";
        public const String Permission_RemoveBigGoal = "Permission_RemoveBigGoal";
        public const String Permission_EditBigGoal = "Permission_EditBigGoal";

        // operational objective
        public const String Permission_AddOperationalObjective = "Permission_AddOperationalObjective";
        public const String Permission_RemoveOperationalObjective = "Permission_RemoveOperationalObjective";
        public const String Permission_EditOperationalObjective = "Permission_EditOperationalObjective";

        // project
        public const String Permission_AddProject = "Permission_AddProject";
        public const String Permission_EditProject = "Permission_EditProject";
        public const String Permission_RemoveProject = "Permission_RemoveProject";

        // practical action
        public const String Permission_AddPracticalAction = "Permission_AddPracticalAction";
        public const String Permission_EditPracticalAction = "Permission_EditPracticalAction";
        public const String Permission_RemovePracticalAction = "Permission_RemovePracticalAction";

        // person
        public const String Permission_AddPerson = "Permission_AddPerson";
        public const String Permission_EditPerson = "Permission_EditPerson";
        public const String Permission_RemovePerson = "Permission_RemovePerson";

        // hardware equipment
        public const String Permission_AddHardwareEquipment = "Permission_AddHardwareEquipment";
        public const String Permission_EditHardwareEquipment = "Permission_EditHardwareEquipment";
        public const String Permission_RemoveHardwareEquipment = "Permission_RemoveHardwareEquipment";

        // system
        public const String Permission_AddSystem = "Permission_AddSystem";
        public const String Permission_EditSystem = "Permission_EditSystem";
        public const String Permission_RemoveSystem = "Permission_RemoveSystem";

        // role
        public const String Permission_AddRole = "Permission_AddRole";
        public const String Permission_EditRole = "Permission_EditRole";
        public const String Permission_RemoveRole = "Permission_RemoveRole";
        #endregion


        public static String GetRedirectUrl(IHttpContextAccessor _accessor)
        {
            return _accessor.HttpContext.Request.Scheme + "://" + _accessor.HttpContext.Request.Host.Value + "/api/account/authorizeLogin";
        }
    }

}
