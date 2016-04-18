﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace publishToolsTwo
{
    public class CopyDirectory
    {
        public static void copyDirectory(string sourceDirectory, string destDirectory)
        {
            destDirectory = destDirectory + @"\Temp";
            //判断源目录和目标目录是否存在，如果不存在，则创建一个目录
            if (!Directory.Exists(sourceDirectory))
            {
                Directory.CreateDirectory(sourceDirectory);
            }
            if (!Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }
            //拷贝文件
            copyFile(sourceDirectory, destDirectory);

            //拷贝子目录       
            //获取所有子目录名称
            string[] directionName = Directory.GetDirectories(sourceDirectory);

            foreach (string directionPath in directionName)
            {
                //根据每个子目录名称生成对应的目标子目录名称
                string directionPathTemp = destDirectory + "\\" + directionPath.Substring(sourceDirectory.Length + 1);

                //递归下去
                copyDirectory(directionPath, directionPathTemp);
            }
        }
        public static void copyFile(string sourceDirectory, string destDirectory)
        {
            //获取所有文件名称
            string[] fileName = Directory.GetFiles(sourceDirectory);

            foreach (string filePath in fileName)
            {
                //根据每个文件名称生成对应的目标文件名称
                string filePathTemp = destDirectory + "\\" + filePath.Substring(sourceDirectory.Length + 1);

                //若不存在，直接复制文件；若存在，覆盖复制
                if (File.Exists(filePathTemp))
                {
                    File.Copy(filePath, filePathTemp, true);
                }
                else
                {
                    File.Copy(filePath, filePathTemp);
                }
            }

        }

        public static void ClearFile(string path, StringCollection scFilter, StringCollection scIncludeFile, StringCollection scIncludeFolder, StringCollection scdelfiles, StringCollection scdelfolders)
        {
            DirectoryInfo dir = new DirectoryInfo(path);

            if (dir.Exists)
            {
                foreach (FileInfo fileInfo in dir.GetFiles())
                {
                    fileInfo.Attributes = FileAttributes.Normal;
                }
                if (scIncludeFolder.Count > 0)
                {
                    foreach (string folder in scIncludeFolder)
                    {
                        DirectoryInfo infolder = new DirectoryInfo(folder);
                        foreach (FileInfo firstFileInfo in infolder.GetFiles())
                        {
                            firstFileInfo.Attributes = FileAttributes.ReadOnly;
                        }
                    }
                }
                if (scIncludeFile.Count > 0)
                {
                    foreach (string include in scIncludeFile)
                    {
                        foreach (FileInfo fileinclude in dir.GetFiles(include, SearchOption.AllDirectories))
                        {
                            fileinclude.Attributes = FileAttributes.ReadOnly;
                        }

                    }
                    foreach (FileInfo file in dir.GetFiles("*.*", SearchOption.AllDirectories))
                    {
                        if (file.Attributes != FileAttributes.ReadOnly)
                        {
                            file.Delete();
                        }
                    }
                }
                else
                {
                    if (scdelfiles.Count > 0)
                    {
                        foreach (string scdelfile in scdelfiles)
                        {
                            foreach (FileInfo delfile in dir.GetFiles(scdelfile, SearchOption.AllDirectories))
                            {
                                delfile.Delete();
                            }

                        }
                    }
                    if (scdelfolders.Count > 0)
                    {
                        foreach (string scdelfolder in scdelfolders)
                        {
                            foreach (DirectoryInfo directory in dir.GetDirectories(scdelfolder))
                            {
                                directory.Delete();
                            }
                        }
                    }
                    foreach (FileInfo fileInfo in dir.GetFiles("*.*", SearchOption.AllDirectories))
                    {
                        foreach (string ft in scFilter)
                        {
                            if (fileInfo.Extension.ToUpper() == ft.ToUpper())
                            {
                                fileInfo.Delete();
                            }
                        }
                    }
                }
                foreach (DirectoryInfo directory in dir.GetDirectories())
                {
                    if (!(directory.GetFiles().Length > 0))
                    {
                        directory.Delete();
                    }
                }
                foreach (FileInfo fileInfo in dir.GetFiles())
                {
                    fileInfo.Attributes = FileAttributes.Normal;
                }
            }
        }
    }
}