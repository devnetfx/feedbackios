using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
using SharableTypes.Model;

namespace Feedback
{
	public class PickerModel : UIPickerViewModel {
		List <QuestionDefinitionResponseM> items;
		public event EventHandler PickerChanged;
		private long _currentSelectedId = -1;
		protected int _selectedIndex = 0;

		public PickerModel (List <QuestionDefinitionResponseM> itemList, long currentSelectedId, int selectedIndex) : base () {
			if (itemList != null) {
				items = itemList;
			} else {
				items = new List<QuestionDefinitionResponseM> ();
			}
			_currentSelectedId = currentSelectedId;
			_selectedIndex = selectedIndex;
		}

		public override int GetRowsInComponent (UIPickerView pickerView, int component) {
			return items.Count;
		}

		public override int GetComponentCount (UIPickerView pickerView) {
			return 1;
		}

		public override string GetTitle(UIPickerView uipv, int row, int comp)
		{

			string output = items[row].Text;
			return(output);
		}

		public override float GetComponentWidth(UIPickerView uipv, int comp){

			return(300f);

		}

		public override float GetRowHeight(UIPickerView uipv, int comp){

			return(40f); 

		}

		public long SelectedId
		{
			get { return _selectedIndex > -1 ? this.items[this._selectedIndex].Id : _currentSelectedId; }
		}

		public override void Selected (UIPickerView picker, int row, int component)
		{
			this._selectedIndex = row;
			if (this.PickerChanged != null)
			{
				this.PickerChanged(this, new PickerChangedEventArgs{SelectedValue = items[row]});
			}
		}
	}

	public class PickerChangedEventArgs : EventArgs
	{
		public PickerChangedEventArgs()
		{

		}
		public QuestionDefinitionResponseM SelectedValue = null;
	}
}

