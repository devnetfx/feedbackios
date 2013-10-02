using System;
using System.Collections.Generic;
using System.Linq;
using FeedbackIOS.Utility;
using MonoTouch.UIKit;
using SharableTypes.Helper;
using SharableTypes.Model;
using System.Globalization;
using Feedback.Class;
using System.Drawing;

namespace Feedback
{
	public partial class QuestionViewController : UIViewController
	{
		SubmitViewController _submitScreen;
		//UIButton submitButton;
		//UIPickerView _picker;
		//PickerModel _pickerModel;
		//LoadingOverlay _loadPop = null;

		//private string _surveyCode;
		//private bool _pageInitialized;
		private bool _finished;
		private string _appIdentifier = string.Empty;
		private int _questionCount = 1;

		// Position / State Variables
		private int _currentQuestionIndex = 1;
		private QuestionM _currentQuestion;
		private int _currentDefinitionLineIndex;
		private int _currentDefinitionLineCount;
		private int _currentDefinitionHeaderIndex;
		private int _currentDefinitionHeaderCount;

		// Context question data containers
		private IEnumerable<QuestionDefinitionResponseM> _definitionResponses;
		private IEnumerable<QuestionDefinitionLineM> _definitionLines;
		private IEnumerable<QuestionDefinitionHeaderM> _definitionHeaders;

		// Variables for web service response data
	    private QuestionContainerM _questionContainer;
		private QuestionM[] _questions;
        private QuestionSetM _questionSet;
		private QuestionDefinitionLineM[] _questionDefinitionLines;
		private QuestionDefinitionResponseM[] _questionDefinitionResponses;
		private QuestionDefinitionHeaderM[] _questionDefinitionHeaders;
		private QuestionEventM[] _questionEvents;
		private QuestionResponseM[] _questionResponseList;

		//private UITextField textResponse;
		private UITextField textAdditionalResponse;
		private UITextField textResponseAdditionalResponse;
		private UITextField referralCodeField;
		private UITableView pickerMultiResponseView;

		private UILabel labelSectionName;
		private UILabel labelQuestionLine;
		private UILabel labelHeader;
		private UILabel labelQuestionName;
		private UILabel labelSurveyName;
		private UILabel labelAdditionalResponse;
		private UILabel labelResponseAdditionalResponse;

		//UITableView table;
		//List<TableItem> _optionList = new List<TableItem>();
		StackPanel _panel;
		UIScrollView _scrollView;
		bool _questionChangeInProgress;
        private UIAlertView _av;

        private bool _consentGranted;

		private string _referralCode = string.Empty;
		private string _trackingCode = string.Empty;
		private bool _referralCodeViewed;

        private ContextCollection _contextCollection;

        private void StartSurveyAfterLoad()
        {
            if (_questionSet.RequireConsent && !_consentGranted)
            {
                RenderConsentForm();
            }
            else
            {
                InitMenu();
                LoadQuestion();
            }
        }

        private void RenderConsentForm()
        {
            if (_panel != null)
            {
                _panel.RemoveFromSuperview();
            }

            _panel = new StackPanel(new RectangleF(0, 0, View.Frame.Width, View.Frame.Height * 6.0f));

            NavigationItem.Title = "Consent";

            var surveyName = GetSurveyName();

            labelSurveyName = UIHelper.AddLabelToPanel(_panel, surveyName);
            labelSurveyName.Font = UIFont.FromName("Arial-BoldMT", 20);
            //labelSurveyName.TextColor = UIColor.White;

			if ((_questionSet.RequireConsent && !_consentGranted) ||
			    _questionSet.ShowReferralCode && !_referralCodeViewed)
			{
				if (_questionSet.RequireConsent)
				{
					UIHelper.AddLabelToPanel(_panel, _questionSet.ConsentMessage);

					//////////////////////////////
					// Yes / No
					//////////////////////////////
					var responses = new List<QuestionDefinitionResponseM>
					{
						new QuestionDefinitionResponseM
						{
							Id = 1,
							Text = "Yes"
						},
						new QuestionDefinitionResponseM
						{
							Id = 2,
							Text = "No"
						}
					};

					var _pickerModel = new PickerModel(responses, 2, 1);
					_pickerModel.PickerChanged += (sender, e) =>
					{
						var pickerArgs = e as PickerChangedEventArgs;

						if (pickerArgs != null)
						{
							_consentGranted = pickerArgs.SelectedValue.Id == 1;
						}
					};

					var _picker = new UIPickerView(new Rectangle(0,0,320, 120)) {ShowSelectionIndicator = true, Model = _pickerModel, Hidden = false};
					_picker.Select(1, 0, true);
					//IndustrialTheme.Apply(_picker);
					_panel.AddSubview(_picker);
				}

				if (_questionSet.ShowReferralCode)
				{
					UIHelper.AddLabelToPanel(_panel, _questionSet.ReferralCodeLabel.Replace("&nbsp;", " "));
					referralCodeField = UIHelper.AddNewTextBoxToPanel (_panel, string.Empty, -1);
					_referralCodeViewed = false;
				}
			}

            var btnConsent = UIHelper.AddButton(_panel, "confirm", "proceed to survey");

            btnConsent.TouchUpInside += (sender, e) =>
            {
				if (_questionSet.RequireConsent && _consentGranted ||
				    (!_questionSet.RequireConsent && _questionSet.ShowReferralCode))
				{
					_referralCodeViewed = true;
					_referralCode = referralCodeField.Text;

					StartSurveyAfterLoad();
				}
				else
				{
					InvokeOnMainThread(() =>
					                   {
						_av = new UIAlertView("Consent Required", "Your consent is required to complete the survey.", null, "OK", null);
						_av.Show();
					});
				}
            };

			UIHelper.AddLabelToPanel(_panel, _questionSet.Description);

			if (_questionSet.ShowFooter)
			{
				UIHelper.AddLabelToPanel(_panel, " ");
				UIHelper.AddLabelToPanel(_panel, _questionSet.FooterMessage);
			}

			_scrollView.ContentSize = new SizeF (_panel.Bounds.Width, View.Frame.Height * 6.0f);
			_scrollView.AddSubview (_panel);
			View.AddSubview(_scrollView);
			//IndustrialTheme.Apply (this);

        }

