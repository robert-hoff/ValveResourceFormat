using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace ValveTrials
{
    // from
    // https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.design.byteviewer?view=windowsdesktop-6.0
    public class ByteViewerForm : Form
    {
        private Button button1;
        private Button button2;
        private Button button3;
        private ByteViewer byteviewer;


        /* ByteViewer class
         *
         * for some reason can't select text and the text doesn't scroll on mouse-wheel (pretty weird really)
         *
         * https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.design.byteviewer?view=windowsdesktop-6.0
         * inherits from Control, Panel, ScrollableControl, Panel, TableLayoutPanel
         * The Control class has a particularly large number of methods and settings
         * https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.design.byteviewer?view=windowsdesktop-6.0
         *
         * using example files here
         * X:\checkouts\SPIRV-Cross\Debug
         *
         * This looks like the *source code* for the class
         * http://www.dotnetframework.org/default.aspx/DotNET/DotNET/8@0/untmp/whidbey/REDBITS/ndp/fx/src/Designer/CompMod/System/ComponentModel/Design/ByteViewer@cs/1/ByteViewer@cs
         *
         * It looks like when the byte viewer is in hex mode, it hides the text box field and instead uses OnPaint() graphics to draw
         * the byte view as a bitmap
         *
         *
         */
        public ByteViewerForm()
        {
            // Initialize the controls other than the ByteViewer.
            InitializeForm();

            // Location = new Point(500,100);


            // Initialize the ByteViewer.
            byteviewer = new ByteViewer
            {
                // this is the location of the byteviewer within the form (NOT the window itself)
                Location = new Point(8, 46),
                Size = new Size(400, 500),
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top,
                // Click += formClickEvent,

        };
            byteviewer.SetBytes(new byte[] { });


            // R: this actually works, it only responds to click if in the hex view mode
            // byteviewer.Click += formClickEvent;

            // R: never triggered ..
            // byteviewer.GotFocus += formClickEvent;

            // R: this triggers ..
            // byteviewer.MouseEnter += formClickEvent;


            // R: I've been trying different things here, but maybe this should go in InitializeForm() method?
            // well actually the InitializeForm() called above, first thing in the constructor
            byteviewer.SetDisplayMode(DisplayMode.Hexdump);
            // byteviewer.SetDisplayMode(DisplayMode.Ansi);
            // byteviewer.AutoScroll = true;
            // Console.WriteLine($"{byteviewer.CanSelect}");
            // byteviewer.Enabled = true;
            // Console.WriteLine($"{byteviewer.Focused}");
            // Console.WriteLine($"{byteviewer.CanFocus}");
            // Console.WriteLine($"{byteviewer.Select() }");
            // Console.WriteLine($"{byteviewer.ColumnCount}");
            // byteviewer.Select();
            this.Controls.Add(byteviewer);



            // this works .. responds to changes in the scroll bar (not exactly meaningful though)
            // VScrollBar scrollBar = (VScrollBar)byteviewer.Controls[0];
            // scrollBar.ValueChanged += vScrollBar1_ValueChanged;
        }



        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Close();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


        // Show a file selection dialog and cues the byte viewer to
        // load the data in a selected file.
        private void loadBytesFromFile(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            byteviewer.SetFile(ofd.FileName);
        }

        // Clear the bytes in the byte viewer.
        private void clearBytes(object sender, EventArgs e)
        {
            byteviewer.SetBytes(new byte[] { });
        }

        private void myEventHandler(object sender, EventArgs e)
        {
            // Console.WriteLine($"{sender}"); // sender is the button
            // Console.WriteLine($"{this}");   // this is the byte viewer form (not the byte viewer)

            // Console.WriteLine($"{byteviewer.ColumnCount}");


            TableLayoutControlCollection controlsCollection = byteviewer.Controls;

            // this returns '2'
            // Console.WriteLine($"{controlsCollection.Count}");
            // Console.WriteLine($"{controlsCollection[0]}"); // System.Windows.Forms.VScrollBar
            // Console.WriteLine($"{controlsCollection[1]}"); // System.Windows.Forms.TextBox

            // R: this sometimes prints the text found in the textfield, but not always .. (not when in hexmode)
            // Console.WriteLine($"Text: {controlsCollection[1].Text}");
            // controlsCollection[1].Text = "hello";


            // Console.WriteLine($"{byteviewer.Enabled}");


            // R: the bytes are loaded into the Byte[] object, but the results are not displayed in a control that I can find
            // through the Controls property
            // Console.WriteLine($"{byteviewer.GetBytes().Length}");



            // Console.WriteLine($"{byteviewer.Enabled}");
            // Console.WriteLine($"{byteviewer.Focused}");
            // Console.WriteLine($"{byteviewer.HasChildren}"); // returns true, but this seems to relate to byteviewer.Controls (is true if >0)
            // Console.WriteLine($"{byteviewer.GetChildAtPoint(new Point(10,10))}");


            // byteviewer.Focus();
            // byteviewer.Hide();


            // Console.WriteLine($"{controlsCollection[1].CanFocus}"); // returns false
            // Console.WriteLine($"{controlsCollection[1].Enabled}");  // returns true (possibly about user interaction)


            // Console.WriteLine($"{controlsCollection.Count}");
            // Console.WriteLine($"{controlsCollection[1]}");
            // Console.WriteLine($"{controlsCollection[1].Controls}");

            // byteviewer.scroll


            // controlsCollection[0].Hide();
            // controlsCollection[1].Hide();


            // Console.WriteLine($"{controlsCollection[0]}");
            // Console.WriteLine($"{controlsCollection[1]}");
            // Console.WriteLine($"{byteviewer.GetColumn(controlsCollection[0])}");


            // this works in ansi mode but not in hex mode. It's like the Text box doesn't
            // exist when in hex mode. It seems it is not displaying the text box, it is displaying something else
            // Console.WriteLine($"{byteviewer.GetChildAtPoint(new Point(10, 10))}");



            // Console.WriteLine($"{byteviewer.RowCount}"); // always 1
            // Console.WriteLine($"{byteviewer.ColumnCount}"); // always 1



        }


        private void vScrollBar1_ValueChanged(Object sender, EventArgs e)
        {
            Console.WriteLine($"scroll value changed");
        }

        private void formClickEvent(object sender, EventArgs e)
        {
            Console.WriteLine($"click click");
        }



        // Changes the display mode of the byte viewer according to the
        // Text property of the RadioButton sender control.
        private void changeByteMode(object sender, EventArgs e)
        {
            RadioButton rbutton = (RadioButton)sender;
            var mode = rbutton.Text switch
            {
                "ANSI" => DisplayMode.Ansi,
                "Hex" => DisplayMode.Hexdump,
                "Unicode" => DisplayMode.Unicode,
                _ => DisplayMode.Auto,
            };

            // Sets the display mode.
            byteviewer.SetDisplayMode(mode);
        }

        private void InitializeForm()
        {
            this.SuspendLayout();
            // R: these things control the size of the windows ..
            this.ClientSize = new Size(680, 440);
            this.MinimumSize = new Size(660, 400);
            this.Size = new Size(800, 600);

            // R: Location setting only works if StartPosition is set to Manual
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(500, 100);

            this.Name = "Byte Viewer Form";
            this.Text = "Byte Viewer Form";
            this.button1 = new Button
            {
                Location = new Point(8, 8),
                Size = new(100, 23),
                Name = "button1",
                Text = "Set Bytes From File...",
                TabIndex = 0
            };
            this.button1.Click += new EventHandler(this.loadBytesFromFile);
            this.Controls.Add(this.button1);
            this.button2 = new Button
            {
                Location = new Point(108, 8),
                Size = new Size(90, 23),
                Name = "button2",
                Text = "Clear Bytes"
            };
            this.button2.Click += new EventHandler(this.clearBytes);
            this.button2.TabIndex = 1;
            this.Controls.Add(this.button2);


            this.button3 = new Button
            {
                Location = new System.Drawing.Point(198, 8),
                Size = new System.Drawing.Size(90, 23),
                Name = "button3",
                Text = "Click me"
            };
            this.button3.Click += new EventHandler(this.myEventHandler);
            this.button3.TabIndex = 2;
            this.Controls.Add(this.button3);

            GroupBox group = new System.Windows.Forms.GroupBox
            {
                Location = new Point(418, 3),
                Size = new Size(220, 36),
                Text = "Display Mode"
            };
            this.Controls.Add(group);

            RadioButton rbutton1 = new System.Windows.Forms.RadioButton
            {
                Location = new Point(6, 15),
                Size = new Size(46, 16),
                Text = "Auto",
                Checked = true
            };
            rbutton1.Click += new EventHandler(this.changeByteMode);
            group.Controls.Add(rbutton1);

            RadioButton rbutton2 = new System.Windows.Forms.RadioButton
            {
                Location = new Point(54, 15),
                Size = new Size(50, 16),
                Text = "ANSI"
            };
            rbutton2.Click += new EventHandler(this.changeByteMode);
            group.Controls.Add(rbutton2);

            RadioButton rbutton3 = new System.Windows.Forms.RadioButton
            {
                Location = new Point(106, 15),
                Size = new Size(46, 16),
                Text = "Hex"
            };
            rbutton3.Click += new EventHandler(this.changeByteMode);
            group.Controls.Add(rbutton3);

            RadioButton rbutton4 = new System.Windows.Forms.RadioButton
            {
                Location = new Point(152, 15),
                Size = new Size(64, 16),
                Text = "Unicode"
            };
            rbutton4.Click += new EventHandler(this.changeByteMode);
            group.Controls.Add(rbutton4);
            this.ResumeLayout(false);
        }


    }
}
