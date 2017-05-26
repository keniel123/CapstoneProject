using UnityEngine;
using System.Collections;


public class Board : MonoBehaviour {
	
	// a SpriteRenderer that will be instantiated in a grid to create our board
	public Transform m_emptySprite;

	// the height of the board
	public int m_height = 30;

	// width of the board
	public int m_width = 10;

	// number of rows where we won't have grid lines at the top
	public int m_header = 8;

	// store inactive shapes here
	public Transform[,] m_grid;



	public int m_completedRows = 0;

	public ParticlePlayer[]  m_rowGlowFx = new ParticlePlayer[4];


	void Awake()
	{
		m_grid = new Transform[m_width,m_height];
	}

	void Start () {
		DrawEmptyCells();
	}
	
	// Update is called once per frame
	void Update () {

	}

	bool IsWithinBoard(int x, int y)
	{
		return (x >= 0 && x < m_width && y >= 0);

	}

	public bool IsOccupied(int x, int y, Shape shape)
	{

		return (m_grid[x,y] !=null && m_grid[x,y].parent != shape.transform);
	}

	public bool IsValidPosition(Shape shape)
	{
		//Debug.Log (shape.tag);
		foreach (Transform child in shape.transform)
		{
			Vector2 pos = Vectorf.Round(child.position);

			if (!IsWithinBoard((int) pos.x, (int) pos.y))
			{
				return false;
			}

			if (IsOccupied((int) pos.x, (int) pos.y, shape))
			{
				return false;
			}
		}
		return true;
	}

