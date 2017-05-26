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



}
