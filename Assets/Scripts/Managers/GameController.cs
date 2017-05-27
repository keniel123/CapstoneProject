using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Threading;
<<<<<<< HEAD
// FILE IMPORTS
using System.IO;
using System.Text;
using System.Linq;
=======
>>>>>>> 708281a44cab52ec2f4339869f1db3b198b1d9ad

namespace AssemblyCSharp{
	public class GameController : MonoBehaviour {

<<<<<<< HEAD
		// FILE VARIABLES
		string path = @"C:/Program Files/Unity/TetrisAgents.txt"; // define path to file
		string newLine = Environment.NewLine; // define new line character
		// define best player over all generations
		float h_all_gens = -1000;
		Player bestEvolvedPlayer;
		Player bestGenPlayer;

=======
>>>>>>> 708281a44cab52ec2f4339869f1db3b198b1d9ad
		public Text generation_count, h_fitness_value, c_fitness_value, species_value, genome_value;

		public static float timer;

		public static bool timeStarted = false;

		FitnessEvaluator fiteval;
		//player's neural network
		float h_all_species = -100000,l_all_species,a_all_species = -1000;

		//Stopwatch stopwatch;

		EvolutionStats Evo;

		int playersSpawned;

		int currentGenome = 1;

		float[] inputArray = new float[Constants.INPUTS];

		//List of all species
		private List<Species> species;

		//list all players
		//private List<GameObject> players;

		//generation counter
		private int generation;
		//number of generations till we replace the worst player
		private int Gen_Till_Remove_Player = 4;

		Player player;
		// reference to our board
		Board m_gameBoard;

		//holds the seconds from gane start
		float m_time;

		//current species
		private int curr_species = 0;

		//current neural network active
		private int curr_net = -1;
		//seconds until we remove worst player
		private int m_sec_remove_player = 6;

		private float h_fitness, low_fitness, avg_fitness;

		// reference to our spawner 
		Spawner m_spawner;

		// reference to our soundManager
		SoundManager m_soundManager;

		// reference to our scoreManager
		ScoreManager m_scoreManager;

		//reference to graph manager
		//GraphController m_graphManager;

		// currently active shape
		Shape m_activeShape;

		// ghost for visualization
		Ghost m_ghost;

		Holder m_holder;

		// starting drop interval value
		public float m_dropInterval = 1.1f;

		// drop interval modified by level
		float m_dropIntervalModded;

		// the next time that the active shape can drop
		float m_timeToDrop;

		// the next time that the active shape can move left or right
		float m_timeToNextKeyLeftRight;

		// the time window we can move left and right
		[Range(0.02f,1f)]
		public float m_keyRepeatRateLeftRight = 0.25f;

		// the next time that the active shape can move down
		float m_timeToNextKeyDown;

		// the time window we can move down
		[Range(0.01f,0.5f)]
		public float m_keyRepeatRateDown = 1.01f;

		// the time window we can rotate 
		float m_timeToNextKeyRotate;

		// the time window we can rotate the shape
		[Range(0.02f,1f)]
		public float m_keyRepeatRateRotate = 0.25f;

		// the panel that displays when our game is over
		public GameObject m_gameOverPanel;

		// whether we have reached the game over condition
		bool m_gameOver = false;

		// toggles the rotation direction icon
		public IconToggle m_rotIconToggle;

		// whether we are rotating clockwise or not when we click the up arrow
		bool m_clockwise = true;

		// whether we are paused
		public bool m_isPaused = false;

		// the panel that display when we Pause
		public GameObject m_pausePanel;

		public ParticlePlayer m_gameOverFx;



