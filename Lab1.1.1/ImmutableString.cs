using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1._1._1
{
    class ImmutableString
    {
        private readonly string immutableString;
        public static ImmutableString Create(string sourse) => new ImmutableString(sourse);
        private ImmutableString(string source) => immutableString = source;

        public static ImmutableString operator +(ImmutableString str1, ImmutableString str2)
            => new ImmutableString(str1.immutableString + str2.immutableString);
        public static ImmutableString operator +(string str1, ImmutableString str2)
            => new ImmutableString(str1 + str2.immutableString);

        #region Methods
        public ImmutableString Substring(int startIndex, int length)
            => new ImmutableString(immutableString.Substring(startIndex, length));
        public ImmutableString Remove(int startIndex, int count)
            => new ImmutableString(immutableString.Remove(startIndex, count));
        public override string ToString() => immutableString;
        public ImmutableString Replace(string oldValue, string newValue)
            => new ImmutableString(immutableString.Replace(oldValue, newValue));
        #endregion

        #region Properties
        public char this[int index] => immutableString[index];
        public int Length => immutableString.Length;

        #endregion    
    }
    
}
