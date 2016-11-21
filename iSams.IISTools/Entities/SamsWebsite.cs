using System.Collections.Generic;
using System.Collections.Specialized;

namespace iSams.IISTools.Entities
{
    public class Parameters
    {
        public Parameters(string name, string branchRootPath, string port)
        {
            this.Name = name;
            this.BranchRootPath = branchRootPath;
            this.Port = port;
        }

        public Parameters(IEnumerable<string> args)
        {
            foreach (var arg in args)
            {
                var keyValue = arg.Split('=');
                var key = keyValue[0].ToLower();
                var value = keyValue[1];

                switch (key)
                {
                    case "name":
                        this.Name = value;
                        break;
                    case "path":
                        this.BranchRootPath = value;
                        break;
                    case "formwidgetpath":
                        this.FormWidgetPath = value;
                        break;
                }
            }
        }

        public string Name { get; set; }

        public string BranchRootPath { get; set; }

        public string RelativePath { get; set; }

        public string FormWidgetPath { get; set; }

        public string Port { get; set; }
    }
}
