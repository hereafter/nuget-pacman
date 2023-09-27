using System.Collections;
using System.Collections.Generic;

namespace CppNugetPacman.Models;

public class MSolution: MItem
{
    public IEnumerable<MProject> Projects { get; set; } = new List<MProject>();
}
