using System.Windows.Forms;
using MyGUI.Utils;

namespace MyGUI.Types.Viewers
{
    public interface IViewer
    {
        public TabPage Create(VrfGuiContext vrfGuiContext, byte[] input);
    }
}
