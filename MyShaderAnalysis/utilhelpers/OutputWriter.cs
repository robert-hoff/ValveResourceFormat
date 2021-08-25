using MyShaderAnalysis.vcsparsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using static MyShaderAnalysis.vcsparsing.ShaderUtilHelpers;


namespace MyShaderAnalysis.utilhelpers
{

    public class OutputWriter : IDisposable
    {
        public StreamWriter sw { get; private set; }
        private bool WriteToConsole = false;
        private bool WriteToDebug = true;
        private bool writeAsHtml = false;
        private bool swWriterAlreadyClosed = false;

        public OutputWriter(bool WriteToConsole = true, bool WriteToDebug = false)
        {
            this.WriteToConsole = WriteToConsole;
            this.WriteToDebug = WriteToDebug;
        }

        public void SetOutputFile(string filenamepath)
        {
            Debug.WriteLine($"writing to {filenamepath}");
            sw = new StreamWriter(filenamepath);
            swWriterAlreadyClosed = false;
        }

        public void WriteAsHtml(string htmlTitle, string htmlHeader)
        {
            writeAsHtml = true;
            sw.WriteLine(GetHtmlHeader(htmlTitle, htmlHeader));
        }

        public void DisableOutput()
        {
            WriteToConsole = false;
            WriteToDebug = false;
        }


        public void Write(string text)
        {
            if (WriteToConsole)
            {
                Console.Write(text);
            }
            if (WriteToDebug)
            {
                Debug.Write(text);
            }
            if (sw != null)
            {
                sw.Write(text);
            }
        }

        public void WriteLine(string text)
        {
            Write(text + "\n");
        }

        public void BreakLine()
        {
            Write("\n");
        }

        public void CloseStreamWriter()
        {
            if (writeAsHtml && !swWriterAlreadyClosed)
            {
                sw.WriteLine(GetHtmlFooter());
            }
            if (sw != null && !swWriterAlreadyClosed)
            {
                sw.Close();
                swWriterAlreadyClosed = true;
            }
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
                sw.Dispose();
                sw = null;
            }
        }



        private List<string> headerValues;
        private List<List<string>> tabulatedValues;
        private List<int> columnWidths;

        public void DefineHeaders(string[] headers)
        {
            headerValues = new();
            tabulatedValues = new();
            columnWidths = new();
            foreach (string s in headers)
            {
                headerValues.Add(s);
                columnWidths.Add(s.Length);
            }
            tabulatedValues.Add(headerValues);
        }

        public void AddTabulatedRow(string[] rowMembers)
        {
            if (headerValues.Count != rowMembers.Length)
            {
                throw new ShaderParserException("wrong number of columns");
            }
            List<string> newRow = new();
            List<List<string>> additionalRows = new();

            for (int i = 0; i < rowMembers.Length; i++)
            {
                string[] multipleLines = rowMembers[i].Split("\n");
                if (multipleLines.Length > 1)
                {
                    addExtraLines(additionalRows, multipleLines, i);
                }

                newRow.Add(multipleLines[0]);
                if (multipleLines[0].Length > columnWidths[i])
                {
                    columnWidths[i] = multipleLines[0].Length;
                }
            }
            tabulatedValues.Add(newRow);
            foreach (var additionalRow in additionalRows)
            {
                tabulatedValues.Add(additionalRow);
            }
        }

        private void addExtraLines(List<List<string>> additionalRows, string[] multipleLines, int ind)
        {
            for (int i = 1; i < multipleLines.Length; i++)
            {
                if (additionalRows.Count < i)
                {
                    additionalRows.Add(emptyRow());
                }
                additionalRows[i - 1][ind] = multipleLines[i];

                if (multipleLines[i].Length > columnWidths[ind])
                {
                    columnWidths[ind] = multipleLines[i].Length;
                }
            }
        }


        private List<string> emptyRow()
        {
            List<string> newRow = new();
            for (int i = 0; i < headerValues.Count; i++)
            {
                newRow.Add("");
            }
            return newRow;
        }

        public void printTabulatedValues(int spacing = 2)
        {
            if (tabulatedValues.Count == 1 && tabulatedValues[0].Count == 0)
            {
                return;
            }
            foreach (var row in tabulatedValues)
            {
                for (int i = 0; i < row.Count; i++)
                {
                    int pad = columnWidths[i] + spacing;
                    Write($"{row[i].PadRight(pad)}");
                }
                Write("\n");
            }
        }




    }
}




