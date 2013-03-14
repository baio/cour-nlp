using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorer
{
    class Program
    {
        //private const string TEST_POSTITIVE = "GENE";

        /*
        * score
        * ..\..\App_Data\gene.dev.out ..\..\App_Data\gene.key.data I-GENE
        */

        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                return;
            }

            Console.SetIn(new StreamReader(args[0]));            
            string testPositive = args[2];

            Scorer scorer = new Scorer();
            using (var goldenStandardFile = new StreamReader(args[1]))
            {
                string line;
                while ((line = Console.ReadLine()) != null)
                {
                    var stdLine = goldenStandardFile.ReadLine();

                    foreach (var word in line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries)
                        .Zip(stdLine.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries), (first, second) => new { test = first.Split(' ')[1], std = second.Split(' ')[1]}))
                    {
                        if (word.std == testPositive)
                        {
                            //positive
                            if (word.test == testPositive)
                                scorer.TruePositive += 1;
                            else
                                scorer.FalseNegative += 1;
                        }
                        else
                        { 
                            //negative
                            if (word.test == testPositive)
                                scorer.FalsePositive += 1;
                            else
                                scorer.TrueNegative += 1;
                        }                       
                    }
                }
            }

            Console.WriteLine("Found {0} {1}s. Expected {2} {1}s; Correct: {3}. Total: {4}",
                scorer.TruePositive + scorer.FalsePositive, 
                testPositive, 
                scorer.TruePositive + scorer.FalseNegative, 
                scorer.TruePositive,
                scorer.TruePositive + scorer.FalsePositive + scorer.TrueNegative + scorer.FalseNegative);
            Console.WriteLine();
            Console.WriteLine("precision: {0}", scorer.Precision);
            Console.WriteLine("recall: {0}", scorer.Recall);
            Console.WriteLine("specifity: {0}", scorer.Specifity);
            Console.WriteLine("F1-Score: {0}", scorer.F1);

            Console.ReadKey();
        }
    }
}
