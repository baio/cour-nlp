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
         * ..\..\App_Data\gene.dev.data ..\..\App_Data\gene.dev.unigram.out ..\..\App_Data\gene.trained 1
         * ..\..\App_Data\gene.dev.data ..\..\App_Data\gene.dev.trigram.out ..\..\App_Data\gene.trained 3
         * 
         */
        static void Main(string[] args)
        {
            int nGramCount = 3;

            var trainedWordsCount = new Dictionary();
            var trainedNGrams = new Dictionary();

            if (args.Length > 1)
            {
                Console.SetIn(new StreamReader(args[0]));
                Console.SetOut(new StreamWriter(args[1]));
            }

            nGramCount = int.Parse(args[3]);

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
                    trainedWordsCount.AddSafely(key, srcVal);
                }
                else
                {
                    trainedNGrams.AddSafely(key, srcVal);
                }
            }

            var allPossibleTags = GetAllPossibleTags(trainedNGrams.Keys.ToArray()); 

            
            string line;
            while ((line = Console.ReadLine()) != null)
            {
                foreach(var word in line.Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries))
                {
                    /*
                    var wordsCount = allPossibleTags.Select((tag) =>
                    {                        
                        var wd = word + " " + tag;

                        if (!trainedWordsCount.ContainsKey(wd))
                        { 
                            wd = "_RARE_" + " " + tag;
                        }
                        
                        return trainedWordsCount.Single(s => s.Key == wd);                        

                    }).ToArray();
                     */
                    
                    //Get [BACKGROUND O, 28] 
                    var wordsCount = trainedWordsCount.GetSafely(word, 0);

                    //If word doesn't in list of trained words, consider it is rare
                    if (wordsCount.Count() == 0)
                    {
                        wordsCount = trainedWordsCount.GetSafely("_RARE_", 0);
                    }

                    var e_x_y = wordsCount.SelectMany(p =>
                        {
                            //Get tag O
                            var tag = Dictionary.GetKeyPart(p.Key, 1);

                            var count_y_x = p.Value;
                            var count_y = trainedNGrams.GetSafely(tag);

                            //Get list of all possible ngrams, ended with this tag
                            //[* * O, * O O, I-GENE O O, ..., O, O, O]
                            var ngrams = GetAllPossibleNgams(trainedNGrams.Keys.ToArray(), nGramCount, tag);

                            return ngrams.Select((ngram) =>
                            {
                                var ngramSplits = ngram.Split(' ');

                                var ngram_perv = ngramSplits.Length == 1 ? ngramSplits[0] : string.Join(" ", ngramSplits.Take(ngramSplits.Length - 1));

                                var ngram_count = trainedNGrams[ngram];

                                var ngram_perv_count = trainedNGrams.GetSafely(ngram_perv);

                                return new { ngram = ngram, tag = tag, e = (double)count_y_x / (double)count_y, q = ngram_count / (double)ngram_perv_count };
                            });
                        }).ToArray();

                    
                    var max_e_x_y = e_x_y.OrderByDescending(p => p.e * p.q).First();

                    Console.Write(string.Format("{0} {1}", word, max_e_x_y.tag));
                    Console.Write("\t");
                }

                Console.Write("\n");
            }

            Console.Out.Close();
        }

        private static string[] GetAllPossibleTags(string[] NGramsList)
        {
            return NGramsList.SelectMany(p => p.Split(' ')).Distinct().Where(p => p != "*" && p != "STOP").ToArray();
        }

        private static string [] GetAllPossibleNgams(string [] NGramsList, int nGramCount, string LastTag)
        {
            return NGramsList.Where((p) =>
                {
                    var splits = p.Split(' ');

                    return splits.Last() == LastTag && splits.Length == nGramCount;
                }).ToArray();
        }
    }
}
