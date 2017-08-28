using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace simpleFuzzer
{
    class Program
    {
        public static int varCount = 0;
        static void Main(string[] args)
        {
            //read grammar
            string gram = System.IO.File.ReadAllText(@"C:\Users\t-mograh\Documents\Visual Studio 2017\Projects\simpleFuzzer\grammar.txt").Trim();
            //parse grammar
            List<Element> gra = ReadGram(gram);
            foreach (Element e in gra)
            {
                Console.WriteLine(e.type);
            }

            //create string of file
            StringBuilder str = new StringBuilder();
            List<Element> created = new List<Element>();
            for (int i = 0; i < 3; i++)
            {
                str.Append(GenerateRandElement(gra, created) + "\n");
            }
            Console.WriteLine(str);
            //turn string into file

            //Append to Dom with appendChild

            Console.ReadLine();
        }
        public static string GenerateRandElement(List<Element> gram, List<Element> created)
        {
            // choose random element index
            Random rand = new Random();
            int r = rand.Next(gram.Count);

            //create element
            Element e = gram[r];
            e.name = e.type + "_" + varCount;
            varCount++;

            //initialize element in string
            String block = "var " + e.name + " = " + "document.createElement(\"" + e.type + "\");\ndocument.body.appendChild(" + e.name + ");";

            //random number of attributes will be assigned
            int num = rand.Next(e.attributes.Count);
            for (int j = 0; j < num; j++)
            {
                block += "\n" + e.name + ".setAttribute(" + e.attributes[rand.Next(e.attributes.Count)] + ", " + "ATTRIBUTEVALUE" + ");";
            }
            //add element to list of created elements
            created.Add(e);

            return block;
        }
        public static List<Element> ReadGram(string gram)
        {
            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\t-mograh\Documents\Visual Studio 2017\Projects\simpleFuzzer\grammar.txt");
            List<Element> elements = new List<Element>();
            Element temp = new Element();
            foreach (string line in lines)
            {
                if (temp != null && line.Trim() != null)
                {
                    if (line.IndexOf("::=") != -1)
                    {
                        elements.Add(temp);
                        temp = new Element(line.Trim().Substring(0, line.Trim().IndexOf("::=")));
                    }
                    else if (line.IndexOf("M: ") != -1)
                    {
                        int start = line.IndexOf("M:") + 3;
                        temp.methods.Add(line.Substring(start, line.Length - start));
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
