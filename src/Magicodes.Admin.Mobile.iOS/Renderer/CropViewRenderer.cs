using System;
using System.Diagnostics;
using Foundation;
using Magicodes.Admin.Controls;
using Magicodes.Admin.Renderer;
using UIKit;
using Xam.Plugins.ImageCropper.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CropView), typeof(CropViewRenderer))]
namespace Magicodes.Admin.Renderer
{
    public class CropViewRenderer : PageRenderer
    {
        private CropViewDelegate _selector;
        private byte[] _image;
        private bool _isShown;
        public bool IsCropped;
        private CropView _page;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (!(Element is CropView page))
            {
                return;
            }

            _page = page;
            _image = page.Image;
            IsCropped = page.IsCropped;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            try
            {
                if (_isShown)
                {
                    return;
                }

                _isShown = true;
                var image = new UIImage(NSData.FromArray(_image));
                _image = null;
                _selector = new CropViewDelegate(this, _page);
                var picker = new TOCropViewController(image) { Delegate = _selector };
                PresentViewController(picker, false, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            try
            {
                if (Element is CropView page)
                {
                    page.IsCropped = _selector.IsCropped;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}