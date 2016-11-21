using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using iSams.IISTools.Entities;
using log4net;
using Microsoft.Web.Administration;

namespace iSams.IISTools.IsamsManagers
{
    public class MainWebsiteManager : AspWebsiteManager
    {        
        private const string MainSubAppRelativePath = "\\iSAMS.New\\iSAMS.Web\\iSAMS.Web";        
        private const string ApiAppRelativePath = "\\iSAMS.Api.Legacy\\iSAMS.RestService";
        

        public MainWebsiteManager(Parameters website, ServerManager serverManager, ILog logger) : base(website, serverManager, logger)
        {
            this.Website.RelativePath = "\\iSAMS.New\\iSAMS.Web\\iSAMS.Root";
            this.Website.Port = "80";
        }

        protected override void SetupWebSite()
        {
            base.SetupWebSite();
            this.SetupApplications();
        }

        protected override void SetupApplicationPool()
        {
            base.SetupApplicationPool();
            this.ApplicationPool.ManagedRuntimeVersion = "v4.0";
            this.ApplicationPool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
        }

        private void SetupApplications()
        {
            Logger.Info("Setting up Applications");
            AddApplication("Main", MainSubAppRelativePath);
            AddApplication("Legacy", LegacySubAppRelativePath);
            AddApplication("Legacy/api", ApiAppRelativePath);
        }

        private void AddApplication(string name, string relativePath)
        {
            var rootPath = new DirectoryInfo(this.Website.BranchRootPath);
            var mainApp = this.Site.Applications.Add("/" + name, rootPath.FullName + relativePath);
            mainApp.ApplicationPoolName = this.ApplicationPool.Name;
        }
    }
}
