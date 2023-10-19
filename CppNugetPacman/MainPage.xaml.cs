using CommunityToolkit.WinUI;
using CppNugetPacman.Models;
using CppNugetPacman.Models.Data;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Pickers;
using WinRT.Interop;

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

        //_ = this.TestAsync();
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

    private async void OnButtonOpenClick(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker();
        picker.SuggestedStartLocation = PickerLocationId.Desktop;

        picker.FileTypeFilter.Add(".sln");

        InitializeWithWindow.Initialize(picker, Process.GetCurrentProcess().MainWindowHandle);

        var file=await picker.PickSingleFileAsync();
        if (file == null) return;

        var solution = new MSolution();
        var success=await solution.LoadAsync(file.Path);
        if (!success) return;

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
            await this.Solution.ReloadAsync();


            var p = this.Solution.Packages.FirstOrDefault(x => x.Data.Id == package.Data.Id);
            _treeView.SelectedItem = p;

        }

        if (item is VmProject project)
        {
            var p=this.Solution.Packages.First(p => p.Projects.Contains(project));
            await project.ApplyAsync(p, location, version);
            await this.Solution.ReloadAsync();

            await Task.Delay(100);
            this.DispatcherQueue.TryEnqueue(() =>
            {
                p = this.Solution.Packages.FirstOrDefault(x => x.Projects.FirstOrDefault(p => p.Data.Name == project.Data.Name) != null);
                var container = _treeView.ContainerFromItem(p);
                var node = _treeView.NodeFromContainer(container);
                _treeView.Expand(node);
                var proj = p.Projects.FirstOrDefault(x => x.Title == project.Title);
                _treeView.SelectedItem = proj;
            });
            
        }




    }


    private void OnPageDropCompleted(UIElement sender, DropCompletedEventArgs args)
    {

    }

    private void OnPageDragEnter(object sender, DragEventArgs e)
    {

    }

    private async void OnPageDragOver(object sender, DragEventArgs e)
    {
        var deferral = e.GetDeferral(); try
        {
            if (!e.DataView.AvailableFormats.Contains(StandardDataFormats.StorageItems))
            {
                e.AcceptedOperation = DataPackageOperation.None;
                return;
            }

            var items = await e.DataView.GetStorageItemsAsync();

            var item = items.FirstOrDefault(x => Path.GetExtension(x.Path).Equals(".sln", StringComparison.OrdinalIgnoreCase));
            if (item == null)
            {
                e.AcceptedOperation = DataPackageOperation.None;
                return;
            }

            e.AcceptedOperation = DataPackageOperation.Link;
            return;
        }
        finally
        {
            deferral.Complete();
        }
        
    }



    
}
