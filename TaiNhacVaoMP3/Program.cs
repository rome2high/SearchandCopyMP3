using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace TaiNhacVaoMP3
{
    class Program
    {
        static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();
        static List<FileInfo> sourceMp3Files = new List<FileInfo>();
        static double dblMinDuration = 120;     //all song must be longer than 3 minutes
        static string rootPath = @"C:\Users\";

        static void Main(string[] args)
        {
            string target = Directory.GetCurrentDirectory() + "\\music\\";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            System.IO.DirectoryInfo rootDir = new System.IO.DirectoryInfo(rootPath);

            Console.WriteLine("Phan mem nay xe copy nhung bay nhac moi nhat");
            Console.WriteLine("va day hon 2 phut vao may mp3 cua bo gia. \n");

            Console.WriteLine("BO GIA CO MUON XOA HET NHAC TRONG TAP TIN 'music' TREN MAY MP3 KHONG?");
            Console.WriteLine("nhan phim 'Y' de xoa tap tinh 'music' va tai nhac MOI");
            Console.WriteLine("nhan phim 'N' de tiep tuc tai THEM nhac vao may MP3 \n");

            ConsoleKeyInfo cki;
            do
            {
                cki = Console.ReadKey();

                
                Console.WriteLine(cki.Key.ToString());
                if (cki.Key.ToString() == "Y")
                {
                    Console.WriteLine("Bo da chon xoa tat ca nhac tren may bo ");
                    if(Directory.Exists(target))
                    {
                        Directory.Delete(target,true);
                    }
                    break;
                }
                else if (cki.Key.ToString() == "N")
                {
                    Console.WriteLine("Phan mem xe tiep tuc tai them nhac vao may");
                    break;
                }
                else
                {
                    if (cki.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                    Console.WriteLine("Xin nhan phim 'Y' hoac 'N'");
                }
            } while (cki.Key != ConsoleKey.Escape);

            if (cki.Key == ConsoleKey.Escape)
            {
                Console.WriteLine("\nBO da chon: " + cki.Key.ToString());
                Console.WriteLine("Khong hieu BO muon gi");
                Console.WriteLine("BO hay khoi dong lai chuong trinh copy nhac tu may MP3");
                System.Threading.Thread.Sleep(5000);
                Environment.Exit(0);
            }


            Console.WriteLine("\nDang tim nhac tren may tinh, xin vui long doi ...");

            WalkDirectoryTree(rootDir);

            List<FileInfo> orderedList = sourceMp3Files.OrderBy(x => x.CreationTime).ToList();

            if (!Directory.Exists(target))
            {
                Directory.CreateDirectory(target);
            }

            Console.WriteLine("\nDang tai nhac vao May MP3, xin vui long doi ...");
            
            foreach (FileInfo fi in orderedList)
            {
                FileInfo targetfi = new FileInfo(target + fi.Name);

                if (!targetfi.Exists)
                {
                    try
                    {
                        fi.CopyTo(targetfi.FullName, false);
                    }
                    catch (IOException ex)
                    {
                        log.Add(ex.Message);
                        continue;
                    }
                }
            }

            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            // Format and display the TimeSpan value. 
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            

            Console.WriteLine("\nTat Ca Da Hoan Tat");
            Console.WriteLine("Thoi gian chay la: " + elapsedTime);
            Console.WriteLine("\nBO GIA nhan bat ky phim nao de thoat!");
            Console.ReadKey();
        }

        static void WalkDirectoryTree(System.IO.DirectoryInfo root)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            WMPLib.WindowsMediaPlayer w = new WMPLib.WindowsMediaPlayer();
            
            try
            {
                files = root.GetFiles("*.*");
            }
            // This is thrown if even one of the files requires permissions greater 
            // than the application provides. 
            catch (UnauthorizedAccessException e)
            {
                log.Add(e.Message);
            }

            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                foreach (System.IO.FileInfo fi in files)
                {
                    if (fi.Extension.ToLower() == ".mp3")
                    {
                        Console.WriteLine(fi.FullName);
                        WMPLib.IWMPMedia m = w.newMedia(fi.FullName);
                        if (m != null || m.duration != null)
                        {
                            if (m.duration >= dblMinDuration)
                            {
                                sourceMp3Files.Add(fi);
                            }
                        }
                       
                    }
                }

                // Now find all the subdirectories under this directory.
                subDirs = root.GetDirectories();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    // Resursive call for each subdirectory.
                    WalkDirectoryTree(dirInfo);
                }
            }

            w.close();
        }
    }
}
