using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp{
public class Species {

				public List<Player> members;

				public Species ()
				{
					members = new List<Player>();
				}

				public void Add(Player player)
				{
					members.Add(player);
				}

				public void Remove(Player player)
				{
					members.Remove(player);
				}

				public void RemovebyPlayerName(string p_name)
				{
					for (int i = 0; i < members.Count; i++) {
						if (members [i].getPlayerName () == p_name) {
								members.RemoveAt (i);
						}
					}
				}

				public Player FindbyPlayerName(string p_name)
				{
					for (int i = 0; i < members.Count; i++) {
						if (members [i].getPlayerName () == p_name) {
					//Debug.Log ("first " + members [i].brain);
							return members [i];
						}
					}
					return null;
				}

				public void addFitness(string p_name,float p_fitness)
				{
					for (int i = 0; i < members.Count; i++) {
						if (members [i].getPlayerName () == p_name) {
							//Debug.Log ("yoooouuuu " + members [i].brain);
							members [i].setPlayerFitness (p_fitness);

						}
					}

				}

				public float GetAverageFitness()
				{
					float averageFitness = 0;
					float sum = 0;
					foreach(Player player in members)
					{
						sum += player.getPlayerFitness ();
					}
					if(members.Count > 0) averageFitness = sum/members.Count;
					return averageFitness;

				}

				public List<Player> GetMembers()
				{
					return members;
				}

				public Player LowestFitnessIndividualInSpecies()
				{
					float minFitness = float.MaxValue;
					Player minplayer = members[0];
					foreach (Player player in members)
					{
				float fitness = player.getPlayerFitness ();
						if (fitness < minFitness)
						{
							minplayer = player;
							minFitness = fitness;
						}
					}
					return minplayer;
				}

			public Dictionary<string, float> CalculateAdjustedFitness()
					{
				Dictionary<string, float> playerFitnessMap = new Dictionary<string, float>();
						

						foreach (Player player in members)
						{
					//Debug.Log (player.name);
				float fitness = player.brain.Evaluate(player.getBoard(),player.get_score(),player.getClears(),player.getTime());
							float adjusted = fitness/(float)members.Count;
							
							playerFitnessMap[player.getPlayerName()]= adjusted;//watch out

						}
						//Debug.Log ("count is " + playerFitnessMap.Count);
						return playerFitnessMap;
					}

				public void ChooseParents(out Player bestPlayer, out Player secondBestPlayer)
				{
					bestPlayer = members[0];
					secondBestPlayer = members[0];

					foreach(Player player in members) {
				float bestPlayerEval = bestPlayer.getPlayerFitness ();
				float secondBestPlayerEval = secondBestPlayer.getPlayerFitness ();
				float PlayerEval = player.getPlayerFitness ();

						if(PlayerEval > bestPlayerEval) {
							bestPlayer = player;
						}
						else if(PlayerEval > secondBestPlayerEval) {
							secondBestPlayer = player;
						}
					}
				}				

}
}