		// Use this for initialization
		void Start () 
		{

			// find spawner and board with GameObject.FindWithTag plus GetComponent; make sure you tag your objects correctly
			//m_gameBoard = GameObject.FindWithTag("Board").GetComponent<Board>();
			//m_spawner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();

			// find spawner and board with generic version of GameObject.FindObjectOfType, slower but less typing
			m_gameBoard = GameObject.FindObjectOfType<Board>();
			m_spawner = GameObject.FindObjectOfType<Spawner>();
			m_soundManager = GameObject.FindObjectOfType<SoundManager>();
			m_scoreManager = GameObject.FindObjectOfType<ScoreManager>();
			//m_graphManager = GameObject.FindObjectOfType<GraphController>();
			m_ghost = GameObject.FindObjectOfType<Ghost>();
			m_holder = GameObject.FindObjectOfType<Holder>();
			species = new List<Species> ();
			//stopwatch = new Stopwatch ();
			timeStarted =true;
			//players = new List<GameObject> ();

			Evo = new EvolutionStats ();
			//inputArray[Constants.INPUTS-1] = 1; // bias node
			//playersSpawned = 1;
			generation = 0;
			species_value.text = (curr_species + 2).ToString();
			genome_value.text = (curr_net + 2).ToString() ;
			generation_count.text = generation.ToString();


			//AssignPlayerToSpecies (player);
			CreateInitialPopulation();
			player = new Player ();
			var net = next_net ();
			player = (Player)net;


			//Debug.LogWarning (player.getPlayerName ());

			m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;
			m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
			m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;

			if (!m_gameBoard)
			{
				Debug.LogWarning("WARNING!  There is no game board defined!");
			}

			if (!m_soundManager)
			{
				Debug.LogWarning("WARNING!  There is no sound manager defined!");
			}

			if (!m_scoreManager)
			{
				Debug.LogWarning("WARNING!  There is no score manager defined!");
			}


			if (!m_spawner)
			{
				Debug.LogWarning("WARNING!  There is no spawner defined!");
			}
			else
			{
				m_spawner.transform.position = Vectorf.Round(m_spawner.transform.position);

				if (!m_activeShape)
				{
					m_activeShape = m_spawner.SpawnShape();
				}
			}

			if (m_gameOverPanel)
			{
				m_gameOverPanel.SetActive(false);
			}

			if (m_pausePanel)
			{
				m_pausePanel.SetActive(false);
			}

			//player.setInputArray (InputBoardStates (inputArray));

			m_dropIntervalModded = Mathf.Clamp(m_dropInterval - ((float)m_scoreManager.m_level * 0.1f), 0.05f, 1f);
			//CreatePlayer ();
			//Debug.Log ("y " + m_gameBoard.m_grid.GetLength(1));
			//Debug.Log ("x " + m_gameBoard.m_grid.GetLength(0));

		}

		// Update is called once per frame
		void Update () 
		{
			
			// if we are missing a spawner or game board or active shape, then we don't do anything
			if (!m_spawner || !m_gameBoard || !m_activeShape || m_gameOver || !m_soundManager || !m_scoreManager )
			{
				return;
			}


			if (Time.time > m_timeToDrop) {
				m_timeToDrop = Time.time + m_dropInterval;
				m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;
				if (m_activeShape) {

					m_activeShape.MoveDown ();
					if (!m_gameBoard.IsValidPosition (m_activeShape)) 
					{
						if (m_gameBoard.IsOverLimit(m_activeShape))
						{
							m_gameOver = true;
						}
						else
						{
							//Debug.Log ("land");
							LandShape ();
						}
					}
				}


			}
			if (timeStarted == true) {
				timer += Time.deltaTime;
			}
			species_value.text = (curr_species + 1).ToString();
			genome_value.text = (curr_net + 1).ToString();
			generation_count.text = generation.ToString();


			if(m_gameOver){
				timeStarted = false;
				player.setBoard (Make2DArray (player.getInputArray(), 22, 10));
				player.setClears (m_scoreManager.getClears());
				player.setScore (m_scoreManager.getscore());
				float seconds = timer % 60;
				player.setTime (seconds);
				//Debug.Log (timer);
				timer = 0f;

				var next_network = next_net ();

				if (next_network == null) {
					
					m_gameOver = false;
					NewGeneration ();
					findHALValues ();
					h_fitness_value.text = h_all_species.ToString ();
				
					//m_graphManager.UpdateGraph ();
				} else {
					Reset ();
					timeStarted = true;
					m_gameOver = false;
					player = (Player) next_network;
					//Debug.LogWarning (player.getPlayerName ());

				}

			}


<<<<<<< HEAD
			// Check for end of set training time
			if (generation > Constants.GENERATIONS){
				// Save best NN at the end of training
				saveBestBrain(bestEvolvedPlayer, h_all_gens);
				// End game
				GameOver();
			}
			else{
				player.setInputArray (InputBoardStates (inputArray));
				PlayerInput (player.getMove());
				player.updateBrain ();
				if (generation > 0) {
					c_fitness_value.text = player.getPlayerFitness ().ToString ();
				}
			}


=======
			player.setInputArray (InputBoardStates (inputArray));
			PlayerInput (player.getMove());
			player.updateBrain ();
			if (generation > 0) {
				c_fitness_value.text = player.getPlayerFitness ().ToString ();

			}



>>>>>>> 708281a44cab52ec2f4339869f1db3b198b1d9ad
		}



