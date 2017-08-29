namespace simpleFuzzer
{
    public class EleAtt
    {
        public string name;
        public string type;
        public string[] values;
        public EleAtt(string nm, string t)
        {
            name = nm;
            type = t;
        }
        public EleAtt(string nm, string t, string[] val)
        {
            name = nm;
            type = t;
            values = val;
        }
    }
}