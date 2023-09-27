using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CppNugetPacman.Models;

public class MProject: MItem
{

    public IEnumerable<MNugetPackage> Packages { get; set; }=Array.Empty<MNugetPackage>();  


    public async Task<bool> LoadAsync(string filePath)
    {
        if (!filePath.EndsWith(".vcxproj")) return false;
        this.FilePath = filePath;
        this.FolderPath= Path.GetDirectoryName(filePath);
        if (this.FolderPath == null) return false;

        //package.config
        var options = new EnumerationOptions { RecurseSubdirectories = true };
        var configFilePath = Directory.EnumerateFiles(this.FolderPath, "packages.config", options).FirstOrDefault();
        if(configFilePath == null) return false;

        var packages = new List<MNugetPackage>();
        var lines = await File.ReadAllLinesAsync(configFilePath);


        var rx = new Regex("(\\S*?)=\"(.*?)\"");

        foreach(var line in lines)
        {
            var content = line.Trim();
            if (!content.StartsWith("<package id")) continue;

            var matches=rx.Matches(content);
            if (matches.Count < 3) continue;

            var id = matches[0].Groups[2].Value.ToString();
            var version = matches[1].Groups[2].Value.ToString();

            var package = new MNugetPackage();
            package.Id = id;
            var versions = new HashSet<string>();
            versions.Add(version);

            Debug.WriteLine($"-> {id}: {version}");
        }

        return true;
    }
}
