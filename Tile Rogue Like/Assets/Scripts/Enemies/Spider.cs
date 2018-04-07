using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour {

	private Enemy enemy;

	void Start(){
		enemy = this.gameObject.GetComponent<Enemy> ();

		enemy.description ("This  spider  just  wants  a  little  bite.");
	}
}
