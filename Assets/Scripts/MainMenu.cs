using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public Text scoreText;

	public void Start() {
		scoreText.text = "HighScore: " + PlayerPrefs.GetInt ("score").ToString();
	}

	public void ToGame()
	{
		SceneManager.LoadScene ("Game");
	}

	public void ToQuit()
	{
		Application.Quit ();
	}
}
