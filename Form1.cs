using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace VSC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                closeWscript();
                cleanComputer();
                cleanUSB();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            MessageBox.Show("All Done. WBS Virus removed.");
        }


        void closeWscript()
        {
            Process[] ps = Process.GetProcessesByName("wscript");
                foreach (Process p in ps)
                {
                    p.Kill();
                    p.WaitForExit();
                }
                ps = Process.GetProcessesByName("wscript");
        }

        void cleanComputer()
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);


            foreach (string s in k.GetValueNames())
            {
                string path = (string)k.GetValue(s);
                if (path.IndexOf("wscript.exe //B \"") == 0)
                {
                    k.DeleteValue(s, false);
                    path = path.Remove(0, 17);
                    path = path.Remove(path.Length - 1, 1);
                    System.IO.File.Delete(path);
                }
            }
            k.Close();
        }

        void cleanUSB()
        {
            var driveList = DriveInfo.GetDrives();

            foreach (DriveInfo drive in driveList)
            {
                if (drive.DriveType == DriveType.Removable)
                {
                   FileInfo[] fis = drive.RootDirectory.GetFiles();
                   foreach (FileInfo fi in fis)
                   {
                       fi.Attributes = RemoveFileAttribute(fi.Attributes, FileAttributes.Hidden | FileAttributes.System);
                       if (fi.FullName.ToUpper().IndexOf(".WSF") == fi.FullName.Length - 4)
                           fi.Delete();
                       else if (fi.FullName.ToUpper().IndexOf(".LNK") == fi.FullName.Length - 4)
                           fi.Delete();

                   }

                   DirectoryInfo[] dis = drive.RootDirectory.GetDirectories();
                   foreach (DirectoryInfo di in dis)
                   {
                       di.Attributes = RemoveFileAttribute(di.Attributes, FileAttributes.Hidden | FileAttributes.System);
                   }
                }
            }
        }
        private static FileAttributes RemoveFileAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }
    }
}
