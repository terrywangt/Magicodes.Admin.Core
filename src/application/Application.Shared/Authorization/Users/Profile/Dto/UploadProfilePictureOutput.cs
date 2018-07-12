using Abp.Web.Models;

namespace Magicodes.Admin.Authorization.Users.Profile.Dto
{
    public class UploadProfilePictureOutput : ErrorInfo
    {
        public string FileName { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public UploadProfilePictureOutput()
        {
            
        }

        public UploadProfilePictureOutput(ErrorInfo error)
        {
            Code = error.Code;
            Details = error.Details;
            Message = error.Message;
            ValidationErrors = error.ValidationErrors;
        }
    }
}