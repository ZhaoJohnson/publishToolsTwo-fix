using System;
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
                    File.SetAttributes(filePathTemp, FileAttributes.Normal);
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
                //清楚只读状态
                File.SetAttributes(path, FileAttributes.Normal);
                dir.Attributes= FileAttributes.Normal;

                //1.开始判断是否有必要保留的文件夹
                if (scIncludeFolder.Count > 0)
                {
                    foreach (string folder in scIncludeFolder)
                    {
                        foreach (DirectoryInfo directory in dir.GetDirectories(folder))
                        {
                            foreach (FileInfo info in directory.GetFiles("*.*", SearchOption.AllDirectories))
                            {
                                info.Attributes= FileAttributes.ReadOnly;
                            }
                        }
                    }
                }
                //2.判断是否有必要保留的文件
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
                            file.Attributes= FileAttributes.Normal;
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
                                delfile.Attributes= FileAttributes.Normal;
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
                                try
                                {
                                    Directory.Delete(directory.FullName,true);
                                }
                                catch (Exception)
                                {
                                    foreach (FileInfo delFileInfo in directory.GetFiles())
                                    {
                                        delFileInfo.Attributes= FileAttributes.Normal;
                                    }
                                    Directory.Delete(directory.FullName, true);
                                }
                            }
                        }
                    }
                    //删除指定文件
                    foreach (FileInfo fileInfo in dir.GetFiles("*.*", SearchOption.AllDirectories))
                    {
                        foreach (string ft in scFilter)
                        {
                            if (fileInfo.Extension.ToUpper() == ft.ToUpper())
                            {
                                try
                                {
                                    fileInfo.Delete();
                                }
                                catch (Exception)
                                {
                                    fileInfo.Attributes= FileAttributes.Normal;
                                    fileInfo.Delete();
                                }
                            }
                        }
                    }
                }
                foreach (DirectoryInfo directory in dir.GetDirectories())
                {
                    if (directory.GetFiles().Count()!= 0)
                    {
                        continue;
                    }
                    Directory.Delete(directory.FullName,true);
                }
                foreach (FileInfo fileInfo in dir.GetFiles("*.*", SearchOption.AllDirectories))
                {
                    fileInfo.Attributes = FileAttributes.Normal;
                }
            }
        }
    }
}
