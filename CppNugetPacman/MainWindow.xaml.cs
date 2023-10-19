using Windows.Win32;
using WinRT.Interop;

namespace CppNugetPacman;


public sealed partial class MainWindow : Window
{



    public MainWindow()
    {
        this.InitializeComponent();
        this.AppWindow.Title = "Nuget Pacman";

        var hWnd=WindowNative.GetWindowHandle(this);
        var dpi = PInvoke.GetDpiForWindow(new Windows.Win32.Foundation.HWND(hWnd))/96;

        this.AppWindow.Resize(new Windows.Graphics.SizeInt32((int)(800*dpi), (int)(600*dpi)));
        

        this.Frame.Navigate(typeof(MainPage));
        
    }



    

    
}
