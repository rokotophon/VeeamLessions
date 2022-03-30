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

        #region Operators
        public static ImmutableString operator +(ImmutableString str1, ImmutableString str2)
            => new ImmutableString(str1.immutableString + str2.immutableString);
        public static ImmutableString operator +(string str1, ImmutableString str2)
            => new ImmutableString(str1 + str2.immutableString);
        public static ImmutableString operator +(ImmutableString str1, string str2)
            => new ImmutableString(str1.immutableString + str2);
        public static bool operator ==(ImmutableString str1, ImmutableString str2)
            => str1.Equals(str2);
        public static bool operator !=(ImmutableString str1, ImmutableString str2)
            => !str1.Equals(str2);
        #endregion
        #region Methods
        public ImmutableString Substring(int startIndex, int length)
            => new ImmutableString(immutableString.Substring(startIndex, length));
        public ImmutableString Remove(int startIndex, int count)
            => new ImmutableString(immutableString.Remove(startIndex, count));
        public ImmutableString Insert(int startIndex, string str)
            => new ImmutableString(immutableString.Insert(startIndex, str));
        public ImmutableString Replace(string oldValue, string newValue)
            => new ImmutableString(immutableString.Replace(oldValue, newValue));
        #endregion

        #region Override
        public override string ToString() => immutableString;
        public override bool Equals(object obj) 
            => string.Equals(immutableString, (obj as ImmutableString)?.immutableString);

        public override int GetHashCode()
            => immutableString?.GetHashCode() ?? 0;
        #endregion

        #region Properties
        public char this[int index] => immutableString?[index] ?? throw new NullReferenceException();
        public int Length => immutableString?.Length ?? 0;
        #endregion    
    }
    
}
