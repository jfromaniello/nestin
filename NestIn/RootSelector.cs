using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace NestIn
{
	public partial class RootSelector : Form, IRootSelector
	{
		public RootSelector()
		{
			InitializeComponent();
		}

		#region IRootSelector Members

		public FileItem Select(IEnumerable<FileItem> candidates)
		{
			comboFileItem.DataSource = candidates.ToList();
			comboFileItem.DisplayMember = "Name";
			if (ShowDialog() == DialogResult.OK)
			{
				return (FileItem) comboFileItem.SelectedItem;
			}
			return null;
		}

		#endregion

		private void OkButtonClick(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void CancelButtonClick(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}