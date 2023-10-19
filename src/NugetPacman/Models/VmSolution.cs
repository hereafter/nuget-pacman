using NugetPacman.Models.Data;

namespace NugetPacman.Models;

public partial class VmSolution : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<VmProject> _projects = new();

    [ObservableProperty]
    private ObservableCollection<VmPackage> _packages = new();


    public MSolution? Data { get; private set; } = null;

    public VmSolution(MSolution data)
    {
        this.Data = data;
        this.UpdateData();

    }

    public async Task ReloadAsync()
    {
        if (this.Data == null) return;
        await this.Data.ReloadAsync();
        this.UpdateData();
    }

    public void UpdateData()
    {
        var data = this.Data;


        this.Projects.Clear();
        this.Packages.Clear();

        if (data == null) return;


        foreach (var proj in data.Projects)
        {
            this.Projects.Add(new VmProject(proj));
        }


        var packages = data.Projects.SelectMany(x => x.Packages).DistinctBy(x => x.Id).ToList();
        foreach (var p in packages)
        {
            var package = new VmPackage(p);

            foreach (var proj in this.Projects.Where(x => x.Packages.FirstOrDefault(x => x.Data.Id == p.Id) != null))
            {
                var vp = new VmProject(proj.Data);
                vp.Update(p.Id);
                package.Projects.Add(vp);
            }


            this.Packages.Add(package);
        }
    }

}
