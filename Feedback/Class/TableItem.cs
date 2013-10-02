using System;
using MonoTouch.UIKit;

namespace Feedback.Class {
	public class TableItem {
		public string Heading { get; set; }
		public string SubHeading { get; set; }
		public string ImageName { get; set; }
		public bool IsChecked { get; set; }
		public long ResponseId { get; set;}
		public long DefinitionLineId { get; set;}
		public long DefinitionHeaderId { get; set;}

		public UITableViewCellStyle CellStyle
		{
			get { return cellStyle; }
			set { cellStyle = value; }
		}
		protected UITableViewCellStyle cellStyle = UITableViewCellStyle.Default;
		
		public UITableViewCellAccessory CellAccessory
		{
			get { return cellAccessory; }
			set { cellAccessory = value; }
		}
		protected UITableViewCellAccessory cellAccessory = UITableViewCellAccessory.None;

		public TableItem () { }
		
		public TableItem (string heading)
		{ 
			Heading = heading; 
		}
	}
}