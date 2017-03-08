using System;
using System.IO;

namespace Disunity.App.Filesystem
{
    public static class FilesystemUtils
    {
        public static string LocalHome
            =>
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "r0t0r-r0t0r",
                    "Disunity");
    }
}
