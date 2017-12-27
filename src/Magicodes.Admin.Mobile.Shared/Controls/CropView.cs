using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Magicodes.Admin.Controls
{
    public class CropView : ContentPage
    {
        public byte[] Image;
        public Func<byte[], string, Task> OnCropCompleted;
        public bool IsCropped;
        public byte[] CroppedImage;
        public string OkText;
        public string RotateText;
        public string FileName;
        public string CancelText;

        public CropView(byte[] imageAsByte,
            string fileName,
            Func<byte[], string, Task> onCropCompleted,
            string okText = "Ok",
            string rotateText = "Rotate",
            string cancelText = "Cancel")
        {
            NavigationPage.SetHasNavigationBar(this, false);
            FileName = fileName;
            Image = imageAsByte;
            Initialize(onCropCompleted, okText, rotateText, cancelText);
        }

        public CropView(string filePath,
            Func<byte[], string, Task> onCropCompleted,
            string okText = "Ok",
            string rotateText = "Rotate",
            string cancelText = "Cancel")
        {
            NavigationPage.SetHasNavigationBar(this, false);
            Image = File.ReadAllBytes(filePath);
            FileName = Path.GetFileName(filePath);
            Initialize(onCropCompleted, okText, rotateText, cancelText);
        }

        private void Initialize(Func<byte[], string, Task> onCropCompleted,
            string okText,
            string rotateText,
            string cancelText)
        {
            IsCropped = false;
            BackgroundColor = (Color)Application.Current.Resources["MasterPageBackgroundColor"];
            OnCropCompleted = onCropCompleted;
            OkText = okText;
            RotateText = rotateText;
            CancelText = cancelText;
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            if (IsCropped)
            {
                await OnCropCompleted(CroppedImage, FileName);
            }
        }
    }
}


