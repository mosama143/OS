using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OS
{
    class Command_Line
    {
        public string[] comm_Arg;


        #region movetoDir me
        //public static Directory? MoveToDir(string fullPath)
        //{
        //    // Split the path into its parts
        //    string[] pathParts = fullPath.Split("\\");

        //    // First, check if the root directory matches
        //    string rootName = new string(Mini_FAT.Root.Dir_Namee).PadRight(11, '\0');
        //    if (rootName.Contains("\0"))
        //    {
        //        rootName = rootName.Replace('\0', ' ');
        //    }

        //    if (pathParts[0].Trim() != rootName.Trim())
        //    {
        //        Console.WriteLine($"Error: The root in the specified path \"{fullPath}\" does not exist.");
        //        return null;
        //    }

        //    // Start from the root directory
        //    Directory currentDir = Mini_FAT.Root;
        //    currentDir.Read_Directory();

        //    for (int i = 1; i < pathParts.Length; i++)
        //    {
        //        string dirName = pathParts[i].Trim();
        //        int index = currentDir.search_Directory(dirName);

        //        if (index == -1)
        //        {
        //            Console.WriteLine($"Error: The path \"{fullPath}\" does not exist.");
        //            return null;
        //        }

        //        Directory_Entry entry = currentDir.DirectoryTable[index];
        //        if (entry.dir_Attr != 0x10) 
        //        {
        //            Console.WriteLine($"Error: The path \"{fullPath}\" is not a directory.");
        //            return null;
        //        }

        //        int firstCluster = entry.dir_First_Cluster;
        //        Directory nextDir = new Directory(dirName.ToCharArray(), 0x10, firstCluster, currentDir);
        //        nextDir.Read_Directory();

        //        // Move to the next directory
        //        currentDir = nextDir;
        //    }

        //    Program.currentDirectory = currentDir;
        //    Program.path = fullPath;

        //    return currentDir;
        //} 
        #endregion

        public Command_Line(string command)
        {
            comm_Arg = command.Split(" ");
            if (comm_Arg.Length == 1)
            {
                Command(comm_Arg);
            }
            else if (comm_Arg.Length > 1)
            {
                Commmand2Arg(comm_Arg);
            }
        }

        static void Command(string[] command_Array)
        {
            if (command_Array[0].ToLower().Trim() == "quit")
            {
                Program.currentDirectory.Write_Directory();
                Environment.Exit(0);
            }
            else if (command_Array[0].ToLower() == "cls")
            {
                Console.Clear();
            }
            else if (command_Array[0].ToLower() == "cd")
            {
                Console.WriteLine(Program.currentDirectory.Dir_Namee);
            }
            else if ( string.IsNullOrWhiteSpace(command_Array[0].ToLower()))
            {
                return;
            }
            else if (command_Array[0].ToLower() == "help")
            {
                Console.WriteLine("cd\t\t- Change the current default directory to .\n\t\tIf the argument is not present, report the current directory.\n\t\tIf the directory does not exist an appropriate error should be reported.");
                Console.WriteLine("cls\t\t- Clear the screen.");
                Console.WriteLine("dir\t\t- List the contents of directory.");
                Console.WriteLine("quit\t\t- Quit the shell.");
                Console.WriteLine("copy\t\t- Copies one or more files to another location.");
                Console.WriteLine("del\t\t- Deletes one or more files.");

                Console.WriteLine("help\t\t- Provides Help information for commands.");
                Console.WriteLine("md\t\t- Creates a directory.");
                Console.WriteLine("rd\t\t- Removes a directory.");
                Console.WriteLine("rename\t\t- Renames a file.");
                Console.WriteLine("type\t\t- Displays the contents of a text file.");
                Console.WriteLine("import\t\t- import text file(s) from your computer.");
                Console.WriteLine("export\t\t- export text file(s) to your computer.");
            }
            else if (command_Array[0].ToLower() == "md")
            {
                Console.WriteLine("Error : md command syntax is \n md [directory] \n[directory] can be a new directory name or fullpath of a new directory\nCreates a directory.");
            }
            else if (command_Array[0].ToLower() == "rename")
            {
                Console.WriteLine("ERROR:\nRenames a file. \r\nrename command syntax is \r\nrename [fileName] [new fileName] \r\n[fileName] can be a file name or fullpath of a filename \r\n[new fileName] can be a new file name not fullpath");
            }
            else if (command_Array[0].ToLower() == "type")
            {
                Console.WriteLine("ERROR:\nDisplays the contents of a text file. \r\ntype command syntax is \r\ntype [file]+ \r\nNOTE: it displays the filename before its content for every \r\nfile \r\n[file] can be file Name (or fullpath of file) of text file \r\n+ after [file] represent that you can pass more than file \r\nName (or fullpath of file).");
            }
            #region dir
            else if (command_Array[0].ToLower() == "dir") 
            {

                int file_Counter = 0;
                int folder_Counter = 0;
                int file_Sizes = 0;
                int total_File_Size = 0;

                string name = new string(Program.currentDirectory.Dir_Namee);
                string current_Name = Program.path;


                Console.WriteLine($"Directory of {name} is  \n");
                //if(current_Name.Contains("\\"))
                //{
                //    ParserClass.Dir_Sup_Dir(true);
                //    return;
                //}
               
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
            #endregion           

            else
            {
                Console.WriteLine(command_Array[0] + " is not a valid command");
                Console.WriteLine("please valid Command");
            }
        }
        static void Commmand2Arg(string[] commandArray_2Agr)
        {
            if (commandArray_2Agr[0].ToLower() == "help" && commandArray_2Agr[1].ToLower() == "cd")
            {
                Console.WriteLine("cd\t\t - Change the current default directory to .\n\t\tIf the argument is not present, report the current directory.\n\t\tIf the directory does not exist an appropriate error should be reported.");
            }
            else if (commandArray_2Agr[0].ToLower() == "help" && commandArray_2Agr[1].ToLower() == "cls")
            {
                Console.WriteLine("cls\t\t- Clear the screen.");

            }
            else if (commandArray_2Agr[0].ToLower() == "help" && commandArray_2Agr[1].ToLower() == "dir")
            {
                Console.WriteLine("dir\t\t- List the contents of directory.");

            }
            else if (commandArray_2Agr[0].ToLower() == "help" && commandArray_2Agr[1].ToLower() == "quit")
            {
                Console.WriteLine("quit\t\t- Quit the shell.");

            }
            else if (commandArray_2Agr[0].ToLower() == "help" && commandArray_2Agr[1].ToLower() == "copy")
            {
                Console.WriteLine("copy\t\t- Copies one or more files to another location.");

            }
            else if (commandArray_2Agr[0].ToLower() == "help" && commandArray_2Agr[1].ToLower() == "del")
            {
                Console.WriteLine("del\t\t- Deletes one or more files.");

            }
            else if (commandArray_2Agr[0].ToLower() == "help" && commandArray_2Agr[1].ToLower() == "help")
            {
                Console.WriteLine("help\t\t- Provides Help information for commands.");

            }
            else if (commandArray_2Agr[0].ToLower() == "help" && commandArray_2Agr[1].ToLower() == "md")
            {
                Console.WriteLine("md\t\t- Creates a directory.");

            }
            else if (commandArray_2Agr[0].ToLower() == "help" && commandArray_2Agr[1].ToLower() == "rd")
            {
                Console.WriteLine("rd\t\t- Removes a directory.");

            }
            else if (commandArray_2Agr[0].ToLower() == "help" && commandArray_2Agr[1].ToLower() == "rename")
            {
                Console.WriteLine("rename\t\t- Renames a file.");
            }
            else if (commandArray_2Agr[0].ToLower() == "help" && commandArray_2Agr[1].ToLower() == "type")
            {
                Console.WriteLine("type\t\t- Displays the contents of a text file.");

            }
            else if (commandArray_2Agr[0].ToLower() == "help" && commandArray_2Agr[1].ToLower() == "import")
            {
                Console.WriteLine("import\t\t- import text file(s) from your computer.");

            }
            else if (commandArray_2Agr[0].ToLower() == "help" && commandArray_2Agr[1].ToLower() == "export")
            {
                Console.WriteLine("export\t\t- export text file(s) to your computer.");

            }
            else if (commandArray_2Agr[0].ToLower() == "cls")
            {
                Console.WriteLine("Error : cls command syntax is \n cls \n function: Clear the screen");
            }
            else if (commandArray_2Agr[0].ToLower() == "quit")
            {
                Console.WriteLine("Error : quit command syntax is \n quit \n function: Quit the shell");
            }


            // dir fullpath

           
            else if (commandArray_2Agr[0].ToLower() == "dir")
            {
                int file_Counter = 0;
                int folder_Counter = 0;
                int file_Sizes = 0;
                int total_File_Size = 0;
                string dname = commandArray_2Agr[1];
                if (dname == ".")
                {
                    // Current directory
                    ParserClass.Dir_Sup_Dir(true);
                    return;
                }
                else if (dname == "..")
                {
                    // Get the parent directory and list its contents
                    if (Program.currentDirectory.Parent != null)
                    {
                        #region ppp
                        //// Use the Parent directory to list contents                       
                        //Directory cc = Program.currentDirectory.Parent;

                        //// Now list the parent directory's contents
                        //Console.WriteLine($"Directory of \"{new string (cc.Dir_Namee)}\"  is:");
                        //for (int i = 0; i < cc.DirectoryTable.Count; i++)
                        //{

                        //    if (cc.DirectoryTable[i].dir_Attr == 0x0)  // File
                        //    {
                        //        Console.WriteLine($"\t\t{cc.DirectoryTable[i].dir_FileSize}\t\t{new string(cc.DirectoryTable[i].Dir_Namee)}");

                        //    }
                        //    else if (cc.DirectoryTable[i].dir_Attr == 0x10)  // Directory
                        //    {
                        //        Console.WriteLine("\t\t<DIR>\t\t" + new string(cc.DirectoryTable[i].Dir_Namee));
                        //    }
                        //}
                        //Console.WriteLine($"\t\t\t{cc.DirectoryTable.Count} Dir(s)\t {Mini_FAT.get_Free_Size()} bytes free"); 
                        #endregion
                        int file_Counter1 = 0;
                        int folder_Counter1 = 0;
                        int file_Sizes1 = 0;
                        int total_File_Size1 = 0;
                        Directory cc = Program.currentDirectory.Parent;
                        string name_l = new string(cc.Dir_Namee);
                        Console.WriteLine($"Directory of {name_l} : \n");
                        for (int i = 0; i < cc.DirectoryTable.Count; i++)
                        {
                            if (cc.DirectoryTable[i].dir_Attr == 0x0)
                            {
                                file_Counter1++;
                                file_Sizes1 += cc.DirectoryTable[i].dir_FileSize;
                                total_File_Size1 += file_Sizes1;
                                string m = string.Empty;
                                m += new string(cc.DirectoryTable[i].Dir_Namee);
                                Console.WriteLine($"\t\t{file_Sizes1}\t\t" + m);
                                // Console.WriteLine();
                            }
                            else if (cc.DirectoryTable[i].dir_Attr == 0x10) // لو في فولدر 
                            {
                                folder_Counter1++;
                                string S = new string(cc.DirectoryTable[i].Dir_Namee);
                                Console.WriteLine("\t\t<DIR>\t\t" + S);
                            }
                            file_Sizes1 = 0;
                        }
                        Console.Write($"\t\t\t{file_Counter1} File(s)\t ");
                        if (file_Counter1 > 0)
                        {
                            Console.Write(total_File_Size1);
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine();
                        }
                        Console.WriteLine($"\t\t\t{folder_Counter1} Dir(s)\t {Mini_FAT.get_Free_Size()} bytes free");
                    }
                    else
                    {
                        Console.WriteLine("Error: Cannot move above the root directory.");
                    }

                    return;
                }
                else
                {
                    // Specific directory (not '.' or '..')
                    ParserClass.list_OF_Directory(dname);
                    return;
                }
            }



            // work
            else if (commandArray_2Agr[0].ToLower() == "md")
            {
                if (commandArray_2Agr.Length < 2 || string.IsNullOrWhiteSpace(commandArray_2Agr[1]) || commandArray_2Agr[1] == "." || commandArray_2Agr[1].ToLower().Contains("."))
                {
                    Console.WriteLine("Error : md command syntax is \n md [directory] \n[directory] can be a new directory name or fullpath of a new directory\nCreates a directory.");
                    return;
                }

                ParserClass.Make_Directory2(commandArray_2Agr[1]);
            }

            // work
            else if (commandArray_2Agr[0].ToLower() == "rd")
            {
                if (commandArray_2Agr.Length < 2)
                {
                    Console.WriteLine("Error: No directories specified to remove.");
                }
                else
                {
                    string[] directoriesToDelete = commandArray_2Agr.Skip(1).ToArray();
                    ParserClass.RemoveDirectory(directoriesToDelete);
                }
            }

            //work xd         
            else if (commandArray_2Agr[0].ToLower() == "cd")
            {
                ParserClass.ChangeDirectory(commandArray_2Agr[1]);
            }
        
            // work
            else if (commandArray_2Agr[0].ToLower() == "rename")
            {

                ParserClass.Rename(commandArray_2Agr[1], commandArray_2Agr[2]);
            }
            // work          
            #region Beta
            //else if (commandArray_2Agr[0].ToLower() == "import")
            //{
            //    string name = commandArray_2Agr[1];
            //    if (System.IO.File.Exists(name))
            //    {
            //        Directory_Entry o = Program.currentDirectory.Get_Directory_Entry();
            //        string fileName = System.IO.Path.GetFileName(name); // Extract file name
            //        string fileContent = System.IO.File.ReadAllText(name);
            //        int size = fileContent.Length;
            //        int index = Program.currentDirectory.search_Directory(fileName);

            //        if (index == -1)
            //        {
            //            int fc = o.dir_First_Cluster;


            //            File_Entry f = new File_Entry(fileName.ToCharArray(), 0x0, fc, size, Program.currentDirectory, fileContent);
            //            f.Write_File_Content();

            //            Directory_Entry d = new Directory_Entry(fileName.ToCharArray(), 0x0, f.dir_First_Cluster, size); 
            //            Program.currentDirectory.DirectoryTable.Add(d);
            //            Program.currentDirectory.Write_Directory();

            //            if (Program.currentDirectory.Parent != null)
            //            {
            //                Program.currentDirectory.Parent.Update_Content(o, Program.currentDirectory.Get_Directory_Entry());
            //            }
            //        }
            //        else
            //        {
            //            Console.WriteLine($"Error: File '{fileName}' already exists in the current directory.");
            //        }
            //    }
            //    else
            //    {
            //        Console.WriteLine($"Error: The specified file '{name}' does not exist.");
            //    }
            //} 
            #endregion
            else if (commandArray_2Agr[0].ToLower()=="import")
            {
                ParserClass.Import(commandArray_2Agr[1]);
            }
            // work
            else if (commandArray_2Agr[0].ToLower() == "export")
            {
                int index = Program.currentDirectory.search_Directory(commandArray_2Agr[1].ToString());
                if (index == -1)
                {
                    Console.WriteLine("The File is not Exist");
                }
                else
                {
                    if (!System.IO.Directory.Exists(commandArray_2Agr[2].ToString()))
                    {
                        Console.WriteLine("The System Canot find the folder Destination in your computer");
                    }
                    else
                    {
                        if (Program.currentDirectory.DirectoryTable[index].dir_Attr == 0x0)
                        {
                            int FirstCluster = Program.currentDirectory.DirectoryTable[index].dir_First_Cluster;
                            int fileSize = Program.currentDirectory.DirectoryTable[index].dir_FileSize;
                            string temp = "";
                            File_Entry f = new File_Entry(commandArray_2Agr[1].ToCharArray(), 0, FirstCluster, fileSize, Program.currentDirectory, temp);
                            f.Write_File_Content();
                            f.Read_File_Content();
                            StreamWriter StreamWriter = new StreamWriter(commandArray_2Agr[2].ToString() + "\\" + commandArray_2Agr[1].ToString());
                            StreamWriter.Write(f.content);
                            StreamWriter.Flush();
                            StreamWriter.Close();
                        }
                        else if (Program.currentDirectory.DirectoryTable[index].dir_Attr == 0x10)
                        {
                            int FirstCluster = Program.currentDirectory.DirectoryTable[index].dir_First_Cluster;
                            int fileSize = Program.currentDirectory.DirectoryTable[index].dir_FileSize;
                            Directory f = new Directory(commandArray_2Agr[1].ToCharArray(), 1, FirstCluster,Program.currentDirectory);
                            f.Write_Directory();
                            f.Read_Directory();
                            StreamWriter StreamWriter = new StreamWriter(commandArray_2Agr[2].ToString() + "\\" + commandArray_2Agr[1].ToString());
                            StreamWriter.Write(f);
                            StreamWriter.Flush();
                            StreamWriter.Close();
                        }
                    }
                }
            }
            //del work
            else if (commandArray_2Agr[0].ToLower() == "del") // for only files
            {
                ParserClass.del(commandArray_2Agr[1]);
            }
            // work
            else if (commandArray_2Agr[0].ToLower() == "type")//display the file content
            {
                int index = Program.currentDirectory.search_Directory(commandArray_2Agr[1].ToString());
                if (index != -1)
                {
                    int firstcluster = Program.currentDirectory.DirectoryTable[index].dir_First_Cluster;
                    int filesize = Program.currentDirectory.DirectoryTable[index].dir_FileSize;
                    string content = "";
                    File_Entry FE = new File_Entry(commandArray_2Agr[1].ToCharArray(), 0, firstcluster, filesize, Program.currentDirectory, content);
                    FE.Read_File_Content();
                    Console.WriteLine(FE.content);
                }
                else
                {
                    Console.WriteLine("The system cannot find the file specified.");
                }

                //ParserClass.Type(commandArray_2Agr[1]);
            }           
            // work 
            else if (commandArray_2Agr[0].ToLower() == "copy")// for only files
            {
                int indexSource = Program.currentDirectory.search_Directory(commandArray_2Agr[1].ToString());
                if (indexSource == -1)  // السورس مش موجود
                {
                    Console.WriteLine("The File is not Exist");
                }
                else // لقى السورس
                {
                    string fileName = "";

                    fileName = commandArray_2Agr[2].ToString();
                    int destination_index = Program.currentDirectory.search_Directory(fileName);
                    if (destination_index != -1)
                    {
                        if (Program.currentDirectory.Dir_Namee == commandArray_2Agr[2].ToCharArray())
                        {
                            Console.WriteLine(" The main destination and the new one are the same.. please enter another destination");
                        }
                        else
                        {
                            int F_Cluster = Program.currentDirectory.DirectoryTable[destination_index].dir_First_Cluster;
                            Directory d = new Directory(commandArray_2Agr[2].ToCharArray(), 1, F_Cluster, Program.currentDirectory);
                            int f_cluster = Program.currentDirectory.DirectoryTable[indexSource].dir_First_Cluster;
                            int file_size = Program.currentDirectory.DirectoryTable[indexSource].dir_FileSize;
                            Program.currentDirectory = d;

                            Program.path += "\\" + commandArray_2Agr[2].ToString();
                            File_Entry f = new File_Entry(commandArray_2Agr[1].ToCharArray(), 0, f_cluster, file_size, Program.currentDirectory, "");
                            Program.currentDirectory.DirectoryTable.Add(f);
                            Program.currentDirectory.Write_Directory();
                            Program.currentDirectory.Read_Directory();
                        }
                    }
                    else
                    {
                        int F_Cluster = Mini_FAT.get_Availabel_Cluster();
                        Directory d = new Directory(commandArray_2Agr[2].ToCharArray(), 0, F_Cluster, Program.currentDirectory);
                        Program.currentDirectory.DirectoryTable.Add(d);

                        int f_cluster = Program.currentDirectory.DirectoryTable[indexSource].dir_First_Cluster;
                        int file_size = Program.currentDirectory.DirectoryTable[indexSource].dir_FileSize;
                        Program.currentDirectory = d;

                        Program.path += "\\" + commandArray_2Agr[2].ToString();
                        File_Entry f = new File_Entry(commandArray_2Agr[1].ToCharArray(), 0, f_cluster, file_size, Program.currentDirectory, "");
                        Program.currentDirectory.DirectoryTable.Add(f);
                        Program.currentDirectory.Write_Directory();
                        Program.currentDirectory.Read_Directory();

                    }
                }
            }

            else if (
                commandArray_2Agr[0].ToLower() == "help" && commandArray_2Agr[1].ToLower() != "cd" && commandArray_2Agr[1].ToLower() != "cls" && commandArray_2Agr[1].ToLower() != "quit" && commandArray_2Agr[1].ToLower() != "copy" && commandArray_2Agr[1].ToLower() != "del"
                && commandArray_2Agr[1].ToLower() !="help" && commandArray_2Agr[1].ToLower() != "md" && commandArray_2Agr[1].ToLower() !="rd" && commandArray_2Agr[1].ToLower() !="rename" && commandArray_2Agr[1].ToLower() !="type" 
                && commandArray_2Agr[1].ToLower() !="import" && commandArray_2Agr[1].ToLower() !="export"
                )
                
            {
                Console.WriteLine(commandArray_2Agr[1].ToLower() + " is not a valid command.");
                Console.WriteLine("please valid Command ");
            }
          
            else
            {
                Console.WriteLine(commandArray_2Agr[0] + " is not a valid command.");
                Console.WriteLine("please valid Command ");
            }
        }
    }
}
