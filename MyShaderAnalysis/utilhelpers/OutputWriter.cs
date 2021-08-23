using MyShaderAnalysis.vcsparsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyShaderAnalysis.vcsparsing.ShaderUtilHelpers;


namespace MyShaderAnalysis.utilhelpers
{

    public class OutputWriter : IDisposable
    {

        public OutputWriter()
        {

        }


        private bool WriteToConsole = false;
        private bool WriteToDebug = true;
        private StreamWriter sw;
        private bool writeAsHtml = false;
        private bool swWriterAlreadyClosed = false;

        public void SetOutputFile(string filenamepath)
        {
            Debug.WriteLine($"writing to {filenamepath}");
            sw = new StreamWriter(filenamepath);
        }

        public void WriteAsHtml(string htmlTitle, string htmlHeader)
        {
            writeAsHtml = true;
            sw.WriteLine(GetHtmlHeader(htmlTitle, htmlHeader));
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
            //foreach (string s in rowMembers)
            //{
            //    newRow.Add(s);
            //}
            for (int i = 0; i < rowMembers.Length; i++)
            {
                newRow.Add(rowMembers[i]);
                if (rowMembers[i].Length > columnWidths[i])
                {
                    columnWidths[i] = rowMembers[i].Length;
                }

            }
            tabulatedValues.Add(newRow);
        }

        public void printTabulatedValues()
        {
            foreach (var row in tabulatedValues)
            {
                for (int i = 0; i < row.Count; i++)
                {
                    int pad = columnWidths[i] + 2;
                    Write($"{row[i].PadRight(pad)}");
                }
                Write("\n");
            }
        }




    }
}




