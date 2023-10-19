using System.Diagnostics;

namespace NugetPacman.Models.Data;

public class MSolution : MItem
{
    private const string VisualStudioFileFormatHeader = "Microsoft Visual Studio Solution File, Format Version";
    private const string VisualStudioProjectLead = "Project(";

    public IEnumerable<MProject> Projects { get; set; } = new List<MProject>();

    public string FormatVersion { get; private set; } = string.Empty;

    public async Task<bool> LoadAsync(string filePath)
    {
        if (!filePath.EndsWith(".sln", StringComparison.OrdinalIgnoreCase))
            return false;

        var lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8);

        var signature = lines.FirstOrDefault(l => l.StartsWith(VisualStudioFileFormatHeader));
        if (signature == null) return false;

        FormatVersion = signature.Replace(VisualStudioFileFormatHeader, string.Empty).Trim();

        FilePath = filePath;
        FolderPath = Path.GetDirectoryName(filePath);

        if (FolderPath == null) return false;

        var projects = new List<MProject>();
        foreach (var line in lines)
        {
            if (!line.StartsWith(VisualStudioProjectLead, StringComparison.OrdinalIgnoreCase))
                continue;

            var segments = line.Split('=');
            if (segments.Length < 2) continue;

            segments = segments[1].Trim().Split(',');
            if (segments.Length < 3) continue;

            var name = segments[0].Trim().Trim('\"').Trim();
            var path = segments[1].Trim().Trim('\"').Trim();

            if (!path.EndsWith(".vcxproj", StringComparison.OrdinalIgnoreCase)) continue;



            var proj = new MProject();
            if (!Path.IsPathRooted(path))
            {
                path = Path.GetFullPath(Path.Combine(FolderPath, path));
            }

            Debug.WriteLine($"<!> {name}: {path}");
            proj.Name = name;
            if (await proj.LoadAsync(path))
            {
                projects.Add(proj);
            }


        }

        Projects = projects;
        return true;
    }

    public async Task<bool> ReloadAsync()
    {
        return await this.LoadAsync(this.FilePath ?? string.Empty);
    }


}
