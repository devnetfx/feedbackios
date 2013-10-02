using MonoTouch.UIKit;
using System.Drawing;
using FeedbackIOS.Utility;
using System;
using SharableTypes.Model;
using System.IO;
using System.Net;

namespace Feedback
{
	public partial class FeedbackViewController : UIViewController
	{
		QuestionViewController _questionScreen;
		ScannerViewController _scanScreen;

		// Variables for web service response data
		private string _surveyCode;
		private int _questionCount;
		LoadingOverlay _loadPop;

		public FeedbackViewController () : base ("FeedbackViewController", null)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			NavigationItem.Title = "Feedback Survey App";

			StorageHelper.SaveToIsolatedStorage(StorageHelper.SURVEY_NAME, "");
			StorageHelper.SaveToIsolatedStorage(StorageHelper.RESPONSES, "");

			var fromScan = StorageHelper.LoadFromIsolatedStorage(StorageHelper.FROM_SCAN);

			if (fromScan == "Y") {
				_surveyCode = StorageHelper.LoadFromIsolatedStorage(StorageHelper.SURVEY_CODE);
				StorageHelper.SaveToIsolatedStorage(StorageHelper.SUBMISSION_ID, Guid.NewGuid().ToString());

				if (!string.IsNullOrEmpty (_surveyCode)) {
					LoadQuestions();
				}
				StorageHelper.SaveToIsolatedStorage(StorageHelper.FROM_SCAN, "N");
			}
			else
			{
				StorageHelper.SaveToIsolatedStorage(StorageHelper.SURVEY_CODE, "");
			}
		}

		public override void ViewWillDisappear (bool animated)
		{
			NavigationItem.Title = "";
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var stackPanel = new StackPanel (View.Bounds);
			Title = "Feedback Survey App";

			//IndustrialTheme.Apply(this);

			var header = new UILabel (new RectangleF(25,40,200,40)) {
				Text = "Welcome to the Feedback survey application",
				LineBreakMode = UILineBreakMode.WordWrap,
				Lines = 0
			};

			//IndustrialTheme.Apply (header, "aluminum");
			header.Font = UIFont.FromName ("Arial-BoldMT", 18);
		
			stackPanel.AddSubview (header);

			var subHeader = new UILabel (new RectangleF(0,0,200,40)) {
				Text = "Enter your survey code"
			};

			//IndustrialTheme.Apply (subHeader);
			stackPanel.AddSubview (subHeader);

			var surveyCode = new UITextField (new RectangleF (25, 80, 268, 40)) {
				Placeholder = "",
				BorderStyle = UITextBorderStyle.RoundedRect,
				ShouldReturn = field => {
					field.ResignFirstResponder();
					return true;
				}
			};

			//IndustrialTheme.Apply (surveyCode);

			//var paddingView = new UIView(new RectangleF(0, 0, 10, 10));
			//surveyCode.LeftView = paddingView;
			//surveyCode.LeftViewMode = UITextFieldViewMode.WhileEditing;
		
			stackPanel.AddSubview (surveyCode);

			var startSurvey = AddButton(stackPanel, "confirm", "start survey");

			startSurvey.TouchUpInside += (sender, e) => {
				if (string.IsNullOrEmpty(surveyCode.Text))
				{
					InvokeOnMainThread(() =>
					                   {
						var av = new UIAlertView("Validation Message", "Please enter a survey code", null, "OK", null);
						av.Show();
					});	
				}
				else
				{
					_surveyCode = surveyCode.Text;
					StorageHelper.SaveToIsolatedStorage(StorageHelper.SURVEY_CODE, surveyCode.Text);
					StorageHelper.SaveToIsolatedStorage(StorageHelper.SUBMISSION_ID, Guid.NewGuid().ToString());
					LoadQuestions();
				}
			};

			var scanText = new UILabel (new RectangleF(25,40,200,40)) {
				Text = "Have a QR code?",
				LineBreakMode = UILineBreakMode.WordWrap,
				Lines = 0
			};

			//IndustrialTheme.Apply (scanText);
			stackPanel.AddSubview (scanText);
				
			var startScan = AddButton(stackPanel, "confirm", "start scan");

			startScan.TouchUpInside += (sender, e) => {
				_scanScreen = new ScannerViewController(); 
				NavigationController.PushViewController(_scanScreen, true);
			};

			View.AddSubview (stackPanel);
			View.LayoutSubviews ();
			base.EdgesForExtendedLayout = UIRectEdge.None;
		}

		private UIButton AddButton (StackPanel panel, string options, string title)
		{
			var button = new UIButton (new RectangleF(25, 120, 298, 57));
			//IndustrialTheme.Apply (button, options);
			button.BackgroundColor = UIColor.LightGray;
			button.SetTitleColor (UIColor.Black, UIControlState.Normal);
			button.SetTitle (title, UIControlState.Normal);
			panel.AddSubview (button);

			return button;
		}

		#region Webservice calls

		private void ShowLoading()
		{
			InvokeOnMainThread(() =>
			{                   
				_loadPop = new LoadingOverlay(UIScreen.MainScreen.Bounds, "Loading Questions...");
				View.Add(_loadPop);                   
			});
		}

		private void HideLoading()
		{
			InvokeOnMainThread(() =>
			{
				if (_loadPop != null) {
					_loadPop.Hide();
				}
			});
		}

		private void DisplayErrorMessage()
		{
			InvokeOnMainThread(() =>
			{
				var av = new UIAlertView("Error", "Unable to retrieve the survey data.  Please try again", null, "OK", null);
				av.Show();
			});
		}

