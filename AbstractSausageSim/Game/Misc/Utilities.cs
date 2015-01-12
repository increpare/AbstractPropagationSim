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
}

