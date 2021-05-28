using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace OrmCodeGeneratorApp
{
    public enum SqlType
    {
        GetAll, GetByID, InsertByID, UpdateByID, DeleteByID
    }

    public class Argument 
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("object")]
        public string Object { get; set; }

        public Argument()
        {
        }

        public Argument(string T, string obj)
        {
            Type = T;
            Object = obj;
        }
    }

    public class Arguments
    {
        [XmlElement("argument")]
        public List<Argument> Argumentso { get; set; }

        public string GetParameters()
        {
            StringBuilder sb = new StringBuilder();

            if (this.Argumentso != null)
            {
                if (Argumentso.Count > 0)
                {
                    foreach (Argument a in this.Argumentso)
                    {
                        if (a.Type != Consts.TransactionManager)
                        {
                            sb.Append(a.Type);
                            sb.Append(" ");
                            sb.Append(a.Object);
                            sb.Append(", ");
                        }
                    }

                    if (sb.Length >= 2)
                    {
                        sb.Remove(sb.Length - 2, 2);
                    }
                }
            }
            return sb.ToString();
        }

        public string GetArguments()
        {
            StringBuilder sb = new StringBuilder();

            if (this.Argumentso != null)
            {
                if (Argumentso.Count > 0)
                {
                    foreach (Argument a in this.Argumentso)
                    {
                        if (a.Type != Consts.TransactionManager)
                        {
                            sb.Append(a.Object);
                            sb.Append(", ");
                        }
                    }

                    if (sb.Length >= 2)
                    {
                        sb.Remove(sb.Length - 2, 2);
                    }
                }
            }

            return sb.ToString();
        }
    }

    [XmlRoot("function", Namespace = "urn:sform.com")]
    public class Function
    {
        [XmlElement("file")]
        public string File { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("sql")]
        public string Sql { get; set; }

        [XmlElement("visibility")]
        public string Visibility { get; set; }

        [XmlElement("return_type")]
        public string ReturnType{ get; set; }

        [XmlElement("default_value")]
        public string DefaultValue { get; set; }

        [XmlElement("arguments")]
        public Arguments Arguments { get; set; }

        [XmlElement("content")]
        public string Content { get; set; }

        public static Function Deserialize(string fullPath)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(Function));
            TextReader reader = new StreamReader(fullPath);
            object obj = deserializer.Deserialize(reader);

            Function f = (Function)obj;
            reader.Close();

            return f;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(this.Visibility);
            sb.Append(this.ReturnType);
            sb.Append(this.Name);
            

            if (this.Arguments != null)
            {
                sb.Append("(");
                sb.Append(this.Arguments.GetParameters());
                sb.Append(")\r\n");
            }

            if (!string.IsNullOrEmpty(this.Content))
            {
                //sb.Append("{\r\n");
                sb.Append(this.Content + "\r\n");
                //sb.Append("}\r\n");
            }

            return sb.ToString();
        }

        public void AddArgument(Argument arg)
        {
            if(this.Arguments.Argumentso==null)
            {
                this.Arguments.Argumentso = new List<Argument>();
            }

            
                this.Arguments.Argumentso.Add(arg);
        }

        public void Replace(string old, string newt)
        {
            StringBuilder sb = new StringBuilder(Content);
            sb.Replace(old, newt);

            Content = sb.ToString();
        }
    }
}
