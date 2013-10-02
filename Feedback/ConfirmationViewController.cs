using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using FeedbackIOS.Utility;

namespace Feedback
{
	public partial class ConfirmationViewController : UIViewController
	{
		StackPanel panel = null;

		public ConfirmationViewController () : base ("ConfirmationViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			panel = new StackPanel (View.Bounds);
			View.AddSubview (panel);

			this.Title = "Confirmation";

			var surveyName = StorageHelper.LoadFromIsolatedStorage(StorageHelper.SURVEY_NAME);

			// 2 Create the controls and add to the panel
			///////////////////////////////////////////
			var header = new UILabel (new RectangleF(15,40,200,40)) {
				Text = surveyName,
				LineBreakMode = UILineBreakMode.WordWrap,
				Lines = 0
			};

			header.Font = UIFont.FromName ("Arial-BoldMT", 18);
	
			panel.AddSubview (header);

			var subHeader = new UILabel (new RectangleF(15,40,200,40)) {
				Text = "Your answers have been submitted.  Thank you for your time!",
				LineBreakMode = UILineBreakMode.WordWrap,
				Lines = 0
			};

			//IndustrialTheme.Apply (subHeader);
			panel.AddSubview (subHeader);

			var startScan = UIHelper.AddButton(panel, "confirm", "start another survey");

			startScan.TouchUpInside += (sender, e) => {
				StorageHelper.SaveToIsolatedStorage(StorageHelper.SURVEY_NAME, "");
				StorageHelper.SaveToIsolatedStorage(StorageHelper.SURVEY_CODE, "");

				this.NavigationController.PopToRootViewController(true);
			};

			View.AddSubview (panel);
			base.EdgesForExtendedLayout = UIRectEdge.None;
		}
	}
}

