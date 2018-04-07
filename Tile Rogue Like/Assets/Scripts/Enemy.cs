using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * This Enemy class will have the basic attributes of an enemy.
 * I.E. the enemy's health, damage it can do, whether it is passive or aggressive, speed, etc.
 * 
 * When more specific enemies are introduced, different classes will be created which are 
 * going to inherit the attributes of the enemy.
 * 
 * These more specific classes can add more unique skills and attributes to the particular enemy.
 * */
public class Enemy : MonoBehaviour{

	public GameObject target;

	Transform enemyPos;

	public int maxHealth;
	public int damage;
	public int currentHealth;

	// string which signifies the health of the enemy without explicitly stating its health
	string wellness;

	// game object that stores info about enemy
	public GameObject enemyInfo;

	// makes a copy of enemyInfo so not to mess with the prefab
	GameObject enemyInfoTemp;



	Text enemyText, enemyDescription;

	// Use this for initialization
	void Start () {
		enemyPos = this.gameObject.transform;

		currentHealth = maxHealth;

		enemyInfoTemp = Instantiate (enemyInfo, GameObject.Find ("Canvas").transform);
		enemyInfoTemp.SetActive (false);

		wellness = "Perfect";
		enemyText = enemyInfoTemp.transform.Find("EnemyInfo").GetComponent<Text> ();
		enemyText.text = wellness;

		enemyDescription = enemyInfoTemp.transform.Find ("EnemyDescription").GetComponent<Text> ();
	}

	void Update(){
		if (currentHealth < maxHealth && currentHealth >= (maxHealth/2)) {
			wellness = "Fine";
			enemyText.text = wellness;
		}
		if (currentHealth < (maxHealth/2) && currentHealth >= (maxHealth/4)) {
			wellness = "Injured";
			enemyText.text = wellness;
		}
		if (currentHealth < (maxHealth/4) && currentHealth >= 0) {
			wellness = "Dying";
			enemyText.text = wellness;
		}
		if (currentHealth <= 0) {
			Destroy (this.gameObject);
		}
	}

	// moves the enemy towards the player given a vector
	public void moveTowardsPlayer(Vector3 pos){
		enemyPos.Translate (pos);
	}

	// does damage to the enemy
	public void doDamage(int damage){
		currentHealth -= damage;
	}

	// will show the enemy information panel when hovering over the enemy
	void OnMouseEnter(){
		enemyInfoTemp.SetActive (true);
		enemyInfoTemp.transform.position = Input.mousePosition;
	}



	void OnMouseExit(){
		enemyInfoTemp.SetActive(false);
	}

	// calls this function when enemy is killed
	void OnDestroy(){
		// destroy enemyPanel info
		Destroy(enemyInfoTemp);
	}

	public void description(string newDescription){
		enemyDescription.text = newDescription;
	}
}
