using System;
using System.IO;

namespace Disunity.App.Filesystem
{
    public static class FilesystemUtils
    {
        public static string RoamingHome
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "r0t0r-r0t0r", "Render"); }
        }

        public static string LocalHome
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "r0t0r-r0t0r", "Render"); }
        }
    }
}
