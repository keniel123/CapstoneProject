using System.Collections;
using System.Collections.Generic;



public enum NodeType
	{
		INPUT,
		OUTPUT,
		HIDDEN
	}


public class NodeGene {
	public int nodeId;
	public NodeType type;



	public NodeGene(int id, NodeType type)
	{
		this.nodeId = id;
		this.type = type;

	}

	public NodeGene(NodeGene nodeToCopy)
	{
		this.nodeId = nodeToCopy.nodeId;
		this.type = nodeToCopy.type;

	}

	public override bool Equals(System.Object obj)
	{
		NodeGene n = (NodeGene)obj;
		if (this.nodeId == n.nodeId && this.type.Equals (n.type)) 
		{

			return true;
		}
		return false;

	}

<<<<<<< HEAD
	public override string ToString()
	{
		string result;

		// [i] = input node , [o] = output node, [h] = hidden node
		if (this.type.Equals(NodeType.INPUT))
		{
			result = "[I] ID: " + this.nodeId.ToString();
		}
		else if (this.type.Equals(NodeType.OUTPUT))
		{
			result = "[O] ID: " + this.nodeId.ToString();
		}
		else
		{
			result = "[H] ID: " + this.nodeId.ToString();
		}

		return result; 
	}

=======
>>>>>>> 708281a44cab52ec2f4339869f1db3b198b1d9ad


}
