using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
// using System.Windows.Documents;


namespace ValveTrials
{

    /*
     *
     * Flowdocument - maybe a way to do it? don't know how to import it though. Documentation suggests
     * it's part of System.Windows.Documents
     * https://docs.microsoft.com/en-us/dotnet/api/system.windows.documents.flowdocument?view=net-5.0
     *
     *
     *
     *
     * Adding links in RichTextBox
     * https://stackoverflow.com/questions/12303414/add-clickable-hyperlinks-to-a-richtextbox-without-new-paragraph
     * https://stackoverflow.com/questions/9279061/dynamically-adding-hyperlinks-to-a-richtextbox
     *
     *
     *
     *
     *
     *
     */
    class MyRichTextBox : RichTextBox
    {


        public MyRichTextBox() : base()
        {
            Font = new Font(FontFamily.GenericMonospace, Font.Size);
            DetectUrls = true;
            Dock = DockStyle.Fill;
            Multiline = true;
            ReadOnly = true;
            WordWrap = false;
            // Text = "link link <a href=\"\\\\stackoverflow.com\\questions\\435607\">gogo</a>";
            // Text = "link link <a href=\"http://www.stackoverflow.com/\">gogo</a>";

            Text = "link link \\\\hello jhjhjhg jg   \\\\hello\\hello sdfdsf";
            ScrollBars = RichTextBoxScrollBars.Both;


            // FlowDocument doc = new FlowDocument();
            // Paragraph para = new Paragraph();
        }

        public void Link_Clicked(object sender, LinkClickedEventArgs e)
        {
            Debug.WriteLine($"{e.LinkText.ToString()}");
        }


    }
}