	public bool IsValidPosition(Shape active, Cell to, int rotation)
	{
		Vector2 midposition = new Vector2(to.x,to.y); 
		if (active.gameObject.tag == "piece-I") {
			if (rotation == 0 || rotation == 2) {//original state 
				if (!IsWithinBoard((int) midposition.x , (int) midposition.y) || IsOccupied ((int)midposition.x, (int)midposition.y, active)) {
					return false;
				} 
				else if (!IsWithinBoard((int) midposition.x , (int) midposition.y - 1) || IsOccupied ((int)midposition.x, (int)midposition.y - 1, active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x, (int) midposition.y + 1) || IsOccupied ((int)midposition.x, (int)midposition.y + 1, active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x , (int) midposition.y + 2) || IsOccupied ((int)midposition.x, (int)midposition.y + 2, active)) {
					return false;
				}
			} else  {//any other rotation 
				//active.RotateRight ();
				if (!IsWithinBoard((int) midposition.x , (int) midposition.y) || IsOccupied ((int)midposition.x, (int)midposition.y, active)) {
					return false;
				} 
				else if (!IsWithinBoard((int) midposition.x + 1, (int) midposition.y) || IsOccupied ((int)midposition.x + 1, (int)midposition.y, active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x - 1, (int) midposition.y) || IsOccupied ((int)midposition.x -1 , (int)midposition.y , active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x + 2, (int) midposition.y) || IsOccupied ((int)midposition.x + 2, (int)midposition.y , active)) {
					return false;
				}

			}
			return true;
		}
		else if (active.gameObject.tag == "piece-Z") {
			if (rotation == 0 || rotation == 2) {
				if (!IsWithinBoard((int) midposition.x , (int) midposition.y) || IsOccupied ((int)midposition.x, (int)midposition.y, active)) {
					return false;
				} 
				else if (!IsWithinBoard((int) midposition.x + 1 , (int) midposition.y ) || IsOccupied ((int)midposition.x + 1, (int)midposition.y, active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x, (int) midposition.y + 1) || IsOccupied ((int)midposition.x, (int)midposition.y + 1, active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x - 1, (int) midposition.y + 1) || IsOccupied ((int)midposition.x - 1, (int)midposition.y + 1, active)) {
					return false;
				}
			} else {
				//active.RotateRight ();
				if (!IsWithinBoard((int) midposition.x , (int) midposition.y) || IsOccupied ((int)midposition.x, (int)midposition.y, active)) {
					return false;
				} 
				else if (!IsWithinBoard((int) midposition.x + 1, (int) midposition.y + 1) || IsOccupied ((int)midposition.x + 1, (int)midposition.y + 1, active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x + 1, (int) midposition.y) || IsOccupied ((int)midposition.x  + 1 , (int)midposition.y , active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x , (int) midposition.y - 1) || IsOccupied ((int)midposition.x , (int)midposition.y - 1, active)) {
					return false;
				}

			}
			return true;
		}
		else if (active.gameObject.tag == "piece-T") {
			if (rotation == 0) {
				if (!IsWithinBoard((int) midposition.x , (int) midposition.y) || IsOccupied ((int)midposition.x, (int)midposition.y, active)) {
					return false;
				} 
				else if (!IsWithinBoard((int) midposition.x , (int) midposition.y - 1) || IsOccupied ((int)midposition.x, (int)midposition.y - 1, active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x + 1, (int) midposition.y) || IsOccupied ((int)midposition.x + 1, (int)midposition.y , active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x - 1, (int) midposition.y ) || IsOccupied ((int)midposition.x - 1, (int)midposition.y , active)) {
					return false;
				}
			} else if (rotation == 1) {
				//active.RotateRight ();
				if (!IsWithinBoard((int) midposition.x , (int) midposition.y) || IsOccupied ((int)midposition.x, (int)midposition.y, active)) {
					return false;
				} 
				else if (!IsWithinBoard((int) midposition.x - 1, (int) midposition.y) || IsOccupied ((int)midposition.x - 1, (int)midposition.y, active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x , (int) midposition.y + 1) || IsOccupied ((int)midposition.x  , (int)midposition.y + 1 ,active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x , (int) midposition.y - 1) || IsOccupied ((int)midposition.x , (int)midposition.y - 1,active)) {
					return false;
				}

			}
			else if (rotation == 2) {
				//active.RotateRight ();
				if (!IsWithinBoard((int) midposition.x , (int) midposition.y) || IsOccupied ((int)midposition.x, (int)midposition.y, active)) {
					return false;
				} 
				else if (!IsWithinBoard((int) midposition.x - 1, (int) midposition.y) || IsOccupied ((int)midposition.x - 1, (int)midposition.y, active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x + 1 , (int) midposition.y ) || IsOccupied ((int)midposition.x + 1 , (int)midposition.y ,active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x , (int) midposition.y + 1) || IsOccupied ((int)midposition.x , (int)midposition.y + 1,active)) {
					return false;
				}

			}
			else if (rotation == 3) {
				//active.RotateRight ();
				if (!IsWithinBoard((int) midposition.x , (int) midposition.y) || IsOccupied ((int)midposition.x, (int)midposition.y, active)) {
					return false;
				} 
				else if (!IsWithinBoard((int) midposition.x + 1, (int) midposition.y) || IsOccupied ((int)midposition.x + 1, (int)midposition.y, active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x , (int) midposition.y + 1) || IsOccupied ((int)midposition.x  , (int)midposition.y + 1 ,active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x , (int) midposition.y - 1) || IsOccupied ((int)midposition.x , (int)midposition.y - 1,active)) {
					return false;
				}

			}
			return true;
		}
		else if (active.gameObject.tag == "piece-S") {
			if (rotation == 0) {
				if (!IsWithinBoard((int) midposition.x , (int) midposition.y) || IsOccupied ((int)midposition.x, (int)midposition.y, active)) {
					return false;
				} 
				else if (!IsWithinBoard((int) midposition.x , (int) midposition.y - 1) || IsOccupied ((int)midposition.x, (int)midposition.y - 1, active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x - 1, (int) midposition.y - 1) || IsOccupied ((int)midposition.x - 1, (int)midposition.y - 1 , active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x + 1, (int) midposition.y ) || IsOccupied ((int)midposition.x + 1, (int)midposition.y , active)) {
					return false;
				}
			} else if (rotation == 1 || rotation == 3) {
				//active.RotateRight ();
				if (!IsWithinBoard((int) midposition.x , (int) midposition.y) || IsOccupied ((int)midposition.x, (int)midposition.y, active)) {
					return false;
				} 
				else if (!IsWithinBoard((int) midposition.x , (int) midposition.y - 1) || IsOccupied ((int)midposition.x , (int)midposition.y - 1, active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x - 1, (int) midposition.y) || IsOccupied ((int)midposition.x -1 , (int)midposition.y, active )) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x - 1 , (int) midposition.y + 1) || IsOccupied ((int)midposition.x - 1, (int)midposition.y + 1 , active)) {
					return false;
				}

			}
			else if (rotation == 2) {
				//active.RotateRight ();
				if (!IsWithinBoard((int) midposition.x , (int) midposition.y) || IsOccupied ((int)midposition.x, (int)midposition.y, active)) {
					return false;
				} 
				else if (!IsWithinBoard((int) midposition.x , (int) midposition.y - 1) || IsOccupied ((int)midposition.x , (int)midposition.y - 1, active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x - 1, (int) midposition.y) || IsOccupied ((int)midposition.x -1 , (int)midposition.y, active )) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x - 1 , (int) midposition.y + 1) || IsOccupied ((int)midposition.x - 1, (int)midposition.y + 1 , active)) {
					return false;
				}

			}

			return true;
		}
		else if (active.gameObject.tag == "piece-O") {
			//Debug.Log ("in O >>>>> ");
			if (rotation == 0 || rotation == 1 || rotation == 2 || rotation == 3) {
				if (!IsWithinBoard((int) midposition.x , (int) midposition.y) || IsOccupied ((int)midposition.x, (int)midposition.y, active)) {
					return false;
				} 
				else if (!IsWithinBoard ((int)to.x + 1, (int)to.y) || IsOccupied ((int)to.x + 1, (int)to.y, active)) {
					return false;
				} else if (!IsWithinBoard ((int)to.x + 1, (int)to.y - 1) || IsOccupied ((int)to.x + 1, (int)to.y - 1, active)) {
					return false;
				} else if (!IsWithinBoard ((int)to.x, (int)to.y - 1) || IsOccupied ((int)to.x, (int)to.y - 1, active)) {
					return false;
				} else {
					return true;
				}
			} else {
				return true;
			}

		}
		else if (active.gameObject.tag == "piece-L") {
			if (rotation == 0) {
				if (!IsWithinBoard((int) midposition.x , (int) midposition.y) || IsOccupied ((int)midposition.x, (int)midposition.y, active)) {
					return false;
				} 
				else if (!IsWithinBoard((int) midposition.x , (int) midposition.y + 1) || IsOccupied ((int)midposition.x, (int)midposition.y + 1 , active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x, (int) midposition.y - 1) || IsOccupied ((int)midposition.x, (int)midposition.y - 1, active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x + 1 , (int) midposition.y - 1) || IsOccupied ((int)midposition.x + 1, (int)midposition.y - 1, active)) {
					return false;
				}
			} else if (rotation == 1) {
				//active.RotateRight ();
				if (!IsWithinBoard((int) midposition.x , (int) midposition.y) || IsOccupied ((int)midposition.x, (int)midposition.y, active)) {
					return false;
				} 
				else if (!IsWithinBoard((int) midposition.x - 1, (int) midposition.y) || IsOccupied ((int)midposition.x - 1, (int)midposition.y, active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x + 1, (int) midposition.y) || IsOccupied ((int)midposition.x + 1 , (int)midposition.y, active )) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x - 1, (int) midposition.y - 1) || IsOccupied ((int)midposition.x - 1, (int)midposition.y - 1 , active)) {
					return false;
				}

			}
			else if (rotation == 2) {
				//active.RotateRight ();
				if (!IsWithinBoard((int) midposition.x , (int) midposition.y) || IsOccupied ((int)midposition.x, (int)midposition.y, active)) {
					return false;
				} 
				else if (!IsWithinBoard((int) midposition.x - 1, (int) midposition.y + 1) || IsOccupied ((int)midposition.x - 1, (int)midposition.y + 1, active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x , (int) midposition.y + 1) || IsOccupied ((int)midposition.x , (int)midposition.y + 1, active )) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x , (int) midposition.y - 1) || IsOccupied ((int)midposition.x , (int)midposition.y - 1 , active)) {
					return false;
				}

			}
			else if (rotation == 3) {
				//active.RotateRight ();
				if (!IsWithinBoard((int) midposition.x , (int) midposition.y) || IsOccupied ((int)midposition.x, (int)midposition.y, active)) {
					return false;
				} 
				else if (!IsWithinBoard((int) midposition.x - 1, (int) midposition.y) || IsOccupied ((int)midposition.x - 1, (int)midposition.y, active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x + 1, (int) midposition.y) || IsOccupied ((int)midposition.x + 1 , (int)midposition.y, active )) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x + 1, (int) midposition.y + 1) || IsOccupied ((int)midposition.x + 1, (int)midposition.y + 1 , active)) {
					return false;
				}

			}
			return true;
		}
		else if (active.gameObject.tag == "piece-J") {
			if (rotation == 0) {
				if (!IsWithinBoard((int) midposition.x , (int) midposition.y) || IsOccupied ((int)midposition.x, (int)midposition.y, active)) {
					return false;
				} 
				else if (!IsWithinBoard((int) midposition.x , (int) midposition.y + 1) || IsOccupied ((int)midposition.x, (int)midposition.y + 1 , active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x, (int) midposition.y - 1) || IsOccupied ((int)midposition.x, (int)midposition.y - 1, active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x - 1 , (int) midposition.y - 1) || IsOccupied ((int)midposition.x - 1, (int)midposition.y - 1, active)) {
					return false;
				}
			} else if (rotation == 1) {
				//active.RotateRight ();
				if (!IsWithinBoard((int) midposition.x , (int) midposition.y) || IsOccupied ((int)midposition.x, (int)midposition.y, active)) {
					return false;
				} 
				else if (!IsWithinBoard((int) midposition.x - 1, (int) midposition.y) || IsOccupied ((int)midposition.x - 1, (int)midposition.y, active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x + 1, (int) midposition.y) || IsOccupied ((int)midposition.x + 1 , (int)midposition.y, active )) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x - 1, (int) midposition.y + 1) || IsOccupied ((int)midposition.x - 1, (int)midposition.y + 1 , active)) {
					return false;
				}

			}
			else if (rotation == 2) {
				//active.RotateRight ();
				if (!IsWithinBoard((int) midposition.x , (int) midposition.y) || IsOccupied ((int)midposition.x, (int)midposition.y, active)) {
					return false;
				} 
				else if (!IsWithinBoard((int) midposition.x , (int) midposition.y + 1) || IsOccupied ((int)midposition.x , (int)midposition.y + 1, active )) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x , (int) midposition.y - 1) || IsOccupied ((int)midposition.x, (int)midposition.y - 1 , active )) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x + 1, (int) midposition.y + 1) || IsOccupied ((int)midposition.x + 1, (int)midposition.y + 1 , active)) {
					return false;
				}

			}
			else if (rotation == 3) {
				//active.RotateRight ();
				if (!IsWithinBoard((int) midposition.x , (int) midposition.y) || IsOccupied ((int)midposition.x, (int)midposition.y, active)) {
					return false;
				} 
				else if (!IsWithinBoard((int) midposition.x - 1, (int) midposition.y) || IsOccupied ((int)midposition.x - 1, (int)midposition.y, active)) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x + 1, (int) midposition.y) || IsOccupied ((int)midposition.x + 1 , (int)midposition.y, active )) {
					return false;
				} else if (!IsWithinBoard((int) midposition.x + 1, (int) midposition.y - 1) || IsOccupied ((int)midposition.x + 1, (int)midposition.y - 1 , active)) {
					return false;
				}

			}
			return true;
		}
		return false;

	}

	public void setClears(int clear)
	{
		m_completedRows = clear;

	}
	// draw our empty board with our empty sprite object
	void DrawEmptyCells() {
		if (m_emptySprite)
		{
			for (int y = 0; y < m_height - m_header; y++)
			{
				for (int x = 0; x < m_width; x++) 
				{
					Transform clone;
					clone = Instantiate(m_emptySprite, new Vector3(x, y, 0), Quaternion.identity) as Transform;

					// names the empty squares for organizational purposes
					clone.name = "Board Space ( x = " + x.ToString() +  " , y =" + y.ToString() + " )"; 

					// parents all of the empty squares to the Board object
					clone.transform.parent = transform;
				}
			}
		}
	}

	public void StoreShapeInGrid(Shape shape)
	{
		if (shape == null)
		{
			return;
		}

		foreach (Transform child in shape.transform)
		{
			Vector2 pos = Vectorf.Round(child.position);
			m_grid[(int) pos.x, (int) pos.y] = child;
		}
	}
		
	bool IsComplete(int y)
	{
		for (int x = 0; x < m_width; ++x)
		{
			if (m_grid[x,y] == null)
			{
				return false;
			}

		}
		return true;
	}

	public void ClearRow(int y)
	{
		for (int x = 0; x < m_width; ++x)
		{
			if (m_grid[x,y] !=null)
			{
				Destroy(m_grid[x,y].gameObject);

			}
			m_grid[x,y] = null;

		}

	}

	void ShiftOneRowDown(int y)
	{

		for (int x = 0; x < m_width; ++x)
		{
			if (m_grid[x,y] !=null)
			{
				m_grid[x, y-1] = m_grid[x,y];
				m_grid[x,y] = null;
				m_grid[x, y-1].position += new Vector3(0,-1,0);
			}
		}
	}



	void ShiftRowsDown(int startY)
	{
		for (int i = startY; i < m_height; ++i)
		{
			ShiftOneRowDown(i);
		}
	}

	public IEnumerator ClearAllRows()
	{
		m_completedRows = 0;

		for (int y = 0; y < m_height; ++y)
		{
			if (IsComplete(y)) 
			{
				ClearRowFX(m_completedRows,y);
				m_completedRows++;
			}
		}
		yield return new WaitForSeconds(0.3f);

		for (int y = 0; y < m_height; ++y)
		{
			if (IsComplete(y)) 
			{
				ClearRow(y);
				ShiftRowsDown(y+1);
				yield return new WaitForSeconds(0.25f);
				y--;
			}

		}
	}

	public bool IsOverLimit(Shape shape)
	{
		foreach (Transform child in shape.transform) 
		{
			if (child.transform.position.y >= m_height - m_header)
			{
				return true;
			}
		}
		return false;
	}

	void ClearRowFX(int idx, int y)
	{

		if (m_rowGlowFx[idx])
		{
			m_rowGlowFx[idx].transform.position = new Vector3 (0, y, -1.1f);
			m_rowGlowFx[idx].Play();
		}

			
	}






}
