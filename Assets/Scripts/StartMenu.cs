using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour 
{
	public Text cursor;

	private Vector2 topPosition;
	private Vector2 bottomPostion;
	private bool onTop = true;

	void Awake()
	{
		topPosition = cursor.transform.localPosition;
		bottomPostion = topPosition;
		bottomPostion.y -= 50;
	}

	void Update()
	{
		if (Input.GetAxis("Vertical") == 1 || Input.GetAxis("Vertical_2") == 1)
		{
			cursor.transform.localPosition = topPosition;
			onTop = true;
		}
		else if (Input.GetAxis("Vertical") == -1 || Input.GetAxis("Vertical_2") == -1)
		{
			cursor.transform.localPosition = bottomPostion;
			onTop = false;
		}

		if (Input.GetButtonDown("Seperate"))
		{
			if (onTop)
			{
				onStart();
			}
			else
			{
				onQuit();
			}

		}
	}

	public void onStart()
	{
		Application.LoadLevel(1);
	}

	public void onQuit()
	{
		Application.Quit();
	}
}
