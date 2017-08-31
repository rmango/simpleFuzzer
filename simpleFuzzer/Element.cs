using System.Collections.Generic;
using System.Linq;

namespace simpleFuzzer
{
    public class Element
    {
        public string name;
        public string type;
        public List<EleAtt> attributes;
        public List<EleMethod> methods;

        public Element()
        {
            name = "";
            type = "";
            attributes = new List<EleAtt>();
            methods = new List<EleMethod>();
        }
        public Element(string t)
        {
            name = "";
            type = t;
            attributes = new List<EleAtt>();
            methods = new List<EleMethod>();
        }
        public Element(string t, Element inherit)
        {
            name = name;
            type = t;
            attributes = new List<EleAtt>();
            for (int i = 0; i < inherit.attributes.Count(); i++)
            {
                attributes.Add(inherit.attributes[i]);
            }
            methods = new List<EleMethod>();
            for (int j = 0; j < inherit.methods.Count(); j++)
            {
                methods.Add(inherit.methods[j]);
            }
        }
        public Element(string nm, string t)
        {
            name = nm;
            type = t;
            attributes = new List<EleAtt>();
            methods = new List<EleMethod>();
        }

        public Element(string n, string t, List<EleAtt> att, List<EleMethod> me)
        {
            name = n;
            type = t;
            attributes = att;
            methods = me;
        }


    }
}