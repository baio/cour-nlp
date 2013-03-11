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
            
            /*
            var outFile = "../../App_Data/gene.r.count";
            NGram ngram = new NGram("../../App_Data/gene.train", 3);
            var res = ngram.Train();
            NGram.RelpaceRare(res);
            NGram.WriteToFile(outFile, res);
             */
             

            var res = NGramTagger.Read("../../App_Data/gene.r.count");
            NGramTagger.Tag("../../App_Data/gene.test", res, "../../App_Data/gene_test.p1.out");

        }
    }
}
