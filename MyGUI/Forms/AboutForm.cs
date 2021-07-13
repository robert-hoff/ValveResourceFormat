using System.Diagnostics;
using System.Windows.Forms;

namespace MyGUI.Forms
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();

            this.label2.Text = $"Version: {Application.ProductVersion}";
        }

        private void website_Click(object sender, System.EventArgs e)
        {
            OpenUrl("https://vrf.steamdb.info");
        }

        private void github_Click(object sender, System.EventArgs e)
        {
            OpenUrl("https://github.com/SteamDatabase/MyValveResourceFormat");
        }

        private void releases_Click(object sender, System.EventArgs e)
        {
            OpenUrl("https://github.com/SteamDatabase/MyValveResourceFormat/releases");
        }

        private static void OpenUrl(string url)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}")
            {
                CreateNoWindow = true,
            });
        }
    }
}
