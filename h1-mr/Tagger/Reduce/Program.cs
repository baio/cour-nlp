using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Map
{
    class Program
    {
        /*
         * gene.maped -> gene.reduced
         * ..\..\App_Data\gene.maped ..\..\App_Data\gene.reduced ..\..\App_Data\gene.trained
         */
        static void Main(string[] args)
        {
            int nGramCount = 3;

            var trainWordsCount = new Dictionary();
            var trainNGrams = new Dictionary();

            if (args.Length > 0)
            {
                Console.SetIn(new StreamReader(args[0]));
                Console.SetOut(new StreamWriter(args[1]));
               
                foreach(var ln in File.ReadAllLines(args[2]))
                {
                    var kvp = ln.Split('\t');   
                    
                    var srcKey = kvp[0];
                    var srcVal = kvp[1];

                    var keySplits = srcKey.Split(' ');
                    var keyTag = keySplits.Last();
                    var key = string.Join(" ", keySplits.Take(keySplits.Count() - 1));

                    if (keyTag == "WORDTAG")
                    {
                        trainWordsCount.AddSafely(key, srcVal);
                    }
                    else
                    {
                        trainNGrams.AddSafely(key, srcVal);
                    }
                }
            }
            

            string line;
            while ((line = Console.ReadLine()) != null)
            {
                foreach(var word in line.Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries))
                {
                    var wordsCount = trainWordsCount.GetSafely(word, 0);

                    if (wordsCount.Count() == 0)
                    {
                        wordsCount = trainWordsCount.GetSafely("_RARE_", 0);
                    }

                    var e_x_y = wordsCount.Select(p =>
                        {
                            var tag = Dictionary.GetKeyPart(p.Key, 1);

                            var count_y_x = p.Value;
                            var count_y = trainNGrams.GetSafely(tag);

                            return new { tag = tag, prob = (double)count_y_x / (double)count_y };
                        });

                    var max_e_x_y = e_x_y.OrderByDescending(p => p.prob).First();

                    Console.Write(string.Format("{0} {1}", word, max_e_x_y.tag));
                    Console.Write("\t");
                }

                Console.Write("\n");
            }

            Console.Out.Close();
        }
    }
}
