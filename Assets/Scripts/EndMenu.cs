using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndMenu : MonoBehaviour 
{
	private GameManager gameManager;
	private Text gameStatusText;

	void Start () 
	{
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		gameStatusText = GameObject.Find("GameStatusText").GetComponent<Text>();

		updateLabels();
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
