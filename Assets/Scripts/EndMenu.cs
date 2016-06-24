using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndMenu : MonoBehaviour 
{
	public Text cursor;
	
	private Vector2 topPosition;
	private Vector2 bottomPostion;
	private bool onTop = true;

	private GameManager gameManager;
	private Text gameStatusText;

	void Start () 
	{
		topPosition = cursor.transform.localPosition;
		bottomPostion = topPosition;
		bottomPostion.y -= 50;

		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		gameStatusText = GameObject.Find("GameStatusText").GetComponent<Text>();

		updateLabels();
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
				onPlayAgain();
			}
			else
			{
				onQuit();
			}
			
		}
	}

	void updateLabels()
	{
		if (gameManager.win)
		{
			gameStatusText.text = "You won!";
		}
		else
		{
			gameStatusText.text = "You lost!";
		}
	}

	public void onPlayAgain()
	{
		Application.LoadLevel(1);
	}

	public void onQuit()
	{
		Application.Quit();
	}
}
