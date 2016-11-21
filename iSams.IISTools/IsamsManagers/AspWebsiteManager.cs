using System;
using System.Linq;
using iSams.IISTools.Entities;
using log4net;
using Microsoft.Web.Administration;

namespace iSams.IISTools.IsamsManagers
{
    public class AspWebsiteManager : BaseWebsiteManager
    {
        public AspWebsiteManager(Parameters website, ServerManager serverManager, ILog logger) : base(website, serverManager, logger) { }

        protected override void SetupApplicationPool()
        {
            base.SetupApplicationPool();
            this.ApplicationPool.Enable32BitAppOnWin64 = true;
            this.ApplicationPool.ProcessModel.MaxProcesses = 1;
        }

        protected override void SetupWebSite()
        {
            base.SetupWebSite();
            this.SetupAspSection();
        }

        protected void SetupAspSection()
        {
            Logger.Info("Setting up ASP section");
            var config = this.ServerManager.GetApplicationHostConfiguration();
            var aspSection = config.GetSection("system.webServer/asp", this.Website.Name);
            aspSection["EnableParentPaths"] = true;
            aspSection["scriptErrorSentToBrowser"] = true;

            var limitsElement = aspSection.GetChildElement("limits");
            limitsElement["maxRequestEntityAllowed"] = 204857600;
            limitsElement["bufferingLimit"] = 2073741824;
            limitsElement["scriptTimeout"] = new TimeSpan(0, 0, 2, 30);
        }
    }
}
