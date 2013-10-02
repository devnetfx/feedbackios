using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.Foundation;
using System.IO;
using System.Linq;

namespace Feedback.Class {
	public class TableSource : UITableViewSource {
		protected string cellIdentifier = "TableCell";
		
		Dictionary<string, List<TableItem>> indexedTableItems;
		string[] keys;

		public TableSource (List<TableItem> items)
		{
			indexedTableItems = new Dictionary<string, List<TableItem>>();
			foreach (var t in items) {
				if (!String.IsNullOrEmpty (t.SubHeading)) {
					if (indexedTableItems.ContainsKey (t.SubHeading)) {
						indexedTableItems[t.SubHeading].Add(t);
					} else {
						indexedTableItems.Add (t.SubHeading, new List<TableItem>() {t});
					}
				}
			}
			keys = indexedTableItems.Keys.ToArray ();
		}
		
		/// <summary>
		/// Called by the TableView to determine how many sections(groups) there are.
		/// </summary>
		public override int NumberOfSections (UITableView tableView)
		{
			return keys.Length;
		}

		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override int RowsInSection (UITableView tableview, int section)
		{
			return indexedTableItems[keys[section]].Count;
		}
		
		/// <summary>
		/// Sections the index titles.
		/// </summary>
//		public override string[] SectionIndexTitles (UITableView tableView)
//		{
//			return indexedTableItems.Keys.ToArray ();
//		}
		
		/// <summary>
		/// The string to show in the section header
		/// </summary>
		public override string TitleForHeader (UITableView tableView, int section)
		{
			return string.Empty; //keys[section];
		}
		
		/// <summary>
		/// The string to show in the section footer
		/// </summary>
		public override string TitleForFooter (UITableView tableView, int section)
		{
			return string.Empty; //indexedTableItems[keys[section]].Count + " items";
		}

		public void ProcessSelection (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.CellAt(indexPath);

			if (cell.Accessory == UITableViewCellAccessory.None) {
				indexedTableItems [keys[indexPath.Section]] [indexPath.Row].IsChecked = true;
				cell.Accessory = UITableViewCellAccessory.Checkmark;
			}
			else{
				indexedTableItems [keys[indexPath.Section]] [indexPath.Row].IsChecked = false;
				cell.Accessory = UITableViewCellAccessory.None;
			}

			tableView.DeselectRow (indexPath, false);
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			ProcessSelection (tableView, indexPath);
		}

		public override void RowDeselected (UITableView tableView, NSIndexPath indexPath)
		{
			ProcessSelection (tableView, indexPath);
		}

		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular section and row
		/// </summary>
		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			//---- declare vars
			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);
			TableItem item = indexedTableItems[keys[indexPath.Section]][indexPath.Row];
			
			//---- if there are no cells to reuse, create a new one
			if (cell == null)
			{ 
				cell = new UITableViewCell (item.CellStyle, cellIdentifier); 
			}
			
			//---- set the item text
			cell.TextLabel.Text = item.Heading;	
			
			//---- if the item has a valid image, and it's not the contact style (doesn't support images)
			if(!string.IsNullOrEmpty(item.ImageName) && item.CellStyle != UITableViewCellStyle.Value2)
			{
				if(File.Exists(item.ImageName))
				{ cell.ImageView.Image = UIImage.FromBundle(item.ImageName); }
			}
			
			//---- set the accessory
			cell.Accessory = item.IsChecked ? UITableViewCellAccessory.Checkmark : item.CellAccessory;
			
			return cell;
		}

		public List<TableItem> SelectedOptions()
		{
			return indexedTableItems.First().Value.Where(c => c.IsChecked).ToList();

		}
	}
}