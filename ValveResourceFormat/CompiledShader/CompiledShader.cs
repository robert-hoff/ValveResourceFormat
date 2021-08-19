using System;
using System.IO;
using ValveResourceFormat.ShaderParser;

namespace ValveResourceFormat
{
    public class CompiledShader : IDisposable
    {
        public const int MAGIC = 0x32736376; // "vcs2"
        private ShaderDataReader datareader;

        /// <summary>
        /// Releases binary reader.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && datareader != null)
            {
                datareader.Dispose();
                datareader = null;
            }
        }

        /// <summary>
        /// Opens and reads the given filename.
        /// The file is held open until the object is disposed.
        /// </summary>
        /// <param name="filenamepath">The file to open and read.</param>
        public void Read(string filenamepath)
        {
            var fs = new FileStream(filenamepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            Read(filenamepath, fs);
        }

        /// <summary>
        /// Reads the given <see cref="Stream"/>.
        /// </summary>
        /// <param name="filename">The filename <see cref="string"/>.</param>
        /// <param name="input">The input <see cref="Stream"/> to read from.</param>
        public void Read(string filenamepath, Stream input)
        {
            datareader = new ShaderDataReader(input);
            ShaderFile shaderFile = new ShaderFile(filenamepath, datareader);
            shaderFile.PrintByteAnalysis();


            // shaderFile.GetDecompressedZFrame(0); // retrieves a decompressed zframe
        }

    }
}
