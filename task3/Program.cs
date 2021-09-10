using System;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using ConsoleTableExt;
using System.Data;




namespace task3
{
    class HashStep
    {
        internal static string HMAC { get; set; }
        internal static string step { get; set; }
        internal static string key { get; set; }
        private int bkey { get; set; }
        static void genHMAC(string data, string key)
        {
            byte[] bkey = Encoding.Default.GetBytes(key);
            using (var hmac = new HMACSHA512(bkey))
            {

                byte[] bstr = Encoding.Default.GetBytes(data);
                var bhash = hmac.ComputeHash(bstr);
                HMAC = BitConverter.ToString(bhash).Replace("-", string.Empty);
            }
        }
      
        public void getKey()
        {
            byte[] byteKey = new byte[16];
            RandomNumberGenerator rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(byteKey);
            bkey = BitConverter.ToInt32(byteKey, 0);
            key = BitConverter.ToString(byteKey).Replace("-", string.Empty);
            
        }

        public void genCompStep(string[] mass)
        {
            step = mass[new Random(bkey).Next(0, mass.Length - 1)];
            byte[] byteStep = Encoding.ASCII.GetBytes(step);
            genHMAC(step, key);
        }

    }

    class Rules
    {
        internal static string[,] winTable {get; set;}

        public void makeWinTable(int size)
        {
            winTable = new string[size, size];
            int n = size / 2;
            for (int i = 0; i < size; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    if (i == j)
                    {
                        winTable[i, j] = "Draw";
                    }
                    else if(j+n >= i && i > j)
                    {
                        winTable[i, j] = "Lose";
                    }
                    else if(!(i+n >= j) && j > i)
                    {
                        winTable[i, j] = "Lose";
                    }
                    else
                    {
                        winTable[i, j] = "Win ";
                    }
                }
            }
        }
        public void tellWinner(int userStep, int compStep)
        {
            string res = winTable[compStep, userStep];
            switch (res)
            {
                case "Draw":
                    Console.WriteLine("Draw..");
                    break;
                case "Win ":
                    Console.WriteLine("You Win!");
                    break;
                case "Lose":
                    Console.WriteLine("You Lose.");
                    break;
            }
        }
    }

    class Help
    {
        public void printLine(int maxL)
        {
            string[] array = new string[maxL];

            for (int i = 0; i < maxL; i++)
            {
                array[i] = "-";
            }

            for (int i = 0; i < maxL; i++)
            {
                Console.Write(array[i]);
            }
            Console.WriteLine();

        }
        public void showHelp(string[,] arr, string[] steps)
        {
            int len = (int)(steps.Length);            
            
            int longest = (steps.OrderByDescending(s => s.Length).First()).Length;
            if (longest < 4)
            {
                longest = 4;
            }
            
            string StepsLine = "PC \\ User ";
            int colLength = StepsLine.Length - 2;
            if (StepsLine.Length<longest)
            {
                StepsLine.PadRight(longest);
            }

            for (int i = 0; i < len; i++)
            {
                StepsLine += $"|{steps[i].PadRight(longest)}  ";
            }
            Console.Write(StepsLine + "\n");
            printLine(StepsLine.Length);

            string outLine = "";
            for (int i = 0; i < len; i++)
            {
                outLine += $"{steps[i].PadRight(colLength)}  ";
                for (int j = 0; j < len; j++)
                {
                    outLine += $"|{arr[i, j].PadRight(longest)}  ";
                }
                Console.WriteLine(outLine);
                printLine(StepsLine.Length);
                outLine = "";
            }

        }
    }
    class Program
    {
        static string userStep { get; set; }
        static void menu(string[] arr)
        {
            int lengthArr = arr.Length;
            for (int i = 0; i < lengthArr; i++)
            {
                Console.WriteLine(Convert.ToString(1 + i) + " - " + arr[i]);
            }
            Console.WriteLine("0 - Exit");
            Console.WriteLine("? - Help");
            Console.Write("Enter your move:");
            Program.userStep = Console.ReadLine();
        }
        static void Main(string[] args)    
        {
            if (args.Length < 3 || args.Length%2==0)
            {
                Console.WriteLine("Wrong input!");
                Console.WriteLine("Expected: >task3.exe rock paper scissors lizard Spock");
            }
            else if (args.Distinct().ToArray().Length != args.Length)
            {
                Console.WriteLine("Invalid parameters! Do not use similar variants!");
                return; 
            }
            else
            {
                bool play = true;
                do
                {
                    
                    
                    int lengthArr = args.Length;

                    HashStep HS = new HashStep();
                    HS.getKey();
                    HS.genCompStep(args);
                    Rules rule = new Rules();
                    rule.makeWinTable(lengthArr);
                    Console.WriteLine("HMAC:" + HashStep.HMAC);
                    menu(args);
                    Help hell = new Help();
                    int CompStepIndex = Array.FindIndex(args, item => item == HashStep.step);
                    try
                    {
                        if (userStep == "?")
                    {
                        hell.showHelp(Rules.winTable, args);
                    }
                    else if ((Convert.ToInt32(userStep) > 0) && (Convert.ToInt32(userStep) <= lengthArr))
                    {
                    Console.WriteLine("Your move:" + args[Convert.ToInt32(userStep)-1]);
                    Console.WriteLine("Computer move:" + args[CompStepIndex]);
                    rule.tellWinner((Convert.ToInt32(userStep) - 1), CompStepIndex);
                    Console.WriteLine("HMAC key=" + HashStep.key);
                    Environment.Exit(-1);
                    play = false;
                    }
                    else if (Convert.ToInt32(userStep) == 0)
                    {
                    Environment.Exit(-1);
                    play = false;
                    }
                    else
                    {
                        Console.WriteLine("Wrong input");
                    } 
                    }
                    catch
                    {
                        Console.WriteLine("Wrong input");
                    }
                    
                } while (play != false);
            }
        }
    }
}
