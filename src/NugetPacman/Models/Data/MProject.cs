using System.Diagnostics;
using System.Text.RegularExpressions;

namespace NugetPacman.Models.Data;

public class MProject : MItem
{
    public IEnumerable<MNugetPackage> Packages { get; set; } = Array.Empty<MNugetPackage>();

    public async Task<bool> LoadAsync(string filePath)
    {
        if (!filePath.EndsWith(".vcxproj")) return false;
        FilePath = filePath;
        FolderPath = Path.GetDirectoryName(filePath);
        if (FolderPath == null) return false;

        //package.config
        var options = new EnumerationOptions { RecurseSubdirectories = true };
        var configFilePath = Directory.EnumerateFiles(FolderPath, "packages.config", options).FirstOrDefault();
        if (configFilePath == null) return false;

        var packages = new List<MNugetPackage>();

        var rx = new Regex("(\\S*?)=\"(.*?)\"");

        await this.ParseLinesAsync(configFilePath, (line) =>
        {

            var content = line.Trim();
            if (!content.StartsWith("<package id")) return;

            var matches = rx.Matches(content);
            if (matches.Count < 3) return;

            var id = matches[0].Groups[2].Value.ToString();
            var version = matches[1].Groups[2].Value.ToString();

            var package = new MNugetPackage
            {
                Id = id
            };
            package.Version = version;
            packages.Add(package);
            Debug.WriteLine($"-> {id}: {version}");
        });

        Packages = packages;
        var lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8);

        foreach (var p in Packages)
        {
            var rx1 = new Regex($"'([^'\\\"]*?{p.Id}[^'\\\"]*?)'");
            var rx2 = new Regex($"\"([^'\\\"]*?{p.Id}[^'\\\"]*?)\"");

            var rx3 = new Regex($"{p.Id}(.*?)\\\\");

            var paths = new HashSet<string>();
            var versions = new HashSet<string>
            {
                p.Version
            };

            foreach (var line in lines)
            {
                var matches = rx1.Matches(line);
                if (matches.Count == 0)
                {
                    matches = rx2.Matches(line);
                }

                if (matches.Count == 0) continue;

                foreach (Match m in matches)
                {
                    var value = m.Groups[1].Value;
                    paths.Add(value.Substring(0, value.IndexOf(p.Id)));
                    var tm = rx3.Match(value);
                    if (tm.Success)
                    {
                        versions.Add(tm.Groups[1].Value.Trim('.'));
                    }

                }
            }

            p.Paths = paths.ToArray();
            p.Versions = versions.ToArray();
        }

        return true;
    }

    public async Task<bool> UpdateLocalFolderPathAsync(string pid, string folderPath = "$(SolutionDir)packages\\")
    {
        var filePath = this.FilePath;
        if (filePath == null || !filePath.EndsWith(".vcxproj")) return false;

        FilePath = filePath;
        FolderPath = Path.GetDirectoryName(filePath);
        if (FolderPath == null) return false;

        var paths = this.Packages.FirstOrDefault(x => x.Id == pid)?.Paths;
        if (paths == null) return false;

        var rx1 = new Regex($"'([^'\\\"]*?{pid}[^'\\\"]*?)'");
        var rx2 = new Regex($"\"([^'\\\"]*?{pid}[^'\\\"]*?)\"");
        await this.TransformLinesAsync(filePath, (line) =>
        {
            var matches = rx1.Matches(line);
            if (matches.Count == 0)
            {
                matches = rx2.Matches(line);
            }

            if (matches.Count == 0) return line;

            var content = line;

            foreach (var v in paths)
            {
                content = content.Replace(v, folderPath);
            }

            return content;
        });



        return true;
    }

    public async Task<bool> UpdatePackageVersionAsync(string pid, string newVersion)
    {
        var filePath = this.FilePath;
        if (filePath == null || !filePath.EndsWith(".vcxproj")) return false;

        FilePath = filePath;
        FolderPath = Path.GetDirectoryName(filePath);
        if (FolderPath == null) return false;

        var versions = this.Packages.FirstOrDefault(x => x.Id == pid)?.Versions;
        if (versions == null) return false;

        //package.config
        var options = new EnumerationOptions { RecurseSubdirectories = true };
        var configFilePath = Directory.EnumerateFiles(FolderPath, "packages.config", options).FirstOrDefault();
        if (configFilePath == null) return false;

        var rx = new Regex("(\\S*?)=\"(.*?)\"");
        await this.TransformLinesAsync(configFilePath, (line) =>
        {
            var content = line.Trim();
            if (!content.StartsWith("<package id"))
            {
                return content;
            }

            var matches = rx.Matches(content);
            if (matches.Count < 3)
            {
                return content;
            }

            var id = matches[0].Groups[2].Value.ToString();
            var version = matches[1].Groups[2].Value.ToString();

            if (id != pid)
            {
                return content;
            }

            return content.Replace(version, newVersion);
        });

        var rx1 = new Regex($"'([^'\\\"]*?{pid}[^'\\\"]*?)'");
        var rx2 = new Regex($"\"([^'\\\"]*?{pid}[^'\\\"]*?)\"");

        await this.TransformLinesAsync(filePath, (line) =>
        {
            var matches = rx1.Matches(line);
            if (matches.Count == 0)
            {
                matches = rx2.Matches(line);
            }

            if (matches.Count == 0) return line;

            var content = line;
            foreach (var v in versions)
            {
                content = line.Replace(v, newVersion);
            }
            return content;
        });


        return true;
    }


    public async Task ParseLinesAsync(string filePath, Action<string> parse)
    {
        var lines = await File.ReadAllLinesAsync(filePath);
        foreach (var line in lines)
        {
            try
            {
                parse(line);
            }
            catch
            {
            }
        }
    }

    public async Task TransformLinesAsync(string filePath, Func<string, string> transform)
    {
        var lines = await File.ReadAllLinesAsync(filePath);
        var targetLines = new List<string>();
        foreach (var line in lines)
        {
            try
            {
                targetLines.Add(transform(line));
            }
            catch
            {
                targetLines.Add(line);
            }
        }
        await File.WriteAllLinesAsync(filePath, targetLines);
    }
}
