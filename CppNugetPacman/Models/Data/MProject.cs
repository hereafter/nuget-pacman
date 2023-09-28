using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CppNugetPacman.Models.Data;

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
        var lines = await File.ReadAllLinesAsync(configFilePath);


        var rx = new Regex("(\\S*?)=\"(.*?)\"");

        foreach (var line in lines)
        {
            var content = line.Trim();
            if (!content.StartsWith("<package id")) continue;

            var matches = rx.Matches(content);
            if (matches.Count < 3) continue;

            var id = matches[0].Groups[2].Value.ToString();
            var version = matches[1].Groups[2].Value.ToString();

            var package = new MNugetPackage
            {
                Id = id
            };
            package.Version = version;
            packages.Add(package);
            Debug.WriteLine($"-> {id}: {version}");
        }
        Packages = packages;

        lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8);




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
}
