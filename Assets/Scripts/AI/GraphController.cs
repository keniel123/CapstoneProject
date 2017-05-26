using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GraphController : MonoBehaviour {

	EvolutionStats ES;

	//list of graphs that will show the best , average and worst fitness in the population
	public LineRenderer[] fitness_graphs;

	//size of graphs 
	public Vector2 graphSizes;

	//instance of graph manager
	public static GraphController instance;

	void Awake(){
		instance = this;
	}

	// Use this for initialization
	void Start () {
		ES = new EvolutionStats();


	}

	// Update is called once per frame
	public void UpdateGraph () {
		Graph1 ();
		Graph2 ();
		Graph3 ();
	}

	//best agent
	void Graph1()
	{
		fitness_graphs [0].SetVertexCount (ES.getTopValues().Count);


		float maxHeight = Mathf.Max (ES.getTopValues().ToArray ());

		float multiY = graphSizes.y / maxHeight;
		float multiX = Mathf.Min (1, graphSizes.x / ES.getTopValues().Count);


		for (int i = 0; i < ES.getTopValues().Count; i++) {
			fitness_graphs [0].SetPosition (i, new Vector3 (i * multiX, ES.getTopValues() [i] * multiY, 0));
		}


	}

	//worst agent
	void Graph2()
	{
		fitness_graphs [1].SetVertexCount (ES.getTopValues().Count);

		float maxHeight = Mathf.Max (ES.getTopValues().ToArray ());

		float multiY = graphSizes.y / maxHeight;
		float multiX = Mathf.Min (1, graphSizes.x / ES.getTopValues().Count);

		for (int i = 0; i < ES.getWorstValues().Count; i++) {
			fitness_graphs [1].SetPosition (i, new Vector3 (i * multiX, ES.getWorstValues() [i] * multiY, 0));
		}



	}

	//avg agent
	void Graph3()
	{
		fitness_graphs [2].SetVertexCount (ES.getTopValues().Count);

		float maxHeight = Mathf.Max (ES.getTopValues().ToArray ());

		float multiY = graphSizes.y / maxHeight;
		float multiX = Mathf.Min (1, graphSizes.x / ES.getTopValues().Count);

		for (int i = 0; i < ES.getAvgValues().Count; i++) {
			fitness_graphs [2].SetPosition (i, new Vector3 (i * multiX, ES.getAvgValues() [i] * multiY, 0));
		}



	}

}

