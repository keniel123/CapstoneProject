using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class GetRow2D {

	public static T[] GetRow<T>(this T[,] input2DArray, int row) where T : IComparable
    {
        var width = input2DArray.GetLength(0);
        var height = input2DArray.GetLength(1);

        if (row >= height)
            throw new IndexOutOfRangeException("Row Index Out of Range");
        // Ensures the row requested is within the range of the 2-d array


        var returnRow = new T[width];
        for(var i = 0; i < width; i++)
            returnRow[i] = input2DArray[i, row];

        return returnRow;
    }

	public static T[,] Make2DArray<T>(T[] input, int height, int width)
	{
		T[,] output = new T[height, width];
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				output[i, j] = input[i * width + j];
			}
		}
		return output;
	}
}
