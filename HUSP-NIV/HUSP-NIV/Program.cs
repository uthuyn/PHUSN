using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace uspan
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());
        }
    }
}
            /*int choose;
            AlgoUSpan algo = new AlgoUSpan();
            do
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("=====================  MENU =======================");
                Console.WriteLine("|| 1-Mining All High Utility Sequential Patterns ||");
                Console.WriteLine("|| 2-Mining Some High Utility Sequential Patterns||");
                Console.WriteLine("|| 3-Hiding All High Utility Sequential Patterns ||");
                Console.WriteLine("|| 4-Hiding Sequential Patterns by MSICF         ||");
                Console.WriteLine("|| 5-Process data                                ||");
                Console.WriteLine("|| 0-Exit                                        ||");
                Console.WriteLine("===================================================");
                Console.Write("Choosing option:");
                while (!int.TryParse(Console.ReadLine().ToString(), out choose)) ;
                switch (choose)
                {
                    case 1:
                        {
                            /*Console.Clear();
                            Console.Write(" Input database name (KOSARAK/FIFA/SIGN/BIBLE/BMS:");
                            string s = Console.ReadLine() + ".txt";
                            // the input database
                            string input = fileToPath(s);
                            Console.Write(" Input minimum utility:");
                            int minutil;
                            while (!int.TryParse(Console.ReadLine().ToString(), out minutil)) ;
                            // the path for saving the patterns found
                            string output = ".//output.txt";
                            // run the algorithm
                            IList<QMatrix> database = algo.loadData(input, minutil);
                            algo.runAlgorithm(database, output);
                            // print statistics
                            algo.miningPrintStats();
                            break;

                            Console.Clear();
                            Console.Write(" Input database name (KOSARAK/FIFA/SIGN/BIBLE/BMS:");
                            string s = Console.ReadLine();
                            // the input database
                            string input = fileToPath(s+".txt");
                            string external = s + "_ExternalUtility.txt";
                            Console.Write(" Input minimum utility:");
                            int minutil;
                            while (!int.TryParse(Console.ReadLine().ToString(), out minutil)) ;
                            // the path for saving the patterns found
                            string output = ".//output.txt";                 
                            // run the algorithm
                            IList<QMatrix> database = algo.loadDataWithInternalExternal(external,input, minutil);
                            //IList<QMatrix> database = algo.loadData(input, minutil);
                            algo.runAlgorithm(database, output);
                            // print statistics
                            algo.miningPrintStats();
                            if(!File.Exists(fileToPath(".ModifiedData_ExternalUtility.txt")))
                                File.Copy(external, ".//.ModifiedData_ExternalUtility.txt", true);
                            break;
                        }
                    case 2:
                        {
                            /*Console.Clear();
                            Console.Write(" Input database name (KOSARAK/FIFA/SIGN/BIBLE/BMS:");
                            string s = Console.ReadLine() + ".txt";
                            // the input database
                            string input = fileToPath(s);
                            Console.Write(" Input minimum utility:");
                            int minutil = System.Convert.ToInt32(Console.ReadLine());
                            // the path for saving the patterns found
                            string output = ".//output.txt";
                            // set the maximum pattern length
                            Console.Write(" Input max pattern length:");
                            int maxPatternLength = System.Convert.ToInt16(Console.ReadLine());
                            algo.setMaxPatternLength(maxPatternLength);
                            // run the algorithm
                            IList<QMatrix> database = algo.loadDataUtility(input, minutil);
                            algo.runAlgorithm(database, output);
                            // print statistics
                            algo.miningPrintStats();
                            break;

                            Console.Clear();
                            Console.Write(" Input database name (KOSARAK/FIFA/SIGN/BIBLE/BMS:");
                            string s = Console.ReadLine();
                            // the input database
                            string input = fileToPath(s + ".txt");
                            string external = s + "_ExternalUtility.txt";
                            Console.Write(" Input minimum utility:");
                            int minutil;
                            while (!int.TryParse(Console.ReadLine().ToString(), out minutil)) ;
                            // the path for saving the patterns found
                            string output = ".//output.txt";
                            // set the maximum pattern length
                            Console.Write(" Input max pattern length:");
                            int maxPatternLength = System.Convert.ToInt16(Console.ReadLine());
                            algo.setMaxPatternLength(maxPatternLength);
                            // run the algorithm
                            IList<QMatrix> database = algo.loadDataWithInternalExternal(external, input, minutil);
                            //IList<QMatrix> database = algo.loadData(input, minutil);
                            algo.runAlgorithm(database, output);
                            // print statistics
                            algo.miningPrintStats();
                            
                            break;
                        }
                    case 3:
                        {
                            algo.hidingAlgorithm();
                            algo.hidingPrintStats();
                            algo.writeModifiedData();
                            break;
                        }
                    case 4:
                        {
                            algo.MSICF();
                            break;
                        }
                    case 5:
                        {
                            Console.Write(" Input database name (KOSARAK/FIFA/SIGN/BIBLE/BMS:");
                            string s = Console.ReadLine() + ".txt";
                            algo.writeData(algo.loadSyntheticData(s));
                            //algo.makeExternalKosarak(s);
                            //algo.writeDataKosarak("kosarak990k_ExternalUtility.txt", "kosarak_990k.txt");
                            Console.Write("Finish!");
                            break;
                        }
                }
            }
            while (choose != 0);
        }
        public static string fileToPath(string fileName)
        {
            return Path.GetFullPath(fileName);
        }
    }
}*/
