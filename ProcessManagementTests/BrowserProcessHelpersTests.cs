namespace ProcessManagementTests
{
    using System;
    using System.Diagnostics;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using ProcessManagement;

    [TestClass()]
    public class BrowserProcessHelpersTests
    {
        [TestMethod()]
        public void GetFireFoxBrowsersUrlTest()
        {
            foreach (Process process in Process.GetProcessesByName("firefox"))
            {
                string url = BrowserProcessHelpers.GetFirefoxUrl(process);
                if (url == null)
                    continue;

                Console.WriteLine("FF Url for '" + process.MainWindowTitle + "' is " + url);
            }
        }

        [TestMethod()]
        public void GetIEBrowsersUrlTest()
        {
          

            foreach (Process process in Process.GetProcessesByName("iexplore"))
            {
                string url = BrowserProcessHelpers.GetInternetExplorerUrl(process);
                if (url == null)
                    continue;

                Console.WriteLine("IE Url for '" + process.MainWindowTitle + "' is " + url);
            }

           
        }

        [TestMethod()]
        public void GetChromeBrowsersUrlTest()
        {
           
            foreach (Process process in Process.GetProcessesByName("chrome"))
            {
                string url = BrowserProcessHelpers.GetChromeUrl(process);
                if (url == null)
                    continue;

                Console.WriteLine("CH Url for '" + process.MainWindowTitle + "' is " + url);
            }
        }

        [TestMethod]
        public void GetFireFoxTabTitle()
        {
            var title = BrowserProcessHelpers.GetWebBrowserTabTitle("firefox");
            Console.WriteLine(title);
        }
    }
}