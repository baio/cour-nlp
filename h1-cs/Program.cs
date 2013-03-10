using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h1_cs
{
    class Program
    {
        static void Main(string[] args)
        {
            NGram ngram = new NGram("../../App_Data/gene.train", 3);
            var res = ngram.Train();

            foreach (var r in res.Tags)
            {
                Console.WriteLine(string.Format("{0} : {1}", r.tags, r.count));
            }

            Console.ReadLine();

        }
    }
}
