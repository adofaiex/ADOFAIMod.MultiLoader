#!/usr/bin/env dotnet-script

using System.IO.Compression;

var projectDir = Directory.GetCurrentDirectory();
var outDir = Path.Combine(projectDir, "out");
var distDir = Path.Combine(projectDir, "dist");

if (!Directory.Exists(outDir))
{
    Console.Error.WriteLine("out/ not found. Run 'dotnet build -c Release' first.");
    return;
}

var coreDll = Directory.GetFiles(outDir, "*.Core.dll").FirstOrDefault();
if (coreDll == null)
{
    Console.Error.WriteLine("Core DLL not found in out/. Build the project first.");
    return;
}

var modName = Path.GetFileNameWithoutExtension(coreDll).Replace(".Core", "");
Directory.CreateDirectory(distDir);

var hasUmm = File.Exists(Path.Combine(outDir, $"{modName}.Loader.UMM.dll"));
var hasMelon = File.Exists(Path.Combine(outDir, $"{modName}.Loader.Melon.dll"));
var hasBepInEx = File.Exists(Path.Combine(outDir, $"{modName}.Loader.BepInEx.dll"));
var hasDoorstop = File.Exists(Path.Combine(outDir, $"{modName}.Loader.Doorstop.dll"));

var topFiles = Directory.GetFiles(outDir, "*", SearchOption.TopDirectoryOnly);
var resDir = Path.Combine(outDir, "Resources");
var hasResources = Directory.Exists(resDir);

void AddFiles(ZipArchive zip, IEnumerable<string> files, string prefix)
{
    foreach (var f in files)
        zip.CreateEntryFromFile(f, prefix + Path.GetFileName(f));
}

void AddResources(ZipArchive zip, string prefix)
{
    if (!hasResources) return;
    foreach (var f in Directory.GetFiles(resDir, "*", SearchOption.AllDirectories))
    {
        var rel = Path.GetRelativePath(resDir, f);
        zip.CreateEntryFromFile(f, prefix + "Resources/" + rel);
    }
}

// shared files go into every loader package
var sharedFiles = topFiles.Where(f =>
    !f.Contains(".Loader.") &&
    !f.EndsWith("doorstop_config.ini") &&
    !f.EndsWith("Info.json"));

if (hasUmm)
{
    var path = Path.Combine(distDir, $"{modName}_umm.zip");
    using var zip = ZipFile.Open(path, ZipArchiveMode.Create);
    var pre = $"Mods/{modName}/";
    AddFiles(zip, sharedFiles, pre);
    AddFiles(zip, topFiles.Where(f => f.EndsWith("Loader.UMM.dll") || f.EndsWith("Info.json")), pre);
    AddResources(zip, pre);
    Console.WriteLine($"  {path}");
}

if (hasMelon)
{
    var path = Path.Combine(distDir, $"{modName}_melon.zip");
    using var zip = ZipFile.Open(path, ZipArchiveMode.Create);
    var pre = "Mods/";
    AddFiles(zip, sharedFiles, pre);
    AddFiles(zip, topFiles.Where(f => f.EndsWith("Loader.Melon.dll")), pre);
    AddResources(zip, pre);
    Console.WriteLine($"  {path}");
}

if (hasBepInEx)
{
    var path = Path.Combine(distDir, $"{modName}_bepinex.zip");
    using var zip = ZipFile.Open(path, ZipArchiveMode.Create);
    var pre = $"BepInEx/plugins/{modName}/";
    AddFiles(zip, sharedFiles, pre);
    AddFiles(zip, topFiles.Where(f => f.EndsWith("Loader.BepInEx.dll")), pre);
    AddResources(zip, pre);
    Console.WriteLine($"  {path}");
}

if (hasDoorstop)
{
    var path = Path.Combine(distDir, $"{modName}_doorstop.zip");
    using var zip = ZipFile.Open(path, ZipArchiveMode.Create);
    AddFiles(zip, sharedFiles, "");
    AddFiles(zip, topFiles.Where(f => f.EndsWith("Loader.Doorstop.dll") || f.EndsWith("doorstop_config.ini")), "");
    AddResources(zip, "");
    Console.WriteLine($"  {path}");
}

Console.WriteLine($"Done → {distDir}/");
