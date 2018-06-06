using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPortable
{
    public class OpenProjectViewModel
    {
        public int ProjectsFound { get; set; }
        public bool DisplayFullName { get; set; }
        public List<PaProjectLite> ProjectNames { get; set; }
    }
}
