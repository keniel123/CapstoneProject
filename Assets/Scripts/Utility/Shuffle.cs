using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Shuffle {
	private static System.Random rng = new System.Random();  

	public static List<T> MixUp<T>(this List<T> list)  
	{  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}
		return list;
	}


}
