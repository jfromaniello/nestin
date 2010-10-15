using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace NestIn
{
	public interface IRootSelector
	{
		FileItem Select(IEnumerable<FileItem> candidates);
	}
	
	public class FileItem
	{
		public string Name { get; set; }
	}

	public class SolutionExplorer
	{
		private readonly DTE envDte;
		private readonly IRootSelector selector;

		public SolutionExplorer(DTE envDte, IRootSelector selector)
		{
			this.envDte = envDte;
			this.selector = selector;
		}

		public void Nest()
		{
			var selectedItems = envDte.SelectedItems.OfType<SelectedItem>().Select(si => new FileItem {Name = si.Name});
			var root = selector.Select(selectedItems);
			if(root == null) return;
			var rootProjectItem = envDte.SelectedItems.OfType<SelectedItem>().First(se => se.Name == root.Name).ProjectItem;
			var childs = envDte.SelectedItems.OfType<SelectedItem>().Select(se => se.ProjectItem).Where(pi => pi.Name != root.Name);
			foreach (var child in childs)
			{
				rootProjectItem.ProjectItems.AddFromFile(child.FileNames[0]);
			}


		}
	}
}