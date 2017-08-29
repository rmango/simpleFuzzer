using System.Collections.Generic;

namespace simpleFuzzer
{
    public class Element
    {
        public string name;
        public string type;
        public List<string> attributes;
        public List<EleMethod> methods;

        public Element()
        {
            name = "";
            type = "";
            attributes = new List<string>();
            methods = new List<EleMethod>();
        }
        public Element(string t)
        {
            name = "";
            type = t;
            attributes = new List<string>();
            methods = new List<EleMethod>();
        }

        public Element(string n, string t, List<string> att, List<EleMethod> me)
        {
            name = n;
            type = t;
            attributes = att;
            methods = me;
        }


    }
}