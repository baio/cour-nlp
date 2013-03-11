using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reduce
{
    class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                return;
            }
             
            Console.SetIn(new StreamReader(args[0]));
            Console.SetOut(new StreamWriter(args[1]));

            var wordsCount = new Dictionary<string, int>();
            var ngramsCount = new Dictionary<string, int>();

            string line;
            while ((line = Console.ReadLine()) != null)
            {
                var kvp = line.Split(new [] {'\t'}, StringSplitOptions.RemoveEmptyEntries);

                var val = int.Parse(kvp[1]);

                var keySplits = kvp[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var typeTag = keySplits.Last();
                var key = string.Join(" ", keySplits.Except(new[] { typeTag }));

                if (keySplits.Last() == "WORDTAG")
                {
                    //this is word count
                    if (!wordsCount.ContainsKey(key))
                        wordsCount.Add(key, val);
                    else
                        wordsCount[key] += val ;
                }
                else if (keySplits.Last() == "NGRAM")
                {                    
                    //this is NGRAM
                    if (!ngramsCount.ContainsKey(key))
                        ngramsCount.Add(key, val);
                    else
                        ngramsCount[key] += val;
                }
            }

            foreach (var wordCount in wordsCount.OrderByDescending(p => p.Value))
            {
                Console.WriteLine(string.Format("{0} WORDTAG {1}", wordCount.Value, wordCount.Key));
            }
            foreach (var ngramCount in ngramsCount.OrderByDescending(p => p.Value))
            {
                Console.WriteLine(string.Format("{0} {1}-GRAM {2}", ngramCount.Value, ngramCount.Key.Split(new [] { ' ' }).Count(), ngramCount.Key));
            }

            Console.Out.Close();
        }
    }
}
