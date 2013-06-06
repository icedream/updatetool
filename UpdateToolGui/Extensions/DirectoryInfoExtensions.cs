using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UpdateToolGui.Extensions
{
    static class DirectoryInfoExtensions
    {
        public static string GetRelativePathFrom(this DirectoryInfo directoryInfo, DirectoryInfo baseDirectory)
        {
            return Uri.UnescapeDataString(
                new Uri(
                    baseDirectory.FullName
                    + Path.DirectorySeparatorChar
                )
                .MakeRelativeUri(
                    new Uri(
                        directoryInfo.FullName
                        + Path.DirectorySeparatorChar
                    )
                )
                .ToString()
                .Replace('/', Path.DirectorySeparatorChar)
            );
        }

        public static string GetRelativePathFrom(this DirectoryInfo directoryInfo, FileInfo baseFile)
        {
            return Uri.UnescapeDataString(
                new Uri(
                    baseFile.FullName
                )
                .MakeRelativeUri(
                    new Uri(
                        directoryInfo.FullName
                        + Path.DirectorySeparatorChar
                    )
                )
                .ToString()
                .Replace('/', Path.DirectorySeparatorChar)
            );
        }

        public static string GetRelativePathFrom(this DirectoryInfo directoryInfo, string baseDirectoryPath)
        {
            return GetRelativePathFrom(directoryInfo, new DirectoryInfo(baseDirectoryPath));
        }
    }
}
