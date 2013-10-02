using System.Drawing;
using Feedback;
using FeedbackIOS.Utility;
using MonoTouch.UIKit;
using ZXing.Mobile;
using ZXing.MonoTouch.Sample;

namespace Feedback
{
	public class ScannerViewController : UIViewController
	{
        MobileBarcodeScanner _scanner;
		CustomOverlayView _customOverlay;

		public ScannerViewController() : base()
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			NavigationItem.Title = "Scan QR Code";
		}

		public override void ViewWillDisappear (bool animated)
		{
			NavigationItem.Title = "";
		}

		public override void ViewDidLoad ()
		{
			_scanner = new MobileBarcodeScanner(this.NavigationController);
		
			_customOverlay = new CustomOverlayView();

			_customOverlay.ButtonTorch.TouchUpInside += delegate {
				if (UIDevice.CurrentDevice.Model.ToLower().IndexOf("ipad") == -1) {
					_scanner.ToggleTorch();		
				}
			};

			_customOverlay.ButtonCancel.TouchUpInside += delegate {
				_scanner.Cancel();
			};

			_scanner.UseCustomOverlay = true;
			_scanner.CustomOverlay = _customOverlay;

			_scanner.Scan ().ContinueWith((t) => 
			{
				if (t != null && t.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
				{
					HandleScanResult(t.Result);
				}
			});

			base.EdgesForExtendedLayout = UIRectEdge.None;
		}

		void HandleScanResult(ZXing.Result result)
		{
			if (result != null && !string.IsNullOrEmpty(result.Text))
			{
				InvokeOnMainThread (() =>
				{
					StorageHelper.SaveToIsolatedStorage(StorageHelper.FROM_SCAN, "Y");
					StorageHelper.SaveToIsolatedStorage(StorageHelper.SURVEY_CODE, result.Text);
					this.NavigationController.PopToRootViewController(true);
				});
			    
			}
			else
			{
                InvokeOnMainThread(() =>
                {
       				this.NavigationController.PopToRootViewController(true);
	         	});
			}
		}
	}
}

