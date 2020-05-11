using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomPackage.UtililtyExtensions
{
	public static class StringExtensions
	{
		public static string ToKebab(this string src)
		{
			string dst = src;
			int i = 1;
			while (i < dst.Length)
			{
				char c = dst[i];
				if (dst[i - 1] != '-' && 'A' <= c && c <= 'Z')
				{
					dst = dst.Insert(i, "-");
					i++;
				}
				i++;
			}
			return dst.ToLower();
		}
	}
}