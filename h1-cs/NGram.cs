using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h1_cs
{
    public class NGram
    {
        public NGram(string File, int N)
        {
            file = File;
            n = N;
        }

        private readonly string file;
        private readonly int n;

        private Dictionary<NGramTaggedWord, int> _emission = new Dictionary<NGramTaggedWord, int>();
        private Dictionary<string, int> _ngrams = new Dictionary<string, int>();

        private const string NON_GENE_TAG = "O";
        private const string GENE_TAG = "I_GENE";
        private const string START_TAG = "*";
        private const string STOP_TAG = "STOP";

        /// <summary>
        /// Increase value of the dictionary on 1, if exists.
        /// If not create new dictionary entry with initial value count equal 1.
        /// </summary>
        private static void IncDict<K>(Dictionary<K, int> Dict, K Key)
        {
            if (Dict.ContainsKey(Key))
            {
                Dict[Key] = Dict[Key] + 1;
            }
            else
            {
                Dict.Add(Key, 1);
            }        
        }

        private void HandleTags(IEnumerable<NGramTaggedWord> SentenceTags)
        {
            for (var i = 1; i <= n; i++)
            { 
                HandleTagsN(SentenceTags, i);
            }
        }

        private void HandleTagsN(IEnumerable<NGramTaggedWord> SentenceTags, int N)
        {
            var spt = Enumerable.Repeat(START_TAG, N - 1).Concat(SentenceTags.Select(p => p.tag));
            
            if (N > 1)
                spt = spt.Concat(new[] { STOP_TAG });

            for (var i = 0; i < spt.Count() - N + 1; i++)
            {
                var s = spt.Skip(i).Take(N);

                IncDict(_ngrams, string.Join(" ", s));
            }           
        }

        public NGramCount Train(string OutFile = null)
        {
            var tags = new List<NGramTaggedWord>();

            var lines = File.ReadAllLines(this.file);
            
            //guarantee last line is empty line
            lines = lines.Concat(new[] { "" }).ToArray();

            foreach(var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    //construct ngram for particular sentence
                    HandleTags(tags);

                    //new sentence
                    tags.Clear();
                }
                else
                {
                    var taggedWord = new NGramTaggedWord(line);

                    tags.Add(taggedWord);

                    IncDict(_emission, taggedWord);

                }
            }

            var res = new NGramCount
            {
                Emission = _emission.Select((kvp) => new NGramEmission { word = kvp.Key, count = kvp.Value }).ToArray(),
                Tags = _ngrams.Select((kvp) => new NGramTag {  tags = kvp.Key, count = kvp.Value }).ToArray()
            };

            if (!string.IsNullOrEmpty(OutFile))
            { 
                File.WriteAllLines(OutFile, 
                    res.Emission.Select(p => string.Format("{0} WORDTAG {1} {2}", p.count, p.word.tag, p.word.word)).Concat(
                    res.Tags.Select(p => string.Format("{0} {1}-GRAM {2}", p.count, p.tags.Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries).Count(), p.tags))));
            }

            return res;
        }
    }

    public struct NGramTaggedWord
    {
        public NGramTaggedWord(string Line)
        {
            var spt = Line.Split(' ');
            this.word = spt[0].Trim();
            this.tag = spt[1].Trim();
        }

        public NGramTaggedWord(string Word, string Tag)
        {
            this.word = Word.Trim();
            this.tag = Tag.Trim();
        }

        public string word;

        public string tag;
    }


    public struct NGramEmission
    {
        public NGramTaggedWord word;

        public int count;
    }

    public struct NGramTag
    {
        public string tags;

        public int count;
    }

    public class NGramCount     
    {
        public NGramEmission[] Emission { get; set; }

        public NGramTag[] Tags { get; set; }
    }
}
