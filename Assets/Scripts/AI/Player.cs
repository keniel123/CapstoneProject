using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AssemblyCSharp {
	public class Player :MonoBehaviour {


		//get inputs from game manager

		//send move to game manager 

		//abstract away brain components from game controller 

		//try to complete speciation tonight 

		//player neural network
		public AgentNeuralNetwork brain;

		private string player_name;

		private float p_fitness;

		private float p_time;

		private int numb_clears;

		private int p_score;

		float[,] board = new float[10,22];

		float[] inputArray = new float[Constants.INPUTS];

		public enum Moves
		{
			MoveLeft,
			MoveRight,
			Rotate
		}
			
		void Start () {
			
	
			}

		public int getClears()
		{
			return this.numb_clears;
		}

		public int get_score()
		{
			return this.p_score;
		}

		public void setScore(int score)
		{
			this.p_score = score;

		}
		public void setClears(int clears)
		{
			this.numb_clears = clears;

		}

		public void setTime(float time)
		{
			this.p_time = time;

		}

		public float getTime()
		{
			return this.p_time;

		}
	
		public void updateBrain()
		{
			brain.changeWeights();		
			brain.InputSignalArray = inputArray;
			brain.Activate();

		}


		public void setInputArray(float[] boardArr)
		{
			inputArray = boardArr;

		}

		public float[] getInputArray()
		{
			return inputArray;

		}

		public string getPlayerName()
		{

			return this.player_name;
		}

		public void setPlayerName(string name)
		{
			this.player_name = name;

		}

		public float getPlayerFitness()
		{
			return p_fitness;
		}

		public void setPlayerFitness(float fitness)
		{
			p_fitness = fitness;
		}


		public Moves getMove()
		{
			float maxValue = 0f;
			Moves moves = new Moves();
			for(int i=0; i<brain.OutputSignalArray.Length; i++)
			{

				if (Mathf.Abs(brain.OutputSignalArray[i]) > Mathf.Abs(maxValue))
				{

					maxValue = brain.OutputSignalArray[i];


					if(maxValue <1)
					{
						moves = (Moves)(i*2);

					}
					else
					{
						moves = (Moves)(i*2 + 1);

					}
				}
			}
			//Debug.Log (moves);
			return moves;

		}

		public void setBoard(float[,] f_board)
		{
			this.board = f_board;

		}

		public float[,] getBoard()
		{
			return this.board;

		}


	}

}
