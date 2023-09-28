﻿using CppNugetPacman.Models.Data;

namespace CppNugetPacman.Models;

public partial class VmPackage: ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _version = string.Empty;

    [ObservableProperty]
    private bool _isUnique = false;

    [ObservableProperty]
    private string _folderPath = string.Empty;

    [ObservableProperty]
    private ObservableCollection<VmProject> _projects = new();

    [ObservableProperty]
    private string _details = string.Empty;

    public MNugetPackage Data { get; set; }

    public VmPackage(MNugetPackage data)
    {
        this.Data = data;

        this.Name = data.Id;
        this.IsUnique= data.IsUnique;
        this.FolderPath = data.Path;
        this.Version = data.Version;

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

}



