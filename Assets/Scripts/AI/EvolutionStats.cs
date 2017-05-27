using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionStats : MonoBehaviour {

	private List<float> topValue = new List<float> ();
	private List<float> lowValue = new List<float> ();
	private List<float> avgValue = new List<float> ();
	private float highestFit = 0;


	public void AnalyzeStats()
	{
		GraphController.instance.UpdateGraph();

	}

	public List<float> getTopValues()
	{
		return topValue;
	}

	public List<float> getWorstValues()
	{
		return lowValue;
	}

	public List<float> getAvgValues()
	{
		return avgValue;
	}

	public void addTopValue(float fitness)
	{
		topValue.Add (fitness);
		//Debug.Log ("high " + topValue [0].ToString());
	}

	public void addLowValue(float fitness)
	{
		lowValue.Add (fitness);
		//Debug.Log ("low " + lowValue [0].ToString());

	}

	public void addAvgValue(float fitness)
	{
		avgValue.Add (fitness);

	}

	public float highestFitness()
	{
		
		for (int i = 0; i < topValue.Count; i++) {
			if (topValue [i] > highestFit) {
				highestFit = topValue [i];
			}
		}
		return highestFit;
	}
		
}

