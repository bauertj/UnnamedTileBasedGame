using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

	public static bool GameIsPaused = false;
	public GameObject pauseMenuUI;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (GameIsPaused) {
				resume ();
			} else {
				pause ();
			}
		}
	}

	void resume(){
		pauseMenuUI.SetActive (false);
		Time.fixedDeltaTime = 1f;
		GameIsPaused = false;
	}

	void pause(){
		pauseMenuUI.SetActive (true);
		Time.fixedDeltaTime = 0f;
		GameIsPaused = true;
		Button cont = GameObject.Find ("Continue").GetComponent<Button> ();
		cont.onClick.AddListener (continueGame);
	}

	void continueGame(){
		pauseMenuUI.SetActive (false);

	}
}
