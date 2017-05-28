using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using System;


namespace AssemblyCSharp{
	public class GameController : MonoBehaviour {

		public Text generation_count, h_fitness_value, c_fitness_value, species_value, genome_value;

		public static float timer;

		public static bool timeStarted = false;

		FitnessEvaluator fiteval;
		//player's neural network
		float h_all_species = -100000,l_all_species,a_all_species = -1000;

		EvolutionStats Evo;

		A_Star search;

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
		GraphController m_graphManager;

		// currently active shape
		Shape m_activeShape;

		// ghost for visualization
		Ghost m_ghost;

		Holder m_holder;

		//time to wait before make next move in follow path
		//public float timeLeft = 30.0f;

		// starting drop interval value
		public float m_dropInterval = 1.1f;

		public List<Player.Moves> MovesList;
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

		List<Cell> pathSolution = new List<Cell>();
		// whether we are rotating clockwise or not when we click the up arrow
		bool m_clockwise = true;

		// whether we are paused
		public bool m_isPaused = false;

		//bool that says path is being followed to not call over followingupdate
		public bool is_followingpath = false;

		// the panel that display when we Pause
		public GameObject m_pausePanel;

		int move_index = 0;

		DecisionFunction df;

		float[,] decisionarray = new float[10,22];

		float[] tempArray = new float[220];

		Vector2 destinationVector;

		Vector3 currentVector;

		int destinationRotation;

		bool completed_move = true;

		bool finisheddecision = false;

		public ParticlePlayer m_gameOverFx;

		private enum AIStates
		{
			MakingDecision,
			Idle,
			FollowingPath,
			PathCompleted

		}
		AIStates currentState;

		Cell curr_loc;

		Cell dest_cell;

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
			m_graphManager = GameObject.FindObjectOfType<GraphController>();
			m_ghost = GameObject.FindObjectOfType<Ghost>();
			m_holder = GameObject.FindObjectOfType<Holder>();
			species = new List<Species> ();
			currentState = AIStates.Idle;
			//players = new List<GameObject> ();
			player = new Player ();
			MovesList = new List<Player.Moves> ();
			Evo = new EvolutionStats ();
			//inputArray[Constants.INPUTS-1] = 1; // bias node
			//playersSpawned = 1;
			timeStarted =true;

			generation = 0;
			species_value.text = (curr_species + 2).ToString();
			genome_value.text = (curr_net + 2).ToString() ;
			generation_count.text = generation.ToString();
			search = new A_Star ();

			//AssignPlayerToSpecies (player);
			CreateInitialPopulation();
			//InvokeRepeating("FollowingPathUpdate",0f,.10f);
			//CreateInitialPopulation();
			player = new Player ();
			var net = next_net ();
			player = (Player)net;

			df = new DecisionFunction ();

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
			if (!m_graphManager)
			{
				Debug.LogWarning("WARNING!  There is no graph manager defined!");
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

			this.decisionarray = Make2DArray (InputBoardStates (this.tempArray), 22, 10);
			m_dropIntervalModded = Mathf.Clamp(m_dropInterval - ((float)m_scoreManager.m_level * 0.1f), 0.05f, 1f);
			//CreatePlayer ();
			currentState = AIStates.MakingDecision;

		}

		// Update is called once per frame
		void Update () 
		{
			// if we are missing a spawner or game board or active shape, then we don't do anything
			if (!m_spawner || !m_gameBoard || !m_activeShape || m_gameOver || !m_soundManager || !m_scoreManager || !m_graphManager)
			{
				return;
			}

		

				if (m_activeShape) {

					//m_activeShape.MoveDown ();
					if (!m_gameBoard.IsValidPosition (m_activeShape)) {
						if (m_gameBoard.IsOverLimit (m_activeShape)) {
							m_gameOver = true;
						} else {
							LandShape ();
						}
					}




			}


			//Debug.Log (currentState);
			switch (currentState) {
				case AIStates.MakingDecision:
					if (finisheddecision == true) {
						currentState = AIStates.Idle;
					}
					break;
			case AIStates.Idle:
					IdleUpdate ();
					break;
				case AIStates.FollowingPath:
				if (completed_move == true) {
					player.setInputArray (InputBoardStates (inputArray));
					player.updateBrain ();
					completed_move = false;
					StartCoroutine ("followingpath");
						//FollowingPathUpdate ();

					}
					
					break;
				case AIStates.PathCompleted:
					
					currentState = AIStates.MakingDecision;
					move_index = 0;
					//this.MovesList.Clear ();
					//Debug.Log ("Moveslist >>>>> idle " + this.MovesList.Count);
					break;
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
				timer = 0f;

				var next_network = next_net ();

				if (next_network == null) {
					Debug.Log ("next gen");
					m_gameOver = false;
					NewGeneration ();
					findHALValues ();
					h_fitness_value.text = h_all_species.ToString();
					//m_graphManager.UpdateGraph ();
				} else {
					Debug.Log ("reset");
					Reset ();
					timeStarted = true;
					m_gameOver = false;
					player = (Player) next_network;

				}

			}


			//player.setInputArray (InputBoardStates (inputArray));
//			PlayerInput (player.getMove());
			//player.updateBrain ();
			if (generation > 0) {
				c_fitness_value.text = player.getPlayerFitness ().ToString ();
			}



		}

