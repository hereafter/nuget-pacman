using CommunityToolkit.WinUI;
using CppNugetPacman.Models;
using CppNugetPacman.Models.Data;
using Microsoft.UI.Xaml.Input;

namespace CppNugetPacman;

public sealed partial class MainPage : Page
{
    public VmSolution? Solution
    {
        get { return (VmSolution?)GetValue(SolutionProperty); }
        set { SetValue(SolutionProperty, value); }
    }

    public static readonly DependencyProperty SolutionProperty =
        DependencyProperty.Register("Solution", typeof(VmSolution), typeof(MainPage), new PropertyMetadata(null));


    public MainPage()
    {
        this.InitializeComponent();

        _ = this.TestAsync();
    }

    async Task TestAsync()
    {
        MSolution solution = new();
        await solution.LoadAsync(@"P:\Projects\apps\taskbar-calendar\src\taskbar-calendar.sln");

        await this.DispatcherQueue.EnqueueAsync(() =>
        {
            this.Solution = new VmSolution(solution);
        });
    }


    private async void OnButtonUpdateClick(object sender, RoutedEventArgs e)
    {
        var item=_treeView.SelectedItem;
        if (item == null) return;

        if (this.Solution == null) return;


        var location = TextBoxFolderPath.Text.Trim();
        var version = TextBoxVersion.Text.Trim();

        if(item is VmPackage package)
        {
            await package.ApplyAsync(location, version);
        }

        if (item is VmProject project)
        {
            var p=this.Solution.Packages.First(p => p.Projects.Contains(project));
            await project.ApplyAsync(p, location, version);
        }
    }
}
