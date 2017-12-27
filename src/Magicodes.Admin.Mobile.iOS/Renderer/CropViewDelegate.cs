using System;
using System.Diagnostics;
using Magicodes.Admin.Controls;
using UIKit;
using Xam.Plugins.ImageCropper.iOS;

namespace Magicodes.Admin.Renderer
{
    public class CropViewDelegate : TOCropViewControllerDelegate
    {
        readonly UIViewController _parent;
        public bool IsCropped;
        private readonly CropView _page;

        public CropViewDelegate(UIViewController parent, CropView page)
        {
            _parent = parent;
            _page = page;
        }

        public override void DidCropToImage(TOCropViewController cropViewController, UIImage image, CoreGraphics.CGRect cropRect, nint angle)
        {
            IsCropped = true;

            try
            {
                if (image != null)
                {
                    _page.CroppedImage = image.AsJPEG().ToArray();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                if (image != null)
                {
                    image.Dispose();
                    image = null;
                }

                CloseView();
            }
        }

        public override void DidFinishCancelled(TOCropViewController cropViewController, bool cancelled)
        {
            CloseView();
        }

        private void CloseView()
        {
            _parent.DismissViewController(true, () =>
            {
                Xamarin.Forms.Application.Current.MainPage.Navigation.PopModalAsync();
            });
        }
    }
}