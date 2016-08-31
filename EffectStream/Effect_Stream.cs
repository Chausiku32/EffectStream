using System;

using NAudio.Wave;
using System.Collections.Generic;

namespace EffectStream
{
    class Effect_Stream : WaveStream
    {
        public WaveStream SourceStream
        {
            get;
            set;
        }

        public List<IEffect> Effect { get; private set; }

        public Effect_Stream(WaveStream stream)
        {
            this.SourceStream = stream;
            this.Effect = new List<IEffect>();
        }

        public override long Length
        {
            get { return SourceStream.Length; }
        }

        public override long Position
        {
            get { return SourceStream.Position; }
            set { SourceStream.Position = value; }
        }

        public override WaveFormat WaveFormat
        {
            get { return SourceStream.WaveFormat; }
        }

        private int channel = 0;

        public override int Read(byte[] buffer, int offset, int count)
        {
            Console.WriteLine("DirectSoundOut requested {0} bytes", count);

            int read = SourceStream.Read(buffer,offset,count);

            for (int i = 0; i < read / 4; i++)
            {
                float sample = BitConverter.ToSingle(buffer, i*4);

                if (Effect.Count == WaveFormat.Channels) 
                {
                    sample = Effect[channel].ApplyEffect(sample);
                    channel = (channel + 1) % WaveFormat.Channels;
                }

                byte[] bytes = BitConverter.GetBytes(sample);
                buffer[i * 4 + 0] = bytes[0];
                buffer[i * 4 + 1] = bytes[1];
                buffer[i * 4 + 2] = bytes[2];
                buffer[i * 4 + 3] = bytes[3];
            }
                return read;
        }

    }
}
