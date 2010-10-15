using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace NestIn
{
	[Export(typeof(SolutionExplorer))]
	internal class SolutionExplorer
	{
		private readonly DTE envDte;

		[ImportingConstructor]
		internal SolutionExplorer() 
		    : this((DTE)Package.GetGlobalService(typeof(DTE)))
		{
		}

		public SolutionExplorer(DTE envDte)
		{
			this.envDte = envDte;
		}

		public IEnumerable<FileItem> GetSelectedItems()
		{
			return envDte.SelectedItems.OfType<SelectedItem>().Select(se => new FileItem
			                                                                	{
			                                                                		Name = se.Name
			                                                                	});
		}

		public void Nest()
		{
			var items = envDte.SelectedItems.OfType<SelectedItem>().Select(si => si.ProjectItem).ToArray();
			for (int i = 1; i < items.Length; i++)
			{
				items[0].ProjectItems.AddFromFile(items[i].FileNames[0]);
			}
		}
	}

	public class FileItem
	{
		public string Name { get; set; }
	}
}