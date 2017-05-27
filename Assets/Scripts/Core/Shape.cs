using UnityEngine;
using System.Collections;

public class Shape : MonoBehaviour {

	// turn this property off if you don't want the shape to rotate (Shape O)
	public bool m_canRotate = true;

	// small offset to shift position while in queue
	public Vector3 m_queueOffset;

	// glowing squares with ParticlePlayer components
	GameObject[] m_glowSquareFx;

	// Tag name for glowing squares
	public string m_glowSquareTag;

	void Start()
	{
		// find the glow squares by Tag
		if (m_glowSquareTag != null && m_glowSquareTag != "")
		{
			m_glowSquareFx = GameObject.FindGameObjectsWithTag(m_glowSquareTag);
		}
			
	}

	// play effect when the Shape lands
	public void LandShapeFX()
	{
		// counter
		int i = 0;

		// loop through each empty square sprite 
		foreach (Transform child in gameObject.transform)
		{
			// if we find a corresponding glow square
			if (m_glowSquareFx[i])
			{

				// move the glow square to the empty square
				m_glowSquareFx[i].transform.position = child.position - new Vector3(0,0,.1f);

				// play the particlePlayer component if we have one
				ParticlePlayer particlePlayer = m_glowSquareFx[i].GetComponent<ParticlePlayer>();
				if (particlePlayer)
				{
					particlePlayer.Play();
				}

				//increment the counter
				i++;

			}
		}
	}


	// general move method
	void Move(Vector3 moveDirection)
	{
		transform.position += moveDirection;
	}


	//public methods for moving left, right, up and down, respectively
	public void MoveLeft()
	{
		Move(new Vector3(-1, 0, 0));
	}

	public void MoveRight()
	{
		Move(new Vector3(1, 0, 0));
	}

	public void MoveUp()
	{
		Move(new Vector3(0, 1, 0));
	}

	public void MoveDown()
	{
		Move(new Vector3(0, -1, 0));
	}


	//public methods for rotating right and left
	public void RotateRight()
	{
		if (m_canRotate)
			transform.Rotate(0, 0, -90);
	}
	public void RotateLeft()
	{
		if (m_canRotate)
			transform.Rotate(0, 0, 90);
	}

	public void RotateClockwise(bool clockwise)
	{
		if (clockwise)
		{
			RotateRight();
		}
		else
		{
			RotateLeft();
		}
	}
		
}
