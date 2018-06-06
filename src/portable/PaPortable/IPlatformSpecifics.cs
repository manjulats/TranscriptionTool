using System.Collections.Generic;

namespace PaPortable
{
    public interface IPlatformSpecifics
    {
        List<string> Projects { get; set; }
        void DisplayPage(string page, int[] coord);
    }
}