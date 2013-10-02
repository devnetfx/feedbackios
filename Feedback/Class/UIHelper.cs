using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;

namespace Feedback
{
	public class UIHelper
	{
		public static UITextField AddNewTextBoxToPanel (StackPanel view, string responseText, long lineId)
		{
			UITextField newTextField = new UITextField (new RectangleF (15, 80, 268, 40)) {
				Placeholder = "",
				BorderStyle = UITextBorderStyle.RoundedRect,
				ShouldReturn = (field) => {
					field.ResignFirstResponder();
					return true;
				},

				Text = responseText,
				Tag = Convert.ToInt32(lineId)
			};
	
			//IndustrialTheme.Apply (newTextField);
			view.AddSubview (newTextField);

			return newTextField;

		}

		public static UILabel AddLabelToPanel(StackPanel view, string labelText)
		{
			var height = 20f;

			if (labelText.Length > 40) {
				//labelText = labelText.Replace ("\n", " ");
				height = Convert.ToInt32 (System.Math.Ceiling(Convert.ToDecimal(labelText.Length / 20)) * 20);
				if (height > 1100) {
					height = 1100;
				}
			}

			UILabel label = new UILabel (new RectangleF(15,40,200,height)) {
				Text = labelText,
				LineBreakMode = UILineBreakMode.WordWrap,
				Lines = 0
			};
			//label.SizeToFit ();
			//IndustrialTheme.Apply (label);
			view.AddSubview (label);


			return label;
		}

        public static UIButton AddButton (UIView view, string options, string title)
		{
			var button = new UIButton (new RectangleF(10, 120, 298, 57));
			button.BackgroundColor = UIColor.LightGray;
			button.SetTitleColor (UIColor.Black, UIControlState.Normal);
			button.SetTitle (title, UIControlState.Normal);

			view.AddSubview (button);

			return button;
		}
	}
}

