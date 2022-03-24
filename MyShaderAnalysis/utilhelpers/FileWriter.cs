using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ValveResourceFormat.CompiledShader.ShaderDataReader;

namespace MyShaderAnalysis.utilhelpers
{
    public class FileWriter : IDisposable
    {
        private StreamWriter sw;
        bool writeToConsole;
        private bool writeAsHtml = false; // is set to true if html header is written

        public FileWriter(string outputFilenamepath, bool showOutputToConsole = true)
        {
            Console.WriteLine($"Writing to {outputFilenamepath}");
            sw = new StreamWriter(outputFilenamepath);
            this.writeToConsole = showOutputToConsole;
        }


        public HandleOutputWrite GetOutputWriter()
        {
            if (writeToConsole)
            {
                return (x) =>
                {
                    sw.Write(x);
                    Console.Write(x);
                };
            } else
            {
                return (x) =>
                {
                    sw.Write(x);
                };
            }
        }
        public void CloseStreamWriter()
        {
            if (writeAsHtml)
            {
                WriteHtmlFooter();
            }
            sw.Close();
        }

        // make sure to close the sw before disposing
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && sw != null)
            {
                sw.Dispose();
                sw = null;
            }
        }


        public void WriteHtmlHeader(string browserTitle, string pageHeader)
        {
            writeAsHtml = true;
            string html_header = "" +
                $"<!DOCTYPE html>\n<html>\n<head>\n  <title>{browserTitle}</title>\n" +
                $"  <link href='/includes/styles.css' rel='stylesheet' type='text/css' />\n" +
                $"</head>\n<body>\n<b>{pageHeader}</b>\n<pre>";
            sw.WriteLine(html_header);
        }

        private void WriteHtmlFooter()
        {
            sw.WriteLine("</pre>\n</html>");
        }
    }
}
