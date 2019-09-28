namespace AudioSamples
{
    using System;

    using NAudio.Wave;
    using NAudio.Wave.SampleProviders;

    public class AudioPlaybackEngine : IDisposable
    {
        private readonly IWavePlayer outputDevice;
        private readonly MixingSampleProvider mixer;

        public AudioPlaybackEngine(int sampleRate = 44100, int channelCount = 2)
        {
            this.outputDevice = new WaveOutEvent();
            this.mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount));
            this.mixer.ReadFully = true;
            this.outputDevice.Init(this.mixer);
            this.outputDevice.Play();
        }

        public void PlaySound(string fileName)
        {
            var input = new AudioFileReader(fileName);
            this.AddMixerInput(new AutoDisposeFileReader(input));
        }

        private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
        {
            if (input.WaveFormat.Channels == this.mixer.WaveFormat.Channels)
            {
                return input;
            }
            if (input.WaveFormat.Channels == 1 && this.mixer.WaveFormat.Channels == 2)
            {
                return new MonoToStereoSampleProvider(input);
            }
            throw new NotImplementedException("Not yet implemented this channel count conversion");
        }

        public void PlaySound(CachedSound sound)
        {
            this.AddMixerInput(new CachedSoundSampleProvider(sound));
        }

        private void AddMixerInput(ISampleProvider input)
        {
            this.mixer.AddMixerInput(this.ConvertToRightChannelCount(input));
        }

        public void Dispose()
        {
            this.outputDevice.Dispose();
        }

        public static readonly AudioPlaybackEngine Instance = new AudioPlaybackEngine(44100, 2);
    }
}