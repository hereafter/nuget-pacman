using CppNugetPacman.Models.Data;

namespace CppNugetPacman.Models;

public partial class VmPackage: VmNode
{    
    [ObservableProperty]
    private bool _isUnique = false;

    [ObservableProperty]
    private string _folderPath = string.Empty;

    [ObservableProperty]
    private string _version = string.Empty;

    [ObservableProperty]
    private string _details = string.Empty;

    [ObservableProperty]
    private ObservableCollection<VmProject> _projects = new();

    public MNugetPackage Data { get; set; }


    public VmPackage(MNugetPackage data)
    {
        this.Data = data;

        this.Title = data.Id;
        this.IsUnique= data.IsUnique;
        this.FolderPath = data.Path;
        this.Version = data.Version;

        this.IsUnique = data.IsUnique;
        this.FolderPath = data.Path;

        var sb = new StringBuilder();


        sb.AppendLine("Folders: ");

        foreach (var p in data.Paths)
        {
            sb.AppendLine($"  {p}");
        }

        sb.AppendLine("Versions: ");

        foreach(var v in data.Versions)
        {
            sb.AppendLine($"  {v}");
        }

        this.Details = sb.ToString();
    }

    
    public async Task ApplyAsync(string location, string version)
    {
        foreach(var project in this.Projects)
        {
            await project.ApplyAsync(this, location, version);
        }
    }

}




