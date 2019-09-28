using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProcessManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessManagement.Tests
{
    [TestClass()]
    public class ProcessManagerTests
    {
        [TestMethod()]
        public void GetProcessHandleTest_Explorer()
        {
            var result = ProcessManager.GetProcessHandle("explorer");
            Console.WriteLine(result);
            Assert.IsTrue(result.ToInt32() > 0 , "Could not find process");
        }

        [TestMethod()]
        public void GetProcessHandleTest_PaintNet()
        {
            var result = ProcessManager.GetProcessHandle("PaintDotNet");
            Console.WriteLine(result);
            Assert.IsTrue(result.ToInt32() > 0, "Could not find process");
        }

        [TestMethod()]
        public void GetProcessHandleTest_MissingProcess()
        {
            var result = ProcessManager.GetProcessHandle("PaintMissing");
            Console.WriteLine(result);
            Assert.IsTrue(result.ToInt32() == 0, "Found a process that was not supposed to be found.");
        }
    }
}