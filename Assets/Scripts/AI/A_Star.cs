using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// A star pathfinding algorithm.
/// </summary>
public class A_Star : MonoBehaviour
{
	#region Member Variables
	public bool 					debugActivated;

	private Cell					start;
	private Cell 					goal;
	private int 					rotation;
	private Board					grid;
	// The set of nodes already evaluated.
	private HashSet<Cell> 			openSet;

	// The set of currently discovered nodes that are not evaluated yet. 
	private HashSet<Cell> 			closedSet;

	// For each node, which node it can most efficiently be reached from.
	// If a node can be reached from many nodes, cameFrom will eventually contain the
	// most efficient previous step.
	private Dictionary<Cell, Cell>	cameFrom;

	private Dictionary<Cell, float> gScore;		// For each node, the cost of getting from the start node to that node. The default value is Infinity
	private Dictionary<Cell, float> fScore;		// For each node, the total cost of getting from the start node to the goal. The default value is Infinity

	private List<Cell>				reconstructedPath;

	private const int 				NODES_PER_FRAME = 10;
	#endregion

	#region Unity Methods
	void OnGUI()
	{
		if (debugActivated)
		{
			GUIStyle guiStyle = new GUIStyle ();
			guiStyle.fontSize = 10;

			if (reconstructedPath != null)
			{
				//Shows the path to the solution
				foreach (var node in reconstructedPath)
				{
					int xOffset = 85 + 10;
					GUI.Label (new Rect (18.3f * node.x + xOffset, 75 + 17.56f * (grid.m_height - node.y), 10, 10), "X", guiStyle);
				}
			}
		}
	}

	void Destroy()
	{
	}
	#endregion

	#region Properties
	public List<Cell> ReconstructedPath
	{
		get
		{
			return reconstructedPath;
		}
	}
	#endregion

	#region Public Methods
	public IEnumerator PerformSearch(Cell _start, Cell _goal, int _rotation, Board _grid, Action<List<Cell>> _OnComplete,Shape active)
	{
		bool bFound = false;

		if (_grid == null)
		{
			Debug.LogError ("_grid is null");
		}

		start 		= _start;
		goal 		= _goal;
		rotation 	= _rotation;
		grid 		= _grid;

		openSet 		= new HashSet<Cell> 	();
		closedSet 		= new HashSet<Cell> 	();

		cameFrom 		= new Dictionary<Cell, Cell> 	();
		gScore			= new Dictionary<Cell, float> 	();
		fScore			= new Dictionary<Cell, float> 	();

		// Initially, only the start node is known.
		openSet.Add(start);
			
		// The cost of going from start to start is zero.
		gScore[start] = 0;

		// For the first node, that value is completely heuristic.
		fScore[start] = HeuristicCostEstimate(start, goal);
		int counter = 0;

		while (openSet.Count > 0 && !bFound)
		{
			Cell current = LowestScoredNode ();

			if (current == goal)
			{
				
				//Debug.Log ("current >>>> goal ");

				//Debug.Log ("current >>>> " + current);
				//Debug.Log ("goal >>>>" + goal);
				reconstructedPath = ReconstructPath (current);

				if (_OnComplete != null)
				{
					_OnComplete (reconstructedPath);
					bFound = true;
					continue;
				}
			}
			++counter;


			closedSet.Add (current);
			openSet.Remove (current);

			List<Cell> neighbours = current.GetNeighbours ();

			foreach (var neighbour in neighbours)
			{
				if (closedSet.Contains (neighbour))
				{
					continue; // Ignore the neighbor which is already evaluated.
				}

				if (!gScore.ContainsKey (current))
				{
					gScore [current] = float.MaxValue;
				}

				// The distance from start to a neighbor
				float tentativeGScore = gScore [current] + DistBetween (active,current, neighbour);

				if (!openSet.Contains (neighbour))
				{
					openSet.Add (neighbour);
				}
				else if (tentativeGScore >= gScore[neighbour])
				{
					continue;
				}
					
				// This path is the best until now. Record it!
				cameFrom[neighbour] = current;
				gScore [neighbour] = tentativeGScore;
				fScore [neighbour] = gScore [neighbour] + HeuristicCostEstimate (neighbour, goal);
			}
		}

		if (_OnComplete != null && !bFound)
		{
			_OnComplete (null);
		}
			
		yield return null;
	}
	#endregion

	#region Private Methods
	private float DistBetween(Shape active_shape,Cell _from, Cell _to)
	{
		float result = 0;

		if (!grid.IsValidPosition (active_shape, _to, rotation))
		{
			result = float.MaxValue;
		}
		else
		{
			result = _from.DistanceSquared (_to);
		}

		return result;
	}

	private float HeuristicCostEstimate(Cell _from, Cell _to)
	{
		return _from.DistanceSquared (_to);
	}

	//the node in openSet having the lowest fScore[] value
	private Cell LowestScoredNode()
	{
		float 	lowestScore 	= float.MaxValue;
		Cell 	lowestScoreNode = new Cell (0, 0, grid);

		bool initialized = false;

		foreach (var currentNode in openSet)
		{
			if (!initialized)
			{
				initialized 	= true;
				lowestScore 	= fScore.ContainsKey(currentNode) ? fScore [currentNode] : float.MaxValue;
				lowestScoreNode = currentNode;
			}

			float currentValue = fScore.ContainsKey(currentNode) ? fScore [currentNode] : float.MaxValue;

			if (currentValue < lowestScore)
			{
				lowestScore 	= currentValue;
				lowestScoreNode = currentNode;
			}
		}

		return lowestScoreNode;
	}

	private List<Cell> ReconstructPath(Cell _current)
	{

		List<Cell> totalPath = new List<Cell> ();
		totalPath.Insert (0, _current);

		while(cameFrom.ContainsKey(_current))
		{
			_current = cameFrom [_current];
			totalPath.Insert (0, _current);
		}

		totalPath.Remove (start);

		return totalPath;
	}
	#endregion
}