        private string GetSurveyName()
        {
            var surveyName = StorageHelper.LoadFromIsolatedStorage(StorageHelper.SURVEY_NAME);

            if (string.IsNullOrEmpty(surveyName))
            {
                surveyName = _questionSet.Name;
                StorageHelper.SaveToIsolatedStorage(StorageHelper.SURVEY_NAME, surveyName);
            }

            return surveyName;
        }

        public QuestionViewController () : base ("QuestionViewController", null)
		{
		
        }

		public override void ViewWillAppear (bool animated)
		{
			NavigationItem.Title = "Questions";

			_questionResponseList = StorageHelper.LoadFromIsolatedStorage<QuestionResponseM[]>(StorageHelper.RESPONSES);
			_currentQuestionIndex = 1;
			_finished = false;

			StartSurveyAfterLoad();
		}

		public override void ViewWillDisappear (bool animated)
		{
			NavigationItem.Title = "Back";
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = "Questions";

			SetToolbarItems(new [] {
				new UIBarButtonItem(UIBarButtonSystemItem.Rewind, (s,e) => appBar_OnBack())
				,new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) { Width = 100 }
				, new UIBarButtonItem(UIBarButtonSystemItem.Cancel, (s,e) => appBar_OnCancel())
				,new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) { Width = 100 }
			  , new UIBarButtonItem(UIBarButtonSystemItem.FastForward, (s,e) => appBar_OnForward())
			}, false);

			NavigationController.ToolbarHidden = false;

            _contextCollection = new ContextCollection();

			_scrollView = new UIScrollView (
				new RectangleF (0, 0, View.Frame.Width
			                , View.Frame.Height - NavigationController.NavigationBar.Frame.Height));
			View.AddSubview (_scrollView);

			//IndustrialTheme.Apply(this);

			_appIdentifier = StorageHelper.LoadFromIsolatedStorage (StorageHelper.SUBMISSION_ID);

			//_surveyCode = StorageHelper.LoadFromIsolatedStorage(StorageHelper.SURVEY_CODE);

            _questionContainer = StorageHelper.LoadFromIsolatedStorage<QuestionContainerM>(StorageHelper.QUESTIONCONTAINER);
		    _questionSet = _questionContainer.QuestionSet;
            _questions = _questionContainer.QuestionList;
			_questionCount = _questions.Length;
			_questionDefinitionLines = StorageHelper.LoadFromIsolatedStorage<QuestionDefinitionLineM[]> (StorageHelper.QUESTION_DEFINITION_LINES);
			_questionDefinitionResponses = StorageHelper.LoadFromIsolatedStorage<QuestionDefinitionResponseM[]> (StorageHelper.QUESTION_DEFINITION_RESPONSES);
			_questionDefinitionHeaders = StorageHelper.LoadFromIsolatedStorage<QuestionDefinitionHeaderM[]> (StorageHelper.QUESTION_DEFINITION_HEADERS);
			_questionEvents = StorageHelper.LoadFromIsolatedStorage<QuestionEventM[]> (StorageHelper.QUESTION_EVENTS);

