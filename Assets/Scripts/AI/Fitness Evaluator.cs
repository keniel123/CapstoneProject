//Version 1.2 - Average Fitness (Explicit Sharing) & Speciation Preserved :)
//Training --> Start >> 12:00 AM

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitnessEvaluator {
	private int[] columnHeightsArray;

	public float[] findColumnHeights_calcSumBumpiness(float[,] gboard){
		int sum = 0; //summation of column heights

		int previous = 0; //Previous column height
		int total = 0; //Total summation of differences in column heights
		int calc = 0; //Difference in previous and current column heights

		float[] sb = new float[2];

		this.columnHeightsArray = new int[gboard.GetLength(1)]; //Number of Columns

		for(int x  = 0; x < gboard.GetLength(1); x++){
			for (int y = gboard.GetLength(0) - 1; y >= 0; y--){
				this.columnHeightsArray[x] = 0; //No filled cell(s) within column x

				if(gboard[y,x] == 1) {
					this.columnHeightsArray[x] = y + 1; //B0-T21 >>> 1 - 22
					break;
				} //First occurence of a filled cell in a given column 
			} //Traverse by Rows

			sum += columnHeightsArray[x];
			calc = previous - columnHeightsArray[x];

			if(calc < 0){
				calc = calc * -1;
			} //Modulus of the any negative values calculated

			total += calc; //Summation of only positive values
			previous = columnHeightsArray[x]; //Current becomes previous column
		} //Traverse by Columns

		sb[0] = (float)(sum);
		sb[1] = (float)(total - columnHeightsArray[0]);

		return sb;
	}

	public float[] findPlayabilityLevels_calcUpperLower(float[,] gboard){
		float[] playabilityLevelsArray = new float[2]; //Array >>> 0 - Upper Average, 1 - Lower Average
		float[] cntUL_sumUL = new float[4]; //Array >>> 0 - Upper Count, 1 - Lower Count, 2 - Upper Sum, 3 - Lower Sum

		for (int k = 0; k < 4; k++){
			//position[k] = 0;
			cntUL_sumUL[k] = 0.0f;
		} //Initialization for Summation

		int sum_position = 0; //Summation of the # of surrounding empty spaces
		int ltrb_start = 0; //Conditinal value for the loop
		int ltrb_stop = 0; //Sentinel value for the for loop

		for (int x = 0; x < gboard.GetLength(1); x++){
			for (int y = 0; y < gboard.GetLength(0); y++){
				if (gboard[y,x] == 0){
					sum_position = 0; //New Empty Cell >>> Re-set 

					for (int index = 0; index < 4; index++){
						ltrb_start = -1; //Re-set & Flag - Out of Gameboard Range
						ltrb_stop = -1; //Re-set & Flag - Out of Gameboard Range

						if (index == 0){
							if(x != 0){
								ltrb_start = x - 1;
								ltrb_stop = 0;
							} //Except column 0
						} //Left
						else if (index == 1){
							if(y != (gboard.GetLength(0) - 1)){ //Valid --> 0 - 22
								ltrb_start = y + 1;
								ltrb_stop = gboard.GetLength(0) - 1; //Row --> (22) [y0]
							} //Except row 21
						} //Top
						else if (index == 2){
							if(x != (gboard.GetLength(1) - 1)){ //Valid --> 0 - 9
								ltrb_start = x + 1;
								ltrb_stop = gboard.GetLength(1) - 1; //Column --> (10) [x1]
							} //Except column 9
						} //Right
						else if (index == 3){
							if(y != 0){
								ltrb_start = y - 1;
								ltrb_stop = 0;
							} //Except row 0
						} //Bottom

						sum_position += findEmptySpacesRelativeToCurrent(gboard, ltrb_start, ltrb_stop, index, y, x);
					} //0 - Left, 1 - Top, 2 - Right, 3 - Bottom

					if (y < this.columnHeightsArray[x]-1){
						cntUL_sumUL[1] += 1.0f; //Lower Counter
						cntUL_sumUL[3] += sum_position / 4.0f; //Lower Summation
					} //Lower
					else if (y > this.columnHeightsArray[x]-1){
						cntUL_sumUL[0] += 1.0f; //Upper Counter
						cntUL_sumUL[2] += sum_position / 4.0f; //Upper Summation
					} //Upper
					else {
						//Debug.Log("ERROR >>> Empty and filled space can't be in the same location!");
					}
				} //Checks if current cell is empty
			} //Traverses row wise
		} //Traverses column wise

		playabilityLevelsArray[0] = cntUL_sumUL[2] / cntUL_sumUL[0]; //Upper Average
		playabilityLevelsArray[1] = cntUL_sumUL[3] / cntUL_sumUL[1]; //Lower Average

		return playabilityLevelsArray;
	}

	public int findEmptySpacesRelativeToCurrent(float[,] gb, int a, int stop, int step, int rY, int cX){
		int count = 0; //Default assigned if no empty spaces are found/possible to the left, top, right or bottom of the current

		if (a != -1 && stop != -1){
			if (step == 0 || step == 3){
				for (int start = a; start >= stop; start--){
					if (step == 0){
						if (gb[rY,start] == 1){ //Y - Constant; X - Changes
							break;
						} //Checks if the next cell is filled
					} //Left
					else if (step == 3){
						if (gb[start,cX] == 1){ //Y - Changes; X - Constant
							break;
						} //Checks if the next cell is filled
					} //Bottom

					count += 1;
				} //Decrementer
			} //Left(0) or Bottom(3)
			else if (step == 2 || step == 1){
				for (int start = a; start <= stop; start++){
					if (step == 2){
						if (gb[rY,start] == 1){ //Y - Constant; X - Changes
							break;
						} //Checks if the next cell is filled
					} //Right
					else if (step == 1){
						if (gb[start,cX] == 1){ //Y - Changes; X - Constant
							break;
						} //Checks if the next cell is filled
					} //Top

					count += 1;
				} //Incrementer
			} //Right(2) or Top(1)
		} //Valid - No Flag
		else {
			//Debug.Log("Attempt at Going Out of Bound Failed MF!");
		}

		return count;
	}

	public float Evaluate(float[,] board, int s , int c, float t){ 
		float score = (float)(s); //End of Game Score (+ve)
		float numberOfClears = (float)(c); //Number of Rows Cleared (+ve)
		float time = t; //Length of Game Played >>> t/60.0f (+ve)

		float[] sum_bumpiness = findColumnHeights_calcSumBumpiness(board);
		float columnHeights_sum = sum_bumpiness[0];//Sum of Column Heights (-ve)
		float columnHeights_bumpiness = sum_bumpiness[1]; //Variations in Column Heights (-ve)

		float[] upper_lower = findPlayabilityLevels_calcUpperLower(board);
		float upper_playabilityLevel = upper_lower[0]; //Playability ABOVE Column Heights (-ve)
		float lower_playabilityLevel = upper_lower[1]; //Playability BELOW Column Heights (-ve)

		//Distributes Pieces --> result = (float)(31.7*numberOfClears + 10.9*score + 17.4*time - 13.7*columnHeights_sum - 9.3*columnHeights_bumpiness - 10.5*upper_playabilityLevel - 6.5*lower_playabilityLevel); 
		//Stacks in Middle :( --> result = (float)(0.760666*numberOfClears + 1.0*score + 1.0*time - 0.510066*columnHeights_sum - 0.184483*columnHeights_bumpiness - 0.118876*upper_playabilityLevel - 0.237753*lower_playabilityLevel); 
		//Distribute then Stacks :/ --> result = (float)(0.760666*numberOfClears + 1.0*score + 1.0*time - 0.510066*columnHeights_sum - 0.184483*columnHeights_bumpiness - 0.237753*upper_playabilityLevel - 0.118876*lower_playabilityLevel); 
		
		float result = 0.0f;
		result = (float)(0.760666*numberOfClears + 1.0*score + 1.0*time - 0.510066*columnHeights_sum - 0.184483*columnHeights_bumpiness - 0.178315*upper_playabilityLevel - 0.178315*lower_playabilityLevel); 
		return result;
	}
}