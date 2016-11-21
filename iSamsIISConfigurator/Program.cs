using System.Linq;
using iSams.IISTools.Entities;
using iSams.IISTools.IsamsManagers;
using log4net;
using Microsoft.Web.Administration;

namespace iSamsIISConfigurator
{
    internal class Program
    {
        private static Parameters parameters;
        private static ILog logger;

        private static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            parameters = new Parameters(args);

            using (var iisManager = new ServerManager())
            {
                SetupMainWebsite(iisManager);
                SetupParentsPortal(iisManager);
                SetupPupilsPortal(iisManager);
                SetupAdmissionsPortal(iisManager);
            }
        }

        private static void SetupParentsPortal(ServerManager iisManager)
        {
            var parentsPortalWebsite = new Parameters("iSams - Parent", parameters.BranchRootPath, "81");
            parentsPortalWebsite.RelativePath = "\\Portals\\iSAMS.ParentPortal\\iSAMS.ParentPortal";
            var parentsPortalmanager = new PortalsManager(parentsPortalWebsite, iisManager, logger);
            parentsPortalmanager.Setup();
        }

        private static void SetupPupilsPortal(ServerManager iisManager)
        {
            var parentsPortalWebsite = new Parameters("iSams - Pupil", parameters.BranchRootPath, "82");
            parentsPortalWebsite.RelativePath = "\\Portals\\iSAMS.StudentPortal\\iSAMS.StudentPortal";
            var parentsPortalmanager = new PortalsManager(parentsPortalWebsite, iisManager, logger);
            parentsPortalmanager.Setup();
        }

        private static void SetupMainWebsite(ServerManager iisManager)
        {
            var mainManager = new MainWebsiteManager(parameters, iisManager, logger);
            mainManager.SetsHostNameEqualToWebsiteName = true;
            mainManager.Setup();
        }

        private static void SetupAdmissionsPortal(ServerManager iisManager)
        {
            var admissionPortalParameters = new Parameters("admissions-portal-local", parameters.FormWidgetPath + "\\trunk\\AdmissionsSite", "8080");
            var admissionsPortalManager = new BaseWebsiteManager(admissionPortalParameters, iisManager, logger);
            admissionsPortalManager.Setup();


            var widgetParameters = new Parameters("formwidgets", parameters.FormWidgetPath + "\\trunk\\FormWidgets", "80");
            var widgetManager = new BaseWebsiteManager(widgetParameters, iisManager, logger);
            widgetManager.SetsHostNameEqualToWebsiteName = true;
            widgetManager.Setup();


        }
    }
}
