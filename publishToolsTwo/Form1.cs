using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ICSharpCode.SharpZipLib;
using ICSharpCode.TextEditor;
using System.Diagnostics;

namespace publishToolsTwo
{
    public partial class form_main : Form
    {
        public form_main()
        {
            InitializeComponent();
        }

        public string OpenPath { get; set; }
        const int CLOSE_SIZE = 12;
        public static bool getdate = false;
        private void Form1_Load(object sender, EventArgs e)
        {
            this.tc_desktop.TabPages.Clear();
            this.tc_desktop.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.tc_desktop.Padding = new System.Drawing.Point(CLOSE_SIZE, CLOSE_SIZE);
            this.tc_desktop.DrawItem += Tc_desktop_DrawItem;
            this.tc_desktop.MouseDown += Tc_desktop_MouseDown;
        }



        private void command1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sourcePath = Application.StartupPath;
            OpenPath = sourcePath + @"\Defult.ini";

            //defaultBuilder.AppendLine("");
            //defaultBuilder.AppendLine(";Set to delete empty folders");
            //defaultBuilder.AppendLine(";Ture Need configuration settings True/False");
            //defaultBuilder.AppendLine(";Example: key=False");
            //defaultBuilder.AppendLine("[Deletefolder]");
            //defaultBuilder.AppendLine("Deletefolder=True");

            if (!File.Exists(OpenPath))
            {
                StringBuilder defaultBuilder = new StringBuilder();
                defaultBuilder.AppendLine(";Sets the path to the folder needs to be cleaned");
                defaultBuilder.AppendLine(";Need to configure the full path name");
                defaultBuilder.AppendLine(";Example: key = D:");
                defaultBuilder.AppendLine("[Path]");
                defaultBuilder.AppendLine("Path_1=D:\\folder");
                defaultBuilder.AppendLine("");
                defaultBuilder.AppendLine(";Set the file type needs to be filtered");
                defaultBuilder.AppendLine(";Need to configure the full suffix");
                defaultBuilder.AppendLine(";Example: key=.exe");
                defaultBuilder.AppendLine("[Filter]");
                defaultBuilder.AppendLine("Filter_1=.xml");
                defaultBuilder.AppendLine("filter_2=.pdb");
                defaultBuilder.AppendLine("");
                defaultBuilder.AppendLine(";Setup requires only the files required to keep");
                defaultBuilder.AppendLine(";Need to configure the full name Or Null");
                defaultBuilder.AppendLine(";Example: key=setup.exe");
                defaultBuilder.AppendLine("[includefile]");
                defaultBuilder.AppendLine("include=api.dll");
                defaultBuilder.AppendLine("");
                defaultBuilder.AppendLine(";Setup requires only the files required to keep");
                defaultBuilder.AppendLine(";Need to configure the full name Or Null");
                defaultBuilder.AppendLine(";Example: key=windows");
                defaultBuilder.AppendLine("[includefolder]");
                defaultBuilder.AppendLine("includefolder=api.dll");
                defaultBuilder.AppendLine("");
                defaultBuilder.AppendLine(";Setting Deletes the specified file");
                defaultBuilder.AppendLine(";Need to configure the full name Or Null");
                defaultBuilder.AppendLine(";Example: key=fileName.dll");
                defaultBuilder.AppendLine("[DelFile]");
                defaultBuilder.AppendLine("DelFile=fileName.dll");
                defaultBuilder.AppendLine("");
                defaultBuilder.AppendLine(";Setting Deletes the specified Folder");
                defaultBuilder.AppendLine(";Need to configure the full Path Or Null");
                defaultBuilder.AppendLine(";Example: key=folderName");
                defaultBuilder.AppendLine("[DelFolder]");
                defaultBuilder.AppendLine("DelFolder=folderName");
                defaultBuilder.AppendLine("");
                defaultBuilder.AppendLine(";Set the output path of the zip file");
                defaultBuilder.AppendLine(";Need to configure the file name and full address");
                defaultBuilder.AppendLine(";Example: key=D:\\test\\setup.zip");
                defaultBuilder.AppendLine("[OutPath]");
                defaultBuilder.AppendLine("outPath=D:\\Sbin.zip");
                FileStream myfs = new FileStream(OpenPath, FileMode.Create);
                StreamWriter mysw = new StreamWriter(myfs);
                mysw.Write(defaultBuilder.ToString());
                mysw.Flush();
                mysw.Close();
                myfs.Close();
            }
            this.tc_desktop.TabPages.Add(CreateProject(OpenPath));
        }

