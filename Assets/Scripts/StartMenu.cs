using UnityEngine;
using System.Collections;

public class StartMenu : MonoBehaviour 
{
	public void onStart()
	{
		Application.LoadLevel(1);
	}

	public void onQuit()
	{
		Application.Quit();
	}
}
