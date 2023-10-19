namespace NugetPacman.Models.Data;

public class MNugetPackage
{
    public string Id { get; set; } = string.Empty;
    public IEnumerable<string> Versions { get; set; } = Array.Empty<string>();
    public IEnumerable<string> Paths { get; set; } = Array.Empty<string>();



    public string Version
    {
        get => Versions.FirstOrDefault() ?? string.Empty;
        set => Versions = new string[] { value };
    }

    public string Path
    {
        get => Paths.FirstOrDefault() ?? string.Empty;
        set => Paths = new string[] { value };
    }

    public bool IsUnique => Versions.Count() == 1 && Paths.Count() == 1;

    public bool IsSolutionDirBased
    {
        get
        {
            foreach (var path in Paths)
            {
                if (!path.Contains("$(SolutionDir)")) return false;
            }
            return true;
        }
    }
}
