using System.Collections.Generic;
using System.IO;
using System.Linq;
using iSams.IISTools.Entities;
using log4net;
using Microsoft.Web.Administration;

namespace iSams.IISTools.IsamsManagers
{
    public class PortalsManager : AspWebsiteManager
    {
        public PortalsManager(Parameters website, ServerManager serverManager, ILog logger)
            : base(website, serverManager, logger)
        {
            
        }

        protected override void SetupWebSite()
        {
            base.SetupWebSite();
            this.SetupDocuments();
            this.CopyConnectionStringFile();
        }

        private void SetupDocuments()
        {
            Logger.Info("Setting up default documents");
            var config = this.ServerManager.GetApplicationHostConfiguration();
            var defaultDocumentSection = config.GetSection("system.webServer/defaultDocument");

            var documents = defaultDocumentSection.GetCollection("files");
            documents.Clear();

            var document = documents.CreateElement();
            document["value"] = "index.asp";
            documents.Add(document);            
        }

        override protected void SetupVirtualDirectories()
        {
            base.SetupVirtualDirectories();

            var app = this.Site.Applications.First();
            new List<string> {"themes", "system\\images" }
                .ForEach(AddVirtualDirectory);
            
            app.VirtualDirectories.Add("/xupload", "C:\\Program Files (x86)\\Persits Software\\XUpload\\Samples");            
        }

        private void AddVirtualDirectory(string directoryName)
        {
            var app = this.Site.Applications.First();
            var di = new DirectoryInfo(this.Website.BranchRootPath + "\\iSAMS.Website\\" + directoryName);
            di.Create();
            app.VirtualDirectories.Add("/" + directoryName.Replace("\\","/"), di.FullName);
        }

        private void CopyConnectionStringFile()
        {
            FileInfo targetFile = new FileInfo(this.Website.BranchRootPath + this.Website.RelativePath + "\\connectionstrings.config");
            if (!targetFile.Exists)
            {
                FileInfo sourceFile = new FileInfo(this.Website.BranchRootPath + LegacySubAppRelativePath + "\\connectionstrings.config");
                File.Copy(sourceFile.FullName, targetFile.FullName, false);
            }
        }

        
    }
}
