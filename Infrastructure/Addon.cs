using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Addon.Infrastructure
{
    internal static class Addon
    {
        public static string AssemblyTitle
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                return assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
            }
        }

        public static string AssemblyFileVersion
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                return assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
            }
        }

        public static string AcadRootPath
        {
            get
            {
                dynamic app = Autodesk.AutoCAD.ApplicationServices.Application.AcadApplication;
                return app.Path as string;
            }
        }

        public static string AcadVersion
        {
            get
            {
                dynamic app = Autodesk.AutoCAD.ApplicationServices.Application.AcadApplication;
                return app.Version as string;
            }
        }

        public static string AddonPath
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                return Path.GetDirectoryName(assembly.Location);
            }
        }

        public static void InfoMessage(string message) =>
            MessageBox.Show(message, AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);

        public static void ErrorMessage(string message) =>
            MessageBox.Show(message, AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
