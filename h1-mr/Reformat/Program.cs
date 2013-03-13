using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reformat
{
    class Program
    {
        private const string OUTPUT_WORD_SEPARATOR = " ";
        private const bool formatBack = true;

        /// <summary>
        /// By default, hadoop divides sentences by new line, so format which given (new line divides separate words and empty string divide sentenes) doesn't fit here.
        /// Let's reformat given train file to known for hadoop.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                return;
            }

            Console.SetIn(new StreamReader(args[0]));
            Console.SetOut(new StreamWriter(args[1]));

            string line;
            while ((line = Console.ReadLine()) != null)
            {
                if (!formatBack)
                {
                    if (line == string.Empty)
                    {
                        Console.Write("\n");
                    }
                    else
                    {
                        Console.Write(line);
                        Console.Write(OUTPUT_WORD_SEPARATOR);
                    }
                }
                else
                {
                    foreach (var word in line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        Console.WriteLine(word);
                    }

                    Console.WriteLine();
                }
            }

            Console.Out.Close();

        }
    }
}
