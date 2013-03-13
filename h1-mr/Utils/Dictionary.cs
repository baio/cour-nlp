using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class Dictionary : Dictionary<string, int>
    {
        public static string GetKeyPart(string Key, int KeyPart)
        {
            return Key.Split(' ')[KeyPart];
        }

        public void AddSafely(string Key, string Val)
        {
            AddSafely(Key, int.Parse(Val));
        }

        public int? GetSafely(string Key)
        {
            if (this.ContainsKey(Key))
            {
                return this[Key];
            }
            else
            {
                return null;
            }
        }

        public KeyValuePair<string, int> [] GetSafely(string Key, int KeyPart)
        {
            return this.Where(p => Dictionary.GetKeyPart(p.Key, KeyPart) == Key).ToArray();
        }

        public void AddSafely(string Key, int Val)
        {
            if (this.ContainsKey(Key))
            {
                this[Key] += Val;
            }
            else
            {
                this.Add(Key, Val);
            }
        }
    }
}
