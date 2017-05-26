using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DecisionFunction  {

	private int[] columnHeightsArray;
	private float[] testArray = new float[220];
	private float[,] evalArray = new float[10,22];
	Cell dest;
	Transform[,] test;
	float[,] board = new float[10,22];
	float[] array = new float[220];
//	int[,] shapeO = {{1,1},{1,1}};
//	int[,] shapeJ = {{0,1},{0,1},{1,1}};
//	int[,] shapeI = { { 0, 1, 0, 0 }, { 0, 1, 0, 0 }, { 0, 1, 0, 0 }, { 0, 1, 0, 0 } };
//	int[,] shapeL = {  { 1, 0 }, { 1, 0 }, { 1, 1 } };
//	int[,] shapeT = { { 1,1,1 },{0,1,0} };
//	int[,] shapeS = { { 0, 1, 1 }, { 1, 1, 0 } };
//	int[,] shapeZ = { { 1, 1, 0 }, { 0, 1, 1 } };
//	int[,] shapeO_Rotation = { { 1, 1 }, { 1, 1 } };
	//int[,] shapeJ_Rotation = { { { 0, 1 }, { 0, 1 }, { 1, 1 }  }, { { 1, 0, 0 }, { 1, 1, 1 } }, { { 1, 1 }, { 1, 0 }, { 1, 0 } }, { { 1, 1, 1 }, { 0, 0, 1 } } };
	//int[,] shapeI_Rotation = {{{ 0, 1, 0, 0 }, { 0, 1, 0, 0 }, { 0, 1, 0, 0 }, { 0, 1, 0, 0 }},{ { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 1, 1, 1, 1 }, { 0, 0, 0, 0 } }};
	//int[,] shapeL_Rotation = { {  { 1, 0 }, { 1, 0 }, { 1, 1 } }, { { 1,1,1 }, { 1, 0, 0 } }, { { 1, 1 }, { 0, 1 }, { 0, 1 } }, { { 0, 0, 1 }, { 1, 1, 1 } } };
	//int[,] shapeT_Rotation = { { { 1,1,1 },{0,1,0} }, { { 0,1 }, { 1, 1 },{0,1} }, { { 0, 1, 0 }, { 1, 1, 1 }}, { { 1, 0 }, { 1, 1 },{ 1, 0} } };
	//int[,] shapeS_Rotation = { { { 0,1,1 },{1,1,0} }, { { 1,0 }, { 1, 1 },{0 ,1} }, { { 0, 1, 1 }, { 1, 1, 0 }}, { { 1, 0 }, { 1, 1 },{ 0, 1} } };
	//int[,] shapeZ_Rotation = { { { 1,1,0 },{0,1,1} }, { { 0,1 }, { 1, 1 },{1 ,0} }, { { 1, 1, 0 }, { 0, 1, 1 }}, { { 0, 1 }, { 1, 1 },{ 1, 0} } };

	// Use this for initialization
	void Start () {
		
	}

//	public void setArray(float[] arr)
//	{
//		this.array = arr;
//	}
//
//	public void getArray()
//	{
//		return this.array;
//	}
//
//	public void setBoard(float[,] gm)
//	{
//		this.board = gm;
//	}
//
//	public void getBoard()
//	{
//		return this.board;
//	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void findColumnHeights(float[,] gboard){
		//Debug.Log (gboard.GetLength (1));
		  this.columnHeightsArray = new int[gboard.GetLength(1)]; //Number of columns

		  for (int x  = 0; x < gboard.GetLength(1); x++){

		    for (int y = gboard.GetLength(0) - 1; y >= 0; y--){
				this.columnHeightsArray[x] = 0; //Column x assumed to be empty until otherwise
			      if (gboard[y,x] == 1) {
					this.columnHeightsArray[x] = y + 1; //Debug.Log("Filled!");
			        break;
			      } //Checks if current cell is filled
		  } //Traverses row wise
					
		  } //Traverses column wise
			
		}

	void InputBoardStates(float[] input,Board m_gameBoard)
	{
		for (int i = 0; i < m_gameBoard.m_grid.GetLength (1) - 8; i++) 
		{
			for (int x = 0; x < m_gameBoard.m_grid.GetLength (0) ; x++) 
			{
				int index = i * m_gameBoard.m_grid.GetLength (0) + x;
				if (m_gameBoard.m_grid [x, i] == null) {
					input [index] = 0;


				} else if (m_gameBoard.m_grid [x, i] != null)
				{
					input [index] = 1;


				}

			}

		}	

		this.testArray = input;

	}
		

	int completeLines(float[,] gboard)
	{
		int temp = 0;
		int num_lines = 0;
		for (int i = 0; i < gboard.GetLength (1); i++) {
			for (int x = 0; x < gboard.GetLength (0); x++) {
				if (gboard [x, i] == 1) {
					temp++;
				}
			}
			if (temp == 10) {
				num_lines++;
				temp = 0;
			}

		}
		return num_lines;

	}

	int numberHoles(float[,] gboard)
	{
		int holes = 0;

		for(int y = 0; y < gboard.GetLength(1); y++)
		{
			bool reachedTop = false;

			for (int x = 0; x < gboard.GetLength (0); x++) {

				float field = gboard [x, y];
				if (reachedTop && field != 1f) {
					holes++;
				}
				if (field == 1f) {
					reachedTop = true;
				}
			}


		}
		return holes;

	}

	float[] findColumnHeights_calcSumBumpiness(float[,] gboard){
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


	public ArrayList ExecuteDecision(Shape active,Board gm, float[,] v2d_gboard)
	{
		findColumnHeights (v2d_gboard);
		Array.Sort (this.columnHeightsArray);
		Array.Reverse (this.columnHeightsArray);
		int highest_col = this.columnHeightsArray[0];
		float bestScore = -1000000000000000000000000000000f;
		Vector2 bestposition;
		int bestRotation;
//		float[] topRow = GetRow2D.GetRow (v2d_gboard, hihgest_col);
//		float[] secondTopRow = GetRow2D.GetRow (v2d_gboard, hihgest_col - 1);
//		float[,] z = new float[topRow.Length,secondTopRow.Length];
//		z.CopyTo(topRow,0);
//		z.CopyTo (secondTopRow, topRow.Length);
//		//z = GetRow2D.Make2DArray (z, 2, 10);
		if (active.gameObject.tag == "piece-O") {
			Vector3 oldpos = active.transform.position;
			for (int i = 0; i < 4; i++) {
				for (int a = 0; a < highest_col; a++) {
					for (int x = 0; x < gm.m_width; x++) {
						if (gm.IsValidPosition (active, new Cell (x, a, gm), i) == true) {
							//Debug.Log ("hi");
							active.transform.position = new Vector3 (x, a, 0);
							InputBoardStates (testArray, gm);
							this.evalArray = GetRow2D.Make2DArray (this.testArray, 22, 10);
							float temp = Evaluate (this.evalArray);
							if (temp > bestScore) {
								//Debug.Log (temp);
								bestScore = temp;
								bestposition = new Vector2 (x, a);
								bestRotation = i;
							}
						}
					
					}
				}

			}
			active.transform.position = oldpos;
			ArrayList arr = new ArrayList();
			arr.Add(bestposition);
			arr.Add(bestRotation);
			Vector3 tmp = new Vector3(oldpos.x - 1 ,oldpos.y,oldpos.z);
			arr.Add (tmp);
			return arr;

		}
		else if (active.gameObject.tag == "piece-I") {
			Vector3 oldpos = active.transform.position;
			for (int i = 0; i < 4; i++) {
				for (int a = 0; a < highest_col; a++) {
					for (int x = 0; x < gm.m_width; x++) {
						if (gm.IsValidPosition (active, new Cell (x, a, gm), i)) {
							active.transform.position = new Vector3 (x, a, 0);
							InputBoardStates (testArray, gm);
							this.evalArray = GetRow2D.Make2DArray (this.testArray, 22, 10);
							float temp = Evaluate (this.evalArray);
							if (temp > bestScore) {
								bestScore = temp;
								bestposition = new Vector2 (x, a);
								bestRotation = i;
							}
						}

					}
				}

			}
			active.transform.position = oldpos;
			ArrayList arr = new ArrayList();
			arr.Add(bestposition);
			arr.Add(bestRotation);
			Vector3 tmp = new Vector3(oldpos.x ,oldpos.y - 2,oldpos.z);
			arr.Add (tmp);
			return arr;

		}
		else if (active.gameObject.tag == "piece-Z") {
			Vector3 oldpos = active.transform.position;
			for (int i = 0; i < 4; i++) {
				for (int a = 0; a < highest_col; a++) {
					for (int x = 0; x < gm.m_width; x++) {
						if (gm.IsValidPosition (active, new Cell (x, a, gm), i)) {
							active.transform.position = new Vector3 (x, a, 0);
							InputBoardStates (testArray, gm);
							this.evalArray = GetRow2D.Make2DArray (this.testArray, 22, 10);
							float temp = Evaluate (this.evalArray);
							if (temp > bestScore) {
								bestScore = temp;
								bestposition = new Vector2 (x, a);
								bestRotation = i;
							}
						}

					}
				}

			}
			active.transform.position = oldpos;
			ArrayList arr = new ArrayList();
			arr.Add(bestposition);
			arr.Add(bestRotation);
			Vector3 tmp = new Vector3(oldpos.x - 1 ,oldpos.y - 1,oldpos.z);
			arr.Add (tmp);
			return arr;


		}

		else if (active.gameObject.tag == "piece-T") {
			Vector3 oldpos = active.transform.position;
			for (int i = 0; i < 4; i++) {
				for (int a = 0; a < highest_col; a++) {
					for (int x = 0; x < gm.m_width; x++) {
						if (gm.IsValidPosition (active, new Cell (x, a, gm), i)) {
							active.transform.position = new Vector3 (x, a, 0);
							InputBoardStates (testArray, gm);
							this.evalArray = GetRow2D.Make2DArray (this.testArray, 22, 10);
							float temp = Evaluate (this.evalArray);
							if (temp > bestScore) {
								bestScore = temp;
								bestposition = new Vector2 (x, a);
								bestRotation = i;
							}
						}

					}
				}

			}
			active.transform.position = oldpos;
			ArrayList arr = new ArrayList();
			arr.Add(bestposition);
			arr.Add(bestRotation);
			Vector3 tmp = new Vector3(oldpos.x  ,oldpos.y,oldpos.z);
			arr.Add (tmp);
			return arr;

		}
		else if (active.gameObject.tag == "piece-S") {
			Vector3 oldpos = active.transform.position;
			for (int i = 0; i < 4; i++) {
				for (int a = 0; a < highest_col; a++) {
					for (int x = 0; x < gm.m_width; x++) {
						if (gm.IsValidPosition (active, new Cell (x, a, gm), i)) {
							active.transform.position = new Vector3 (x, a, 0);
							InputBoardStates (testArray, gm);
							this.evalArray = GetRow2D.Make2DArray (this.testArray, 22, 10);
							float temp = Evaluate (this.evalArray);
							if (temp > bestScore) {
								bestScore = temp;
								bestposition = new Vector2 (x, a);
								bestRotation = i;

							}
						}

					}
				}

			}
			active.transform.position = oldpos;
			ArrayList arr = new ArrayList();
			arr.Add(bestposition);
			arr.Add(bestRotation);
			Vector3 tmp = new Vector3(oldpos.x ,oldpos.y,oldpos.z);
			arr.Add (tmp);
			return arr;

		}
		else if (active.gameObject.tag == "piece-L") {
			Vector3 oldpos = active.transform.position;
			for (int i = 0; i < 4; i++) {
				for (int a = 0; a < highest_col; a++) {
					for (int x = 0; x < gm.m_width; x++) {
						if (gm.IsValidPosition (active, new Cell (x, a, gm), i)) {
							active.transform.position = new Vector3 (x, a, 0);
							InputBoardStates (testArray, gm);
							this.evalArray = GetRow2D.Make2DArray (this.testArray, 22, 10);
							float temp = Evaluate (this.evalArray);
							if (temp > bestScore) {
								bestScore = temp;
								bestposition = new Vector2 (x, a);
								bestRotation = i;
							}
						}

					}
				}


			}
			active.transform.position = oldpos;
			ArrayList arr = new ArrayList();
			arr.Add(bestposition);
			arr.Add(bestRotation);
			Vector3 tmp = new Vector3(oldpos.x ,oldpos.y - 1,oldpos.z);
			arr.Add (tmp);
			return arr;

		}

		else if (active.gameObject.tag == "piece-J") {
			Vector3 oldpos = active.transform.position;
			for (int i = 0; i < 4; i++) {
				for (int a = 0; a < highest_col; a++) {
					for (int x = 0; x < gm.m_width; x++) {
						if (gm.IsValidPosition (active, new Cell (x, a, gm), i)) {
							active.transform.position = new Vector3 (x, a, 0);
							InputBoardStates (testArray, gm);
							this.evalArray = GetRow2D.Make2DArray (this.testArray, 22, 10);
							float temp = Evaluate (this.evalArray);
							if (temp > bestScore) {
								bestScore = temp;
								bestposition = new Vector2 (x, a);
								bestRotation = i;
							}
						}

					}
				}

			}
			active.transform.position = oldpos;
			ArrayList arr = new ArrayList();
			arr.Add(bestposition);
			arr.Add(bestRotation);
			Vector3 tmp = new Vector3(oldpos.x  ,oldpos.y - 1,oldpos.z);
			arr.Add (tmp);
			return arr;
		}

		ArrayList ar = new ArrayList();
		Debug.Log ("i am here");
		ar.Add (new Vector2 (0, 0));
		ar.Add (1);
		ar.Add (new Vector3(0,0,0));
		return ar;

	}

	float Evaluate(float[,] board)
	{
		float[] sum_bumpiness = findColumnHeights_calcSumBumpiness(board);
		int number_completelines = completeLines (board);
		int number_of_holes = numberHoles (board);
		float columnHeights_sum = sum_bumpiness[0];//Sum of Column Heights (-ve)
		float columnHeights_bumpiness = sum_bumpiness[1]; //Variations in Column Heights (-ve)
		float columnSum_weight = 5000f * columnHeights_sum; //500f 3000f
		float columnHeight_bump = -50000f * columnHeights_bumpiness;
		float num_complines = 400000f * number_completelines;//400000
		float num_holes =  -1000f * number_of_holes;
		//Debug.Log ("column sum >>>>>> " + columnHeights_sum);
		float result = num_complines + columnSum_weight  + num_holes  + columnHeight_bump;
		return result;

	}

		

	
}
