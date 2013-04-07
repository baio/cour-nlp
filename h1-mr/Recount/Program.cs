using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;

namespace Recount
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetIn(new StreamReader("../../App_Data/gene.counts"));
            Console.SetOut(new StreamWriter("../../App_Data/gene.counts.out"));

            /*
            string line;
            while ((line = Console.ReadLine()) != null)
            {
                if (line != "")
                {

                    var spts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (spts[1] == "WORDTAG")
                    {
                        //"1 WORDTAG O mind"
                        //_RARE_ O WORDTAG	29683

                        if (int.Parse(spts[0]) < 5)
                        {
                            Console.WriteLine("{0} WORDTAG {1} _RARE_", spts[0], spts[2]);
                        }
                        else
                        {
                            Console.WriteLine(line);
                        }
                    }
                }
                else
                {
                    Console.WriteLine(line);
                }

            }
             */

            List<WordCount> counts = new List<WordCount>();
            Dictionary ngrams = new Dictionary();
            
            string line;
            while ((line = Console.ReadLine()) != null)
            {
                if (line != "")
                {
                    var spts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (spts[1] == "WORDTAG")
                    {
                        //"1 WORDTAG O mind"
                        //_RARE_ O WORDTAG	29683

                        var wc = counts.FirstOrDefault(p => p.word == spts[3]);

                        if (wc == null)
                        {
                            wc = new WordCount { word = spts[3], cntG = 0, cntO = 0 };

                            counts.Add(wc);
                        }

                        if (spts[2] == "O")
                        {
                            wc.cntO += int.Parse(spts[0]);
                        }
                        else
                        {
                            wc.cntG += int.Parse(spts[0]);
                        }

                        //counts.AddSafely(string.Format("{1} {0} WORDTAG", spts[2], spts[3]), spts[0]);
                    }
                    else
                    {
                        //749 3-GRAM * * I-GENE
                        //* * O 3-GRAM	13047

                        ngrams.AddSafely(string.Format("{0} {1}", string.Join(" ", spts.Skip(2)), spts[1]), spts[0]);
                    }
                }
            }

            var words = counts.Where(p => p.sum >= 5);
            var rareWords = counts.Where(p => p.sum < 5);

            foreach (var word in words.OrderBy(p => p.word).OrderByDescending(p => p.sum))
            {
                if (word.cntO != 0)
                {
                    Console.WriteLine(string.Format("{0} O WORDTAG\t{1}", word.word, word.cntO));
                }

                if (word.cntG != 0)
                {
                    Console.WriteLine(string.Format("{0} I-GENE WORDTAG\t{1}", word.word, word.cntG));
                }
            }

            var rares = rareWords.GroupBy(p => { 
                if (Regex.IsMatch(p.word, "\\d+"))
                {
                    return "NUMERIC";
                }
                else if (p.word.ToUpper() == p.word)
                {
                    return "ALL_CAPITAL";
                }
                else if (p.word.Last().ToString().ToUpper() == p.word.Last().ToString())
                {
                    return "LAST_CAPITAL";
                }
                else
                {
                    return "RARE";
                }
            });

            foreach (var rare in rares)
            {
                Console.WriteLine(string.Format("_RARE_{0}_ O WORDTAG\t{1}", rare.Key, rare.Sum(p => p.cntO)));
                Console.WriteLine(string.Format("_RARE_{0}_ I-GENE WORDTAG\t{1}", rare.Key, rare.Sum(p => p.cntG)));            
            }

            /*
            Console.WriteLine(string.Format("_RARE_ O WORDTAG\t{0}", rareWords.Sum(p => p.cntO)));
            Console.WriteLine(string.Format("_RARE_ I-GENE WORDTAG\t{0}", rareWords.Sum(p => p.cntG)));
             */

            foreach (var kvp in ngrams.OrderBy(p => p.Key).OrderByDescending(p => p.Value))
            {
                Console.WriteLine(string.Format("{0}\t{1}", kvp.Key, kvp.Value));
            }


            Console.Out.Close();
        }

        class WordCount
        {
            public string word;

            public int cntO;

            public int cntG;

            public int sum { get { return cntO + cntG; } }
        }
    }
}
