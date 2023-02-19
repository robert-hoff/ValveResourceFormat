using System.Diagnostics;

namespace MyShaderFileKristiker.MyHelperClasses
{
    public class FileWriter : IDisposable
    {
        private StreamWriter sw;
        private bool writeAsHtml = false;
        private bool swOpen = true;

        private static string DEFAULT_DIR = @"Z:\dev\www\vcs.codecreation.dev\GEN-output2\";

        public FileWriter(
            string outputFilenamepath,
            bool writeAsHtml = false,
            bool useDefaultDir = false,
            string htmlTitle = "",
            string htmlHeader = ""
        )
        {
            if (useDefaultDir)
            {
                outputFilenamepath = $"{DEFAULT_DIR}{outputFilenamepath}";
            }

            this.writeAsHtml = writeAsHtml;
            Debug.WriteLine($"Writing to {outputFilenamepath}");
            sw = new StreamWriter(outputFilenamepath);
            if (writeAsHtml)
            {
                if (htmlHeader.Length == 0)
                {
                    string filename = Path.GetFileName(outputFilenamepath);
                    WriteHtmlHeader(filename, filename);
                }
                else
                {
                    WriteHtmlHeader(htmlTitle, htmlHeader);
                }

            }
            sw.NewLine = "\n";
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

        public void Write(string text)
        {
            sw.Write(text);
        }

        public void WriteLine(string text)
        {
            sw.WriteLine(text);
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

