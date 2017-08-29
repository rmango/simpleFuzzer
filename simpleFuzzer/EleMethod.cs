using System.Collections.Generic;

namespace simpleFuzzer
{
    public class EleMethod
    {
        public string name;
        public List<string> parameters;
        public List<string> re;
        public EleMethod(string n)
        {
            name = n;
            parameters = new List<string>();
            re = new List<string>();
        }
        public EleMethod(string nm, List<string> para, List<string> r)
        {
            name = nm;
            parameters = para;
            re = r;
        }
    }
}