using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace gui.forms
{
    public class XmlForm
    {
        public Form Window { get; set; }

        public List<Control> Controls { get; set; } = new List<Control>();

        public XmlForm() {}

        public void Run()
        {
            Application.Run(Window);
        }

        public Control get(string name)
        {
            foreach(var c in Controls)
            {
                if (c.Name == name)
                    return c;
            }
            return null;
        }

        public Assembly asm = null;
        public void LoadNode(XmlNode node, object obj)
        {
            foreach (var prop in obj.GetType().GetProperties())
            {
                foreach (XmlAttribute attr in node.Attributes)
                {
                    if (attr.Name == prop.Name)
                    {
                        if (prop.GetValue(obj).GetType() == typeof(Int32))
                        {
                            prop.SetValue(obj, int.Parse(attr.Value));
                        }
                        else if (prop.GetValue(obj).GetType() == typeof(Int16))
                        {
                            prop.SetValue(obj, short.Parse(attr.Value));
                        }
                        else if (prop.GetValue(obj).GetType() == typeof(Int64))
                        {
                            prop.SetValue(obj, long.Parse(attr.Value));
                        }
                        else
                        {
                            prop.SetValue(obj, attr.Value);
                        }
                    }
                }
            }
        }
        public Control ParseControl(XmlNode node)
        {
            Control control = null;


            if (asm == null)
                foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (item.FullName.Contains("Forms"))
                    {
                        asm = item;
                        break;
                    }
                }


            if (asm != null)
            {
                foreach (var type in asm.GetTypes())
                {
                    if (type.Name == node.Name)
                    {
                        control = (Control)asm.CreateInstance(type.FullName);
                        LoadNode(node, control);
                        Controls.Add(control);
                        break;
                    }
                }
            }

            if (control != null)
                foreach (XmlNode nod in node.ChildNodes)
                {
                    control.Controls.Add(ParseControl(nod));
                }

            return control;
        }

        public XmlForm LoadForm(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            Window = new Form();
            LoadNode(doc.FirstChild, Window);
            foreach (XmlNode item in doc.ChildNodes[0].ChildNodes)
            {
                Window.Controls.Add(ParseControl(item));
            }

            return this;
        }

        public static XmlForm Load(string xmlpath) => new XmlForm().LoadForm(File.ReadAllText(xmlpath));
    }
}
