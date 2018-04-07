using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedSquare : MonoBehaviour {

	private Enemy enemy;

	void Start(){
		enemy = this.gameObject.GetComponent<Enemy> ();

		enemy.description ("This  is  just  an  average  red  sqaure.");
	}

}
