using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AssemblyCSharp {
	public class AgentNeuralNetwork : INeuralNetwork 
	{
		private System.Random generation = new System.Random();

		private static int nodeId;
		private static int innovationNumber;

		readonly int inputCount;
		readonly int outputCount;

		public float[] inputArray;
		public float[] outputArray;

		private List<NodeGene> nodeGenes;
		private List<ConnectionGene> connectionGenes;

		private FitnessEvaluator fitevaluator;
		private Dictionary<int, List<ConnectionGene>> adjacenyList;


		public AgentNeuralNetwork(int inputCount , int outputCount)
		{
			fitevaluator = new FitnessEvaluator ();
			this.inputCount = inputCount;
			this.outputCount = outputCount;

			this.inputArray = new float[inputCount];
			this.outputArray = new float[outputCount];

			nodeId = 0;
			innovationNumber = 0;

			// Initial node list size = Inputs + Ouputs
			this.nodeGenes = new List<NodeGene>();

			// Initial connection list size = Inputs * Outputs
			this.connectionGenes = new List<ConnectionGene>();

			this.adjacenyList = new Dictionary<int, List<ConnectionGene>>();

			// Create the input nodes
			for(int i=0; i<inputCount; i++)
			{
				NodeGene toAdd = new NodeGene(nodeId, NodeType.INPUT);
				this.nodeGenes.Add(toAdd);

				// Instantiate adjacency list for this node
				List<ConnectionGene> newList = new List<ConnectionGene>();
				this.adjacenyList.Add(toAdd.nodeId, newList);

				nodeId++;
			}

			// Create the output nodes
			for(int i=0; i<outputCount; i++)
			{
				NodeGene toAdd = new NodeGene(nodeId, NodeType.OUTPUT);
				this.nodeGenes.Add(toAdd);	

				// Instantiate adjacency list for this node
				List<ConnectionGene> newList = new List<ConnectionGene>();
				this.adjacenyList.Add(toAdd.nodeId, newList);

				nodeId++;
			}

			// Create the connections - 1 for each input to output pair of nodes
			for(int i=0; i<inputCount; i++)
			{
				int fromNode = i;
				for(int j=0; j<outputCount; j++)
				{
					int toNode = j+inputCount;
					double randomWeight = ((double)generation.Next(-100,100))/100.0;
					ConnectionGene toAdd = new ConnectionGene(innovationNumber, fromNode, toNode, randomWeight);
					this.connectionGenes.Add(toAdd);

					// Add to the adjacency list for this node
					List<ConnectionGene> list = this.adjacenyList[toNode];
					list.Add(toAdd);

					innovationNumber++;
				}
			}
		}

		public AgentNeuralNetwork(Player parent1, Player parent2)
		{	
			fitevaluator = new FitnessEvaluator();

			adjacenyList = new Dictionary<int, List<ConnectionGene>>();

			List<NodeGene> nodes1 = parent1.brain.GetNodes();
			List<NodeGene> nodes2 = parent2.brain.GetNodes();
			List<ConnectionGene> connections1 = parent1.brain.GetConnections();
			List<ConnectionGene> connections2 = parent2.brain.GetConnections();

			List<NodeGene> nodeIntersection = new List<NodeGene>();
			List<NodeGene> node1Disjoint = new List<NodeGene>();
			List<NodeGene> node2Disjoint = new List<NodeGene>();
			foreach(NodeGene n1 in nodes1)
			{
				bool found = false;
				foreach(NodeGene n2 in nodes2)
				{
					if(n1.Equals(n2))
					{
						nodeIntersection.Add(new NodeGene(n1));
						this.adjacenyList.Add(n1.nodeId, new List<ConnectionGene>());
						found = true;
					}
				}
				if(!found)
				{
					node1Disjoint.Add(new NodeGene(n1));
				}
			}
			foreach(NodeGene n2 in nodes2)
			{
				bool found = false;
				foreach(NodeGene n1 in nodes1)
				{
					if(n1.Equals(n2))
					{
						found = true;
					}
				}
				if(!found)
				{
					node2Disjoint.Add(new NodeGene(n2));
				}
			}

			// Find the intersection of connections, as well as the disjoints
			List<ConnectionGene> connectionsIntersection = new List<ConnectionGene>();
			List<ConnectionGene> connections1Disjoint = new List<ConnectionGene>();
			List<ConnectionGene> connections2Disjoint = new List<ConnectionGene>();
			foreach(ConnectionGene c1 in connections1)
			{
				bool found = false;
				foreach(ConnectionGene c2 in connections2)
				{
					if(c1.Equals(c2))
					{
						ConnectionGene toAdd = new ConnectionGene(c1);
						connectionsIntersection.Add(toAdd);
						this.adjacenyList[c1.nodeOut].Add(toAdd);

						found = true;
					}
				}
				if(!found)
				{
					connections1Disjoint.Add(new ConnectionGene(c1));
				}
			}
			foreach(ConnectionGene c2 in connections2)
			{
				bool found = false;
				foreach(ConnectionGene c1 in connections1)
				{
					if(c1.Equals(c2))
					{
						found = true;
					}
				}
				if(!found)
				{
					connections2Disjoint.Add(new ConnectionGene(c2));
				}						
			}

			// Make this neural network's connections the intersection + the disjoint according to the fitness function
			nodeGenes = nodeIntersection;
			connectionGenes = connectionsIntersection;

			float eval1 = parent1.getPlayerFitness ();
			float eval2 = parent2.getPlayerFitness ();
			if(eval1 > eval2)
			{
				foreach(NodeGene n1 in node1Disjoint)
				{
					nodeGenes.Add(n1);

					adjacenyList.Add(n1.nodeId, new List<ConnectionGene>());
				}
				foreach(ConnectionGene c1 in connections1Disjoint)
				{
					connectionGenes.Add(c1);

					adjacenyList[c1.nodeOut].Add(c1);
				}
			}
			else if(eval2 > eval1)
			{
				foreach(NodeGene n2 in node2Disjoint)
				{
					nodeGenes.Add(n2);

					this.adjacenyList.Add(n2.nodeId, new List<ConnectionGene>());
				}
				foreach(ConnectionGene c2 in connections2Disjoint)
				{
					connectionGenes.Add(c2);

					adjacenyList[c2.nodeOut].Add(c2);
				}
			}
			else
			{
				// If they are equal -- add the disjoint nodes from both sets
				foreach(NodeGene n1 in node1Disjoint)
				{
					nodeGenes.Add(n1);

					adjacenyList.Add(n1.nodeId, new List<ConnectionGene>());
				}
				foreach(NodeGene n2 in node2Disjoint)
				{
					nodeGenes.Add(n2);

					adjacenyList.Add(n2.nodeId, new List<ConnectionGene>());
				}
				foreach(ConnectionGene c1 in connections1Disjoint)
				{
					connectionGenes.Add(c1);

					adjacenyList[c1.nodeOut].Add(c1);
				}
				foreach(ConnectionGene c2 in connections2Disjoint)
				{
					connectionGenes.Add(c2);

					adjacenyList[c2.nodeOut].Add(c2);
				}
			}

			// Figure out the nodeId count and output count
			foreach(NodeGene n in nodeGenes)
			{
				if(n.type == NodeType.INPUT)
				{
					this.inputCount++;
				}
				if(n.type == NodeType.OUTPUT)
				{
					this.outputCount++;
				}
			}

			this.inputArray = new float[this.inputCount];
			this.outputArray = new float[this.outputCount];	

			// Randomly mutate neural network weights and structure after creating new brain from parent brains
			mutate();				
		}

		// Randomly mutate the network
		public void mutate()
		{
			changeWeights();

			double prob = generation.NextDouble();
			if(prob <= Constants.PROBABILITY_ADD_NODE)
			{
				addNode();
			}

			prob = generation.NextDouble();
			if(prob <= Constants.PROBABILITY_ADD_CONNECTION)
			{
				addConnection();
			}
		}

		public void changeWeights()
		{
			foreach(ConnectionGene c in this.connectionGenes)
			{
				double prob = generation.NextDouble();
				if(prob <= Constants.PROBABILITY_MUTATE_WEIGHT)
				{
					double change = (generation.Next(-1*Constants.AMOUNT_MUTATE_WEIGHT, Constants.AMOUNT_MUTATE_WEIGHT))/100.0;
					if (c.weight+change < 0)
					{
						c.weight = 0;
					}
					else if(c.weight+change > 1)
					{
						c.weight = 1;
					}
					else
					{
						c.weight += change;
					}
				}
			}
		}

		private void addConnection()
		{
			// TODO: add connections to mutate the network
			while(true)
			{
				int index = generation.Next(0, this.nodeGenes.Count);
				NodeGene toConnect = this.nodeGenes[index];

				List<NodeGene> possibilities = new List<NodeGene>();
				foreach(NodeGene n in this.nodeGenes)
				{
					// If I am not looking at the same node AND
					// If I'm a hidden node OR
					// I'm an Input/Output node and the node I'm looking at is not the same type
					if(!n.Equals(toConnect) &&
						(toConnect.type == NodeType.HIDDEN || (toConnect.type != NodeType.HIDDEN && toConnect.type != n.type)))
					{
						possibilities.Add(n);
					}
				}

				if(possibilities.Count == 0)
				{
					continue;
				}

				int nodeIndex = generation.Next(0, possibilities.Count);
				NodeGene toConnect2 = possibilities[nodeIndex];

				NodeGene nodeIn = toConnect;
				NodeGene nodeOut = toConnect2;

				if(nodeOut.type == NodeType.INPUT || nodeIn.type == NodeType.OUTPUT)
				{
					// Swap them so the connection is created in the right direction
					NodeGene temp = nodeIn;
					nodeIn = nodeOut;
					nodeOut = temp;
				}
				double randomWeight = ((double)generation.Next(-100,100))/100.0;						
				ConnectionGene toAdd = new ConnectionGene(innovationNumber++, nodeIn.nodeId, nodeOut.nodeId, randomWeight);
				this.connectionGenes.Add(toAdd);

				// Add to the adjacency list for this node
				List<ConnectionGene> list = this.adjacenyList[toAdd.nodeOut];
				list.Add(toAdd);
				break;						
			}

		}

		private void addNode()
		{
			// Take an existing connection, and split it.
			int index = generation.Next(0, this.connectionGenes.Count);
			ConnectionGene toDisable = this.connectionGenes[index];
			NodeGene toAdd = new NodeGene(nodeId++, NodeType.HIDDEN);

			this.nodeGenes.Add(toAdd);

			this.adjacenyList.Add(toAdd.nodeId, new List<ConnectionGene>());

			ConnectionGene connect1 = new ConnectionGene(innovationNumber++, toDisable.nodeIn, toAdd.nodeId, 1);
			ConnectionGene connect2 = new ConnectionGene(innovationNumber++, toAdd.nodeId, toDisable.nodeOut, toDisable.weight);

			toDisable.enabled = false;
			// Maybe just remove this connection?

			this.connectionGenes.Add(connect1);
			this.connectionGenes.Add(connect2);

			// Add to the adjacency list for this node
			List<ConnectionGene> list = this.adjacenyList[connect1.nodeOut];
			list.Add(connect1);	

			// Add to the adjacency list for this node
			List<ConnectionGene> list2 = this.adjacenyList[connect2.nodeOut];
			list2.Add(connect2);			
		}

		public List<NodeGene> GetNodes()
		{
			return nodeGenes;
		}

		public List<ConnectionGene> GetConnections()
		{
			return connectionGenes;
		}

		public int InputCount {
			get { return inputCount; }
		}

		public int OutputCount {
			get { return outputCount; }
		}

		public float[] InputSignalArray {
			get { return inputArray; }
			set { inputArray = value; }
		}

		public float[] OutputSignalArray {
			get { return outputArray; }
		}

		public void Activate ()
		{					
			// For each output node calculate the output value				
			for (int j =0; j < this.outputCount; j++)
			{
				this.outputArray[j] = Activate(this.nodeGenes[j+this.inputCount].nodeId);
			}
		}

		public float Activate (int nodeId)
		{
			// Base case - did you reach an input node?
			if(nodeId < this.inputCount)
			{
				float input = this.inputArray[nodeId];
				//if(input < -1) input = -1;
				//if(input > 1) input = 1;
				return input;
			}

			// Calculate weighted sum of inputs
			float sum = 0;
			foreach (ConnectionGene c in this.adjacenyList[nodeId])
			{
				sum += ((float)c.weight*Activate(c.nodeIn));

			}
			return sum;
		}

		public double[] getWeights()
		{
			double[] weights = new double[inputCount*outputCount];
			for(int i=0; i<weights.Length; i++)
			{
				weights[i] = this.connectionGenes[i].weight;
			}
			return weights;
		}

		public void DistanceFrom(AgentNeuralNetwork network2, out int disjoint, out int N, out double weightedAverage)
		{	
			List<NodeGene> nodes1 = this.nodeGenes;
			List<NodeGene> nodes2 = network2.GetNodes();
			List<ConnectionGene> connections1 = this.connectionGenes;
			List<ConnectionGene> connections2 = network2.GetConnections();

			List<NodeGene> nodeIntersection = new List<NodeGene>();
			List<NodeGene> node1Disjoint = new List<NodeGene>();
			List<NodeGene> node2Disjoint = new List<NodeGene>();
			foreach(NodeGene n1 in nodes1)
			{
				bool found = false;
				foreach(NodeGene n2 in nodes2)
				{
					if(n1.Equals(n2))
					{
						nodeIntersection.Add(new NodeGene(n1));
						found = true;
					}
				}
				if(!found)
				{
					node1Disjoint.Add(new NodeGene(n1));
				}
			}
			foreach(NodeGene n2 in nodes2)
			{
				bool found = false;
				foreach(NodeGene n1 in nodes1)
				{
					if(n1.Equals(n2))
					{
						found = true;
					}
				}
				if(!found)
				{
					node2Disjoint.Add(new NodeGene(n2));
				}
			}

			// Find the intersection of connections, as well as the disjoints
			List<ConnectionGene> connectionsIntersection = new List<ConnectionGene>();
			List<ConnectionGene> connections1Disjoint = new List<ConnectionGene>();
			List<ConnectionGene> connections2Disjoint = new List<ConnectionGene>();

			weightedAverage = 0;

			foreach(ConnectionGene c1 in connections1)
			{
				bool found = false;
				foreach(ConnectionGene c2 in connections2)
				{
					if(c1.Equals(c2))
					{
						ConnectionGene toAdd = new ConnectionGene(c1);
						connectionsIntersection.Add(toAdd);

						weightedAverage += System.Math.Abs(c1.weight-c2.weight);

						found = true;
					}
				}
				if(!found)
				{
					connections1Disjoint.Add(new ConnectionGene(c1));
				}
			}
			weightedAverage /= connectionsIntersection.Count;
			foreach(ConnectionGene c2 in connections2)
			{
				bool found = false;
				foreach(ConnectionGene c1 in connections1)
				{
					if(c1.Equals(c2))
					{
						found = true;
					}
				}
				if(!found)
				{
					connections2Disjoint.Add(new ConnectionGene(c2));
				}						
			}


			disjoint = connections1Disjoint.Count + connections2Disjoint.Count;
			N = Mathf.Max(nodes1.Count, nodes2.Count);
		}   
			

		public float Evaluate(float[,] board, int score, int clears,float time) {
			return fitevaluator.Evaluate(board,score,clears,time);
		}

	}

}






	