		IEnumerator followingpath(){
			
			while (move_index < this.MovesList.Count) {
				ExecuteMoves(this.MovesList[move_index]);
				//Debug.Log (this.MovesList [move_index]);
				move_index++;
				yield return new WaitForSeconds(.1f);
			}
			PlayerInput (Player.Moves.None);
			//Debug.Log ("I have reached my target");

			yield return new WaitForSeconds (1f);

			//Debug.Log ("Couroutine Done");



			completed_move = true;
		}


		void FollowingPathUpdate()
		{
			
			if (move_index < this.MovesList.Count) {
				//Debug.Log (this.MovesList [move_index]);
				ExecuteMoves (this.MovesList [move_index]);
			}

			//ExecuteMoves (player.getMove ());

			//currentState = AIStates.PathCompleted;
			
		}

		void IdleUpdate()
		{
			runSearch (this.curr_loc,this.dest_cell,this.destinationRotation);
			//Debug.Log ("in you ");
		}

		void ExecuteMoves(Player.Moves move)
		{
			//
			PlayerInput (move);

		}

			//is_followingpath = true;

		IEnumerator makingDecision()
		{
			
			ArrayList result = df.ExecuteDecision (m_activeShape, m_gameBoard, this.decisionarray);
			//Debug.Log ("ggggggggggg   " + result);
			this.destinationVector = (Vector2) result [0];
			//Debug.Log ("ggggggggggg   " + destinationVector);
			this.destinationRotation = (int) result [1];
			//Debug.Log ("ggggggggggg   " + destinationRotation);
			this.currentVector = (Vector3) result [2];
			//Debug.Log ("ggggggggggg   " + currentVector);
			this.curr_loc = new Cell ((int)currentVector.x, (int)currentVector.y, m_gameBoard);
			this.dest_cell = new Cell ((int)destinationVector.x, (int)destinationVector.y, m_gameBoard);

			yield return new WaitForSeconds (.1f);

			finisheddecision = true;


		}