        private TabPage CreateProject(string path)
        {
            TabPage newpage = new TabPage(); //新增一个PAGE
            newpage.Text = Path.GetFileNameWithoutExtension(path);
            newpage.ToolTipText = path;
            TextEditorControl nbTextEdit = new TextEditorControl
            {
                Dock = DockStyle.Fill,
                Parent = newpage,
                Text = File.ReadAllText(path)
            };
            newpage.Controls.Add(nbTextEdit);
            return newpage;
        }

        private void userToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog dg = new OpenFileDialog();
            dg.Filter = "ini files(*.ini)|*.ini";
            dg.FilterIndex = 1;
            dg.RestoreDirectory = true;
            dg.InitialDirectory = Application.StartupPath;
            if (dg.ShowDialog() == DialogResult.OK)
            {
                this.tc_desktop.TabPages.Add(CreateProject(dg.FileName));
            }

        }

        private void goToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string scOutput = "";
            string scPath = "";
            //bool deleteFolder = false;

            TabPage currentPage = this.tc_desktop.SelectedTab;
            if (currentPage == null) return;
            SaveIniFile(currentPage);
            //StringCollection scPath = new StringCollection();
            StringCollection scFilter = new StringCollection();
            StringCollection scIncluedfile = new StringCollection();
            StringCollection scIncluedfolder = new StringCollection();
            StringCollection scdelfiles = new StringCollection();
            StringCollection scdelfolders = new StringCollection();
            ReadIniFile(ref scOutput, currentPage, ref scPath, scFilter, scIncluedfile, scIncluedfolder, scdelfiles, scdelfolders);
            Working(scPath, scFilter, scIncluedfile, scIncluedfolder, scdelfiles, scdelfolders, scOutput);

            System.Diagnostics.Process.Start("Explorer.exe", $"/select,{scOutput}");
        }

        private static void ReadIniFile(ref string scOutput, TabPage currentPage, ref string scPath, StringCollection scFilter, StringCollection scIncluedfile, StringCollection scIncluedFolder, StringCollection scdelfiles, StringCollection scdelfolders)
        {
            IniFiles iniFiles = new IniFiles(currentPage.ToolTipText);
            //读取所有Section
            StringCollection inisection = new StringCollection();
            iniFiles.ReadSections(inisection);
            foreach (string section in inisection)
            {
                //Section内的所有KEY
                StringCollection Key = new StringCollection();
                iniFiles.ReadSection(section, Key);
                foreach (string values in Key)
                {
                    switch (section)
                    {
                        case "Path":
                            scPath=(iniFiles.ReadString(section, values, ""));
                            break;
                        case "Filter":
                            scFilter.Add(iniFiles.ReadString(section, values, ""));
                            break;
                        case "includefile":
                            scIncluedfile.Add(iniFiles.ReadString(section, values, ""));
                            break;
                        case "includefolder":
                            scIncluedFolder.Add(iniFiles.ReadString(section, values, ""));
                            break;
                        case "DelFile":
                            scdelfiles.Add(iniFiles.ReadString(section, values, ""));
                            break;
                        case "DelFolder":
                            scdelfolders.Add(iniFiles.ReadString(section, values, ""));
                            break;
                        case "OutPath":
                            scOutput = iniFiles.ReadString(section, values, "");
                            break;
                        case "GetDate":
                            getdate = true;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private static void SaveIniFile(TabPage currentPage)
        {
            TextEditorControl nbTextEdit = currentPage.Controls[0] as TextEditorControl;
            File.WriteAllText(currentPage.ToolTipText, nbTextEdit.Text);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Working(string scpath, StringCollection scFilter, StringCollection scIncludeFile, StringCollection scIncludeFolder, StringCollection scdelfiles, StringCollection scdelfolders
            , string output)
        {
            FileInfo outfile = new FileInfo(output);
            var outfilepath = outfile.Directory;

            CopyDirectory.copyDirectory(scpath, outfilepath.FullName);
            CopyDirectory.ClearFile(outfilepath.FullName, scFilter, scIncludeFile, scIncludeFolder, scdelfiles, scdelfolders);

            //clearFile(scpath, scFilter, scInclude, scdelfiles, scdelfolders);


            ZipHelper.CreateZip(outfilepath.FullName, output);

        }

        public void clearFile(StringCollection scpath, StringCollection scFilter, StringCollection scInclude, StringCollection scdelfiles, StringCollection scdelfolders)
        {
            foreach (string path in scpath)
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                if (dir.Exists)
                {
                    if (scInclude.Count > 0)
                    {
                        foreach (string include in scInclude)
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
                }
            }
        }


        //添加Tabpage关闭按钮
        private void Tc_desktop_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x = e.X, y = e.Y;
                //计算关闭区域   
                Rectangle myTabRect = this.tc_desktop.GetTabRect(this.tc_desktop.SelectedIndex);
                myTabRect.Offset(myTabRect.Width - (CLOSE_SIZE + 3), 2);
                myTabRect.Width = CLOSE_SIZE;
                myTabRect.Height = CLOSE_SIZE - 4;
                //如果鼠标在区域内就关闭选项卡   
                bool isClose = x > myTabRect.X && x < myTabRect.Right
                 && y > myTabRect.Y && y < myTabRect.Bottom;
                if (isClose == true)
                {
                    SaveIniFile(this.tc_desktop.SelectedTab);
                    this.tc_desktop.TabPages.Remove(this.tc_desktop.SelectedTab);
                }
            }
        }

        private void Tc_desktop_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                Rectangle myTabRect = this.tc_desktop.GetTabRect(e.Index);
                //先添加TabPage属性   
                e.Graphics.DrawString(this.tc_desktop.TabPages[e.Index].Text
                , this.Font, SystemBrushes.ControlText, myTabRect.X + 2, myTabRect.Y + 2);
                //再画一个矩形框
                using (Pen p = new Pen(Color.Black))
                {
                    myTabRect.Offset(myTabRect.Width - (CLOSE_SIZE + 3), 2);
                    myTabRect.Width = CLOSE_SIZE;
                    myTabRect.Height = CLOSE_SIZE;
                    e.Graphics.DrawRectangle(p, myTabRect);
                }
                //填充矩形框
                Color recColor = e.State == DrawItemState.Selected ? Color.DarkRed : Color.DarkGray;
                using (Brush b = new SolidBrush(recColor))
                {
                    e.Graphics.FillRectangle(b, myTabRect);
                }
                //画关闭符号
                using (Pen p = new Pen(Color.White))
                {
                    //画"/"线
                    Point p1 = new Point(myTabRect.X + 3, myTabRect.Y + 3);
                    Point p2 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + myTabRect.Height - 3);
                    e.Graphics.DrawLine(p, p1, p2);
                    //画"/"线
                    Point p3 = new Point(myTabRect.X + 3, myTabRect.Y + myTabRect.Height - 3);
                    Point p4 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + 3);
                    e.Graphics.DrawLine(p, p3, p4);
                }
                e.Graphics.Dispose();
            }
            catch (Exception ex)
            {

            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage currentPage = this.tc_desktop.SelectedTab;
            if (currentPage == null) return;
            SaveIniFile(currentPage);
        }
    }
}
