using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OS
{
    internal class ParserClass
    {
        // method dir to help me 
        public static void Dir()
        {
            int file_Counter = 0;
            int folder_Counter = 0;
            int file_Sizes = 0;
            int total_File_Size = 0;

            string name = new string(Program.currentDirectory.Dir_Namee);
            string current_Name = Program.path;


            Console.WriteLine($"Directory of {name} is  \n");


            for (int i = 0; i < Program.currentDirectory.DirectoryTable.Count; i++)
            {
                if (Program.currentDirectory.DirectoryTable[i].dir_Attr == 0x0)
                {
                    file_Counter++;
                    file_Sizes += Program.currentDirectory.DirectoryTable[i].dir_FileSize;
                    total_File_Size += file_Sizes;
                    // file_Sizes = 0;
                    string m = string.Empty;
                    m += new string(Program.currentDirectory.DirectoryTable[i].Dir_Namee);
                    Console.WriteLine($"\t\t{file_Sizes}\t\t" + m);
                    // Console.WriteLine();
                }
                else if (Program.currentDirectory.DirectoryTable[i].dir_Attr == 0x10) // لو في فولدر 
                {
                    folder_Counter++;
                    string S = new string(Program.currentDirectory.DirectoryTable[i].Dir_Namee);
                    Console.WriteLine("\t\t<DIR>\t\t" + S.Trim());
                }

            }
            Console.Write($"\t\t\t{file_Counter} File(s)\t ");
            if (file_Counter > 0)
            {
                Console.Write(total_File_Size);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine();
            }
            Console.WriteLine($"\t\t\t{folder_Counter} Dir(s)\t {Mini_FAT.get_Free_Size()} bytes free");


        }
        public static void Dir(string name)
        {
            int file_Counter = 0;
            int folder_Counter = 0;
            int total_File_Size = 0;
            int file_Sizes = 0;

            // محاولة البحث عن العنصر (ملف أو مجلد) في الدليل الحالي
            Directory targetDirectory = Program.currentDirectory;
            Directory_Entry entry = null;

            // البحث في الجدول الحالي إذا كان اسم الدليل موجودًا
            int index = targetDirectory.search_Directory(name);
            if (index != -1) // إذا تم العثور على العنصر في الجدول
            {
                entry = targetDirectory.DirectoryTable[index];
            }
            else // إذا لم يتم العثور عليه، محاولة الانتقال إلى المجلد
            {
                targetDirectory = ParserClass.MoveToDir(name, Program.currentDirectory);
                if (targetDirectory == null)
                {
                    Console.WriteLine($"Error: Path '{name}' not found or is not a directory.");
                    return;
                }
                Console.WriteLine($"Directory of {name}: \n");
            }

            // إذا كان العنصر ملفًا (dir_Attr == 0x0)
            if (entry != null && entry.dir_Attr == 0x0)
            {
                // التعامل مع الملف
                Console.WriteLine($"Directory of {new string(targetDirectory.Dir_Namee)}: \n");
                Console.WriteLine($"\t\t{entry.dir_FileSize}\t\t{new string(entry.Dir_Namee)}");
                Console.WriteLine($"\t\t\t1 File(s)\t{entry.dir_FileSize} bytes");
                Console.WriteLine($"\t\t\t0 Dir(s)\t{Mini_FAT.get_Free_Size()} bytes free");
            }
            // إذا كان العنصر مجلدًا (dir_Attr == 0x10)
            else if (entry != null && entry.dir_Attr == 0x10)
            {
                // التعامل مع المجلد
                Console.WriteLine($"Directory of {name}: \n");

                for (int i = 0; i < targetDirectory.DirectoryTable.Count; i++)
                {
                    var currentEntry = targetDirectory.DirectoryTable[i];
                    string entryName = new string(currentEntry.Dir_Namee);

                    if (currentEntry.dir_Attr == 0x0) // إذا كان ملف
                    {
                        file_Counter++;
                        file_Sizes += currentEntry.dir_FileSize;
                        total_File_Size += file_Sizes;
                        Console.WriteLine($"\t\t{file_Sizes}\t\t{entryName}");
                    }
                    else if (currentEntry.dir_Attr == 0x10) // إذا كان مجلد
                    {
                        folder_Counter++;
                        Console.WriteLine($"\t\t<DIR>\t\t{entryName}");
                    }
                    file_Sizes = 0;
                }

                // عرض ملخص التفاصيل
                Console.WriteLine($"\t\t\t{file_Counter} File(s)\t{total_File_Size} bytes");
                Console.WriteLine($"\t\t\t{folder_Counter} Dir(s)\t{Mini_FAT.get_Free_Size()} bytes free");
            }
            // في حالة عدم وجود العنصر أو كونه ليس ملفًا أو مجلدًا صالحًا
            else
            {
                Console.WriteLine($"Error: '{name}' is not a valid file or directory.");
            }
        }

        public static void Dir_Sup_Dir(bool x)
        {
            Program.currentDirectory.Read_Directory();
            int file_Counter = 0;
            int folder_Counter = 0;
            int file_Sizes = 0;
            int total_File_Size = 0;
            Console.WriteLine($"Directory of \"{new string (Program.currentDirectory.Dir_Namee)}\"  is: ");
            Console.WriteLine();
            if(x == true)
            {
                Console.WriteLine("\t\t<DIR>\t\t . ");
                Console.WriteLine("\t\t<DIR>\t\t .. ");
                folder_Counter += 2;
            }
            for (int i = 0; i < Program.currentDirectory.DirectoryTable.Count; i++)
            {
                if (Program.currentDirectory.DirectoryTable[i].dir_Attr == 0x0)
                {
                    file_Counter++;
                    file_Sizes += Program.currentDirectory.DirectoryTable[i].dir_FileSize;
                    total_File_Size += file_Sizes;
                    string m = string.Empty;
                    m += new string(Program.currentDirectory.DirectoryTable[i].Dir_Namee);
                    Console.WriteLine($"\t\t{file_Sizes}\t\t" + m);
                }
                else if (Program.currentDirectory.DirectoryTable[i].dir_Attr == 0x10) // لو في فولدر 
                {
                    folder_Counter++;
                    string S = new string(Program.currentDirectory.DirectoryTable[i].Dir_Namee);
                    Console.WriteLine("\t\t<DIR>\t\t" + S);
                }
                file_Sizes = 0;
            }
            Console.Write($"\t\t\t{file_Counter} File(s)\t ");
            if (file_Counter > 0)
            {
                Console.Write(total_File_Size);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine();
            }
            Console.WriteLine($"\t\t\t{folder_Counter} Dir(s)\t {Mini_FAT.get_Free_Size()} bytes free");

        }
        public static Directory MoveToDir(string fullPath, Directory currentDirectory)
        {
            string[] parts = fullPath.Split('\\');
            if (parts.Length == 0)
            {
                Console.WriteLine("Error: Invalid path.");
                return null;
            }

            Directory targetDirectory = currentDirectory;

            string s = new string(currentDirectory.Dir_Namee);
            if (s.Contains("\0"))
                s = s.Replace("\0", " ");
            s = s.Trim();
            if (parts[0].Equals(s.Trim('\0'), StringComparison.OrdinalIgnoreCase))
            {
                parts = parts.Skip(1).ToArray();
            }

            foreach (var part in parts)
            {
                if (string.IsNullOrWhiteSpace(part))
                {
                    continue;
                }

                int index = targetDirectory.search_Directory(part);
                if (index == -1)
                {
                    //Console.WriteLine($"Error: Directory '{part}' not found.");
                    
                    return null;
                }

                Directory_Entry entry = targetDirectory.DirectoryTable[index];

                if ((entry.dir_Attr & 0x10) != 0x10)
                {
                    Console.WriteLine($"Error: '{part}' is not a directory.");
                    return null;
                }

                targetDirectory = new Directory(entry.Dir_Namee, entry.dir_Attr, entry.dir_First_Cluster, targetDirectory);
                targetDirectory.Read_Directory();
            }


            return targetDirectory;
        }


        //khater

        public static Directory_Entry MoveToFile(string fullPath, Directory currentDirectory)
        {
            string[] parts = fullPath.Split('\\');
            if (parts.Length == 0)
            {
                Console.WriteLine("Error: Invalid path.");
                return null;
            }

            Directory targetDirectory = currentDirectory;

            string s = new string(currentDirectory.Dir_Namee);
            if (s.Contains("\0"))
                s = s.Replace("\0", " ");
            s = s.Trim();
            if (parts[0].Equals(s.Trim('\0'), StringComparison.OrdinalIgnoreCase))
            {
                parts = parts.Skip(1).ToArray();
            }

            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (string.IsNullOrWhiteSpace(parts[i]))
                {
                    continue;
                }

                int index = targetDirectory.search_Directory(parts[i]);
                if (index == -1)
                {
                    Console.WriteLine($"Error: Directory '{parts[i]}' not found.");
                    return null;
                }

                Directory_Entry entry = targetDirectory.DirectoryTable[index];

                if ((entry.dir_Attr & 0x10) != 0x10)
                {
                    Console.WriteLine($"Error: '{parts[i]}' is not a directory.");
                    return null;
                }

                targetDirectory = new Directory(entry.Dir_Namee, entry.dir_Attr, entry.dir_First_Cluster, targetDirectory);
                targetDirectory.Read_Directory();
            }

            string fileName = parts[^1]; // اسم الملف
            int fileIndex = targetDirectory.search_Directory(fileName);

            if (fileIndex == -1)
            {
                return null;
            }

            Directory_Entry fileEntry = targetDirectory.DirectoryTable[fileIndex];

            if ((fileEntry.dir_Attr & 0x10) == 0x10)
            {
                Console.WriteLine($"Error: '{fileName}' is a directory, not a file.");
                return null;
            }

            return fileEntry;
        }







        public static void RemoveDirectory(string[] paths)
        {
            foreach (var originalPath in paths)
            {
                string path = originalPath.Trim();
                string[] parts = path.Split(new char[] { ':', '\\' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 2)
                {
                    Console.WriteLine($"Error: Invalid path '{path}'.");
                    continue;
                }

                Directory targetDirectory = Program.currentDirectory;
                Directory parentDirectory = null;
                bool pathExists = true;
                int deleteIndex = -1;

                for (int i = 1; i < parts.Length; i++)
                {
                    int index = targetDirectory.search_Directory(parts[i]);

                    if (index == -1)
                    {
                        Console.WriteLine($"Error: Directory '{path}' not found.");
                        pathExists = false;
                        break;
                    }

                    Directory_Entry entry = targetDirectory.DirectoryTable[index];

                    if (entry.dir_Attr != 0x10)
                    {
                        Console.WriteLine($"Error: '{path}' is not a directory.");
                        pathExists = false;
                        break;
                    }

                    parentDirectory = targetDirectory;
                    deleteIndex = index;

                    int firstCluster = entry.dir_First_Cluster;
                    targetDirectory = new Directory(entry.Dir_Namee, entry.dir_Attr, firstCluster, targetDirectory);
                    targetDirectory.Read_Directory();
                }

                if (pathExists && targetDirectory != Program.currentDirectory && parentDirectory != null)
                {
                    if (targetDirectory.DirectoryTable.Count > 0)
                    {
                        Console.WriteLine($"Error: Directory '{path}' is not empty.");
                        Console.Write($"Are you sure you want to delete the directory '{path}' and all its contents? [yes, no]: ");
                        string answer = Console.ReadLine();

                        if (answer?.ToLower() == "yes" || answer?.ToLower() == "y")
                        {
                            targetDirectory.delete_Directory();
                            parentDirectory.DirectoryTable.RemoveAt(deleteIndex);
                            Program.currentDirectory.Write_Directory();
                            Console.WriteLine($"Directory '{path}' and its contents have been deleted successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"Directory '{path}' was not deleted.");
                        }
                    }
                    else
                    {
                        Console.Write($"Are you sure you want to delete the empty directory '{path}'? [yes, no]: ");
                        string answer = Console.ReadLine();

                        if (answer?.ToLower() == "yes" || answer?.ToLower() == "y")
                        {
                            targetDirectory.delete_Directory();
                            parentDirectory.DirectoryTable.RemoveAt(deleteIndex);
                            Program.currentDirectory.Write_Directory();
                            Console.WriteLine($"Directory '{path}' has been deleted successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"Directory '{path}' was not deleted.");
                        }
                    }
                }
            }
        }






        // khater



        public static void ChangeDirectory(string path)
        {
           
            if (path == ".")
            {
                return;
            }
            if (path.StartsWith(".."))
            {
                string[] levelsUp = path.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
                int l = levelsUp.Length;
                for (int i = 0; i < l; i++)
                {
                    int lastBackslash = Program.path.LastIndexOf("\\");

                    if (Program.currentDirectory.Parent != null)
                    {
                        Program.currentDirectory = Program.currentDirectory.Parent;
                        Program.path = Program.path.Substring(0, lastBackslash);
                        Program.currentDirectory.Read_Directory();

                    }
                    else
                    {
                        Console.WriteLine("Error: Cannot move above the root directory.");
                        return;
                    }
                }
                Console.WriteLine($"Changed to directory: '{new string(Program.currentDirectory.Dir_Namee).Trim()}'");
                return;
            }
            if (path.Contains("\\") || path.Contains("/"))
            {
                Directory targetDir = MoveToDir(path, Program.currentDirectory);
                if (targetDir != null)
                {
                    Program.currentDirectory = targetDir;
                    // need to update path here 
                    Program.path = path;
                    Console.WriteLine($"Changed to directory: '{new string(Program.currentDirectory.Dir_Namee).Trim()}'");
                }
                else
                {
                    Console.WriteLine("Error: The system cannot find the specified folder.");
                }
                return;
            }
            int index = Program.currentDirectory.search_Directory(path);
            if (index == -1)
            {
                Console.WriteLine($"Error: Directory '{path}' not found.");
                return;
            }

            Directory_Entry entry = Program.currentDirectory.DirectoryTable[index];
            if (entry.dir_Attr != 0x10)
            {
                Console.WriteLine($"Error: '{path}' is not a directory.");
                return;
            }

            else
            {
                string name = new string(entry.Dir_Namee).Trim();
                if (name.Contains("\0"))
                {
                    name = name.Replace("\0", " ");
                }
                name = name.Trim();
                Directory newDir = new Directory(name.ToCharArray(), entry.dir_Attr, entry.dir_First_Cluster, Program.currentDirectory);
                newDir.Read_Directory();
                Program.currentDirectory = newDir;
                Program.path = Program.path + "\\" + path;
                Console.WriteLine($"Changed to directory to '{new string(Program.currentDirectory.Dir_Namee).Trim()}'");
            }

        } //cd 
        
        public static void list_OF_Directory(string name)
        {
            if(name == "")
            {
                Console.WriteLine($"Error : dir command syntax is \r\ndir \r\nor \r\ndir [directory] \r\n[directory] can be directory name or fullpath of a directory \r\nor file name or full path of a file.");
                return;
            }
            if (name.Contains("\\"))
            {
                object o;
                o = MoveToDir(name, Program.currentDirectory);
                if (o != null)
                {
                    
                    Dir(name);
                    return;
                }
                else
                {
                    Console.WriteLine($"Error this path \"{name}\" is not exists ! ");
                    return;
                }


            }

            int index = Program.currentDirectory.search_Directory(name);
            if(index == -1) 
            {
                Console.WriteLine($"Error : Directory \"{name}\" not exists !");
                return;
            }
            else
            {
                Dir(name);
            }
        }//dir
        public static void Rename(string _Old,string _New) // rename
        {
            int index_old = Program.currentDirectory.search_Directory(_Old);
            int index_new = Program.currentDirectory.search_Directory(_New);

            if (index_old != -1) 
            {
                if(index_new == -1) 
                {
                    Directory_Entry e = Program.currentDirectory.DirectoryTable[index_old];

                    e.Dir_Namee = _New.ToCharArray();
                    Program.currentDirectory.Write_Directory();
                }
                else
                {
                    Console.WriteLine($"Error : {_New} already exists !");
                    return;
                }
               
            }
            else
            {
                Console.WriteLine($"Error this Filename \"{_Old}\" not exists !");
                return;
            }
        }
        #region Beta MD
        //public static void Make_Directory(string name)
        //{
        //    string dirName = name;


        //    if(dirName.Contains("\\"))
        //    {
        //        #region uu
        //        Directory cc;
        //        cc = MoveToDir(dirName, Program.currentDirectory); // N:\k\lol\nin so he got nin 
        //       // ok check if cc is null ? 
        //       if(cc == null)
        //       {

        //            string name_Of_Directory = new string(cc.Dir_Namee);
        //            int index = Program.currentDirectory.search_Directory(name_Of_Directory);
        //            if (index != -1)
        //            {
        //                Console.WriteLine("Folder already exists. ");
        //                return;
        //            }
        //            else
        //            {
        //                Directory newDIR = new Directory(name_Of_Directory.ToCharArray(), 0x10, 0, Program.currentDirectory);
        //                if (Program.currentDirectory.Can_Add_Entry(newDIR))
        //                {
        //                    Program.currentDirectory.add_Entry(newDIR);
        //                    Program.currentDirectory.Write_Directory();
        //                    if (Program.currentDirectory.Parent != null)
        //                    {
        //                        Program.currentDirectory.Parent.Write_Directory();
        //                        Program.currentDirectory.Update_Content(Program.currentDirectory.Get_Directory_Entry(), Program.currentDirectory.Parent.Get_Directory_Entry());
        //                    }
        //                    Console.WriteLine($"Directory '{name_Of_Directory}' created successfully.");
        //                    return;
        //                }
        //                else
        //                {
        //                    Console.WriteLine("Error : can't create the directory ");
        //                }
        //            }
        //        }              
        //        #endregion
        //    }
        //    if (Program.currentDirectory.search_Directory(dirName) != -1)
        //    {
        //        Console.WriteLine("Folder already exists.");
        //        return;
        //    }
        //    //int fc = Mini_FAT.get_Availabel_Cluster();
        //    Directory newDir = new Directory(dirName.ToCharArray(), 0x10, 0, Program.currentDirectory);
        //    if (Program.currentDirectory.Can_Add_Entry(newDir))
        //    {
        //        Program.currentDirectory.add_Entry(newDir);
        //        Program.currentDirectory.Write_Directory();
        //        if (Program.currentDirectory.Parent != null)
        //        {
        //            Program.currentDirectory.Parent.Write_Directory();
        //            Program.currentDirectory.Update_Content(Program.currentDirectory.Get_Directory_Entry(), Program.currentDirectory.Parent.Get_Directory_Entry());
        //        }

        //        Console.WriteLine($"Directory '{dirName}' created successfully.");

        //    }
        //    else
        //    {
        //        Console.WriteLine("Error: Could not create the directory.");
        //    }
        //} 
        #endregion
        public static void Make_Directory2(string name)
        {
            string dirName = name;

            // If the path contains a backslash, we need to handle nested directories
            if (dirName.Contains("\\"))
            {
             
                string[] parts = dirName.Split('\\');
                string parentPath = string.Join("\\", parts.Take(parts.Length - 1)); 
                string newDirName = parts.Last(); 

                Directory parentDir = MoveToDir(parentPath, Program.currentDirectory);

                if (parentDir == null)
                {
                    // Recursively create the missing parent directories
                    Console.WriteLine($"Creating missing parent directories for '{parentPath}'...");
                    Make_Directory2(parentPath); 

                    parentDir = MoveToDir(parentPath, Program.currentDirectory);
                    if (parentDir == null)
                    {
                        Console.WriteLine($"Error: Unable to navigate to the parent directory '{parentPath}'.");
                        return;
                    }
                }

                int index = parentDir.search_Directory(newDirName);
                if (index != -1)
                {
                    Console.WriteLine($"Error: Directory '{newDirName}' already exists.");
                    return;
                }

                Directory newDIR = new Directory(newDirName.ToCharArray(), 0x10, 0, parentDir);
                if (parentDir.Can_Add_Entry(newDIR))
                {
                    parentDir.add_Entry(newDIR);
                    parentDir.Write_Directory();

                    if (parentDir.Parent != null)
                    {
                        parentDir.Parent.Write_Directory();
                        parentDir.Update_Content(parentDir.Get_Directory_Entry(), parentDir.Parent.Get_Directory_Entry());
                    }

                    Console.WriteLine($"Directory '{newDirName}' created successfully in '{parentPath}'.");
                }
                else
                {
                    Console.WriteLine($"Error: Unable to create the directory '{newDirName}'.");
                }
            }
            else
            {
                if (Program.currentDirectory.search_Directory(dirName) != -1)
                {
                    Console.WriteLine("Error: Folder already exists.");
                    return;
                }

                Directory newDir = new Directory(dirName.ToCharArray(), 0x10, 0, Program.currentDirectory);
                if (Program.currentDirectory.Can_Add_Entry(newDir))
                {
                    Program.currentDirectory.add_Entry(newDir);
                    Program.currentDirectory.Write_Directory();

                    // Update the parent directory if necessary
                    if (Program.currentDirectory.Parent != null)
                    {
                        Program.currentDirectory.Parent.Write_Directory();
                        Program.currentDirectory.Update_Content(Program.currentDirectory.Get_Directory_Entry(), Program.currentDirectory.Parent.Get_Directory_Entry());
                    }

                    Console.WriteLine($"Directory '{dirName}' created successfully.");
                }
                else
                {
                    Console.WriteLine("Error: Could not create the directory.");
                }
            }
        } //md        
        public static void Type(string name)
        {
            int index = Program.currentDirectory.search_Directory(name);
            if (index != -1)
            {
                int fc = Program.currentDirectory.DirectoryTable[index].dir_First_Cluster;
                int sz = Program.currentDirectory.DirectoryTable[index].dir_FileSize;
                File_Entry f = new File_Entry(name.ToCharArray(), 0x0, fc, sz, Program.currentDirectory,"");
                f.Read_File_Content();
                f.Print_Content();
            }
            else
            {
                Console.WriteLine("Error: The specified name is not a file.");
            }
        }
        public static void Import(string path)  //import 
        {
            //Console.WriteLine(path);
            if (System.IO.File.Exists(path))
            {

                string[] pathParts = path.Split('\\'); // Split the path(extract name & size)
                string fileName = pathParts[pathParts.Length - 1];
                string fileContent = System.IO.File.ReadAllText(path);
                int size = fileContent.Length;
                int index = Program.currentDirectory.search_Directory(fileName);
                int fc = 0;

                if (index == -1)
                {
                    File_Entry f = new File_Entry(fileName.ToCharArray(), 0, fc, size, Program.currentDirectory, fileContent);
                    f.Write_File_Content();
                    Directory_Entry d = new Directory_Entry(fileName.ToCharArray(), 0,f.dir_First_Cluster,size);
                    Program.currentDirectory.DirectoryTable.Add(d);
                    Program.currentDirectory.Write_Directory();

                    if (Program.currentDirectory.Parent != null)
                    {
                        Program.currentDirectory.Update_Content(Program.currentDirectory.Get_Directory_Entry(), Program.currentDirectory.Parent.Get_Directory_Entry());

                        //Program.currentDirectory.Parent.Update_Content(Program.currentDirectory.Get_Directory_Entry());
                    }
                }
                else
                {
                    Console.WriteLine("Error: File with the same name already exists.");
                }
            }
            else
            {
                Console.WriteLine("Error: The specified name is not a file.");
            }

        }    
        public static void del(string name)
        {
            Directory dir = Program.currentDirectory;
            string[] name2 = name.Split('\\');
            if (name2.Length > 1)
            {
                name = string.Join("\\", name2, 0, name2.Length - 1);
                //if (ChangeDirectory(name) == -1)
                //{
                //    return;
                //}
                name = name2[(name2.Length) - 1];
            }
            int index = Program.currentDirectory.search_Directory(name);
            if (index != -1 && Program.currentDirectory.DirectoryTable[index].dir_Attr == 0x10)
            {
                Console.WriteLine($"This file : {name} is not file name or ACCESS DENIE!");
            }
            else
            {
                if (index != -1)
                {
                    int fc = Program.currentDirectory.DirectoryTable[index].dir_First_Cluster;
                    int sz = Program.currentDirectory.DirectoryTable[index].dir_FileSize;
                    File_Entry file = new File_Entry(name.ToCharArray(), 0x0, fc, sz, Program.currentDirectory,null);
                    file.Delete_File(name);
                    Program.currentDirectory.Write_Directory();
                    Program.currentDirectory.Read_Directory();

                    Console.WriteLine($"Directory '{name}' deleted successfully.");
                }
                else
                {
                    Console.WriteLine($"This file : {name} does not exist on your Disk!");
                }
            }
            Program.currentDirectory = dir;
        }//del


    }
}

