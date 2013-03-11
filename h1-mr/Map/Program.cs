﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Map
{
    class Program
    {
        const string START_TAG = "*";
        const string STOP_TAG = "STOP";

        /// <summary>
        /// Map 
        /// 1. Words count
        /// 2. N-Grams
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                return;
            }

            Console.SetIn(new StreamReader(args[0]));
            Console.SetOut(new StreamWriter(args[1]));
            int nGramCount = int.Parse(args[2]);
            
            var counters = new Dictionary<string, int>();
            var tags = new List<string>();

            string line;
            while ((line = Console.ReadLine()) != null)
            {
                var wordTagPairs = line.Split(new [] {'\t'}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var wordTagPair in wordTagPairs)
                {
                    //Words count
                    if (!counters.ContainsKey(wordTagPair))
                        counters.Add(wordTagPair, 1);
                    else
                        counters[wordTagPair]++;

                    //Collect tags
                    tags.Add(wordTagPair.Split(' ')[1]);
                }
                
                foreach (var counter in counters)
                {
                    Console.WriteLine(string.Format("{0} WORDTAG\t{1}", counter.Key, counter.Value));
                }

                //NGRAMS
                var ngrams = new Dictionary<string, int>();
                for (var i = 1; i <= nGramCount; i++)
                {
                    var spt = Enumerable.Repeat(START_TAG, i - 1).Concat(tags);

                    
                    int iterCnt;

                    if (i > 1)
                    {
                        //Add stop tag if ngrams > 1
                        spt = spt.Concat(new[] { STOP_TAG });

                        //iter till STOP word (total number of iterations = number of tags + 1), example * * A B STOP = 3 & * A B STOP = 3
                        iterCnt = tags.Count() + 1; 
                    }
                    else
                    {

                        //iter = number of tags, example: AB = 2 iters
                        iterCnt = tags.Count();
                    }

                    for (var k = 0; k < iterCnt; k++)
                    {
                        //move to the next tag and compose ngram of length i
                        var ngram = string.Join(" ", spt.Skip(k).Take(i));

                        if (!ngrams.ContainsKey(ngram))
                            ngrams.Add(ngram, 1);
                        else
                            ngrams[ngram]++;
                    }           
                }

                foreach (var ngram in ngrams)
                {
                    Console.WriteLine(string.Format("{0} NGRAM\t{1}", ngram.Key, ngram.Value));
                }

                counters.Clear();
                tags.Clear();
                ngrams.Clear();
            }

            Console.Out.Close();
        }
    }
}
