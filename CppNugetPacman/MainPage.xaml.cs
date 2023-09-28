using CppNugetPacman.Models.Data;

namespace CppNugetPacman;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();

        _ = this.TestAsync();
    }

    async Task TestAsync()
    {
        MSolution solution = new();
        await solution.LoadAsync(@"P:\Projects\apps\taskbar-calendar\src\taskbar-calendar.sln");
    }
}
