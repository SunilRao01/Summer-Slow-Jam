using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	public bool win;

	void Awake () 
	{
		DontDestroyOnLoad(gameObject);
	}


	public void endGame()
	{

	}
}
