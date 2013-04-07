using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLP
{
    public class WordsCounter
    {
        public static string START_TAG = "*";
        public static string STOP_TAG = "STOP";
        public static string RARE_WORD = "_RARE_";

        public static string WordOutputFormatString  = "{0} WORDTAG\t{1}"; 
        public static string NGramOutputFormatString = "{0} NGRAM\t{1}";

        public void Map(int nGramCount = 3)
        {
            var counters = new Dictionary();
            var ngrams = new Dictionary();
            var tags = new List<string>();

            string line;
            while ((line = Console.ReadLine()) != null)
            {
                var wordTagPairs = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var wordTagPair in wordTagPairs)
                {
                    //Words count
                    counters.AddSafely(wordTagPair, 1);

                    //Collect tags
                    tags.Add(wordTagPair.Split(' ')[1]);
                }

                foreach (var counter in counters)
                {
                    Console.WriteLine(string.Format(WordOutputFormatString, counter.Key, counter.Value));
                }

                //NGRAMS                                
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

                        ngrams.AddSafely(ngram, 1);
                    }

                    //Add start sentence tags, will be needed for calculating probabilities in HMM
                    if (i > 1)
                    {
                        var startSentenceTag = string.Join(" ", Enumerable.Repeat(START_TAG, i - 1));

                        ngrams.AddSafely(startSentenceTag, 1);
                    }
                }


                foreach (var ngram in ngrams)
                {
                    Console.WriteLine(string.Format(NGramOutputFormatString, ngram.Key, ngram.Value));
                }

                counters.Clear();
                tags.Clear();
                ngrams.Clear();
            }

            Console.Out.Close();        
        }

        public void Reduce(int rareWordsLimit = 3)
        {
            var wordsCount = new Dictionary();
            var ngramsCount = new Dictionary();

            string line;
            while ((line = Console.ReadLine()) != null)
            {
                var kvp = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

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
            var wordsCountOrdered = wordsCount.OrderBy(p => p.Key).OrderByDescending(p => p.Value);
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

            foreach (var ngramCount in ngramsCount.OrderBy(p => p.Key).OrderByDescending(p => p.Value))
            {
                Console.WriteLine(string.Format("{2} {1}-GRAM\t{0}", ngramCount.Value, ngramCount.Key.Split(new[] { ' ' }).Count(), ngramCount.Key));
            }

            Console.Out.Close();
        }
    }
}
