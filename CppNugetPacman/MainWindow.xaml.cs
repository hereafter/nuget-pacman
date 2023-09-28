using CppNugetPacman.Models.Data;
using Microsoft.UI.Xaml;namespace CppNugetPacman;


public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        this.AppWindow.Title = "Nuget Pacman";

        this.Frame.Navigate(typeof(MainPage));
        
    }



    

    
}
