using System.Drawing;
using System.Windows.Forms;

namespace MyNamespace
{
    public class MyClass
    {
        public void Create()
        {
            // This Font.Size doesn't work here
            //var myRichTextBox = new RichTextBox
            //{
            //    Font = new Font(FontFamily.GenericMonospace, Font.Size),
            //};
        }



        private class MyRichTextBox : RichTextBox
        {
            public MyRichTextBox()
            {
                Font = new Font(FontFamily.GenericMonospace, this.Font.Size);
            }
        }
    }
}