		void LateUpdate()
		{
			if (m_ghost)
			{
				m_ghost.DrawGhost(m_activeShape,m_gameBoard);
			}

		}

		private static T[,] Make2DArray<T>(T[] input, int height, int width)
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


		void MoveRight()
		{
			m_activeShape.MoveRight ();
			//m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;

			if (!m_gameBoard.IsValidPosition (m_activeShape)) 
			{
				m_activeShape.MoveLeft ();
				PlaySound (m_soundManager.m_errorSound,0.5f);
			}
			else
			{
				PlaySound (m_soundManager.m_moveSound,0.5f);

			}

		}



		void MoveLeft()
		{
			m_activeShape.MoveLeft ();
			//m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;

			if (!m_gameBoard.IsValidPosition (m_activeShape)) 
			{
				m_activeShape.MoveRight ();
				PlaySound (m_soundManager.m_errorSound,0.5f);
			}
			else
			{
				PlaySound (m_soundManager.m_moveSound,0.5f);

			}

		}

		void Rotate()
		{
			//m_activeShape.RotateRight();
			m_activeShape.RotateClockwise(m_clockwise);

			//m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;

			if (!m_gameBoard.IsValidPosition (m_activeShape)) 
			{
				//m_activeShape.RotateLeft();
				m_activeShape.RotateClockwise(!m_clockwise);

				PlaySound (m_soundManager.m_errorSound,0.5f);
			}
			else
			{
				PlaySound (m_soundManager.m_moveSound,0.5f);

			}
		}

		void PlayerInput(Player.Moves m)
		{
			if (Player.Moves.MoveLeft == m) {
				MoveLeft ();

			} else if (Player.Moves.MoveRight == m) {
				MoveRight ();

			} else if (m == Player.Moves.Rotate) {
				Rotate ();

			} 
		}


		void CreatePlayer()
		{
			//playerObj = GameObject.Instantiate (player);
			Player p = new Player();
			p.brain = new AgentNeuralNetwork (Constants.INPUTS, Constants.OUTPUTS);
			//players.Add (playerObj);
			AssignPlayerToSpecies (p);


		}

