using Gtk;

namespace Shelly.Gtk.Windows.AUR;

public class AurInstall() : IShellyWindow
{

    public Widget CreateWindow()
    {
        var builder = Builder.NewFromFile("UiFiles/AUR/AurWindow.ui");
        var box = (Box)builder.GetObject("AurInstallWindow")!;
        

        return box;
    }

}