using NugetPacman.Models.Data;

namespace NugetPacman.Models;

public partial class VmProject : VmNode
{
    [ObservableProperty]
    private ObservableCollection<VmPackage> _packages = new();

    [ObservableProperty]
    private string _version = string.Empty;

    [ObservableProperty]
    private string _folderPath = string.Empty;

    [ObservableProperty]
    private bool _isUnique = false;

    [ObservableProperty]
    private string _details = string.Empty;


    public MProject Data { get; private set; }

    public VmProject(MProject data)
    {
        this.Data = data;

        this.Title = data.Name;

        foreach (var p in data.Packages)
        {
            this.Packages.Add(new VmPackage(p));
        }
    }

    public void Update(string id)
    {
        var package = this.Packages.FirstOrDefault(p => p.Data.Id == id);
        if (package == null) return;

        this.Version = package.Version;
        this.FolderPath = package.FolderPath;

        this.IsUnique = package.IsUnique;
        this.Details = package.Details;
    }

    public async Task ApplyAsync(VmPackage package, string location, string version)
    {
        var packageData = this.Data.Packages.FirstOrDefault(x => x.Id == package.Title);
        if (packageData == null) return;


        //apply path

        if (location != packageData.Path)
        {
            await this.Data.UpdateLocalFolderPathAsync(package.Data.Id, location);
        }

        if (version != packageData.Version)
        {
            await this.Data.UpdatePackageVersionAsync(package.Data.Id, version);
        }

    }
}
