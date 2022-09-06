using System.IO;

namespace Addon.Infrastructure
{
    internal static class Directories
    {
        /// <summary>
        /// Директория с определениями блоков.
        /// <para/>
        /// Файл DWG при записи блока в файл.
        /// </summary>
        public static string BlockDefinitionsDir => Path.Combine(Addon.AddonPath, "BlockDefinitions");
    }
}
