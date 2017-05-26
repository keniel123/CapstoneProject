using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Represent a grid's cell
/// </summary>
public struct Cell
{
	public int 	x;
	public int 	y;
	public Board parent;


	public Cell(int _x, int _y, Board _parent)
	{
		x 		= _x;
		y 		= _y;
		parent 	= _parent;

		if (parent == null)
			Debug.LogError ("Parent is null");
	}

	public List<Cell> GetNeighbours()
	{
		if (parent == null)
		{
			Debug.LogError ("Cell's parent is null");
			return new List<Cell> ();
		}

		int width 	= parent.m_width;
		int height 	= parent.m_height;
		List<Cell> result = new List<Cell> ();

		Cell right 	= new Cell (x + 1, y, parent);
		Cell left	= new Cell (x - 1, y, parent);
		Cell down	= new Cell (x, y - 1, parent);

		if (x + 1 < width)
			result.Add (right);

		if (x - 1 >= 0)
			result.Add (left);

		if (y - 1 >= 0)
			result.Add (down);

		return result;
	}

	public float ManhattanDistance(Cell _to)
	{
		return _to.x - x + _to.y - y;
	}

	public float DistanceSquared(Cell _to)
	{
		int deltaX = _to.x - x;
		int deltaY = _to.y - y;

		return deltaX * deltaX + deltaY * deltaY;
	}

	public bool IsInsideGrid()
	{
		return x >= 0 && x < parent.m_grid.GetLength(0) && y >= 0 && y < parent.m_grid.GetLength(1);
	}

	public bool Empty()
	{
		return parent.m_grid [x, y] == null;
	}



	public override string ToString()
	{
		return string.Format ("Cell ({0}, {1}), Parent: {2}, HashCode: {3}", x, y, (parent == null)? string.Empty : parent.name, GetHashCode());
	}

	public override int GetHashCode()
	{
		int hCode = -1;

		if (parent != null)
		{
			hCode = x * parent.m_width + y;
		}

		return hCode.GetHashCode();
	}

	public static bool operator ==(Cell _p1, Cell _p2) 
	{
		return (_p1.parent == _p2.parent) && (_p1.x == _p2.x) && (_p1.y == _p2.y); 
	}

	public static bool operator !=(Cell _p1, Cell _p2) 
	{
		return !(_p1 == _p2);
	}

	public static Cell operator +(Cell _cell1, Cell _cell2)
	{
		if (_cell1.parent != _cell2.parent)
			throw new ArgumentException ("Operands are from different grids");
		
		return new Cell (_cell1.x + _cell2.x, _cell1.y + _cell2.y, _cell1.parent);
	}

	public static Cell operator -(Cell _cell1, Cell _cell2)
	{
		if (_cell1.parent != _cell2.parent)
			throw new ArgumentException ("Operands are from different grids");

		return new Cell (_cell1.x - _cell2.x, _cell1.y - _cell2.y, _cell1.parent);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Cell))
		{
			return false;
		}

		Cell cell = (Cell)obj;
		return cell == this;
	}
}