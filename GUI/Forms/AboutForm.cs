using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GUI.Forms
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();

            labelVersion.Text = $"Version: {Application.ProductVersion}";
            labelRuntime.Text = $"Runtime: {RuntimeInformation.FrameworkDescription}";
        }

        private void OnWebsiteClick(object sender, System.EventArgs e)
        {
            OpenUrl("https://vrf.steamdb.info");
        }

        private void OnGithubClick(object sender, System.EventArgs e)
        {
            OpenUrl("https://github.com/SteamDatabase/ValveResourceFormat");
        }

        private void OnReleasesClick(object sender, System.EventArgs e)
        {
            OpenUrl("https://github.com/SteamDatabase/ValveResourceFormat/releases");
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
