// Source code is modified from Mike Jones's JSON Serialization and Deserialization library (https://www.ghielectronics.com/community/codeshare/entry/357)

using System;
using System.Collections;

namespace nanoFramework.SignalR.Client.json

{
    internal static class StringExtensions
	{
        internal static bool EndsWith(this string s, string value)
        {
            return s.IndexOf(value) == s.Length - value.Length;
        }

        internal static bool StartsWith(this string s, string value)
        {
            return s.IndexOf(value) == 0;
        }

        internal static bool Contains(this string s, string value)
        {
            return s.IndexOf(value) >= 0;
        }
	}
}
