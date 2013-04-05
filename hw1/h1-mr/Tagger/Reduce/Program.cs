using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Map
{
    class Program
    {
        /*
         * ..\..\App_Data\gene.dev.data ..\..\App_Data\gene.dev.out ..\..\App_Data\gene.trained
         * ..\..\App_Data\gene.test.data ..\..\App_Data\gene.test.out ..\..\App_Data\gene.trained
         * 
         */
        static void Main(string[] args)
        {

            if (args.Length > 1)
            {
                Console.SetIn(new StreamReader(args[0]));
                Console.SetOut(new StreamWriter(args[1]));
                NLP.PosTagger.TagTrigram((string)args[2]);
            }
            else
            {
                NLP.PosTagger.TagTrigram(args[0]);
            }

            /*
            var trainedNGrams = new Dictionary();
            var trainedWordsCount = new Dictionary();

            //read trained n-grams and word counts from file, fill-up dictionary
            foreach (var ln in File.ReadAllLines(args[2]))
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

            var S = trainedNGrams.Keys.ToArray().SelectMany(p => p.Split(' ')).Distinct().Where(p => p != "*" && p != "STOP").ToArray();

            string line;
            while ((line = Console.ReadLine()) != null)
            {
                var words = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();

                var pi_k_1_w_u = new Dictionary<string, double> { { "* *", 1 } };
                var bp = new List<Dictionary<string, string>>();

                for (var k = 0; k < words.Length; k++)
                {
                    string word = words[k];
                    string[] S_k = null;
                    string[] S_k_1 = null;

                    if (k == 0)
                    {
                        S_k = S_k_1 = new[] { "*" };
                    }
                    else if (k == 1)
                    {
                        S_k = S.ToArray();
                        S_k_1 = new[] { "*" };
                    }
                    else
                    {
                        S_k = S_k_1 = S.ToArray();
                    }

                    var pi_k_u_v = new Dictionary<string, double>();

                    bp.Add(new Dictionary<string, string>());
                    
                    foreach (var u in S_k)
                    {
                        foreach (var v in S)
                        {
                            var pi_k_tmp = new Dictionary<string, double>();

                            foreach (var w in S_k_1)
                            {
                                var ngram = w + " " + u + " " + v;
                                var ngram_1 = w + " " + u;

                                var cnt_word_y = trainedWordsCount.GetSafely(word + " " + v);
                                var cnt_y = trainedWordsCount.GetCount(v, 1);

                                if (cnt_word_y == null)
                                {
                                    if (trainedWordsCount.GetCount(word, 0) != 0)
                                    {
                                        //exist in dictionary
                                        cnt_word_y = 0;
                                    }
                                    else
                                    {
                                        //not exist in dictionary => it is rare
                                        cnt_word_y = trainedWordsCount.GetSafely("_RARE_ " + v);
                                    }
                                }

                                var q_v_w_u = (double)(trainedNGrams.GetSafely(ngram) / (double)trainedNGrams.GetSafely(ngram_1));
                                var e_v = (double)(cnt_word_y / (double)cnt_y);

                                var p = pi_k_1_w_u[ngram_1] * q_v_w_u * e_v * 100;

                                pi_k_tmp.Add(w, p);
                            }

                            var max_w = pi_k_tmp.OrderByDescending(p => p.Value).First();

                            pi_k_u_v.Add(u + " " + v, max_w.Value);

                            bp.Last().Add(u + " " + v, max_w.Key);
                        }
                    }

                    pi_k_1_w_u = pi_k_u_v;


                    if (words.Length - 1 == k)
                    {
                        var tags = new string[words.Length];

                        double max_p = 0;
                        //Last word
                        foreach (var u in S_k)
                        {
                            foreach (var v in S)
                            {
                                var ngram = u + " " + v + " STOP";
                                var ngram_1 = u + " " + v;

                                var q_stop = (double)(trainedNGrams.GetSafely(ngram) / (double)trainedNGrams.GetSafely(ngram_1));
                                var p = q_stop * pi_k_u_v[ngram_1];

                                if (max_p < p)
                                {
                                    max_p = p;
                                    tags[k] = v;
                                    tags[k - 1] = u;
                                }
                            }
                        }


                        //backword iter
                        for (int i = (k - 2); i != -1; i--)
                        {
                            var ptr = bp.ElementAt(i + 2);

                            tags[i] = ptr[tags[i + 1] + " " + tags[i + 2]];
                        }

                        for (var n = 0; n < words.Length; n++)
                        {
                            Console.Write("{0} {1}", words[n], tags[n]);
                            Console.Write("\t");
                        }
                    }
                }

                Console.Write("\n");
            }

            Console.Out.Close();
             */
        }
    }
}
