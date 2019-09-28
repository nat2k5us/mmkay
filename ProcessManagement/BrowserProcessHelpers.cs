namespace ProcessManagement
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Automation;

    public static class BrowserProcessHelpers
    {
        public static string GetChromeUrl(Process process)
        {
            if (process == null)
                throw new ArgumentNullException("process");

            if (process.MainWindowHandle == IntPtr.Zero)
                return null;

            AutomationElement element = AutomationElement.FromHandle(process.MainWindowHandle);
            if (element == null)
                return null;

            AutomationElement edit = element.FindFirst(TreeScope.Subtree,
                 new AndCondition(
                      new PropertyCondition(AutomationElement.NameProperty, "address and search bar", PropertyConditionFlags.IgnoreCase),
                      new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit)));

            return ((ValuePattern)edit.GetCurrentPattern(ValuePattern.Pattern)).Current.Value as string;
        }

        public static string GetInternetExplorerUrl(Process process)
        {
            if (process == null) throw new ArgumentNullException("process");

            if (process.MainWindowHandle == IntPtr.Zero) return null;

            var element = AutomationElement.FromHandle(process.MainWindowHandle);
            if (element == null) return null;

            var rebar = element.FindFirst(
                TreeScope.Children,
                new PropertyCondition(AutomationElement.ClassNameProperty, "ReBarWindow32"));
            if (rebar == null) return null;

            var edit = rebar.FindFirst(
                TreeScope.Subtree,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));

            return ((ValuePattern)edit.GetCurrentPattern(ValuePattern.Pattern)).Current.Value;
        }

        public static string GetFirefoxUrl(Process process)
        {
            if (process == null)
                throw new ArgumentNullException("process");

            if (process.MainWindowHandle == IntPtr.Zero)
                return null;

            AutomationElement element = AutomationElement.FromHandle(process.MainWindowHandle);
            if (element == null)
                return null;


            element = element.FindFirst(TreeScope.Subtree,
                  new AndCondition(
                      new PropertyCondition(AutomationElement.NameProperty, "search or enter address", PropertyConditionFlags.IgnoreCase),
                      new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit)));


            if (element == null)
                return null;

            return ((ValuePattern)element.GetCurrentPattern(ValuePattern.Pattern)).Current.Value as string;
        }

        public static string GetWebBrowserTabTitle(string processName)
        {
            try
            {
                IntPtr ptr = ProcessManager.GetProcessHandle(processName);
                var window = AutomationElement.FromHandle(ptr);
              //  Console.WriteLine("window: " + window.Current.Name);

                // note: carefully choose the tree scope for perf reasons
                // try to avoid SubTree although it seems easier...
                var titleBar = window.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TitleBar));
                if (!string.IsNullOrEmpty(titleBar.Current.Name))
                {
                    Console.WriteLine("titleBar: " + titleBar.Current.Name);
                    return titleBar.Current.Name;
                }
                else
                {
                    return window.Current.Name;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return string.Empty;
            }
        }
    
    }
}