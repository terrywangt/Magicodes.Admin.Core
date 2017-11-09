namespace Magicodes.Admin.Web.Areas.Admin.Models.Common
{
    public class PermissionTreeItemModel
    {
        public IPermissionsEditViewModel EditModel { get; set; }

        public string ParentName { get; set; }

        public PermissionTreeItemModel()
        {
            
        }

        public PermissionTreeItemModel(IPermissionsEditViewModel editModel, string parentName)
        {
            EditModel = editModel;
            ParentName = parentName;
        }
    }
}