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

		private float p_time;

		private string player_name;

		private float p_fitness;

		private int numb_clears;

		private int p_score;

		float[,] board = new float[10,22];

		List<List<Moves>> moveSequence = new List<List<Moves>>();

		float[] inputArray = new float[Constants.INPUTS];

		public enum Moves
		{
			MoveLeft,
			MoveRight,
			Rotate,
			None
		}
			
		void Start () {
			
	
			}

		public int getClears()
		{
			return this.numb_clears;
		}

		public void setTime(float time)
		{
			this.p_time = time;

		}

		public float getTime()
		{
			return this.p_time;

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

		public void makeOutputMoves(List<Moves> listp)
		{

			for (int i = 0; i < 4; i++) {
				
					moveSequence.Add (listp);

				//moveSequence.Add (Shuffle.MixUp (listp));
			}
		}



		public List<Moves> getMove()
		{
			float maxValue = 0f;
			Moves moves = Moves.MoveLeft;
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
			if (moves == Moves.MoveRight) {
				return moveSequence [0];

			} else if (moves == Moves.MoveLeft) {
				return moveSequence [1];
			}
			else if (moves == Moves.Rotate) {
				return moveSequence [2];
			}
			else if (moves == Moves.None) {
				return moveSequence [3];
			}
			return moveSequence[0];

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
