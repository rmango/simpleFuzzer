using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace simpleFuzzer {
    class Program {
        public static int varCount = 0;
        public static int fileCount = 0;
        public const int ALLOWED_NESTS = 5; 
        public static List<Element> createdEl = new List<Element>{};
        public static List<JSElement> createdJSEl = new List<JSElement> {};
        public static Random rand = new Random();//Random must be outside of function
        public static List<Element> gram = new List<Element> {};//grammar.txt, parsed

        static void Main(string[] args) {
            //read + parse grammar
            gram = ReadGram();

            for (int fileNum = 0; fileNum < 7; fileNum++) {
                //create string of file
                string str = "";

                //add base elements to str
                createdJSEl.Add(new JSElement("bool_x", "Boolean"));
                //could add elements to arrays in beginning, then use foreach loop to print them out

                for (int i = 0; i < rand.Next(20, 30); i++) {
                    str += DoSomething("", ALLOWED_NESTS);
                }
                Console.WriteLine(str);

                //create an html file of str
                CreateFile(str);
                varCount = 0;
            }

            Console.ReadLine();
        }

        public static void CreateFile(string str) {
            //turn string into file
            string fuzzFile = "fuzzTest_" + fileCount;
            fileCount++;
            FileStream fuzzerFile = File.Create(fuzzFile);
            StreamWriter fileWrite = new StreamWriter(@"C:\Users\Morga\Documents\Github\simpleFuzzer\" + fuzzFile + ".html");
            fileWrite.Write("<head><title>Test</title></head>" +
                "<body><script>" + str.ToString() + "</script>" +
                "<h1>Hello</h1></body>");
            fileWrite.Close();
        }

        public static string DoSomething(string str, int count) {
            if (count > 0) {
                int randInt = rand.Next(3);
                if (randInt == 0) {
                    str = GenerateRandElement(str, count - 1);
                } else if (randInt == 1) {
                    str = IfStatement(str, count - 1);
                } else if (randInt == 2) {
                    str = GenerateRandEventListener(str, count - 1);
                }
            }
            return str;
        }

        private static string GenerateRandEventListener(string str, int count) {
            string[] eventVal= { "click", "mousedown", "keydown", "error", "unload", "drag", "load" };
            string block = "";

            string action = "";
            action = DoSomething("", count);

            block += createdEl[rand.Next(createdEl.Count)].name + "." + "addEventListener(\"" + eventVal[rand.Next(eventVal.Length)] + "\", function() {\n" + action + "});\n";
            return block;
        }


        public static string GenerateRandElement(string str, int count) {
            //choose random element index
            int r = rand.Next(0, gram.Count);

            //create element
            Element e = gram[r];
            e.name = e.type + "_" + varCount;
            varCount++;

            //choose random way to append to dom
            string[] domAppend = { "createTextNode", "appendChild", "insertBefore", "replaceChild" };

            //initialize element in string
            String block = "var " + e.name + " = " + "document.createElement(\"" + e.type + "\");\n";

            //optional - make element by calling method from another element
            /*if(rand.Next() > 0.5)//happens ~1/2 the time
            {

            }*/

            //append element to dom
            string appendTo = "";
            if(e.name.IndexOf("_0") != -1) {//append first element to body
                appendTo = "body";
            }
            else {
                appendTo = createdEl[rand.Next(createdEl.Count)].name;
            }
            block += "document." + appendTo + "." + domAppend[rand.Next(domAppend.Length)] + "(" + e.name + ");\n";

            //if has children, can replace or remove children

            //random number of attributes will be assigned
            int numA = rand.Next(e.attributes.Count);
            for (int j = 0; j < numA; j++) {
                //choose random attribute
                EleAtt at = e.attributes[rand.Next(e.attributes.Count)];
                string valA = "";
                if(at.values != null) {
                    //choose random value from array of values
                    valA = at.values[rand.Next(at.values.Length)];
                }
                block += e.name + "." + at.name + " = \"" + valA + "\";\n";
            }
            //add element to list of created elements
            createdEl.Add(e);

            //random number of methods to be called
            int numM = rand.Next(e.methods.Count);
            for (int j = 0; j < numM; j++) {
                //choose random method + call method on element
                int randM = rand.Next(e.methods.Count);
                string[] p = e.methods[randM].parameters.ToArray();

                //for each parameter, find another element with attributes or methods of the correct type
                block += e.name + "." + e.methods[randM].name.Trim() + "("; //start of method string
                string block2 = "";
                foreach (string param in p) {
                    string value = "";
                    //if the method requires parameters
                    if (param != "") {
                        //list of all possible elements
                        string[] values = { };
                        //search for created element of correct type - should be fixed so that not the last in list
                        foreach (Element el in createdEl) {
                            //see if any methods return the correct type
                            foreach(EleMethod m in el.methods) {
                                foreach(string s in m.re) {
                                    if(s.Trim().Equals(param.Trim())) {
                                        value = el.name + "." + m +"add parameters here";
                                    }
                                }
                            }
                            //see if any attributes are correct type
                            foreach(EleAtt a in el.attributes) {
                                if(a.type == param) {
                                    value = el.name + "." + a;
                                }
                            }
                        }
                        //see if any matches in createdJSEL
                        if(values.Length == 0) {
                            foreach(JSElement jel in createdJSEl) {
                                if(jel.type == param) {
                                    if(jel.name == null) {
                                        jel.name = jel.type + "_" + varCount;
                                        varCount++;
                                    }
                                    value = jel.name;
                                }
                            }
                        }
                        //if there were no matches in createdEl, make a new js object with attributes or methods of correct type
                        if (values.Length == 0) {
                            JSElement newEl = new JSElement(param + "_" + varCount, param);
                            block2 += "var " + newEl.name + " = new " + newEl.type + "();\n";
                            value = newEl.name;
                            varCount++;
                            createdJSEl.Add(newEl);
                        } else {
                            //randomly choose from list of possible elements
                            value = values[rand.Next(values.Length)];
                        }
                    }
                    //commas between parameters in method string
                    block += value + ", ";
                }
                //end of method string
                block = block.Substring(0, block.Length - 2); //to remove ", "
                block += ");\n";
                block = block2 + block;
            }
            return block;
        }

        public static string IfStatement(string str, int count) {
            string block = "";
            //search for bool elements
            List<string> matches = new List<string>{ };
            foreach (JSElement ele in createdJSEl) {
                if (ele.type == "Boolean") {
                    matches.Add(ele.name);
                    //ifVal = ele.name;
                }
            }
            string ifVal = "";
            if(matches.Count != 0) {
                ifVal = matches[rand.Next(matches.Count)];
            }
            //if there were no matches in created, make a new element
            if (ifVal == "") {
                JSElement newEl = new JSElement("bool_" + varCount, "Boolean");
                block += "var " + newEl.name + " = new " + newEl.type + "();\n";
                ifVal = newEl.name;
                varCount++;
                createdJSEl.Add(newEl);
            }

            string action = DoSomething("", count);

            block += "if(" + ifVal + ") {\n" + action + "}\n";
            return block;
        }
        public static List<Element> ReadGram() {
            string[] lines = File.ReadAllLines(@"C:\Users\Morga\Documents\Github\simpleFuzzer\grammar.txt");
            //List<Element> elements = new List<Element>();//list of elements in grammar
            Element temp = new Element();
            foreach (string line in lines) {
                if (temp != null && line.Trim() != null) {
                    bool inherit = false;

                    if (line.IndexOf("::") != -1) {//if heading line of grammar 
                        //start of new element: add previous element to gram
                        if (gram.Count() > 0) {
                            gram.Add(temp);

                            //search for inheritance
                            for (int i = 0; i < gram.Count(); i++) {
                                //if there is an element to inherit from within the grammar
                                if (gram[i].type.IndexOf(line.Substring(line.IndexOf("::") + 2).Trim()) != -1) {
                                    temp = new Element(line.Trim().Substring(0, line.Trim().IndexOf("::")), gram[i]);
                                    temp.name = temp.type + "_gram";
                                    inherit = true;
                                    i = gram.Count();//end loop
                                }
                            }
                        }
                        //if no inheritance
                        if (!inherit) {
                            temp = new Element(line.Trim().Substring(0, line.Trim().IndexOf("::")));
                        }
                        gram.Add(temp);
                    } else if (line.IndexOf("M: ") != -1) {
                        //create method object w/name
                        int start = line.IndexOf("M: ") + 2;
                        EleMethod m = new EleMethod(line.Trim().Substring(start, line.Trim().IndexOf("|") - start));

                        //add parameters to method object
                        int pstart = line.IndexOf("|") + 2;
                        int plen = line.IndexOf("-->") - pstart;
                        string ptemp = line.Substring(pstart, plen);
                        if (ptemp.IndexOf(",") != -1 && ptemp.Trim().Length > 2) {
                            m.parameters.Add(ptemp.Substring(0, ptemp.IndexOf(",")).Trim());
                            ptemp = ptemp.Substring(ptemp.IndexOf(",")).Trim().TrimStart(',');
                        }
                        m.parameters.Add(ptemp.Trim());

                        //add return types to method object
                        int rstart = line.IndexOf("-->");
                        int rlen = line.Length - rstart;
                        string rtemp = line.Substring(rstart, rlen);
                        if (rtemp.IndexOf(",") != -1 && rtemp.Trim().Length > 2) {
                            m.re.Add(rtemp.Substring(0, ptemp.IndexOf(",")).Trim());
                            rtemp = rtemp.Substring(rtemp.IndexOf(",") + 1);
                        }
                        if (rtemp.Trim() != "")
                        {
                            m.re.Add(rtemp.Trim());
                        }

                        //add method object to element object
                        temp.methods.Add(m);

                    } else if (line.IndexOf("A: ") != -1) {
                        //create attribute object
                        int Astart = line.IndexOf("A: ") + 3;
                        int Amid = line.IndexOf("|") - 5;
                        int Aend = line.IndexOf("--");
                        string nameA = line.Substring(Astart, Amid);
                        string typeA = line.Substring(Amid + 7, Aend - Amid - 7);
                        string vals = line.Substring(Aend + 2);
                        string[] valA = new string[] { };
                        while (vals.Trim().Length > 0 && vals.IndexOf(",") != -1) {
                            string[] tempVal = { vals.Substring(0, vals.IndexOf(", ")) };
                            valA = valA.Union(tempVal).ToArray();
                            vals = vals.Substring(vals.IndexOf(", ") + 1).Trim();
                        }
                        if (vals.Trim() == "") {//if no values, assign random values
                            //generate random string
                            string randomStr = "";
                            for(int k = 0; k < rand.Next(5, 25); k++) {
                                int dec = rand.Next(0, 254);
                                randomStr += Convert.ToChar(dec).ToString();
                            }
                            valA = new string[] { "abc", "123", "true", randomStr };
                        } else {
                            valA = new string[]{ vals };
                        }
                        EleAtt a = new EleAtt(nameA, typeA, valA);
                        temp.attributes.Add(a);
                    }
                }
            }
            return gram;
        }
    }
}
