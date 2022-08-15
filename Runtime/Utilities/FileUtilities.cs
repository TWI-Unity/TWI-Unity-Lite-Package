namespace TWILite.Utilities
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using UnityEngine;

    public static class FileUtilities
    {
        public enum CreateOptions : byte
        {
            UniqueID,
            UniqueGUID,
            Overwrite,
            Skip,
        }

        public static string Create(string path, string text, CreateOptions options = default) => Create(path, Encoding.UTF8.GetBytes(text), options);
        public static string Create(string path, Texture2D texture, CreateOptions options = default) => Create(path, texture.EncodeToPNG(), options);
        public static string Create(string path, byte[] data, CreateOptions options = default)
        {
            path = GetFullPath(path);
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException("path");
            switch (options)
            {
                case CreateOptions.UniqueID: path = GetUniqueFilepathWithID(path); break;
                case CreateOptions.UniqueGUID: path = GetUniqueFilepathWithGUID(path); break;
                case CreateOptions.Skip: if (File.Exists(path)) return path; break;
            }

            if (data == null) data = new byte[0];
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                stream.Write(data, 0, data.Length);
            return path;
        }

        public static string GetFullPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;
            else path = Environment.ExpandEnvironmentVariables(path);
            return Path.GetFullPath(path).Replace('\\', '/');
        }
        public static string GetUniqueFilepathWithGUID(string path, Func<string, Guid, string, string> func = null, ulong max = 99)
        {
            path = GetFullPath(path);
            if (!File.Exists(path)) return path;
            else if (max < 1) throw new ArgumentOutOfRangeException();
            else if (func == null) func = (p, v, e) => p + '.' + v + e;

            string fullpath;
            string ext = Path.GetExtension(path);
            path = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));

            for (ulong i = 1; i < max; i++)
            {
                fullpath = func(path, Guid.NewGuid(), ext);
                if (!File.Exists(fullpath)) return fullpath;
            }

            throw new StackOverflowException();
        }
        public static string GetUniqueFilepathWithID(string path, Func<string, ulong, string, string> func = null, ulong max = 99)
        {
            path = GetFullPath(path);
            if (!File.Exists(path)) return path;
            else if (max < 1) throw new ArgumentOutOfRangeException();
            else if (func == null) func = (p, v, e) => p + ' ' + v + e;

            string fullpath;
            string ext = Path.GetExtension(path);
            path = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));

            for (ulong i = 1; i < max; i++)
            {
                fullpath = func(path, i, ext);
                if (!File.Exists(fullpath)) return fullpath;
            }

            throw new StackOverflowException();
        }

        public static void Open(string filePath, string appPath, params string[] args)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new FileNotFoundException("File not specified.");
            else filePath = GetFullPath(filePath);

            if (!File.Exists(filePath)) throw new FileNotFoundException("File not found.", filePath);
            else if (string.IsNullOrWhiteSpace(appPath)) Process.Start(filePath, string.Join(" ", args));
            else
            {
                appPath = GetFullPath(appPath);
                if (File.Exists(appPath)) Process.Start(appPath, $"\"{filePath}\" " + string.Join(" ", args));
                else throw new FileNotFoundException("Application path not found. " + appPath);
            }
        }
    }
}