using CppNugetPacman.Models;

namespace CppNugetPacman;

public class TreeViewItemTemplateSelector: DataTemplateSelector
{

    public DataTemplate? PackageTemplate { get; set; } = null;
    public DataTemplate? ProjectTemplate { get; set; } = null;


    protected override DataTemplate SelectTemplateCore(object item)
    {
        DataTemplate? template = null;
        if(item is VmPackage) template = PackageTemplate;
        if (item is VmProject) template = ProjectTemplate;
        if (template != null) return template;

        return base.SelectTemplateCore(item);
    }
}
