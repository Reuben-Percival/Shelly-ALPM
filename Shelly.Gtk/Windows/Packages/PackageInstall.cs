using Gtk;

namespace Shelly.Gtk.Windows.Packages;

public class PackageInstall : IShellyWindow
{
    public Widget CreateWindow()
    {
        var builder = Builder.NewFromFile("UiFiles/Package/PackageWindow.ui");
        return (Overlay)builder.GetObject("PackageWindow")!;
    }
}