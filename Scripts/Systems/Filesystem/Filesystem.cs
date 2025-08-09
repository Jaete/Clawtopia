using Godot;
using Godot.Collections;

public partial class Filesystem
{
    public static bool DirectoryExists(string path)
    {
        return DirAccess.Open(path) != null;
    }

    public static bool FileExists(string path)
    {
        return FileAccess.Open(path, FileAccess.ModeFlags.Read) != null;
    }

    public static void CreateDir(string basePath, string dirName)
    {
        DirAccess.Open(basePath).MakeDir(dirName);
        EditorInterface.Singleton.GetResourceFilesystem().Scan();
    }

    public static void DeleteDir(string path)
    {
        OS.MoveToTrash(path);
        EditorInterface.Singleton.GetResourceFilesystem().Scan();
    }

    public static string[] GetDirectories(string path)
    {
        DirAccess dir = DirAccess.Open(path);
        return dir.GetDirectories();
    }
}
