using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
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

	public class Worker
	{
		private readonly DTE envDte;
		private readonly IRootSelector selector;

		public Worker(DTE envDte, IRootSelector selector)
		{
			this.envDte = envDte;
			this.selector = selector;
		}

		public void Nest()
		{
			var selectedItems = envDte.SelectedItems
										.OfType<SelectedItem>()
										.Select(si => new FileItem {Name = si.Name})
										.Reverse();

			var root = selector.Select(selectedItems);
			if(root == null) return;
			var rootProjectItem = envDte.SelectedItems.OfType<SelectedItem>().First(se => se.Name == root.Name).ProjectItem;
			var childs = envDte.SelectedItems.OfType<SelectedItem>().Select(se => se.ProjectItem).Where(pi => pi.Name != root.Name);
			foreach (var child in childs)
			{
				rootProjectItem.ProjectItems.AddFromFile(child.FileNames[0]);
			}
		}

		public void UnNest()
		{
			var selectedProjectItems = envDte.SelectedItems.OfType<SelectedItem>().Select(si => si.ProjectItem).ToArray();
			var selectedNames = selectedProjectItems.Select(pi => pi.Name).ToArray();
			var rootOfUnNest = selectedProjectItems.Where(pi => pi.ProjectItems.OfType<ProjectItem>().Any(child => selectedNames.Contains(child.Name))).ToArray();
			var toUnNest = rootOfUnNest.ToDictionary(r => r, r => r.ProjectItems.OfType<ProjectItem>().Where( pi => selectedNames.Contains(pi.Name)));

			
			foreach (var keyValue in toUnNest)
			{
				foreach (var childItem in keyValue.Value)
				{
					//childItem.Remove();
					//keyValue.Key.ContainingProject.ProjectItems.AddFromFile(childItem.FileNames[0]);
					UnNest(keyValue.Key.ContainingProject, childItem);
				}
			}
		}

		private static void UnNest(Project parent, ProjectItem childItem)
		{
			foreach (var grandChild in childItem.ProjectItems.OfType<ProjectItem>())
			{
				UnNest(parent, grandChild);
			}

			var filePath = childItem.FileNames[0];
			var tempFile = Path.GetTempFileName();
			File.Copy(filePath, tempFile, true); 
			childItem.Delete();
			File.Copy(tempFile, filePath); 
			File.Delete(tempFile);
			parent.ProjectItems.AddFromFile(filePath);
		}
	}
}