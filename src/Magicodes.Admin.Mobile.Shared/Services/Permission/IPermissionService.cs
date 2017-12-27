namespace Magicodes.Admin.Services.Permission
{
    public interface IPermissionService
    {
        bool HasPermission(string key);
    }
}