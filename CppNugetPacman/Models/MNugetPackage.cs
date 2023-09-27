namespace CppNugetPacman.Models;

public class MNugetPackage
{
    public string Id { get; set; } = string.Empty;
    public IEnumerable<string> Versions { get; set; } = Array.Empty<string>();
    public string Path = string.Empty;

    public string Version { get; set; } = string.Empty;

    public bool IsSolutionDirBased { get; set; } = false;
}