		private void LoadQuestions()
		{
			var question = new QuestionM { SurveyCode = _surveyCode };
			var requestContent = Newtonsoft.Json.JsonConvert.SerializeObject(question);

			var request = APIHelper.InitializeRequest(UIConstants.GET_QUESTION_CONTAINER);

			if (ConnectionHelper.IsConnected())
            {
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

							string responseContent;
							using (var streamReader = new StreamReader(response.GetResponseStream()))
							{
								responseContent = streamReader.ReadToEnd();
							}

                            var questionContainer = Newtonsoft.Json.JsonConvert.DeserializeObject<QuestionContainerM>(responseContent);

                            if (questionContainer != null && questionContainer.QuestionList != null &&
                                questionContainer.QuestionList.Length > 0)
                            {
                                _questionCount = questionContainer.QuestionList.Length;
                                StorageHelper.SaveToIsolatedStorage(StorageHelper.QUESTIONCONTAINER, responseContent);
                                LoadQuestionDefinitionLines();
                            }
                            else
                            {
                                InvokeOnMainThread(() =>
                                {
                                    if (_loadPop != null)
                                    {
                                        _loadPop.Hide();
                                        _loadPop.RemoveFromSuperview();
                                    }

                                    var av = new UIAlertView("Survey Error", "Unable to locate survey.  Please check your code and try again", null, "OK", null);
                                    av.Show();
                                });
                            }
						}
						catch (Exception ex)
						{
							LittleWatson.ReportException(ex);
							DisplayErrorMessage();
						}
						finally
						{
							HideLoading();
						}
					}, request);
				}, request);
			}
			else
            {
				InvokeOnMainThread(() =>
			    {
					var av = new UIAlertView("Connection Error", "Unable to detect a network connection.  Please try again", null, "OK", null);
					av.Show();
				});	
			}
		}

		private void LoadQuestionDefinitionLines()
		{
			var question = new QuestionM { SurveyCode = _surveyCode };
			var requestContent = Newtonsoft.Json.JsonConvert.SerializeObject(question);

			var request = APIHelper.InitializeRequest(UIConstants.GET_QUESTION_DEFINITION_LINES);

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

						string responseContent;
						using (var streamReader = new StreamReader(response.GetResponseStream()))
						{
							responseContent = streamReader.ReadToEnd();
						}

						InvokeOnMainThread(() =>
						{
							StorageHelper.SaveToIsolatedStorage(StorageHelper.QUESTION_DEFINITION_LINES, responseContent);
							LoadQuestionDefinitionResponses();
						});
					}
					catch (Exception ex)
					{
						HideLoading();
						LittleWatson.ReportException(ex);
						DisplayErrorMessage();
					}

				}, request);
			}, request);
		}

		private void LoadQuestionDefinitionResponses()
		{
			var question = new QuestionM { SurveyCode = _surveyCode };
			var requestContent = Newtonsoft.Json.JsonConvert.SerializeObject(question);

			var request = APIHelper.InitializeRequest(UIConstants.GET_QUESTION_DEFINITION_RESPONSES);

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

						string responseContent;
						using (var streamReader = new StreamReader(response.GetResponseStream()))
						{
							responseContent = streamReader.ReadToEnd();
						}

						InvokeOnMainThread(() =>
						                   {
							StorageHelper.SaveToIsolatedStorage(StorageHelper.QUESTION_DEFINITION_RESPONSES, responseContent);
							LoadQuestionDefinitionHeaders();
						});
					}
					catch (Exception ex)
					{
						HideLoading();
						LittleWatson.ReportException(ex);
						DisplayErrorMessage();
					}
				}, request);
			}, request);
		}

		private void LoadQuestionDefinitionHeaders()
		{
			var question = new QuestionM { SurveyCode = _surveyCode };
			var requestContent = Newtonsoft.Json.JsonConvert.SerializeObject(question);

			var request = APIHelper.InitializeRequest(UIConstants.GET_QUESTION_DEFINITION_HEADERS);

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

						string responseContent;
						using (var streamReader = new StreamReader(response.GetResponseStream()))
						{
							responseContent = streamReader.ReadToEnd();
						}

						InvokeOnMainThread(() =>
						{
							StorageHelper.SaveToIsolatedStorage(StorageHelper.QUESTION_DEFINITION_HEADERS, responseContent);
							LoadQuestionEvents();
						});
					}
					catch (Exception ex)
					{
						HideLoading();
						LittleWatson.ReportException(ex);
						DisplayErrorMessage();
					}
				}, request);
			}, request);
		}

		private void LoadQuestionEvents()
		{
			try
			{
				var question = new QuestionM { SurveyCode = _surveyCode };
				var requestContent = Newtonsoft.Json.JsonConvert.SerializeObject(question);

				var request = APIHelper.InitializeRequest(UIConstants.GET_QUESTION_EVENTS);

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

							string responseContent;
							using (var streamReader = new StreamReader(response.GetResponseStream()))
							{
								responseContent = streamReader.ReadToEnd();
							}

							InvokeOnMainThread(() =>
							                   {
								_questionScreen = new QuestionViewController(); 
								NavigationController.PushViewController(_questionScreen, true);
							});

							StorageHelper.SaveToIsolatedStorage(StorageHelper.QUESTION_EVENTS, responseContent);
						}
						catch (Exception ex)
						{
							HideLoading();
							LittleWatson.ReportException(ex);
							DisplayErrorMessage();
						}
						finally
						{
							HideLoading();
						}
					}, request);
				}, request);
			}
			catch (Exception ex)
			{
				LittleWatson.ReportException(ex);
			}
		}

		#endregion
	}
}

