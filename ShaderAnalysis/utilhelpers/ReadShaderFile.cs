using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.ShaderParser;

namespace ShaderAnalysis.utilhelpers
{


    public class ReadShaderFile : IDisposable
    {

        private BinaryReader Reader;
        private ShaderDataReader datareader;

        public ReadShaderFile(string filenamepath)
        {

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && Reader != null)
            {
                Reader.Dispose();
                Reader = null;
            }
        }




    }
}



