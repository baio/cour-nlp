using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorer
{
    //http://en.wikipedia.org/wiki/Sensitivity_and_specificity
    //http://en.wikipedia.org/wiki/Precision_and_recall
    public class Scorer
    {
        public int TruePositive { get; set; }

        public int TrueNegative { get; set; }

        public int FalsePositive { get; set; }

        public int FalseNegative { get; set; }

        /// <summary>
        /// Probabilty of GEN, given word is O 
        /// </summary>
        public double Recall
        {
            get { return this.TruePositive / (double)(this.TruePositive + this.FalseNegative); }
        }

        /// <summary>
        /// Probabilty of GEN, given word is O 
        /// </summary>
        public double Precision
        {
            get { return this.TruePositive / (double)(this.TruePositive + this.FalsePositive); }
        }

        /// <summary>
        /// Specificity : Probability of O, given word is GENE
        /// </summary>
        public double Specifity
        {
            get { return this.TrueNegative / (double)(this.TrueNegative + this.FalsePositive); }
        }


        public double Accuracy
        {
            get { return this.TruePositive + this.TrueNegative / (double)(this.TruePositive + this.FalsePositive + this.TrueNegative + this.FalseNegative); }
        }

        public double F1 { get { return (2 * this.Precision * this.Recall) / (this.Precision + this.Recall); } }

    }
}
