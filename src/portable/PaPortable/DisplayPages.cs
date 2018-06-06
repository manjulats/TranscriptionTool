using PaPortable.Views;

namespace PaPortable
{
    public class DisplayPages
    {
        private readonly IPlatformSpecifics _platformSpecifics;

        public DisplayPages(IPlatformSpecifics platformSpecifics)
        {
            _platformSpecifics = platformSpecifics;
        }

        public IPlatformSpecifics PlatformSpecifics => _platformSpecifics;

        public void DisplayData()
        {
            var model = new Lip3Data().MyRecs;
            var template = new DataCorpus() { Model = model };
            var page = template.GenerateString();
            PlatformSpecifics.DisplayPage(page, new int[] { 750, 725 });
        }

        public void DisplayOpenProject()
        {
            var model = new OpenProjectViewModel { ProjectNames = PlatformSpecifics.Projects, ProjectsFound = PlatformSpecifics.Projects.Count };
            var template = new OpenProject() { Model = model };
            var page = template.GenerateString();
            PlatformSpecifics.DisplayPage(page, new int[] { 500, 500 });
        }
    }
}
