using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h1_cs
{
    public class NGramTagger
    {
        public static NGramCount Read(string File)
        {
            var lines = System.IO.File.ReadAllLines(File);

            return new NGramCount
            {
                Emission = lines.Where(p => p.Contains("WORDTAG")).Select(p =>
                {
                    var spt = p.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    return new NGramEmission { count = int.Parse(spt[0]), word = new NGramTaggedWord(spt[3], spt[2]) };
                }).ToArray(),
                Tags = lines.Where(p => p.Contains("GRAM")).Select(p =>
                {
                    var spt = p.Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                    return new NGramTag { count = int.Parse(spt[0]), tags = spt[2] };
                }).ToArray()
            };
        }

        public static IEnumerable<KeyValuePair<string, string>> Tag(string File, NGramCount NGram, string OutFile = null)
        {
            var lines = System.IO.File.ReadAllLines(File);

            lines = lines.Concat(new[] { "" }).ToArray();

            var res = new List<KeyValuePair<string, string>>();

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    res.Add(default(KeyValuePair<string, string>));
                }
                else
                {
                    var q = NGram.Emission.Where(p => p.word.word == line.Trim());

                    if (q.Count() == 0)
                        q = NGram.Emission.Where(p => p.word.word == "_RARE_");

                    var r = q.OrderByDescending(p => (double)p.count / (double)NGram.Tags.Single(s => s.tags == p.word.tag).count);

                    res.Add(new KeyValuePair<string, string>(line, r.First().word.tag));

                }
            }

            if (!string.IsNullOrEmpty(OutFile))
            {
                System.IO.File.WriteAllLines(OutFile, res.Select(p => p.Key + " " + p.Value));                
            }

            return res;
        }
    }
}
