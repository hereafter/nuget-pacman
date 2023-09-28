

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
}


