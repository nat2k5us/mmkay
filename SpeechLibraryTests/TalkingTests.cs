using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpeechLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechLibrary.Tests
{
    [TestClass()]
    public class TalkingTests
    {
        [TestMethod()]
        public void SpeakTextTest()
        {
            Talking talking = new Talking();
            talking.SpeakText("Welcome");
        }
    }
}