using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using DiscUtils;
using DiscUtils.Ntfs;
using DiscUtils.Setup;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Security.Policy;
using DiscUtils.Streams;

namespace vmdk
{
    class Program
    {
        public static void GetHelp()
        {
            string ascii = @"                                                                                                       
     _  _        _             _                             
    | |(_)      | |           (_)                            
  __| | _   ___ | |  _   ____  _  ____   ____   _____   ____ 
 / _  || | /___)| |_/ ) / ___)| ||  _ \ |  _ \ | ___ | / ___)
( (_| || ||___ ||  _ ( | |    | || |_| || |_| || ____|| |    
 \____||_|(___/ |_| \_)|_|    |_||  __/ |  __/ |_____)|_|    
                                 |_|    |_| 
                                            Version: 0.2                                                                
                                            Made By: Abdelhamid Jami
                                             GitHub: Skixie";
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ascii);
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Usage:");
            Console.WriteLine("       diskripper.exe --command [type] --source [C:\\..\\..] --action [action]\n");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[?] Command: dir - Will output a directory listing of the provided folder");
            Console.ResetColor();
            Console.WriteLine("      --source: The source of where the image file is stored. Network paths are also accepted");
            Console.WriteLine("      --directory: The directory you want to list from the virtual disk. If no path is provided,");
            Console.WriteLine("                   the tool will default to root path\n");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[?] Command: cp - Will copy a file from the virtual disk to the destination provided");
            Console.ResetColor();
            Console.WriteLine("      --source: The source of the virtual disk drive. Network paths are also accepted");
            Console.WriteLine("      --file2copy: The file you want to copy from the virtual disk ");
            Console.WriteLine("      --destination: The destination where to save the file\n");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[?] Command: cpfile - Will copy the files that are specified within a .txt file");
            Console.ResetColor();
            Console.WriteLine("      --source: The source of where the image file is stored. Network paths are also accepted");
            Console.WriteLine("      --file: Specify path to .txt file which contain the desired files to download (paths must be seperated per line)");
            Console.WriteLine("      --destinationdir: Specify in which directory you want the files to be saved\n");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[?] Command: pf - Will list the contents in the Program Files and Program Files (x86) directories");
            Console.ResetColor();
            Console.WriteLine("      --source: The source of where the image file is stored. Network paths are also accepted\n");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[?] Command: sam - Will extract the SAM, SECURITY and SYSTEM files from given image file");
            Console.ResetColor();
            Console.WriteLine("      --source: The source of where the image file is stored. Network paths are also accepted\n");
        }

        // Argumenten aanmaken
        public static string GetArgument(IEnumerable<string> args, string option)
            => args.SkipWhile(i => i != option).Skip(1).Take(1).FirstOrDefault();

        static void Main(string[] args)
        {
            SetupHelper.RegisterAssembly(typeof(NtfsFileSystem).Assembly);
            SetupHelper.RegisterAssembly(typeof(DiscUtils.Vmdk.Disk).Assembly);
            SetupHelper.RegisterAssembly(typeof(VirtualDiskManager).Assembly);
            SetupHelper.RegisterAssembly(typeof(VirtualDisk).Assembly);
            SetupHelper.RegisterAssembly((typeof(DiscUtils.Vhd.Disk).Assembly));

            if (args.Length != 0 && !string.IsNullOrEmpty(GetArgument(args, "--command")))
            {
                // Command voor directory listing
                string command = GetArgument(args, "--command");
                if (command.ToLower() == "dir" && GetArgument(args, "--source") != null)
                {
                    var diskimagepath = GetArgument(args, "--source");
                    var directorypath = GetArgument(args, "--directory");
                    try
                    {
                        GetDirListing(diskimagepath, directorypath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\r\n [!] An exception occured: {0}", ex);
                        throw;
                    }

                }
                
                // Command voor kopieren van een file
                else if (command.ToLower() == "cp" && GetArgument(args, "--source") != null &&
                         GetArgument(args, "--file2copy") != null && GetArgument(args, "--destination") != null)
                {
                    var diskimagepath = GetArgument(args, "--source");
                    var filepath = GetArgument(args, "--file2copy");
                    var destination = GetArgument(args, "--destination");

                    try
                    {
                        GetFile(diskimagepath, filepath, destination);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\r\n [!] An exception occured: {0}", ex);
                        throw;
                    }
                }

                // Command voor kopieren van bestanden die in een .txt file worden meegegeven
                else if (command.ToLower() == "cpfile" && GetArgument(args, "--source") != null 
                    && GetArgument(args, "--file") != null && GetArgument(args, "--destinationdir") != null )
                {
                    var diskimagepath = GetArgument(args, "--source");
                    var filepath = GetArgument(args, "--file");
                    var destination = GetArgument(args, "--destinationdir");
                    var destinationfile = "";
                    var filename = "";

                    try
                    {
                        foreach (string line in System.IO.File.ReadLines(filepath)) {
                            filename = Path.GetFileName(line);
                            destinationfile = destination + "\\" + filename;
                            GetFile(diskimagepath, line, destinationfile);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\r\n [!] An exception occured: {0}", ex);
                        throw;
                    }

                }
                
                // Command om Program Files en Program Files (x86) te listen
                else if (command.ToLower() == "pf" && GetArgument(args, "--source") != null)
                {
                    var diskimagepath = GetArgument(args, "--source");
                    var directorypath = ("\\Program Files");
                    try
                    {
                        GetDirListing(diskimagepath, directorypath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\r\n [!] An exception occured: {0}", ex);
                        Console.WriteLine(ex);
                    }

                    directorypath = ("\\Program Files (x86)");
                    
                    try
                    {
                        GetDirListing(diskimagepath, directorypath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\r\n [!] An exception occured: {0}", ex);
                        Console.WriteLine(ex);
                    }

                }

                // Command voor kopieren van SAM
                else if (command.ToLower() == "sam" && GetArgument(args, "--source") != null && GetArgument(args, "--destinationdir") != null)
                {
                    var diskimagepath = GetArgument(args, "--source");
                    var filepath = ("\\Windows\\System32\\config\\SAM");

                    var destination = GetArgument(args, "--destinationdir");
                    var destinationfile = "";

                    try
                    {
                        destinationfile = destination + "\\" + "SAM";
                        GetFile(diskimagepath, filepath, destinationfile);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\r\n [!] An exception occured: {0}", ex);
                        throw;
                    }

                    filepath = ("\\Windows\\System32\\config\\SYSTEM");

                    try
                    {
                        destinationfile = destination + "\\" + "SYSTEM";
                        GetFile(diskimagepath, filepath, destinationfile);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\r\n [!] An exception occured: {0}", ex);
                        throw;
                    }

                    filepath = ("\\Windows\\System32\\config\\SECURITY");

                    try
                    {
                        destinationfile = destination + "\\" + "SECURITY";
                        GetFile(diskimagepath, filepath, destinationfile);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\r\n [!] An exception occured: {0}", ex);
                        throw;
                    }

                }

                else if (command.ToLower() == "help")
                {
                   GetHelp();
                }

                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("");
                    Console.WriteLine("Command not found or completed. Please check your command or check if you have all the required arguments.");
                    Console.WriteLine("Type '--command help' to see the available commands if you're stuck.");
                    Console.ResetColor();
                }
            }
            else
            {
                GetHelp();
            }
        }

        // Methode om te controleren of image bestand versleuteld is met Bitlocker
        public static bool isBitlocker(PhysicalVolumeInfo volume)
        {
            byte[] buffer = new byte[16];
            volume.Open().Read(buffer, 0, buffer.Length);
            string s = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);

            if (s.Contains("-FVE-FS-"))
            {
                Console.WriteLine("");
                Console.WriteLine("[!] Checking if image file is encrypted...");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[X] Can't read this partition because it might be encrypted");
                Console.ResetColor();
                return true;
            }
            //Console.WriteLine("");
            //Console.WriteLine("[!] Checking if partition is encrypted...");
            //Console.ForegroundColor = ConsoleColor.Green;
            //Console.WriteLine("[V] No encryption has been found on this partition");
            //Console.ResetColor();
            return false;
        }

        // Methode om het Directory Listing te krijgen
        public static void GetDirListing(string DiskPath, string directory)
        {
            if (File.Exists(DiskPath))
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("[INFO] Multiple partitions have been detected");
                Console.ResetColor();
                // VMDK bestand openen en checken hoeveel partities er zijn 
                VolumeManager volMgr = new VolumeManager();
                VirtualDisk vhdx = VirtualDisk.OpenDisk(DiskPath, FileAccess.Read);
                volMgr.AddDisk(vhdx);
                VolumeInfo volInfo = null;
                foreach (var physVol in volMgr.GetPhysicalVolumes())
                {
                    isBitlocker(physVol);
                    if (!string.IsNullOrEmpty(physVol.Identity))
                    {
                        volInfo = volMgr.GetVolume(physVol.Identity);
                    }
                    try
                    {   // NTFS partitie aanmaken en openen
                        NtfsFileSystem vhdbNtfs = new NtfsFileSystem(physVol.Partition.Open());
                    }
                    catch
                    {
                        continue;
                    }
                    using (NtfsFileSystem vhdbNtfs = new NtfsFileSystem(physVol.Partition.Open()))

                    {   // De eerder opgegeven directories worden hier opgehaald en samen met gevonden bestanden in een lijst weergegeven
                        if (vhdbNtfs.DirectoryExists("\\\\" + directory))
                        {
                            if (vhdbNtfs.GetDirectories(vhdbNtfs.Root.FullName).Length == 0)
                            {
                                continue;
                            }

                            string[] filelist = vhdbNtfs.GetFiles(vhdbNtfs.Root.FullName + directory);
                            string[] dirlist = vhdbNtfs.GetDirectories(vhdbNtfs.Root.FullName + directory);

                            if (dirlist.Length > 0)
                            {
                                Console.WriteLine("");
                                Console.WriteLine("-----------------------------------------------------");
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("[!] Found the following directories:");
                                Console.ResetColor();
                                Console.WriteLine("");
                                foreach (var dir in dirlist)
                                {
                                    Console.WriteLine("[+] {0}", dir);
                                }
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("");
                                Console.WriteLine("[X] No directories have been found.");
                                Console.ResetColor();
                            }

                            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - -");

                            if (filelist.Length > 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("");
                                Console.WriteLine("[!] Found the following files:");
                                Console.ResetColor();
                                Console.WriteLine("");
                                foreach (var file in filelist)
                                {
                                    Console.WriteLine("[+] {0} [{1} bytes]", file, vhdbNtfs.GetFileLength(file));
                                }
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("[X] No files have been found.");
                                Console.ResetColor();
                                Console.WriteLine("-----------------------------------------------------");
                            }
                        }
                    }
                }
            }
        }

        // Methode om bestand te kopieren
        public static void GetFile(string DiskPath, string FilePath, string DestinationFile)
        {
            // Checken of opgegeven pad naar vmdk bestand bestaat
            if (File.Exists(DiskPath) && Directory.Exists(Path.GetDirectoryName(DestinationFile)))
            {
                if (Path.GetFileName(DestinationFile) == "")
                {
                    DestinationFile += Path.GetFileName(FilePath);
                }
 
                VolumeManager volMgr = new VolumeManager();
                VirtualDisk disk = VirtualDisk.OpenDisk(DiskPath, FileAccess.Read);
                volMgr.AddDisk(disk);
                VolumeInfo volInfo = null;

                foreach (var physVol in volMgr.GetPhysicalVolumes())
                {
                    if (!string.IsNullOrEmpty(physVol.Identity))
                    {
                        volInfo = volMgr.GetVolume(physVol.Identity);
                    }
                    try
                    {
                        DiscUtils.FileSystemInfo fsInfo = FileSystemManager.DetectFileSystems(volInfo)[0];
                    }
                    catch
                    {
                        continue;
                    }
                    using (NtfsFileSystem diskntfs = new NtfsFileSystem(physVol.Partition.Open()))
                    {
                        if (diskntfs.FileExists("\\\\" + FilePath))
                        {
                            long fileLength = diskntfs.GetFileLength("\\\\" + FilePath);
                            using (Stream bootStream = diskntfs.OpenFile("\\\\" + FilePath, FileMode.Open,
                                FileAccess.Read))
                            {
                                byte[] file = new byte[bootStream.Length];
                                int totalRead = 0;
                                while (totalRead < file.Length)
                                {
                                    totalRead += bootStream.Read(file, totalRead, file.Length - totalRead);
                                    FileStream fileStream =
                                        File.Create(DestinationFile, (int)bootStream.Length);
                                    bootStream.CopyTo(fileStream);
                                    fileStream.Write(file, 0, (int)bootStream.Length);

                                    List <string> desFile = new List<string>();
                                    desFile.Add(DestinationFile);
                                    String[] str = desFile.ToArray();

                                    long destinationLength = new FileInfo(DestinationFile).Length;
                                    if (fileLength != destinationLength)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("[!] Something went wrong. Source file has size {0} and destination file has size {1}", fileLength, destinationLength);
                                        Console.ResetColor();
                                    }

                                    else
                                    {
                                        Console.WriteLine("---------------------------------------------------------------------------------------------------");
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine("[V] {0} has succesfully been copied to {1}", FilePath, DestinationFile);
                                        Console.ResetColor();
                                    }
                                }
                            }

                        }
                    }
                }

            }
        }
    }
}
