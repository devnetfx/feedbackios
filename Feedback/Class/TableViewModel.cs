using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;

namespace Feedback
{
	public class TableSSource : UITableViewSource {
		string[] tableItems;
		string cellIdentifier = "TableCell";
		Dictionary<string, List<string>> indexedTableItems;
		string[] keys;

		public TableSSource (string[] items)
		{
			tableItems = items;

			indexedTableItems = new Dictionary<string, List<string>>();
			foreach (var t in items) {
				if (indexedTableItems.ContainsKey (t[0].ToString ())) {
					indexedTableItems[t[0].ToString ()].Add(t);
				} else {
					indexedTableItems.Add (t[0].ToString (), new List<string>() {t});
				}
			}
			keys = indexedTableItems.Keys.ToArray ();
		}
		public override int RowsInSection (UITableView tableview, int section)
		{
			return tableItems.Length;
		}

		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);
			// if there are no cells to reuse, create a new one
			if (cell == null)
				cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier);
			cell.TextLabel.Text = tableItems[indexPath.Row];
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			new UIAlertView("Row Selected", tableItems[indexPath.Row], null, "OK", null).Show();
			tableView.DeselectRow (indexPath, true); // normal iOS behaviour is to remove the blue highlight
		}

		public override string TitleForHeader (UITableView tableView, int section)
		{
			return keys[section];
		}

		public override string TitleForFooter (UITableView tableView, int section)
		{
			return indexedTableItems[keys[section]].Count + " items";
		}
	}
}

