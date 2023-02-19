using System;
using System.IO;
using HandleOutputWrite = MyShaderFile.CompiledShader.HandleOutputWrite;

namespace MyShaderAnalysis.util
{
    public class FileWriter : IDisposable
    {
        private StreamWriter sw;
        private bool writeToConsole;
        private bool writeAsHtml = false; // is set to true if html header is written
        private bool swOpen = true;

        public FileWriter(string outputFilenamepath, bool showOutputToConsole = false, bool writeAsHtml = false)
        {
            Console.WriteLine($"Writing to {outputFilenamepath}");
            sw = new StreamWriter(outputFilenamepath);
            if (writeAsHtml)
            {
                WriteHtmlHeader("output", "");
            }
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
            swOpen = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && sw != null)
            {
                if (swOpen)
                {
                    CloseStreamWriter();
                }
                sw.Dispose();
                sw = null;
            }
        }

        public void WriteLine(string text)
        {
            sw.WriteLine(text);
            if (writeToConsole)
            {
                Console.WriteLine($"{text}");
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

