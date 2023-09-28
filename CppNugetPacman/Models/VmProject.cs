namespace CppNugetPacman.Models;

public partial class VmProject : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<VmPackage> _packages = new();

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private bool _isSelected = false;
}
