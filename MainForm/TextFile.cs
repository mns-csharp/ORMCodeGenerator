using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace OrmCodeGeneratorApp
{
    public enum WriteMode
    {
        Overwrite, Append
    }
    public enum FileType
    {
        CSharp, Text
    }
    public class TextFile
    {
        public string Text { get; private set; }
        public FileType FileType { get; set; }
        public Function Function { get; private set; }
        public SqlType SqlType { get; private set; }

        private static string _applicationDirectory;
        public static string ApplicationDirectory
        {
            get 
            {
                if (string.IsNullOrEmpty(_applicationDirectory))
                {
                    _applicationDirectory = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                }

                return _applicationDirectory;
            }
        }

        public static void CreateDirectory(string directory)
        {
            System.IO.Directory.CreateDirectory(directory);
        }

        public static List<FileInfo> GetFileNames(string directory)
        {
            DirectoryInfo d = new DirectoryInfo(directory);//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.xml"); //Getting Text files

            return new List<FileInfo>(Files);
        }

        private string _readPath;
        public string ReadPath 
        {
            set 
            {
                _readPath = value;
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(value);

                if ((fileInfo.Extension != ".txt") && (fileInfo.Extension != ".cs") && (fileInfo.Extension != ".xml"))
                {
                    throw new Exception("Error: Not a text file!");
                }
                if(fileInfo.Extension == ".txt")
                {
                    FileType = FileType.Text;
                }
                else if(fileInfo.Extension == ".cs")
                {
                    FileType = FileType.Text;
                }
            }
            get 
            {
                return _readPath ;
            }
        }

        public void Read()
        {
            try
            {
                Text = System.IO.File.ReadAllText(ReadPath);
            }
            catch
            {
                throw;
            }
        }

        public void ReadXml()
        {
            try
            {
                // Read xml file
                Function = Function.Deserialize(ReadPath);

                // segregate info
                StringBuilder sb = new StringBuilder("\t\tpublic " + Function.ReturnType + " " + Function.Name+"(");
                foreach(Argument a in Function.Arguments.Argumentso)
                {
                    sb.Append(a.Type + " " + a.Object+", ");
                }
                sb.Remove(sb.Length - 2, 2);
                sb.Append(")\r\n");
                sb.Append("\t\t{");
                sb.Append(Function.Content);
                sb.Append("\t\t}\r\n\r\n");

                Text = sb.ToString();
                SqlType = (SqlType)Enum.Parse(typeof(SqlType), Function.Sql);
            }
            catch
            {
                throw;
            }
        }

        public string _writePath;
        public string WritePath
        {
            set
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(value);
                TextFile.CreateDirectory(fileInfo.DirectoryName);
                _writePath = fileInfo.FullName;
            }
            get 
            { 
                return _writePath; 
            }
        }

        public void Write()
        {
            try
            {
                //Function.Content = Text;
                System.IO.File.WriteAllText(WritePath, Text);
            }
            catch
            {
                throw;
            }
        }

        public void Replace(string old, string newt)
        {
            StringBuilder sb = new StringBuilder(Text);
            sb.Replace(old, newt);

            Text = sb.ToString();
        }

        public void Append(string append)
        {
            StringBuilder sb = new StringBuilder(Text);
            sb.Append(append);

            Text = sb.ToString();
        }

        public bool Contains(string str)
        {
            return Text.Contains(str);
        }

        public void Clear()
        {
            Text = string.Empty;
        }
    }
}
