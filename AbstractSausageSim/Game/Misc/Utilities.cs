using System;
using System.Collections.Generic;
using System.Linq;

public static class Utilities
{
	public static bool AddUnique<T>(this List<T> list, T elem)
	{
		if (list.IndexOf(elem)==-1)
		{
			list.Add(elem);
			return true;
		}
		return false;
	}

	public static string ToSubScript(string s){
		var result="";
		foreach(var c in s){
			switch (c) {
			case '0':
				result += '₀';
				break;
			case '1':
				result += '₁';
				break;
			case '2':
				result += '₂';
				break;
			case '3':
				result += '₃';
				break;
			case '4':
				result += '₄';
				break;
			case '5':
				result += '₅';
				break;
			case '6':
				result += '₆';
				break;
			case '7':
				result += '₇';
				break;
			case '8':
				result += '₈';
				break;
			case '9':
				result += '₉';
				break;
			case '-':
				result += '₋';
				break;
			default:
				result += ' ';
				break;
			}
		}
		return result;
	}
}

