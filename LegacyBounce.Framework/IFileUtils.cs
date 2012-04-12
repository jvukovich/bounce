using System;

namespace LegacyBounce.Framework {
    public interface IFileUtils {
        bool FileExists(string filename);
        DateTime LastWriteTimeForFile(string filename);
        void DeleteFile(string file);
        void CopyFile(string from, string to);
    }
}