		void LateUpdate()
		{
			if (m_ghost)
			{
				m_ghost.DrawGhost(m_activeShape,m_gameBoard);
			}
			StartCoroutine("makingDecision");

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
		void PlayerInput(Player.Moves m)
		{
			
			if (Player.Moves.MoveLeft == m) {
				//Debug.Log ("make left");
				MoveLeft ();
				//completed_move = false;
				//move_index++;
				//completed_move = true;

			} else if (Player.Moves.MoveRight == m) {
				//Debug.Log ("make right");
				MoveRight ();
				//completed_move = false;
				//move_index++;
				//completed_move = true;

			} else if (m == Player.Moves.Rotate) {
				//Debug.Log ("make rotate");
				Rotate ();
				//completed_move = false;
				//move_index++;
				//completed_move = true;

			} else if (m == Player.Moves.None) {
				//Debug.Log ("make down");
				MoveDown ();
				//completed_move = false;
				//move_index++;
				//completed_move = true;
			}
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
			//Debug.Log ("make left");
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

		void MoveDown()
		{
			m_activeShape.MoveDown ();
			//m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;

			if (!m_gameBoard.IsValidPosition (m_activeShape)) 
			{
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
			float sum = 0;
			foreach (Species s in species) {
				foreach (Player p in s.members) {
					sum += p.getPlayerFitness();
					if (p.getPlayerFitness () > h_all_species) {
						h_all_species = p.getPlayerFitness ();

					}
					if (p.getPlayerFitness () < l_all_species) {
						l_all_species = p.getPlayerFitness ();
					}
				}

			}
			a_all_species = sum / Constants.POPULATION;
			Evo.addTopValue (h_all_species);
			Evo.addAvgValue (a_all_species);
			Evo.addLowValue (l_all_species);
			Debug.Log ("high " + h_all_species.ToString());
			Debug.Log ("low " + l_all_species.ToString());

		}
		void ReplaceWorstPlayer()
		{
			if(this.generation > 0)
			{
				//Debug.Log ("yaaaassss queen");
				Player worstPlayer = RemoveWorstPlayer();
				//Evo.addLowValue (worstPlayer.brain.Evaluate (worstPlayer.getBoard (), worstPlayer.get_score (), worstPlayer.getClears ()));
				if (worstPlayer.getPlayerName() == null) 
				{
					return;
				}

				//choose the best parent species
				Species parentSpecies = ChooseParentSpecies ();
				if(parentSpecies == null) parentSpecies = species[0];


				Player bestPlayer = worstPlayer;
				Player secondbestPlayer = worstPlayer;
				parentSpecies.ChooseParents (out bestPlayer, out secondbestPlayer);

				//best player for specie Evo.addTopValue(bestPlayer.brain.Evaluate (bestPlayer.getBoard (), bestPlayer.get_score (), bestPlayer.getClears ()));

				//Create a new brain from the 2 best parents
				AgentNeuralNetwork fitterbrain = new AgentNeuralNetwork(bestPlayer, secondbestPlayer);
				//Debug.Log ("fitter brain  " + fitterbrain);
				worstPlayer.brain = fitterbrain;

				AssignPlayerToSpecies (worstPlayer);

			}


		}

		void Reset()
		{
			
			m_scoreManager.setScore (0);
			m_scoreManager.setLevel (1);
			m_gameBoard.setClears (0);
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
			for (int x = 0; x <= Constants.SPECIES_COUNT; x++) {
				Species newSpecies = new Species ();
				species.Add (newSpecies);

			}
			for(int i = 0; i<= Constants.POPULATION; i++)
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


					//Debug.Log ("the fitness value for " + m + " is " + adjustedFitnessMapping [m]);
					s.addFitness(m,adjustedFitnessMapping[m]);
					if(adjustedFitnessMapping[m] < minFitness)
					{

						minFitness = adjustedFitnessMapping [m];
						minPlayer = m;
						minPlayerSpecies = s;


					}

				}


			}

			//Debug.Log ("name " + minPlayer);
			//Debug.Log ("count before " + minPlayerSpecies.members.Count);
			Player m_player = new Player();
			//low_fitness = minFitness;
			if (minPlayer != null) 
			{
				m_player = minPlayerSpecies.FindbyPlayerName (minPlayer);

				//remove player from its specie
				minPlayerSpecies.RemovebyPlayerName(minPlayer);
			}
			//Debug.Log ("count after " + minPlayerSpecies.members.Count);
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


		bool HasBeenAliveLongEnough(Player p)
		{
			return true;


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
			is_followingpath = true;
			m_activeShape.MoveUp ();
			//Debug.Log ("IN LANDY");

			// move the shape up, store it in the Board's grid array

			m_gameBoard.StoreShapeInGrid (m_activeShape);
			this.decisionarray = Make2DArray (InputBoardStates (this.tempArray), 22, 10);

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

			currentState = AIStates.PathCompleted;

			// set all of the timeToNextKey variables to current time, so no input delay for the next spawned shape
			m_timeToNextKeyLeftRight = Time.time;
			m_timeToNextKeyDown = Time.time;
			m_timeToNextKeyRotate = Time.time;

			// remove completed rows from the board if we have any 
			//m_gameBoard.ClearAllRows();

			m_gameBoard.StartCoroutine("ClearAllRows");


			PlaySound (m_soundManager.m_dropSound);

			if (m_gameBoard.m_completedRows > 0)
			{
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

		void runSearch(Cell start,Cell end, int rotation)
		{
			List<Player.Moves> temp_arr = new List<Player.Moves>();

			StartCoroutine(search.PerformSearch(start,end,rotation,m_gameBoard,delegate(List<Cell> _path) {
				pathSolution = _path;

			},m_activeShape));
		

			 if (pathSolution.Count != 0) {
				//Cell star = new Cell (1, 6, m_gameBoard);
				for (int i = 0; i < pathSolution.Count; i++) {
					if (i == 0) {
						temp_arr.Add (GetMovement2Cell (pathSolution [i], start));
					} else {
						temp_arr.Add(GetMovement2Cell(pathSolution [i], pathSolution [i - 1]));
					}
					//Debug.Log (pathSolution [i]);
					////GetMovement2Cell (pathSolution [i], star);
					//star = pathSolution [i];
				}
//				Debug.Log ("end >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>  ");
//				Debug.Log ("start  >>>>>>" + start);
//				Debug.Log ("end >>>>>>>>>  " + end);
				//Debug.Log ("rotation >>>> " + rotation);
//				for (int i = 0; i < pathSolution.Count; i++) {
//					Debug.Log ("solution  >>>>>> " + pathSolution[i]);
//				}
//				Debug.Log ("end >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
//
				//player.makeOutputMoves (MovesList);
				//this.MovesList = player.getMove ();
				this.MovesList = temp_arr;

				if (rotation == 1) {
					Rotate ();
				} else if (rotation == 2) {
					Rotate ();
					Rotate ();
				}
				else if (rotation == 3) {
					Rotate ();
					Rotate ();
					Rotate ();
				}
				currentState = AIStates.FollowingPath;
				//Debug.Log (MovesList.Count);
			}




		}

		Player.Moves GetMovement2Cell(Cell _to, Cell _from)
		{
			//Debug.Log ("From   >>> " + _from);
			//Debug.Log ("To  >>>>" + _to);
			Player.Moves move = Player.Moves.None;
			if (_to.x == (_from.x + 1) && _to.y == _from.y) {
				move = Player.Moves.MoveRight;
			}
			if (_to.x == (_from.x - 1) && _to.y == _from.y) {
				move = Player.Moves.MoveLeft;
			}
			return move;



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
