using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Reduce
{
    class Program
    {
        private const string RARE_WORD = "_RARE_";

        /*
         * ../../App_Data/gene.src.mapped ../../App_Data/gene.trained 5
         */

        private static void Main(string[] args)
        {
            int rareWordsLimit = 5;

            if (args.Length > 0)
            {
                Console.SetIn(new StreamReader(args[0]));
                Console.SetOut(new StreamWriter(args[1]));
                rareWordsLimit = int.Parse(args[2]);
            }             

            var wordsCount = new Dictionary();
            var ngramsCount = new Dictionary();

            string line;
            while ((line = Console.ReadLine()) != null)
            {
                var kvp = line.Split(new [] {'\t'}, StringSplitOptions.RemoveEmptyEntries);

                var val = int.Parse(kvp[1]);

                var keySplits = kvp[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var typeTag = keySplits.Last();
                var key = string.Join(" ", keySplits.Take(keySplits.Count() - 1));

                if (keySplits.Last() == "WORDTAG")
                {
                    wordsCount.AddSafely(key, val);
                }
                else if (keySplits.Last() == "NGRAM")
                {
                    ngramsCount.AddSafely(key, val);
                }
            }
            
            //store pairs for rare words - tag : quanity
            var rareWordsCount = new Dictionary();
            var wordsCountOrdered = wordsCount.OrderByDescending(p => p.Value);
            foreach (var wordCount in wordsCountOrdered)
            {
                if (wordCount.Value >= rareWordsLimit)
                {
                    Console.WriteLine(string.Format("{1} WORDTAG\t{0}", wordCount.Value, wordCount.Key));
                }
                else
                {
                    var tag = wordCount.Key.Split(' ').Last();
                    rareWordsCount.AddSafely(tag, wordCount.Value);
                }
            }

            foreach (var rareWordCount in rareWordsCount)
            {
                Console.WriteLine(string.Format("{1} {2} WORDTAG\t{0}", rareWordCount.Value, RARE_WORD, rareWordCount.Key));
            }

            foreach (var ngramCount in ngramsCount.OrderByDescending(p => p.Value))
            {
                Console.WriteLine(string.Format("{2} {1}-GRAM\t{0}", ngramCount.Value, ngramCount.Key.Split(new[] { ' ' }).Count(), ngramCount.Key));
            }

            Console.Out.Close();
        }
    }
}
