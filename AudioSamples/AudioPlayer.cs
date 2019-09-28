namespace AudioSamples
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Media;
    using System.Threading;
    using System.Threading.Tasks;

    using NAudio.Wave;

    public class AudioPlayer
    {
        private AudioFileReader audioFileReader;

        private IWavePlayer waveOutDevice;

        public AudioPlayer()
        {
            this.waveOutDevice = new WaveOut();
        }

        public void CloseWaveOut()
        {
            this.waveOutDevice?.Stop();

            if (this.waveOutDevice != null)
            {
                this.waveOutDevice.Dispose();
                this.waveOutDevice = null;
            }
        }

        public void Play(string fileName)
        {
            Task.Factory.StartNew(
                () =>
                    {
                       var reader = new Mp3FileReader(fileName);
            var waveOut = new WaveOut(); // or WaveOutEvent()
            waveOut.Init(reader);
            waveOut.Play();
            while (waveOut.PlaybackState == PlaybackState.Playing)
            {
                Thread.Sleep(500);
            }
                    });

            
        }

        public void PlayWav(string filename)
        {
            if (!filename.Contains(".wav"))
            {
                Console.WriteLine($"Only .wav files can be played using the sound player.");
                return;
            }
            var simpleSound = new SoundPlayer(filename);
            simpleSound.Play();
        }

        public List<string> LoadAllAudioFromDirectory(string path)
        {
            var wavFileNames = Directory.GetFiles(path, "*.wav");
            var allAudio = wavFileNames.ToList();
            var mp3FileNames = Directory.GetFiles(path, "*.mp3");
            allAudio.AddRange(mp3FileNames);
            return allAudio;
        }
    }
}