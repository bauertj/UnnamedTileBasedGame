using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {

	public Button retry;
	public Button quit;

	// Use this for initialization
	void Start () {
		retry.onClick.AddListener (restart);

		quit.onClick.AddListener (stopGame);
	}


	void restart(){
		SceneManager.LoadScene (SceneManager.GetActiveScene().name);
	}

	void stopGame(){
		Application.Quit ();
	}
}