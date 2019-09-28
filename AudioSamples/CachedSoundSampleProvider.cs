namespace AudioSamples
{
    using System;

    using NAudio.Wave;

    class CachedSoundSampleProvider : ISampleProvider
    {
        private readonly CachedSound cachedSound;
        private long position;

        public CachedSoundSampleProvider(CachedSound cachedSound)
        {
            this.cachedSound = cachedSound;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            var availableSamples = this.cachedSound.AudioData.Length - this.position;
            var samplesToCopy = Math.Min(availableSamples, count);
            Array.Copy(this.cachedSound.AudioData, this.position, buffer, offset, samplesToCopy);
            this.position += samplesToCopy;
            return (int)samplesToCopy;
        }

        public WaveFormat WaveFormat { get { return this.cachedSound.WaveFormat; } }
    }
}