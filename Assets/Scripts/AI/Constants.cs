﻿using System;

 
public class Constants
{
	public const int INPUTS = (22 * 10) + 1;	// 1 bias node
	public const int OUTPUTS = 3;

	public const int POPULATION = 10;
	public const int SPECIES_COUNT = 2;

	public const int GENERATIONS = 252;


	public const int TIME_ALIVE_THRESHOLD = 25;

	public const double PROBABILITY_MUTATE_WEIGHT = 1;
	public const int AMOUNT_MUTATE_WEIGHT = 5;	// In hundredths
	public const double PROBABILITY_ADD_NODE = 1;
	public const double PROBABILITY_ADD_CONNECTION = 1;

	/** USED FOR SPECIATION **/
	public const double COMPATABILITY_THRESHOLD = .9;
	public const float DISJOINT_MULTIPLIER = 1; // c2 in distance algorithm
	public const float WEIGHT_AVERAGE_MULTIPLIER = 1; // c3 in distance algorithm

}
