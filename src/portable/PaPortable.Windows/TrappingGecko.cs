using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Gecko;
using SIL.IO;

namespace PaPortable.Windows
{
    public class TrappingGecko : GeckoWebBrowser//, IPlatformSpecifics
    {
        //public DisplayPages DisplayPages { get; set; }
        public List<string> SupportFile { get; set; }

        protected override void OnDomClick(DomMouseEventArgs e)
        {
            var elem = Document.ActiveElement;
            var uri = elem.HasAttribute("Href") ? elem.GetAttribute("Href") :
                elem.Parent.HasAttribute("Href") ? elem.Parent.GetAttribute("Href") :
                "";
            const string scheme = "hybrid:";
            if (!uri.StartsWith(scheme)) base.OnDomClick(e);
            e.Handled = true;
            var resources = uri.Substring(scheme.Length).Split('?');
            var method = resources[0];
            var parameters = resources.Length > 1? System.Web.HttpUtility.ParseQueryString(resources[1]): null;
            switch (method)
            {
                case "VowelChart": break;
                case "ConstChart": break;
                //case "DataCorpus": DisplayPages.DisplayData(); break;
                //case "Search": break;
                //case "Project": DisplayPages.DisplayOpenProject(); break;
                //case "Settings": break;
                //case "DistChart": break;
                //case "Steps": break;
                //case "Close": Program.DisplayMenu(); break;
            }
        }
        public void DisplayPage(string page, int[] coords)
        {
            FindForm().Size = new Size(coords[0], coords[1]);
            var tempFile = TempFile.WithExtension("html");
            var tempFolder = Path.GetDirectoryName(tempFile.Path);
            var tempName = Path.GetFileName(tempFile.Path);
            tempFile.MoveTo(Path.Combine(tempFolder, "Content", tempName));
            File.WriteAllText(tempFile.Path, page);
            var uri = new Uri(tempFile.Path);
            Navigate(uri.AbsoluteUri);
            SupportFile.Add(tempFile.Path);
        }

        public List<string> Projects
        {
            get
            {
                var documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var result = new List<string>();
                var dirInfo = new DirectoryInfo(documentFolder);
                AddMatchingFiles("*.pap", dirInfo, result);
                return result;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        private void AddMatchingFiles(string v, DirectoryInfo dirInfo, List<string> result)
        {
            try
            {
                foreach (var file in dirInfo.GetFiles(v))
                {
                    result.Add(Path.GetFileNameWithoutExtension(file.Name));
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore unauthorized access
            }
            try
            {
                foreach (var folder in dirInfo.GetDirectories())
                {
                    AddMatchingFiles(v, folder, result);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore unauthorized access
            }
        }
    }
}
