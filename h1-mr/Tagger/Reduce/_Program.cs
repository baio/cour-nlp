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

            var pi_x_y_ini = trainedNGrams.Keys.Where((p) => p.Split(' ').Length == nGramCount - 1).Select(p => {
                    var splits = p.Split(' ');
                    double val = 0;
                    if (splits.All(s => s == "*"))
                        val = 1;
                    return new KeyValuePair<string, double>(p, val);
                });
            
            string line;
            while ((line = Console.ReadLine()) != null)
            {
                var pi_x_y = pi_x_y_ini.ToArray();
                var words = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                var i = 0;

                foreach(var word in words)
                {
                    var wordsCount = allPossibleTags.Select((tag) =>
                    {                        
                        var wd = word + " " + tag;

                        if (!trainedWordsCount.ContainsKey(wd))
                        {
                            return new KeyValuePair<string, int>(wd, 0);
                        }
                        else
                        {
                            return trainedWordsCount.Single(s => s.Key == wd);
                        }

                    }).ToArray();

                    
                    if (wordsCount.Count(p => p.Value != 0) == 0)
                    {
                        //this is a rare word (for this words doesn't exist any tag)
                        wordsCount = trainedWordsCount.GetSafely("_RARE_", 0);
                    }

                    var r_y = wordsCount.SelectMany(p =>
                        {
                            //Get tag O
                            var tag = Dictionary.GetKeyPart(p.Key, 1);

                            var count_y_x = p.Value;
                            var count_y = trainedNGrams.GetSafely(tag);

                            var ngrams = GetAllPossibleNgams(trainedNGrams.Keys.ToArray(), nGramCount, i).Where(s => s.EndsWith(" " + tag));

                            return ngrams.Select((ngram) =>
                            {
                                var ngramSplits = ngram.Split(' ');

                                var ngram_perv = ngramSplits.Length == 1 ? ngramSplits[0] : string.Join(" ", ngramSplits.Take(ngramSplits.Length - 1));

                                var ngram_next = ngramSplits.Length == 1 ? null : string.Join(" ", ngramSplits.Skip(1));

                                var ngram_count = trainedNGrams[ngram];

                                var ngram_perv_count = trainedNGrams.GetSafely(ngram_perv);

                                var pi = pi_x_y.Single(s => s.Key == ngram_perv);

                                return new { ngram = ngram, tag = tag, ngram_perv = ngram_perv, ngram_next = ngram_next, e = (double)count_y_x / (double)count_y, q = ngram_count / (double)ngram_perv_count, pi = pi.Value };
                            });
                        }).OrderBy(p => p.ngram).ToArray();


                    pi_x_y = r_y.GroupBy(p => p.ngram_next).Select(p => {
                        
                        var max_r = p.Max(s => s.pi * s.q * s.e);

                        return new KeyValuePair<string, double>(p.Key, max_r);
                    }).ToArray();

                    i++;
                }

                Console.Write("\n");
            }

            Console.Out.Close();
        }

        private static string[] GetAllPossibleTags(string[] NGramsList)
        {
            return NGramsList.SelectMany(p => p.Split(' ')).Distinct().Where(p => p != "*" && p != "STOP").ToArray();
        }

        private static string [] GetAllPossibleNgams(string [] NGramsList, int nGramCount, int Step = 0)
        {
            return NGramsList.Where((p) =>
                {
                    var splits = p.Split(' ');

                    if (Step != -1)
                    {
                        if (Step < nGramCount)
                        {
                            //with start tags
                            return splits.Length == nGramCount && splits.Take(nGramCount - Step - 1).Where(s => s == "*").Count() == nGramCount - Step - 1;
                        }
                        else
                        {
                            return splits.Length == nGramCount && splits.All(s => s != "*");
                        }
                    }
                    else
                    {
                        return splits.Length == nGramCount;
                    }

                }).ToArray();
        }        
    }
}
