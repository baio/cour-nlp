using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reformat
{
    class Program
    {
        /*
         * test - reforamat
         * ..\..\App_Data\gene.test  ..\..\..\Tagger\Reduce\App_Data\gene.maped forward
         * 
         * key - reforamat
         * ..\..\App_Data\gene.key  ..\..\..\Scorer\App_Data\gene.key.data forward
         * 
         * dev - reforamat
         * ..\..\App_Data\gene.dev  ..\..\..\Tagger\Reduce\App_Data\gene.dev.data forward \s
         * 
         * Tagger dev -> reforamat back  
         * ..\..\..\Tagger\Reduce\App_Data\gene.reduced ..\..\..\..\h1\gene_dev.p1.out back
         * 
         * Tagger test -> reforamat back  
         * ..\..\..\Tagger\Reduce\App_Data\gene.reduced ..\..\..\..\h1\gene_test.p1.out back
         * 
         */

        /// <summary>
        /// By default, hadoop divides sentences by new line, so format which given (new line divides separate words and empty string divide sentenes) doesn't fit here.
        /// Let's reformat given train file to known for hadoop.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                return;
            }

            Console.SetIn(new StreamReader(args[0]));
            Console.SetOut(new StreamWriter(args[1]));
            bool formatBack = args[2] == "back";
            string outputWordSeparator = args[3];

            if (outputWordSeparator == @"\s")
                outputWordSeparator = " ";

            string line;
            while ((line = Console.ReadLine()) != null)
            {
                if (!formatBack)
                {
                    if (line == string.Empty)
                    {
                        Console.Write("\n");
                    }
                    else
                    {
                        Console.Write(line);
                        Console.Write(outputWordSeparator);
                    }
                }
                else
                {
                    foreach (var word in line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        Console.WriteLine(word);
                    }

                    Console.WriteLine();
                }
            }

            Console.Out.Close();

        }
    }
}
