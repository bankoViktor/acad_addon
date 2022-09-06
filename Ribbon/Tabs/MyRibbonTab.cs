using Addon.Resources;
using Addon.Ribbon.Panels;
using Autodesk.Windows;

namespace Addon.Ribbon.Tabs
{
    class MyRibbonTab : RibbonTab
    {
        public const string RibbonTabId = "830E2A70-A058-4FD9-A290-52A8760E37DB";

        public MyRibbonTab()
        {
            Id = RibbonTabId;
            Title = Strings.RibbonTab_Text;

            Panels.Add(new GeneralRibbonPanel());
            Panels.Add(new MiscRibbonPanel());
        }
    }
}
