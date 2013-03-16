using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tagger.Reduce
{
    public static class WordsFilter
    {
        /*
         * Numeric The word is rare and contains at least one numeric characters.
         *  All Capitals The word is rare and consists entirely of capitalized letters.
         *  Last Capital The word is rare, not all capitals, and ends with a capital letter.
         *   Rare The word is rare and does not fit in the other classes.
        */
        public static string Filter(string Word)
        {
            return null;
        }
    }
}
