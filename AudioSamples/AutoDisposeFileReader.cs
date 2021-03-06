namespace AudioSamples
{
    using NAudio.Wave;

    class AutoDisposeFileReader : ISampleProvider
    {
        private readonly AudioFileReader reader;
        private bool isDisposed;
        public AutoDisposeFileReader(AudioFileReader reader)
        {
            this.reader = reader;
            this.WaveFormat = reader.WaveFormat;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            if (this.isDisposed)
                return 0;
            int read = this.reader.Read(buffer, offset, count);
            if (read == 0)
            {
                this.reader.Dispose();
                this.isDisposed = true;
            }
            return read;
        }

        public WaveFormat WaveFormat { get; private set; }
    }
}