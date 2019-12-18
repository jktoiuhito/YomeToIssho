using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// A static class handling all serialization and deserialization of data of GameObjects.
/// For safety reasons, data can only be read and written to Application.persistentDataPath.
/// </summary>
public static class GenericSerializer
{
    private static readonly string ROOT = Application.persistentDataPath + "/";
    private static readonly IFormatter Formatter = new BinaryFormatter();

    public static void Serialize(System.Object data, string path, string filename)
    {
        var wholepath = ROOT + path;
        Directory.CreateDirectory(wholepath);
        using (Stream s = File.OpenWrite(wholepath + filename))
        {
            Formatter.Serialize(s, data);
        }
    }

    /// <summary>
    /// Deserializes an object from disk to the given type T.
    /// If no file is found, returns null.
    /// </summary>
    /// <typeparam name="T">Type of the serialized object.</typeparam>
    /// <param name="path">Path to the file, without file name.</param>
    /// <param name="filename">File name.</param>
    /// <returns></returns>
    public static T DeserializeReferenceType<T>(string path, string filename)
        where T : class
    {
        var wholepath = ROOT + path;
        if (File.Exists(wholepath + filename))
        {
            using (Stream s = File.OpenRead(wholepath + filename))
            {
                try { return (T)Formatter.Deserialize(s); }
                catch { return null; }
            }
        }
        return null;
    }

    public static T DeserializeValueType<T>(string path, string filename)
        where T : struct
    {
        var wholepath = ROOT + path;
        if (File.Exists(wholepath + filename))
        {
            using (Stream s = File.OpenRead(wholepath + filename))
            {
                try { return (T)Formatter.Deserialize(s); }
                catch { return new T(); }
            }
        }
        return new T();
    }

    /// <summary>
    /// Enumerates files in the given directory path, and returns only their names.
    /// </summary>
    /// <param name="path">Path to the directory.</param>
    /// <returns>Files located in the directory.</returns>
    public static List<string> EnumerateFilenames(string path)
    {
        try     //Most likely happens when the path doesn't exist.
        {
            var fullpaths = Directory.EnumerateFiles(ROOT + path);
            List<string> fileNames = new List<string>();
            foreach (var fullpath in fullpaths)
            {
                fileNames.Add(new FileInfo(fullpath).Name);
            }
            return fileNames;
        }
        catch (DirectoryNotFoundException)
        {
            Debug.LogErrorFormat("Enumeration of files at {0} failed: directory was not found." +
                "Most likely reason is that the path does not exist in its entirety.", ROOT + path);
            return null;
        }
    }
}
