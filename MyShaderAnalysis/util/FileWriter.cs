using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using HandleOutputWrite = MyShaderFile.CompiledShader.HandleOutputWrite;

#pragma warning disable IDE0003 // Remove qualification
#pragma warning disable IDE0017 // Simplify object initialization
namespace MyShaderAnalysis.util
{
    public class FileWriter : IDisposable
    {
        private StreamWriter sw;
        private bool swOpen = true;

        private readonly bool showOutputToConsole;
        private readonly string newLineCharacters;

        /*
         * writes an html footer if a call to WriteHtmlHeader(..) has been made
         */
        private bool writeAsHtml = false;

        public const int SAVE_UTF_FILE_WITH_BOM = 0; // this seems to be what they like at Microsoft
        public const int SAVE_UTF_FILE_WITHOUT_BOM = 1;
        public const int LINUX_ENDINGS = 0;
        public const int WINDOWS_ENDINGS = 1;

        public FileWriter(
            string outputFilenamepath,
            bool showOutputToConsole = false,
            int EOL = WINDOWS_ENDINGS,
            int useBom = SAVE_UTF_FILE_WITHOUT_BOM
            )
        {
            this.showOutputToConsole = showOutputToConsole;
            this.newLineCharacters = EOL switch
            {
                LINUX_ENDINGS => "\n",
                WINDOWS_ENDINGS => "\r\n",
                _ => throw new Exception($"Unrecognized value for EOL {EOL}")
            };
            bool saveUtfFileWithBom = useBom switch
            {
                SAVE_UTF_FILE_WITH_BOM => true,
                SAVE_UTF_FILE_WITHOUT_BOM => false,
                _ => throw new Exception($"Unrecognized value for useBom {useBom}")
            };
            Debug.WriteLine($"Writing to {outputFilenamepath}");
            sw = new StreamWriter(outputFilenamepath, false, new UTF8Encoding(saveUtfFileWithBom));
            sw.NewLine = newLineCharacters;
        }

        public HandleOutputWrite GetOutputWriter()
        {
            if (showOutputToConsole)
            {
                return (x) =>
                {
                    sw.Write(x);
                    Console.Write(x);
                };
            }
            else
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

        /*
         * If Write("..") contains /n or /r/n it can mess up the specified line endings
         *
         */
        public void Write(string text)
        {
            sw.Write(text.ReplaceLineEndings(newLineCharacters));
            if (showOutputToConsole)
            {
                Debug.Write($"{text}");
            }
        }

        public void WriteLine(string text)
        {
            sw.WriteLine(text);
            if (showOutputToConsole)
            {
                Debug.WriteLine($"{text}");
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