			//LoadQuestion ();
			base.EdgesForExtendedLayout = UIRectEdge.None;

		    StartSurveyAfterLoad();
		}

		#region General Navigation

		private void appBar_OnBack()
		{
			try
			{
				if (!_questionChangeInProgress) {
					_questionChangeInProgress = true;
					if (UpdateResponse(-1))
					{
						Move(-1);
					}
				}
				_questionChangeInProgress = false;
			}
			catch(Exception ex) {
				InvokeOnMainThread(() =>
				                   {
					_av = new UIAlertView("Application Error", ex.Message, null, "OK", null);
					_av.Show();
				});
			}
		}
				
		private void appBar_OnCancel()
		{
			InvokeOnMainThread(() =>
			                   {
				_av = new UIAlertView("Exit Survey", "Are you sure you want to quit?", null, "Yes", null);
				_av.AddButton("No");
				_av.Clicked += (sender, e) => 
				{
				    if (e.ButtonIndex != 0) return;
				    NavigationController.ToolbarHidden = true;
				    NavigationController.PopToRootViewController(true);
				};

				_av.Show();
			});
		}

		private void appBar_OnForward()
		{
			try
			{
				if (!_questionChangeInProgress) {
					_questionChangeInProgress = true;
					if (UpdateResponse(1))
					{
						Move(1);
					}
				}
				_questionChangeInProgress = false;
			}
			catch(Exception ex) {
				InvokeOnMainThread(() =>
				                   {
					var av5 = new UIAlertView("Application Error", ex.Message, null, "OK", null);
					av5.Show();
				});
			}
		}

		#endregion

		#region Methods

		private void InitMenu()
		{
			NavigationController.ToolbarHidden = false;
		}

		private bool UpdateResponse(int increment)
		{
			var question = _questions[_currentQuestionIndex - 1];
			//var currentLine = CurrentLine(_currentDefinitionLineIndex);
			var currentHeader = CurrentHeader(_currentDefinitionHeaderIndex);

			long questionDefinitionLineId = 0;
			long questionDefinitionHeaderId = -1;
			long questionDefinitionResponseId = 0;

			var questionResponseCaptured = false;

			/*
			if (currentLine != null)
			{
				questionDefinitionLineId = currentLine.Id;
			}
			*/

			if (currentHeader != null)
			{
				questionDefinitionHeaderId = currentHeader.Id;
			}

			string response = string.Empty;

			if (question.InputType == "RADIO")
			{
				foreach (var pi in _panel.Subviews)
				{
					if (pi is UIPickerView)
					{
						var pickerView = pi as UIPickerView;
						var model = pickerView.Model as PickerModel;
						questionDefinitionResponseId = model.SelectedId;
						var tagId = Convert.ToInt32(pi.Tag.ToString());
						var context = _contextCollection.GetFromCollection(tagId);

						if (context != null)
						{
							questionDefinitionLineId = context.LineId;
							SaveResponseToCollection (questionDefinitionLineId, questionDefinitionResponseId, response, null, questionDefinitionHeaderId);
							questionResponseCaptured = true;
						}
					}
				}

				if (textResponseAdditionalResponse != null)
				{
					response = textResponseAdditionalResponse.Text;
					var addrContext = _contextCollection.GetFromCollection(Convert.ToInt32(textResponseAdditionalResponse.Tag));

					if (!String.IsNullOrEmpty(response))
					{
						SaveAdditionalResponseToCollection (question.Id, response, -1, addrContext.ResponseId);
					}
				}
			}

			if (question.InputType == "SELECT")
			{
				foreach (var pi in _panel.Subviews)
				{
					if (pi is UIPickerView)
					{
						var pickerView = pi as UIPickerView;
						var model = pickerView.Model as PickerModel;
						questionDefinitionResponseId = model.SelectedId;	
						var tagId = Convert.ToInt32(pi.Tag.ToString());
						var context = _contextCollection.GetFromCollection(tagId);

						if (context != null)
						{
							questionDefinitionLineId = context.LineId;
							SaveResponseToCollection (questionDefinitionLineId, questionDefinitionResponseId, response, null, questionDefinitionHeaderId);
							questionResponseCaptured = true;
						}
					}
				}

				if (textResponseAdditionalResponse != null && textResponseAdditionalResponse.Tag.ToString(CultureInfo.InvariantCulture) == "radr")
				{
					response = textResponseAdditionalResponse.Text;
					var addrContext = _contextCollection.GetFromCollection(Convert.ToInt32(textResponseAdditionalResponse.Tag));

					if (!String.IsNullOrEmpty(response))
					{
						SaveAdditionalResponseToCollection (question.Id, response, -1, addrContext.ResponseId);
					}
				}
			}

			if (question.InputType == "CHECKBOX")
			{
				foreach(var tv in _panel.Subviews)
				{
					if (tv is UITableView)
					{
						var tableView = tv as UITableView;
						var checkedItems = new List<long>();

						var tagId = Convert.ToInt32(tableView.Tag.ToString());
						var context = _contextCollection.GetFromCollection(tagId);

						if (context != null)
						{
							questionDefinitionLineId = context.LineId;
						}

						response = string.Empty;

						var selectedOptions = (tableView.Source as TableSource).SelectedOptions();

						for (var i = 0; i < selectedOptions.Count(); i++)
						{
							questionDefinitionResponseId = selectedOptions.ElementAt(i).ResponseId;
							checkedItems.Add(questionDefinitionResponseId);
						}

						SaveResponseToCollection(questionDefinitionLineId, -1, response, checkedItems, -1);

						questionResponseCaptured = true;
					}
				}
			}

			if (question.InputType == "TEXT" || question.InputType == "TEXTAREA")
			{
				foreach(var t in _panel.Subviews)
				{
					if (t is UITextField)
					{
						questionDefinitionResponseId = 0;
						response = (t as UITextField).Text;

						questionDefinitionLineId = Convert.ToInt32(t.Tag.ToString());
		
						if (!String.IsNullOrEmpty(response))
						{
							SaveResponseToCollection(questionDefinitionLineId, questionDefinitionResponseId, response, null, -1);
							questionResponseCaptured = true;
						}
					}
				}


			}

			if (question.HasAdditionalResponse && textAdditionalResponse != null)
			{
				response = textAdditionalResponse.Text;
				if (!String.IsNullOrEmpty(response))
				{
					SaveAdditionalResponseToCollection (question.Id, response, -1, -1);
					questionResponseCaptured = true;
				}
			}

			if (!questionResponseCaptured && increment == 1)
			{
				if (question.Mandatory)
				{
					var message = question.MandatoryMessage ?? "Please enter a response for the question";

					InvokeOnMainThread(() =>
					                   {
						var av2 = new UIAlertView("Validation Message", message, null, "OK", null);
						av2.Show();
					});

					return false;
				}
			}

			if (!String.IsNullOrEmpty(question.InputFormat) && question.InputFormat != "TEXT" && increment == 1)
			{
				{
					if (question.InputType == "TEXT" && question.InputFormat == "NUMBER")
					{
						decimal number;
						if (!decimal.TryParse(response, out number))
						{
							InvokeOnMainThread(() =>
							                   {
								var av4 = new UIAlertView("Validation Message", "Your answer must be a number", null, "OK", null);
								av4.Show();
							});
							return false;
						}
					}

					if (question.InputType == "TEXT" && question.InputFormat == "EMAIL")
					{

						if (response.IndexOf("@", System.StringComparison.Ordinal) == -1 ||
						    response.IndexOf(".", System.StringComparison.Ordinal) == -1)
						{
							InvokeOnMainThread(() =>
							                   {
								var av3 = new UIAlertView("Validation Message", "Please enter a valid email address", null, "OK", null);
								av3.Show();
							});
							return false;
						}
					}
				}
			}
			return true;
		}

		private void Move(int increment)
		{
			var incrementQuestion = true;
			var incrementDefinitionLine = true;

			if (_currentQuestion != null && _currentQuestion.Multiline == "Y")
			{
				// mulitple header line logic
				if (_currentDefinitionHeaderCount > 0)
				{
					if (_currentDefinitionHeaderIndex + increment < 1)
					{
						if (_currentDefinitionLineIndex > 1)
						{
							_currentDefinitionHeaderIndex = _currentDefinitionHeaderCount;
						}
					}
					else if (_currentDefinitionHeaderIndex + increment > _currentDefinitionHeaderCount)
					{
						// reset the header back to the first one
						_currentDefinitionHeaderIndex = 1;
					}
					else
					{
						incrementQuestion = false;
						_currentDefinitionHeaderIndex = _currentDefinitionHeaderIndex + increment;
					}
				}
			}

			if (incrementQuestion)
			{
				if (_currentQuestionIndex + increment < 1)
				{
					_currentQuestionIndex = 1;
				}
				else if (_currentQuestionIndex + increment > _questionCount)
				{
					_finished = true;
					_currentQuestionIndex = _questionCount;
				}
				else
				{
					_currentQuestionIndex = _currentQuestionIndex + increment;
				}

				if (_finished)
				{
					// Save questions to storage
					var responseContent = Newtonsoft.Json.JsonConvert.SerializeObject(_questionResponseList);
					StorageHelper.SaveToIsolatedStorage(StorageHelper.RESPONSES, responseContent);

					if (_submitScreen == null)
					{
						_submitScreen = new SubmitViewController();
					}
					NavigationController.PushViewController(_submitScreen, true);
				}
				else
				{
					LoadQuestion(increment);
				}
			}
			else
			{
				RenderQuestionControls();
			}
		}

		private void RenderQuestionControls()
		{
			InitMenu();
			if (_panel != null)
			{
				_panel.RemoveFromSuperview ();
			}

			_panel = new StackPanel (new RectangleF (0, 0, View.Frame.Width, _currentQuestion.Multiline == "N" ? View.Frame.Height * 2.0f : View.Frame.Height * 6.0f));

			var headerTitle = string.Empty;
			var questionTitleText = string.Empty;
			var questionLineText = string.Empty;

			var sectionName = string.Empty;

			if (!String.IsNullOrEmpty(_currentQuestion.SectionName))
			{
				sectionName = "> " + _currentQuestion.SectionName;
			}

			// 2 Create the controls and add to the panel
			///////////////////////////////////////////
			var surveyName = GetSurveyName();
			var headerText = string.Empty;

			labelSurveyName = UIHelper.AddLabelToPanel(_panel, surveyName);
			labelSurveyName.Font = UIFont.FromName("Arial-BoldMT", 20);
			//labelSurveyName.TextColor = UIColor.White;

			if (!String.IsNullOrEmpty(sectionName))
			{
				labelSectionName = UIHelper.AddLabelToPanel(_panel, sectionName);
				labelSectionName.Font = UIFont.FromName("Arial-BoldMT", 16);
				//labelSectionName.TextColor = UIColor.White;
			}

			if (_currentQuestion.Multiline == "Y")
			{
				if (_definitionHeaders.Any())
				{
					headerTitle = "." + _currentDefinitionHeaderIndex.ToString(CultureInfo.InvariantCulture);
				
					var header = _definitionHeaders.ToList().ElementAt(_currentDefinitionHeaderIndex - 1);
					if (header.Text != "[DEFAULT]")
					{
						headerText = "> " + header.Text;
					}
				}
				questionTitleText = _currentQuestionIndex.ToString(CultureInfo.InvariantCulture) + headerTitle + " " + _currentQuestion.Name;
			}
			else
			{
				questionTitleText = _currentQuestionIndex.ToString(CultureInfo.InvariantCulture) + ". " + _currentQuestion.Name;
			}

			labelQuestionName = UIHelper.AddLabelToPanel(_panel, questionTitleText);

			if (!string.IsNullOrEmpty(headerText))
			{
				labelHeader = UIHelper.AddLabelToPanel(_panel, headerText);
				labelHeader.Font = UIFont.FromName("Arial-BoldMT", 16);

			}

		    foreach (var line in _definitionLines)
		    {
                if (_currentQuestion.Multiline == "Y")
                {
                    questionLineText = line.Text;
                }
           
				if (!string.IsNullOrEmpty(questionLineText) && questionLineText != "[DEFAULT]")
                {
                    labelQuestionLine = UIHelper.AddLabelToPanel(_panel, questionLineText);
                    labelQuestionLine.TextColor = UIColor.Blue;
                }

                // 3 Configure the question layout
                if (_currentQuestion.InputType == "RADIO" || _currentQuestion.InputType == "SELECT")
                {
                    var contextResponses = new List<QuestionDefinitionResponseM>();
                    for (var k = 0; k < _definitionResponses.ToList().Count(); k++)
                    {
                        if (_definitionResponses.ElementAt(k).QuestionId == _currentQuestion.Id)
                        {
                            contextResponses.Add(_definitionResponses.ElementAt(k));
                        }
                    }

                    long checkedId = -1;
                    long headerId = -1;

                    if (_currentQuestion.InputType == "SELECT")
                    {
                        var currentHeader = CurrentHeader(_currentDefinitionHeaderIndex);
                        headerId = currentHeader.Id;
                    }

                    var previousAnswer = GetAnswerFromCollection(line.Id, headerId);
                    if (previousAnswer != null)
                        checkedId = previousAnswer.QuestionDefinitionResponseId;

                    var responseIndex = 0;

                    if (checkedId > -1)
                    {
                        if (contextResponses.Any())
                        {
                            var def = contextResponses.FirstOrDefault(c => c.Id == checkedId);

                            if (def != null)
                            {
                                responseIndex = contextResponses.ToList().IndexOf(def);
                            }
                        }
                        ItemSelected(checkedId);
                    }

                    var _pickerModel = new PickerModel(contextResponses, checkedId, responseIndex);
                    _pickerModel.PickerChanged += (object sender, EventArgs e) =>
                    {
                        var pickerArgs = e as PickerChangedEventArgs;

                        if (pickerArgs != null)
                        {
                            ItemSelected(pickerArgs.SelectedValue.Id);
                        }
                    };

					var tagId = _contextCollection.AddToCollection(new QuestionContextM
					                                               {
						HeaderId = _currentDefinitionHeaderIndex,
						LineId = line.Id,
						QuestionId = _currentQuestion.Id,
						ResponseId = -1
					});

					var _picker = new UIPickerView(new Rectangle(0,0,320, 160));
                    _picker.ShowSelectionIndicator = true;
                    _picker.Model = _pickerModel;
                    _picker.Hidden = false;
                    _picker.Select(responseIndex, 0, true);
					_picker.Tag = tagId;
				
                    //IndustrialTheme.Apply(_picker);

					_panel.AddSubview(_picker);

                    foreach (var questionDefinitionResponseM in contextResponses.OrderBy(c => c.SequenceNumber))
                    {
                        if (questionDefinitionResponseM.HasAdditionalResponse)
                        {
                            var previousRespAnswer = GetResponseAnswerFromCollection(_currentQuestion.Id, questionDefinitionResponseM.Id);
                            var previousResponseText = previousRespAnswer != null ? previousRespAnswer.AdditionalResponse : string.Empty;

                            labelResponseAdditionalResponse = UIHelper.AddLabelToPanel(_panel, questionDefinitionResponseM.AdditionalResponseTitle);
					
							var addrTagId = _contextCollection.AddToCollection(new QuestionContextM
							                                               {
								HeaderId = _currentDefinitionHeaderIndex,
								LineId = line.Id,
								QuestionId = _currentQuestion.Id,
								ResponseId = questionDefinitionResponseM.Id,

							});

							textResponseAdditionalResponse = UIHelper.AddNewTextBoxToPanel(_panel, previousResponseText, addrTagId);
                    
							break;
                        }
                    }
                }

                if (_currentQuestion.InputType == "CHECKBOX")
                {
                    var previousAnswer = GetAnswerFromCollection(line.Id, -1);

                    //////////////////////////////
                    // Table View Definition 
                    //////////////////////////////
					var responseCount = _definitionResponses.Count ();

					var height = responseCount * 50;
                    pickerMultiResponseView = new UITableView(new RectangleF(0, 0, 320, height), UITableViewStyle.Grouped);
                    pickerMultiResponseView.AutoresizingMask = UIViewAutoresizing.All;
                    pickerMultiResponseView.AllowsMultipleSelection = true;

					var tagId = _contextCollection.AddToCollection(new QuestionContextM
					                                                                             {
						HeaderId = _currentDefinitionHeaderIndex,
						LineId = line.Id,
						QuestionId = _currentQuestion.Id,
						ResponseId = -1
					});

					pickerMultiResponseView.Tag = tagId;

					var _optionList = new List<TableItem>();
					CreateTableItems(_definitionResponses.ToList(), "header", "subheader", previousAnswer != null ? previousAnswer.ResponseCollection : null, _optionList);

                    //////////////////////////////
                    // Table Source Definition
                    //////////////////////////////
                    pickerMultiResponseView.Source = new TableSource(_optionList);

                    //IndustrialTheme.Apply(pickerMultiResponseView);
                    _panel.AddSubview(pickerMultiResponseView);
                }

                if (_currentQuestion.InputType == "TEXT")
                {
                    var responseText = string.Empty;
                    var previousAnswer = GetAnswerFromCollection(line.Id, -1);
                    if (previousAnswer != null)
                        responseText = previousAnswer.Response;

		            UIHelper.AddNewTextBoxToPanel(_panel, responseText, line.Id);
                }

                if (_currentQuestion.InputType == "TEXTAREA")
                {
                    var responseText = string.Empty;
                    var previousAnswer = GetAnswerFromCollection(line.Id, -1);
                    if (previousAnswer != null)
                        responseText = previousAnswer.Response;

                    UIHelper.AddNewTextBoxToPanel(_panel, responseText, line.Id);
                }

				if (_currentQuestion.Multiline == "Y") {
					UIHelper.AddLabelToPanel(_panel, " ");
				}
		    }

			if (_currentQuestion.HasAdditionalResponse)
			{
				var show = _currentQuestion.Multiline == "N";
				if (_currentQuestion.Multiline == "Y"
				    && _currentDefinitionLineCount == _currentDefinitionLineIndex
				    && _currentDefinitionHeaderIndex == _currentDefinitionHeaderCount)
				{
					show = true;
				}

				if (show)
				{
					var previousRespAnswer = GetResponseAnswerFromCollection(_currentQuestion.Id, -1);
					var previousResponseText = previousRespAnswer != null ? previousRespAnswer.AdditionalResponse : string.Empty;

					labelAdditionalResponse = UIHelper.AddLabelToPanel (_panel, _currentQuestion.AdditionalResponseTitle);
					textAdditionalResponse = UIHelper.AddNewTextBoxToPanel (_panel, previousResponseText, -1);
				}
			}

            if (_questionSet.ShowFooter)
            {
				UIHelper.AddLabelToPanel(_panel, " ");
                UIHelper.AddLabelToPanel(_panel, _questionSet.FooterMessage);
            }

			_scrollView.ContentSize = new SizeF (_panel.Bounds.Width, _panel.Bounds.Height);
			_scrollView.AddSubview (_panel);
			View.AddSubview(_scrollView);
			//IndustrialTheme.Apply (this);
			_scrollView.ScrollsToTop = true;
			_scrollView.ShowsVerticalScrollIndicator = true;
			_scrollView.SetContentOffset (new PointF (0, 0), true);
		}

		protected void CreateTableItems (IEnumerable<QuestionDefinitionResponseM> list, string header, string subHeader, long[] responseCollection, List<TableItem> optionList)
		{
			long checkId;
			foreach (var response in list) {
				checkId = -1;
				if (responseCollection != null) {
					for (var z = 0; z < responseCollection.Count(); z++) {
						if (responseCollection [z] == response.Id) {
							checkId = response.Id;
						}
					}
				}
				optionList.Add (new TableItem(response.Text) 
				                {
					SubHeading = "default",
					IsChecked = response.Id == checkId,
					ResponseId = response.Id
				} );
			}
		}

		private void LoadQuestion(int increment = 0)
		{
			InitMenu();

			var question = _questions[_currentQuestionIndex - 1];
			_currentQuestion = question;

			_definitionResponses = _questionDefinitionResponses.ToList()
				.Where(c => c.QuestionDefinitionId == question.QuestionDefinitionId);

			_definitionLines = _questionDefinitionLines.ToList()
				.Where(c => c.QuestionDefinitionId == question.QuestionDefinitionId);

			_definitionHeaders = _questionDefinitionHeaders.ToList()
				.Where(c => c.QuestionDefinitionId == question.QuestionDefinitionId);

			if (_currentQuestion.Multiline == "Y")
			{
				_currentDefinitionLineIndex = 1;
				var questionDefinitionLineMs = _definitionLines as IList<QuestionDefinitionLineM> ?? _definitionLines.ToList();
				_currentDefinitionLineCount = questionDefinitionLineMs.Any() ? questionDefinitionLineMs.Count() : 0;

				var questionDefinitionHeaderMs = _definitionHeaders as IList<QuestionDefinitionHeaderM> ?? _definitionHeaders.ToList();
				_currentDefinitionHeaderIndex = questionDefinitionHeaderMs.Any() ? 1 : -1;
				_currentDefinitionHeaderCount = questionDefinitionHeaderMs.Any() ? questionDefinitionHeaderMs.Count() : 0;
			}
			else
			{
				_currentDefinitionLineIndex = 1;
				_currentDefinitionLineCount = 1;

				_currentDefinitionHeaderIndex = 1;
				_currentDefinitionHeaderCount = 1;
			}

			if (!_currentQuestion.VisibleByDefault && _currentQuestionIndex != 1)
			{
				_currentDefinitionHeaderIndex = 0;
				Move(increment);
				return;
			}
			else
			{
				RenderQuestionControls();
			}
		}

		private QuestionM CurrentQuestion(int index)
		{
			return _questions.ToList().ElementAt(index - 1);
		}

		private QuestionDefinitionLineM CurrentLine(int index)
		{
			return _definitionLines.ToList().ElementAt(index - 1);
		}

		private QuestionDefinitionHeaderM CurrentHeader(int index)
		{
			return _definitionHeaders != null && _definitionHeaders.Any() ? _definitionHeaders.ToList().ElementAt(index - 1) : null;
		}

		#endregion

		#region Utility Methods

		private void ItemSelected(long responseId)
		{
			if (_questionEvents.Any())
			{
				var contextEvents = _questionEvents.Where (t => t.EntityId == responseId);

				if (contextEvents.Any())

				foreach (var t in contextEvents)
				{
					ProcessEventAction(t);
				}
			}

		}

		private void ProcessEventAction(QuestionEventM action)
		{
			for (int i = 0; i < _questions.Count(); i++) {
				if (_questions[i].Id == action.TargetEntityId) {
					_questions[i].VisibleByDefault = action.ActionType == "SHOW";
				}
			}
		}

		private QuestionResponseM GetAnswerFromCollection(long lineId, long headerId)
		{
			var responseList = new List<QuestionResponseM>();
			if (_questionResponseList != null && _questionResponseList[0] != null)
			{
				responseList.AddRange(_questionResponseList);
			}

			QuestionResponseM responseM = null;
			if (responseList.Any())
			{
				responseM = responseList.FirstOrDefault(c => c.QuestionDefinitionLineId == lineId && c.QuestionDefinitionHeaderId == headerId); 
			}

			return responseM;
		}

		private QuestionResponseM GetResponseAnswerFromCollection(long questionId, long responseId)
		{
			var responseList = new List<QuestionResponseM>();
			if (_questionResponseList != null && _questionResponseList[0] != null)
			{
				responseList.AddRange(_questionResponseList);
			}

			return responseList.SingleOrDefault(c => c.QuestionId == questionId && c.QuestionDefinitionLineId == -1 && c.QuestionDefinitionResponseId == responseId);
		}

		private void SaveResponseToCollection(long lineId, long responseId, string response, List<long> responseCollection, long headerId)
		{
			var question = _questions[_currentQuestionIndex - 1];
			var responseList = new List<QuestionResponseM>();
			if (_questionResponseList != null && _questionResponseList[0] != null)
			{
				responseList.AddRange(_questionResponseList);
			}

			var item = responseList.SingleOrDefault(c => c.QuestionDefinitionLineId == lineId && c.QuestionDefinitionHeaderId == headerId);

			if (item != null)
			{
				item.QuestionDefinitionResponseId = responseId;
				item.Response = response;
				if (responseCollection != null) item.ResponseCollection = responseCollection.ToArray();
			}
			else
			{
				item = new QuestionResponseM
				{
					QuestionId = question.Id,
					QuestionDefinitionLineId = lineId,
					QuestionDefinitionHeaderId = headerId,
					QuestionDefinitionResponseId = responseId,
					Response = response,
					SubmissionIdentifier = _appIdentifier,
					Channel = "MOBILE",
					DeviceType = "Iphone",
					Latitude = string.Empty,
					Longitude = string.Empty,
					CompanyGlobalId = string.Empty,
					ErrorMessage = string.Empty,
					GlobalId = string.Empty,
					ReferralCode = _referralCode,
					TrackingCode = _trackingCode
				};

				if (responseCollection != null) item.ResponseCollection = responseCollection.ToArray();

				responseList.Add(item);
			}
			_questionResponseList = responseList.ToArray();
			var responseContent = Newtonsoft.Json.JsonConvert.SerializeObject(_questionResponseList);
			StorageHelper.SaveToIsolatedStorage(StorageHelper.RESPONSES, responseContent);

		}

		private void SaveAdditionalResponseToCollection(long questionId, string response, long lineId, long responseId)
		{
			var responseList = new List<QuestionResponseM>();
			if (_questionResponseList != null && _questionResponseList[0] != null)
			{
				responseList.AddRange(_questionResponseList);
			}

			var item = responseList.SingleOrDefault(c => c.QuestionId == questionId
			                                        && c.QuestionDefinitionLineId == lineId
			                                        && c.QuestionDefinitionResponseId == responseId);

			if (item != null)
			{
				item.QuestionDefinitionResponseId = responseId;
				item.AdditionalResponse = response;
			}
			else
			{
				item = new QuestionResponseM
				{
					QuestionId = questionId,
					QuestionDefinitionLineId = lineId,
					QuestionDefinitionHeaderId = -1,
					QuestionDefinitionResponseId = responseId,
					AdditionalResponse = response,
					SubmissionIdentifier = _appIdentifier,
					Channel = "MOBILE",
					DeviceType = "IPhone",
					Latitude = string.Empty,
					Longitude = string.Empty,
					CompanyGlobalId = string.Empty,
					ErrorMessage = string.Empty,
					GlobalId = string.Empty,
					ReferralCode = _referralCode,
					TrackingCode = _trackingCode
				};

				responseList.Add(item);
			}
			_questionResponseList = responseList.ToArray();
			var responseContent = Newtonsoft.Json.JsonConvert.SerializeObject(_questionResponseList);
			StorageHelper.SaveToIsolatedStorage(StorageHelper.RESPONSES, responseContent);
		}

		#endregion

	}
}