		void findHALValues()
		{
<<<<<<< HEAD
			// Initialize
			int species_count = 0;
			float sum = 0;
			int bestSpecies = 0;

			foreach (Species s in species) {
				// Track number of species
				species_count++;
				foreach (Player p in s.members) {
					sum += p.getPlayerFitness();
					if (p.getPlayerFitness () > h_all_species) {
						h_all_species = p.getPlayerFitness ();

						// Store best species for this gen
						bestSpecies = species_count;
						// Store best player for this gen
						bestGenPlayer = (Player) p;
						
						// Calculate best player over all gens
						if (h_all_species > h_all_gens)
						{
							// Update highest fitness
							h_all_gens = h_all_species;
							// Store corresponding player
							bestEvolvedPlayer = (Player) p;
						}

=======
			//Debug.Log ("i am in you hal");
			float sum = 0;
			foreach (Species s in species) {
				foreach (Player p in s.members) {
					//Debug.Log ("yaaaas " + p.getPlayerFitness ());
					sum += p.getPlayerFitness();
					if (p.getPlayerFitness () > h_all_species) {
						//Debug.Log ("i am in you if statement");
						h_all_species = p.getPlayerFitness ();

>>>>>>> 708281a44cab52ec2f4339869f1db3b198b1d9ad
					}
					if (p.getPlayerFitness () < l_all_species) {
						l_all_species = p.getPlayerFitness ();
					}
				}
<<<<<<< HEAD
				
=======

>>>>>>> 708281a44cab52ec2f4339869f1db3b198b1d9ad
			}
			a_all_species = sum / Constants.POPULATION;
			Evo.addTopValue (h_all_species);
			Evo.addAvgValue (a_all_species);
			Evo.addLowValue (l_all_species);
			//Debug.Log ("high " + h_all_species.ToString());
			//Debug.Log ("low " + l_all_species.ToString());

<<<<<<< HEAD
			// Write to file the best player for this gen
			saveBestPlayer(generation,bestSpecies,h_all_species,bestGenPlayer.get_score(),bestGenPlayer.getPlayerName());

		}

		// FUNCTION DEFINITIONS - Saving agent game sessions
		void saveBestPlayer(int gen,int numSpecies,float bestFitness,int score,string playerName)
		{
			// Add header text only once to the file
			if (!File.Exists(path))
			{
				// Header data
				string header = "Tetris Agents Data" + newLine;
				// Create file to write to
				File.WriteAllText(path, header);
			}

			// Generation data
			string genInfo = "Generation: " + gen.ToString();

			// Best player info
			string fitness = "Highest Fitness: " + bestFitness.ToString();
			string gameScore = "End game score: " + score.ToString();
			string bestPlayer = "Best Player: " + playerName;
			string species = "Species: " + numSpecies.ToString();

			// Synthesize 
			string data = newLine + genInfo + newLine + fitness + newLine + bestPlayer + newLine + gameScore + newLine + species + newLine;
			
			// Append data to file
			File.AppendAllText(path, data);

		}

		// Save NN structure of best evolved player
		void saveBestBrain(Player player, float bestOverallFitness)
		{
			string bestp = player.getPlayerName();
			// Neural Net (only need the neural net of the best evolved player)
			string nn = "Best Evolved Player's Brain => " + bestp + " with fitness = " + bestOverallFitness.ToString() + " and game score = " + player.get_score().ToString() + " making line clears = " + player.getClears().ToString();
			// Get Node Genes
			List<NodeGene> nodeGenes = player.brain.GetNodes();
			// Get Connection Genes
			List<ConnectionGene> connectionGenes = player.brain.GetConnections();
			// Node Genes => ID, Type
			//string nodes = "<< Node Genes >> => " + string.Join(", ", nodeGenes.Select(n=>n.ToString()).ToArray<string>());
			// Iterate through list of node genes
			string nodes = "<< Node Genes >> => ";
			foreach (NodeGene n in nodeGenes)
			{
				// Check node type
				if (n.type.Equals(NodeType.HIDDEN))
				{
					nodes += n.ToString() + " )( "; 
				}
			}
			// Connection Genes => InnovationNum, NodeIn, NodeOut, Weight, Enabled
			string connections = "<< Connection Genes >> => " + string.Join(" || ", connectionGenes.Select(c=>c.ToString()).ToArray<string>());
			// Genome
			string genome = "Genome" + newLine + nodes + newLine + connections;

			// Synthesize 
			string data = newLine + nn + newLine + genome + newLine;
			
			// Append data to file
			File.AppendAllText(path, data);
		}

=======
		}
>>>>>>> 708281a44cab52ec2f4339869f1db3b198b1d9ad
		void ReplaceWorstPlayer()
		{
			if(generation > 0)
			{
				
				Player worstPlayer = RemoveWorstPlayer();
				//Evo.addLowValue (worstPlayer.brain.Evaluate (worstPlayer.getBoard (), worstPlayer.get_score (), worstPlayer.getClears ()));
				//Debug.Log ("worst player >> from remove player  " + worstPlayer.getPlayerName());
				if (worstPlayer.getPlayerName() == null) 
				{
					//Debug.Log ("worst player null >>>>>");
					return;
				}

				//choose the best parent species
				Species parentSpecies = ChooseParentSpecies ();
				if(parentSpecies == null) parentSpecies = species[0];


				Player bestPlayer = worstPlayer;
				Player secondbestPlayer = worstPlayer;
				parentSpecies.ChooseParents (out bestPlayer, out secondbestPlayer);

				//best player for specie Evo.addTopValue(bestPlayer.brain.Evaluate (bestPlayer.getBoard (), bestPlayer.get_score (), bestPlayer.getClears ()));
				//Debug.Log ("best player >>   " + bestPlayer.getPlayerName());
				//Debug.Log ("second best player >>   " + secondbestPlayer.getPlayerName());
				//Create a new brain from the 2 best parents
				AgentNeuralNetwork fitterbrain = new AgentNeuralNetwork(bestPlayer,secondbestPlayer);
				//Debug.Log ("worst player >> child  " + worstPlayer.getPlayerName());
				worstPlayer.brain = fitterbrain;
				//Debug.Log ("worst player >> child alive int  " + worstPlayer.getAlive_interval());
				AssignPlayerToSpecies (worstPlayer);

			}


		}

		void Reset()
		{
			m_scoreManager.setScore (0);
			m_scoreManager.setLevel (1);
			m_scoreManager.setClears (0);
			m_scoreManager.Reset ();

			deleteShapes ();

		}
		void deleteShapes(){
			for (int i = 0; i < 23; i++) {
				m_gameBoard.ClearRow (i);
			}




		}

		public object next_net()
		{
			//Debug.Log("The size of specie " + this.curr_species + "is " + species [this.curr_species].members.Count);
			this.curr_net += 1;
			if (this.curr_species >= species.Count) {
				this.curr_species = 0;
				this.curr_net = -1;
				return null;
			}
			else if (this.curr_net >= species[this.curr_species].members.Count) 
			{
				this.curr_net = -1;
				this.curr_species += 1;	
				return next_net ();
			}

			else 
			{
				try
				{

					return (Player) species[this.curr_species].members[this.curr_net];


				}
				catch(IndexOutOfRangeException)
				{

					return 1;
				}



			}
		}

		public void AssignPlayerToSpecies(Player player)
		{

			bool assigned = false;
			foreach (Species s in species) 
			{
				foreach (Player m in s.GetMembers()) 
				{
					int disjoint = 0;
					int N = 1;
					double weightedAverage = 0;
					player.brain.DistanceFrom (m.brain, out disjoint, out N, out weightedAverage);
					float d = (((float)disjoint*Constants.DISJOINT_MULTIPLIER)/(float)N) + ((float)weightedAverage*Constants.WEIGHT_AVERAGE_MULTIPLIER);

					if (d <= Constants.COMPATABILITY_THRESHOLD) 
					{

						s.Add (player);
						assigned = true;
						break;
					}

				}

			}

			if (!assigned) 
			{
				Species newSpecies = new Species ();
				newSpecies.Add (player);
				species.Add (newSpecies);

			}
		}

		public void CreateInitialPopulation()
		{
			for (int x = 0; x < Constants.SPECIES_COUNT; x++) {
				Species newSpecies = new Species ();
				species.Add (newSpecies);

			}
			for(int i = 0; i< Constants.POPULATION; i++)
			{
				player = new Player ();
				player.brain = new AgentNeuralNetwork (Constants.INPUTS, Constants.OUTPUTS);
				player.setPlayerName ("Player" + i);
				species [UnityEngine.Random.Range (0, species.Count)].Add (player);

			}



		}


		public void NewGeneration()
		{
			
			Debug.Log ("new generation");
			//Debug.Log ("h fit" + h_fitness.ToString());
			//Evo.addTopValue (h_fitness);
			//Evo.addLowValue (low_fitness);
			this.generation++;
			Reset ();
			ReplaceWorstPlayer ();
			if (generation > 0 && generation % 15 == 0) {
				PruneStaleSpecies ();
			}
			var net = next_net ();
			player = (Player)net;
			timeStarted = true;
		}

	

		public Player RemoveWorstPlayer()
		{
			float minFitness = float.MaxValue;

			String minPlayer = null;
			Species minPlayerSpecies = null;

			//find player with the lowest adjusted fitness
			 foreach(Species s in species)
			{
				Dictionary<string, float> adjustedFitnessMapping = s.CalculateAdjustedFitness ();

				foreach (String m in adjustedFitnessMapping.Keys) 
				{

					s.addFitness(m,adjustedFitnessMapping[m]);
					if(adjustedFitnessMapping[m] < minFitness)
					{
						minFitness = adjustedFitnessMapping[m];
						minPlayer = m;
						minPlayerSpecies = s;


					}

				}


			}

			//Debug.Log ("name " + minPlayer);
			//Debug.Log ("count before " + minPlayerSpecies.members.Count);
			Player m_player = new Player();

			if (minPlayer != null) {
				
				m_player = minPlayerSpecies.FindbyPlayerName (minPlayer);
					
				minPlayerSpecies.RemovebyPlayerName (minPlayer);
			}
					for (int i = 0; i < species.Count; i++) {
						if (species [i].members.Count == 0) {
							species.RemoveAt (i);
						}
					}

					return m_player;
				
			}



		public Species ChooseParentSpecies()
		{
			float prob = 0;
			System.Random gen = new System.Random ();
			double i = gen.NextDouble ();

			//calculate total fitness of population
			float totalFitness = 0f;
			foreach (Species s in species) 
			{
				totalFitness += s.GetAverageFitness ();
			}


			foreach (Species s in species) 
			{
				float prob2 = (s.GetAverageFitness () / totalFitness);
				if(i >= prob && i < prob2)
				{
					return s;
				}
				prob = prob2;
			}


			return null;

		}



		void PruneStaleSpecies()
		{
			List<string> availableNames = new List<string> ();
			int remainingAgents = 0;
			float sum = 0;
			float avg = 0;
			foreach (Species s in species) {
				sum += s.GetAverageFitness ();
			}
			avg = sum / species.Count;
			for (int i = 0; i < species.Count; i++) {
				if (species [i].GetAverageFitness () < avg) {
					species.RemoveAt (i);
				}
			}
			foreach (Species sp in species) {
				remainingAgents += sp.members.Count;
				foreach (Player p in sp.members) {
					availableNames.Add (p.getPlayerName ());
				}
			}
			if (species.Count < Constants.SPECIES_COUNT) {
				Species newSpecies = new Species ();
				species.Add (newSpecies);
				for (int i = 0; i < Constants.POPULATION - remainingAgents; i++) {
					player = new Player ();
					player.brain = new AgentNeuralNetwork (Constants.INPUTS, Constants.OUTPUTS);
					player.setPlayerName ("Player" + UnityEngine.Random.Range(0,50));
					player.setPlayerFitness (-9999);
					if (availableNames.Contains (player.getPlayerName ())) {
						while (!availableNames.Contains (player.getPlayerName ())) {
							player.setPlayerName ("Player" + UnityEngine.Random.Range (0, 50));
						}
					}
					species[species.Count-1].Add (player);
				}
			}
		}





		//populate inputArray with board states
		float[]InputBoardStates(float[] input)
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

			return input;

		}



		// shape lands
		void LandShape ()
		{
			// move the shape up, store it in the Board's grid array
			m_activeShape.MoveUp ();
			m_gameBoard.StoreShapeInGrid (m_activeShape);

			m_activeShape.LandShapeFX();

			if (m_ghost)
			{
				m_ghost.Reset();
			}

			if (m_holder)
			{
				m_holder.m_canRelease = true;
			}
			// spawn a new shape
			m_activeShape = m_spawner.SpawnShape ();



			// remove completed rows from the board if we have any 
			//m_gameBoard.ClearAllRows();

			m_gameBoard.StartCoroutine("ClearAllRows");



			PlaySound (m_soundManager.m_dropSound);
		
		
			//m_gameBoard.m_completedRows = 1;
			//Debug.Log ("Before >> completed rows =  " + m_gameBoard.m_completedRows);

			if (m_gameBoard.m_completedRows > 0)
			{
				//Debug.Log ("After >> completed rows =  " + m_gameBoard.m_completedRows);
				m_scoreManager.ScoreLines(m_gameBoard.m_completedRows);

				if (m_scoreManager.didLevelUp)
				{
					m_dropIntervalModded = Mathf.Clamp(m_dropInterval - ((float)m_scoreManager.m_level * 0.05f), 0.05f, 1f);
					PlaySound(m_soundManager.m_levelUpVocalClip);
				}
				else
				{
					if (m_gameBoard.m_completedRows > 1)
					{
						AudioClip randomVocal = m_soundManager.GetRandomClip(m_soundManager.m_vocalClips);
						PlaySound(randomVocal);
					}
				}



				PlaySound (m_soundManager.m_clearRowSound);
			}


		}

		// triggered when we are over the board's limit
		void GameOver ()
		{
			// move the shape one row up
			m_activeShape.MoveUp ();

			StartCoroutine("GameOverRoutine");

			// play the failure sound effect
			PlaySound (m_soundManager.m_gameOverSound,5f);

			// play "game over" vocal
			PlaySound (m_soundManager.m_gameOverVocalClip,5f);


			// set the game over condition to true
			m_gameOver = true;
		}

		IEnumerator GameOverRoutine()
		{
			if (m_gameOverFx)
			{
				m_gameOverFx.Play();
			}
			yield return new WaitForSeconds(0.1f);

			// turn on the Game Over Panel
			if (m_gameOverPanel) 
			{
				m_gameOverPanel.SetActive (true);
			}

		}

		// reload the level
		public void Restart()
		{
			Time.timeScale = 1f;
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			generation += 1;
		}

		// plays a sound with an option volume multiplier
		void PlaySound (AudioClip clip, float volMultiplier = 1.0f)
		{
			if (m_soundManager.m_fxEnabled && clip) {
				AudioSource.PlayClipAtPoint (clip, Camera.main.transform.position, Mathf.Clamp(m_soundManager.m_fxVolume*volMultiplier,0.05f,1f));
			}
		}

		public void ToggleRotDirection()
		{
			m_clockwise = !m_clockwise;
			if (m_rotIconToggle)
			{
				m_rotIconToggle.ToggleIcon(m_clockwise);
			}
		}

		public void TogglePause()
		{
			m_isPaused = !m_isPaused;

			if (m_pausePanel)
			{
				m_pausePanel.SetActive(m_isPaused);

				if (m_soundManager)
				{
					m_soundManager.m_musicSource.volume = (m_isPaused) ? m_soundManager.m_musicVolume * 0.25f : m_soundManager.m_musicVolume;
				}

				Time.timeScale = (m_isPaused) ? 0 : 1;
			}
		}

		public void Hold()
		{

			// if the holder is empty...
			if (!m_holder.m_heldShape)
			{
				// catch the current active Shape
				m_holder.Catch(m_activeShape);

				// spawn a new Shape
				m_activeShape = m_spawner.SpawnShape();

				// play a sound
				PlaySound(m_soundManager.m_holdSound);

			} 
			// if the holder is not empty and can release…
			else if (m_holder.m_canRelease)
			{
				// set our active Shape to a temporary Shape
				Shape shape = m_activeShape;

				// release the currently heldShape 
				m_activeShape = m_holder.Release();

				// move the released Shape back to the spawner position
				m_activeShape.transform.position = m_spawner.transform.position;

				// catch the temporary Shape
				m_holder.Catch(shape);

				// play a sound 
				PlaySound(m_soundManager.m_holdSound);

			} 
			// the holder is not empty but cannot release yet
			else
			{
				//Debug.LogWarning("HOLDER WARNING:  Wait for cool down");

				// play an error sound
				PlaySound(m_soundManager.m_errorSound);

			}

			// reset the Ghost every time we tap the Hold button
			if (m_ghost)
			{
				m_ghost.Reset();
			}

		}


		//		public int NumberOfHoles()
		//		{
		//			var holes = 0;
		//
		//			for (var column = 0; column < this.m_gameBoard.m_grid.GetLength(1); column++)
		//			{
		//				var reachedTopColumn = false;
		//
		//				for (var row = this.m_gameBoard.m_grid.GetLength(0) - 1; row >= 0; row--)
		//				{
		//					var field = this.m_gameBoard.m_grid[row,column];
		//					if (reachedTopColumn && !field)
		//					{
		//						holes++;
		//					}
		//
		//					if (field)
		//					{
		//						reachedTopColumn = true;
		//
		//					}
		//				}
		//				Debug.Log (reachedTopColumn);
		//			}
		//
		//			Debug.Log(holes);
		//
		//			return holes;
		//		}


	}
}