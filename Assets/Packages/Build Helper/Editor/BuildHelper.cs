#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Build.Reporting;
using System;
using System.Diagnostics;
using System.IO;
// On Unity 2018.3.7f1, I had to set Scripting Runtime Version to
// .NET 4.0 Equivalent, and add a csc.rsp file as detailed here:
// https://forum.unity.com/threads/extracting-zip-files.472537/#post-4371022
// for System.IO.Compression.ZipFile to be available.
using System.IO.Compression;

public class BuildHelper : Editor {

    // Makes 6 builds: Windows, Mac, Linux, with and without Steam included.
    [MenuItem("Tools/Build Win+Mac+Linux")]
    static void BuildWinMacLinux() {
        // Ask for the directory
        var defaultFolderName = "winmaclinux_"
                                + System.DateTime.Today.ToString("yyyy-MM-dd");
        var baseDir = EditorUtility.SaveFilePanel(
                "Build Win+Mac+Linux", "builds", defaultFolderName, "");
        if (baseDir == "") return; // Cancelled the dialog

        if (Directory.Exists(baseDir)) {
            UnityEngine.Debug.LogError("Directory already exists: "+baseDir);
            return;
        }
        Directory.CreateDirectory(baseDir);



        var stopwatch = new Stopwatch();
        stopwatch.Start();

        // Set build options
        var options = new BuildPlayerOptions {scenes = new string[EditorBuildSettings.scenes.Length]};
        for (var i = 0; i < options.scenes.Length; i++) {
            options.scenes[i] = EditorBuildSettings.scenes[i].path;
        }
        options.options = BuildOptions.None;

        string outputDir;
        
        /*
        // Steam

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "");

        // Steam Windows
        outputDir = baseDir+"/steam_windows/Patrick's Parabox";
        options.target = BuildTarget.StandaloneWindows64;
        options.locationPathName = outputDir+"/Patrick's Parabox.exe";
        Build(options, "Steam Windows");
        ZipFile.CreateFromDirectory(outputDir, baseDir+"/steam_windows.zip");

        // Steam Mac
        outputDir = baseDir+"/steam_mac/Patrick's Parabox.app";
        options.target = BuildTarget.StandaloneOSX;
        options.locationPathName = outputDir;
        Build(options, "Steam Mac");
        ZipFile.CreateFromDirectory(outputDir+"/..", baseDir+"/steam_mac.zip");

        // Steam Linux
        outputDir = baseDir+"/steam_linux/Patrick's Parabox";
        options.target = BuildTarget.StandaloneLinux64;
        options.locationPathName = outputDir+"/Patrick's Parabox.x86_64";
        Build(options, "Steam Linux");
        ZipFile.CreateFromDirectory(outputDir, baseDir+"/steam_linux.zip");
        */

        // Non-Steam

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "DISABLESTEAMWORKS");

        // Windows
        outputDir = baseDir+"/windows/Patrick's Parabox";
        options.target = BuildTarget.StandaloneWindows64;
        options.locationPathName = outputDir+"/Patrick's Parabox.exe";
        Build(options, "Windows");
        ZipFile.CreateFromDirectory(outputDir+"/..", baseDir+"/Patrick's Parabox Windows.zip");

        // Mac
        outputDir = baseDir+"/mac/Patrick's Parabox.app";
        options.target = BuildTarget.StandaloneOSX;
        options.locationPathName = outputDir;
        Build(options, "Mac");
        ZipFile.CreateFromDirectory(outputDir+"/..", baseDir+"/Patrick's Parabox Mac.zip");

        // Linux
        outputDir = baseDir+"/linux/Patrick's Parabox";
        options.target = BuildTarget.StandaloneLinux64;
        options.locationPathName = outputDir+"/Patrick's Parabox.x86_64";
        Build(options, "Linux");
        ZipFile.CreateFromDirectory(outputDir+"/..", baseDir+"/Patrick's Parabox Linux.zip");



        // Reset settings to convenient defaults
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "DISABLESTEAMWORKS");
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);

        stopwatch.Stop();
        var ts = stopwatch.Elapsed;
        UnityEngine.Debug.Log($"Total build time: {ts.TotalSeconds} seconds");
    }

    static void Build(BuildPlayerOptions options, string buildName) {
        var report = BuildPipeline.BuildPlayer(options);
        var summary = report.summary;

        switch (summary.result)
        {
            case BuildResult.Succeeded:
                UnityEngine.Debug.Log(buildName + " succeeded: " + summary.totalSize + " bytes");
                break;
            case BuildResult.Failed:
                UnityEngine.Debug.LogError(buildName + " failed");
                UnityEngine.Debug.LogError(summary);
                throw new Exception();
        }
    }
}

// Copies the contents of the copy_files directory into build folders.
// copy_files contains a readme plus a few other files I wish to bundle with builds.
// https://docs.unity3d.com/ScriptReference/Callbacks.PostProcessBuildAttribute.html
public class MyBuildPostprocessor {
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
        //Debug.Log(pathToBuiltProject);
        var copyTo = "";
        if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64) {
            copyTo = pathToBuiltProject+"/..";
        }
        else if (target == BuildTarget.StandaloneOSX) {
            copyTo = pathToBuiltProject+"/Contents";
        }
        else if (target == BuildTarget.StandaloneLinux64) {
            copyTo = pathToBuiltProject+"/..";
        }
        else {
            UnityEngine.Debug.LogWarning("Unrecognized target - don't know where to put copy_files/");
            return;
        }
        
        // Application.dataPath is /Aseets when running in the editor
        DirectoryCopy(Application.dataPath+"/../copy_files", copyTo, true);
        //UnityEngine.Debug.Log("Copied copy_files/");
    }

    // From https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
    private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        // Get the subdirectories for the specified directory.
        var dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        DirectoryInfo[] dirs = dir.GetDirectories();
        // If the destination directory doesn't exist, create it.
        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }

        // Get the files in the directory and copy them to the new location.
        var files = dir.GetFiles();
        foreach (var file in files)
        {
            var temppath = Path.Combine(destDirName, file.Name);
            file.CopyTo(temppath, false);
        }

        // If copying subdirectories, copy them and their contents to new location.
        if (copySubDirs)
        {
            foreach (var subdir in dirs)
            {
                var temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, copySubDirs);
            }
        }
    }
}

#endif
