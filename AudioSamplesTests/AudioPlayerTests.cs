using Microsoft.VisualStudio.TestTools.UnitTesting;
using AudioSamples;



namespace AudioSamplesTests
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Versioning;
    using System.Threading.Tasks;

    using AudioSamples;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class AudioPlayerTests
    {

        [TestMethod()]
        public void PlayUsingSoundPlayerTest()
        {
            AudioPlayer audioPlayer = new AudioPlayer();
            audioPlayer.PlayWav("annoy.wav");
        }

        [TestMethod()]
        [DeploymentItem(@".\Resources\cant_resist.mp3")]
        public void PlayAudioTest()
        {
        
            AudioPlayer audioPlayer = new AudioPlayer();
            audioPlayer.Play(@".\cant_resist.mp3");
                  
        }


        [TestMethod()]
        public void PlayAudioTest2()
        {

            AudioPlayer audioPlayer = new AudioPlayer();
            audioPlayer.Play(@".\\mask01.mp3");

        }



        [TestMethod()]
        public void LoadAllAudioFromDirectoryTest()
        {
            AudioPlayer audioPlayer = new AudioPlayer();
            audioPlayer.LoadAllAudioFromDirectory(".\\").ForEach(Console.WriteLine);
        }
    }
}