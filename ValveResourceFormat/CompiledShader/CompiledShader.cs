using System;
using System.IO;
using ValveResourceFormat.ShaderParser;

namespace ValveResourceFormat
{
    public class CompiledShader : IDisposable
    {
        public const int MAGIC = 0x32736376; // "vcs2"
        public const uint ZSTD_DELIM = 0xFFFFFFFD;
        public const uint LZMA_DELIM = 0x414D5A4C;
        public const int ZSTD_COMPRESSION = 1;
        public const int LZMA_COMPRESSION = 2;
        public const uint PI_MURMURSEED = 0x31415926;
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

            // todo - let the user switch between byte printout and summary
            // shaderFile.PrintByteAnalysis();
            PrintSingleFileSummary fileSummary = new PrintSingleFileSummary(shaderFile);



            // todo - let the user select their own zframe
            // print a few zframes, if there are any
            //for (int i = 0; i < Math.Min(3, shaderFile.GetZFrameCount()); i++)
            //{
            //    string headerText = $"Byte printout of zframe[{shaderFile.GetZFrameIdByIndex(i):x08}], {Path.GetFileName(filenamepath)}";
            //    Console.WriteLine($"\n\n{headerText}");
            //    Console.WriteLine($"{new string('-', headerText.Length)}");
            //    Console.WriteLine($"{new string('-', headerText.Length)}\n");
            //    shaderFile.GetZFrameFile(0).PrintByteAnalysis();
            //}
        }

    }
}
