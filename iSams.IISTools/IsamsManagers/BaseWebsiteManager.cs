using System.IO;
using System.Linq;
using iSams.IISTools.Entities;
using log4net;
using Microsoft.Web.Administration;

namespace iSams.IISTools.IsamsManagers
{
    public class BaseWebsiteManager
    {
        protected const string LegacySubAppRelativePath = "\\iSAMS.Website";

        public BaseWebsiteManager(Parameters website, ServerManager serverManager, ILog logger)
        {
            this.Website = website;
            this.ServerManager = serverManager;
            this.Logger = logger;
        }

        public bool SetsHostNameEqualToWebsiteName { get; set; }

        protected const string BindingProtocol = "http";

        protected Parameters Website { get; set; }

        protected ServerManager ServerManager { get; set; }

        protected ILog Logger { get; set; }

        protected ApplicationPool ApplicationPool { get; set; }

        protected Site Site { get; set; }

        public void Setup()
        {
            Logger.Info("Setting up website: " + this.Website.Name);
            this.SetupApplicationPool();
            this.SetupWebSite();
            this.SetupVirtualDirectories();
            this.ServerManager.CommitChanges();
        }

        protected virtual void SetupApplicationPool()
        {
            var existingAppPool = this.ServerManager.ApplicationPools.SingleOrDefault(ap => ap.Name == this.Website.Name);
            if (existingAppPool != null)
            {
                Logger.Info("   Deleting existing Application Pool");
                this.ServerManager.ApplicationPools.Remove(existingAppPool);
            }
            Logger.Info("   Adding Application Pool");
            this.ApplicationPool = this.ServerManager.ApplicationPools.Add(this.Website.Name);
        }

        protected virtual void SetupWebSite()
        {
            var existingSite = this.ServerManager.Sites.SingleOrDefault(ap => ap.Name == this.Website.Name);
            if (existingSite != null)
            {
                Logger.Info("   Deleting existing website");
                this.ServerManager.Sites.Remove(existingSite);
            }

            Logger.Info("   Adding website");
            this.Site = this.ServerManager.Sites.Add(this.Website.Name, BindingProtocol, "*:" + Website.Port + ":" + (SetsHostNameEqualToWebsiteName ? this.Website.Name : string.Empty), this.Website.BranchRootPath + this.Website.RelativePath);
            this.Site.Applications.First().ApplicationPoolName = this.ApplicationPool.Name;            
        }

        protected virtual void SetupVirtualDirectories()
        {
            var app = this.Site.Applications.First();
            var filesDirectory = new DirectoryInfo(this.Website.BranchRootPath + "\\files");
            EnsureDirectoryExists(filesDirectory);
            app.VirtualDirectories.Add("/files", filesDirectory.FullName);

            var contentDirectory = new DirectoryInfo(this.Website.BranchRootPath + "\\content");
            EnsureDirectoryExists(contentDirectory);
            app.VirtualDirectories.Add("/content", contentDirectory.FullName);
        }

        private static void EnsureDirectoryExists(DirectoryInfo di)
        {
            if (!di.Exists)
            {
                di.Create();
            }
        }
    }
}
