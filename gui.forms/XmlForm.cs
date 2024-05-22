using System;
using System.Collections.Generic;
using System.Drawing;
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
                        object val = prop.GetValue(obj);
                        Type valType = val.GetType();
                        object setvalue = attr.Value;
                        if (valType.BaseType == typeof(Enum))
                        {
                            setvalue = Enum.Parse(valType,setvalue.ToString());
                        }
                        else if (valType == typeof(Color))
                        {
                            setvalue = ColorTranslator.FromHtml(setvalue.ToString());
                        }
                        else if (valType == typeof(Point))
                        {
                            string[] size = setvalue.ToString().Split(',');
                            setvalue = new Point(int.Parse(size[0]), int.Parse(size[1]));
                        }
                        else if (valType == typeof(Size))
                        {
                            string[] size = setvalue.ToString().Split(',');
                            setvalue = new Size(int.Parse(size[0]), int.Parse(size[1]));
                        }
                        else
                        {
                            setvalue = Convert.ChangeType(attr.Value, valType);
                        }
                        prop.SetValue(obj, setvalue);
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
