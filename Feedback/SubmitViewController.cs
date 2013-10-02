using System;
using System.Drawing;
using System.IO;
using System.Net;
using FeedbackIOS.Utility;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Feedback
{
	public partial class SubmitViewController : UIViewController
	{
		ConfirmationViewController _confirmationScreen;
        QuestionViewController _questionScreen;
        LoadingOverlay _loadPop = null;
		StackPanel panel = null;

		public SubmitViewController () : base ("SubmitViewController", null)
		{	
		}

		public override void ViewWillAppear (bool animated)
		{
			NavigationItem.Title = "Submit";
		}

		public override void ViewWillDisappear (bool animated)
		{
			NavigationItem.Title = "";
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
//			View.AddSubview (panel);
			//IndustrialTheme.Apply(this);

			var surveyName = StorageHelper.LoadFromIsolatedStorage(StorageHelper.SURVEY_NAME);

			// 2 Create the controls and add to the panel
			///////////////////////////////////////////
			var labelSurveyName = UIHelper.AddLabelToPanel(panel, surveyName);
			labelSurveyName.Font = UIFont.FromName ("Arial-BoldMT", 18);
			labelSurveyName.TextColor = UIColor.White;

			var textLabel = UIHelper.AddLabelToPanel(panel, "To save your responses click submit");

			var saveSurvey = UIHelper.AddButton(panel, "confirm", "submit");

			saveSurvey.TouchUpInside += (sender, e) => {
				SaveResponses ();
			};
			View.AddSubview (panel);
		
			base.EdgesForExtendedLayout = UIRectEdge.None;
		}

        private void SaveResponses()
        {
            var requestContent = StorageHelper.LoadFromIsolatedStorage(StorageHelper.RESPONSES);
            var request = APIHelper.InitializeRequest(UIConstants.SAVE_QUESTION_RESPONSES);
            ShowLoading();

            request.BeginGetRequestStream(result =>
            {
                var req = ((HttpWebRequest)result.AsyncState).EndGetRequestStream(result);

                using (var streamWriter = new StreamWriter(req))
                {
                    streamWriter.Write(requestContent);
                }

                request.BeginGetResponse(responseResult =>
                {
                    try
                    {
                        var response = (HttpWebResponse)((HttpWebRequest)result.AsyncState).EndGetResponse(responseResult);

						InvokeOnMainThread(() =>
						{
							GoToConfirmation();
						});
                    }
                    catch (Exception ex)
                    {
                        LittleWatson.ReportException(ex);
                    }
                    finally
                    {
                        HideLoading();               
                    }
                }, request);
            }, request);
        }

        private void ReturnToSurvey()
        {
		    if (_questionScreen == null)
            {
                _questionScreen = new QuestionViewController();
            }
            NavigationController.PushViewController(_questionScreen, true);
        }

		private void GoToConfirmation()
		{
			StorageHelper.SaveToIsolatedStorage(StorageHelper.RESPONSES, "");

			if (_confirmationScreen == null)
			{
				_confirmationScreen = new ConfirmationViewController();
			}
			NavigationController.PushViewController(_confirmationScreen, true);	                   

		}

        private void ShowLoading()
        {
            _loadPop = new LoadingOverlay(UIScreen.MainScreen.Bounds, "Saving Responses...");
            View.Add(_loadPop);
        }

		private void HideLoading()
		{
			InvokeOnMainThread(() =>
			                   {
				if (_loadPop != null) _loadPop.Hide();
			});
		}
	}
}

