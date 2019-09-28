using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeyboardMouse;



namespace KeyboardMouseTests
{
    using System;

    using KeyboardMouse;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ApplicationInputTests
    {
        [TestMethod]
        public void SetFocusToExternalAppFireFoxTest()
        {
            var strProcessName = "firefox";
            Assert.IsTrue(ApplicationInput.SetFocusToExternalApp(strProcessName), $"Could not set Focus to {strProcessName}");
            var activeProcessName = ApplicationInput.GetActiveProcessName();
            Console.WriteLine(activeProcessName);
            Assert.IsTrue(activeProcessName.Contains(strProcessName));
            //   Assert.IsTrue(ApplicationInput.StopFlashingWindow("firefox"),"");
            // ApplicationInput.SetFocusToExternalApp("PaintDotNet");
            //  ApplicationInput.SetFocusToExternalApp("Chrome");
        }

        [TestMethod]
        public void SetFocusToExternalAppCHromeTest()
        {
            var strProcessName = "chrome";
            Assert.IsTrue(ApplicationInput.SetFocusToExternalApp(strProcessName), $"Could not set Focus to {strProcessName} as it was not running.");
            var activeProcessName = ApplicationInput.GetActiveProcessName();
            Console.WriteLine(activeProcessName);
            Assert.IsTrue(activeProcessName.Contains(strProcessName));
         
        }

        [TestMethod]
        public void FlashWindowTest()
        {
            Assert.IsTrue(ApplicationInput.FlashWindow("firefox", 100, 2), "");
        }

        [TestMethod()]
        public void GetActiveProcessNameTest()
        {
            var activeProcessName = ApplicationInput.GetActiveProcessName();
            Console.WriteLine(activeProcessName);
            Assert.IsTrue(activeProcessName.Contains("devenv"));
        }
    }
}