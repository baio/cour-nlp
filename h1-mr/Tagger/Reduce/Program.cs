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

            //initialize start sentence n-grams
            var allPossibleNgrams = allPossibleTags.SelectMany((tag) =>
            {
                return GetAllPossibleNgams(trainedNGrams.Keys.ToArray(), nGramCount, tag);
            });

            var pi_x_y_ini = allPossibleNgrams.Select(p => 
                {

                    //all start sentence probabilities must be null, except 'real' start sentences n-grams
                    var cnt = 0;

                    var spts = p.Split(' ');

                    //confirm first n-1 tags in n-gram is a start sentence tag
                    if (spts.Take(spts.Length - 1).All(s => s == "*"))
                    {
                        cnt = 1;
                    }

                    return new KeyValuePair<string, double>(p, cnt);
                });
            
            string line;
            while ((line = Console.ReadLine()) != null)
            {
                var pi_x_y = pi_x_y_ini.OrderBy(p => p.Key).ToArray();

                foreach(var word in line.Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries))
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

                    var p_x_y = wordsCount.SelectMany(p =>
                        {
                            //Get tag O
                            var tag = Dictionary.GetKeyPart(p.Key, 1);

                            var count_y_x = p.Value;
                            var count_y = trainedNGrams.GetSafely(tag);

                            var ngrams = GetAllPossibleNgams(trainedNGrams.Keys.ToArray(), nGramCount, tag);

                            return ngrams.Select((ngram) =>
                            {
                                var ngramSplits = ngram.Split(' ');

                                var ngram_perv = ngramSplits.Length == 1 ? ngramSplits[0] : string.Join(" ", ngramSplits.Take(ngramSplits.Length - 1));

                                var ngram_nex = ngramSplits.Length == 1 ? null : string.Join(" ", ngramSplits.Skip(1));

                                var ngram_count = trainedNGrams[ngram];

                                var ngram_perv_count = trainedNGrams.GetSafely(ngram_perv);

                                return new { ngram = ngram, tag = tag, ngram_perv = ngram_perv, ngram_next = ngram_nex, e = (double)count_y_x / (double)count_y, q = ngram_count / (double)ngram_perv_count };
                            });
                        }).OrderBy(p => p.ngram).ToArray();

                    /*
                    var r_x_y = pi_x_y.Zip(p_x_y, (first, second) => new { pi = first, r = second });

                    pi_x_y = r_x_y.Select(p => new KeyValuePair<string, double>(p.pi.Key, p.pi.Value * p.r.e * p.r.q)).ToArray();
                    */
                    foreach (var p in p_x_y.GroupBy(p => p.ngram_perv))
                    {
                        var pi = pi_x_y.Single(s => s.Key == p.Key);

                        var max = p.OrderBy(s => pi.Value * s.q * s.e).First();
                    }

                    //shift n-gram
                    
                    //var max_e_x_y = p_x_y.OrderByDescending(p => p.e * p.q).First();

                    //Console.Write(string.Format("{0} {1}", word, max_e_x_y.tag));
                    //Console.Write("\t");
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
