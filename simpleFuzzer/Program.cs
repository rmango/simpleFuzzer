using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace simpleFuzzer
{
    class Program
    {
        public static int varCount = 0;
        public static int fileCount = 0;
        public static List<Element> created = new List<Element>{ new Element("bool_x", "bool", new List<string>{"true", "false"}, new List<EleMethod>())};
        public static Random rand = new Random();//Random must be outside of function
        static void Main(string[] args)
        {
            //read grammar
            string gram = File.ReadAllText(@"C:\Users\t-mograh\Documents\Visual Studio 2017\Projects\simpleFuzzer\grammar.txt").Trim();
            //parse grammar
            List<Element> gra = ReadGram(gram);
            /*foreach (Element e in gra)
            {
                Console.WriteLine(e.type);
            }*/
            //create string of file
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < rand.Next(10, 15); i++)
            {
                str.Append(GenerateRandElement(gra, created));
            }
            Console.WriteLine(str);

            //turn string into file
            string fuzzFile = "fuzzerFile_" + fileCount;
            FileStream fuzzerFile = File.Create(fuzzFile);
            StreamWriter fileWrite = new StreamWriter(@"C:\Users\t-mograh\Documents\Visual Studio 2017\Projects\simpleFuzzer\" + fuzzFile + ".js");
            fileWrite.Write(str.ToString());
            fileWrite.Close();
            Console.ReadLine();
        }
        public static string GenerateRandElement(List<Element> gram, List<Element> created)
        {
            //choose random element index
            int r = rand.Next(1, gram.Count);

            //create element
            Element e = gram[r];
            e.name = e.type + "_" + varCount;
            varCount++;

            //choose random way to append to dom
            string[] domAppend = { "createTextNode", "appendChild", "insertBefore" };

            //initialize element in string
            String block = "var " + e.name + " = " + "document.createElement(\"" + e.type + "\");\ndocument.body." + domAppend[rand.Next(domAppend.Length)] + "(" + e.name + ");\n";

            //if has children, can replace or remove children

            //random number of attributes will be assigned
            int numA = rand.Next(e.attributes.Count);
            for (int j = 0; j < numA; j++)
            {
                block += e.name + ".setAttribute(" + e.attributes[rand.Next(e.attributes.Count)] + ", " + "ATTRIBUTEVALUE" + ");\n";
            }
            //add element to list of created elements
            created.Add(e);

            //random number of methods to be called
            int numM = rand.Next(e.methods.Count);
            for (int j = 0; j < numM; j++)
            {
                //choose random method + call method on element
                int randM = rand.Next(e.methods.Count);
                string[] p = e.methods[randM].parameters.ToArray();
                string str = "";
                foreach (string param in p)
                {
                    str += param + ", ";
                }
                if(str.Trim().Length > 1)
                {
                    str = str.Substring(0, str.Length - 2).Trim();
                }
                string value = "";
                //if the method requires parameters
                if (str != "")
                {
                    //search for created element of correct type - should be fixed so that not the first in list
                    foreach (Element el in created)
                    {
                        if (el.type == str)
                        {
                            value = el.name;
                        }
                    }
                    //if there were no matches in created, make a new element
                    if (value == "")
                    {
                        Element newEl = new Element(str);
                        newEl.name = str + "_" + varCount;
                        value = newEl.name;
                        Console.WriteLine("newEl " + value);
                        varCount++;
                        created.Add(newEl);
                    }
                }

                block += e.name + "." + e.methods[randM].name + "(" +  value + ");\n";
            }
            return block;
        }
        public static List<Element> ReadGram(string gram)
        {
            string[] lines = File.ReadAllLines(@"C:\Users\t-mograh\Documents\Visual Studio 2017\Projects\simpleFuzzer\grammar.txt");
            List<Element> elements = new List<Element>();
            Element temp = new Element();
            foreach (string line in lines)
            {
                if (temp != null && line.Trim() != null)
                {
                    if (line.IndexOf("::") != -1)
                    {
                        elements.Add(temp);
                        temp = new Element(line.Trim().Substring(0, line.Trim().IndexOf("::")));
                    }
                    else if (line.IndexOf("M: ") != -1)
                    {
                        //create method object w/name
                        int start = line.IndexOf("M: ") + 2;
                        EleMethod m = new EleMethod(line.Trim().Substring(start, line.Trim().IndexOf("|") - start - 1));

                        //add parameters to method object
                        int pstart = line.IndexOf("|") + 2;
                        int plen = line.IndexOf("-->") - pstart;
                        string ptemp = line.Substring(pstart, plen);
                        if (ptemp.IndexOf(",") != -1 && ptemp.Trim().Length > 2)
                        {
                            m.parameters.Add(ptemp.Substring(0, ptemp.IndexOf(",")).Trim());
                            ptemp = ptemp.Substring(ptemp.IndexOf(",") + 1);
                        }
                        if(ptemp.Trim() != "")
                        {
                            m.parameters.Add(ptemp.Trim());
                        }
                        temp.methods.Add(m);

                    }
                    else if (line.IndexOf("A: ") != -1)
                    {
                        int start = line.IndexOf("A: ") + 3;
                        temp.attributes.Add(line.Substring(start, line.Length - start));
                    }
                }
            }
            elements.Add(temp);

            return elements;
        }
    }
}
