using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	// the players max health
	public int maxHealth;
	//public int damage;
	public int arrowDamage;

	// chance to hit
	public int chanceToHit;

	// stats
	private int strength = 10;
	private int dexterity = 10;
	private int constitution = 10;
	private int intelligence = 10;
	private int wisdom = 10;
	private int charisma = 10;

	// dice to roll
	private int numberOfDice = 1;
	private int diceTotalValue = 2;

	// armor class
	private int armor = 10;

	// chance to block
	public int chanceToBlock;

	// the speed that the player walks
	public int speed;

	private string damage = "1d2";

	public GameObject healthBar;
	int currentHealth;

	// text that displays player's health
	Text healthAmount;

	// transform that keeps track of player position
	Transform playerPos;

	// gameobject containing all data for the player
	public GameObject playerPanel;

	public GameObject arrowSlot;
	// text which displays how much damage the player does
	Text playerDamage, playerHitChance, playerBlockChance, strengthText, dexterityText, constitutionText, intelligenceText, wisdomText, charismaText;

	// gameobjects that will represent different pickups on map
	GameObject sword, shield, bow, arrow;

	private Inventory inventory;

	int arrowAmount;
	Text arrowText;

	//int amountOfTurns;

	void Start () {
		inventory = GameObject.Find ("Inventory").GetComponent<Inventory> ();

		arrowText = GameObject.Find ("AmmoAmount").GetComponent<Text> ();

		healthAmount = healthBar.GetComponentInChildren<Text> ();
		healthAmount.text = maxHealth.ToString();

		playerPos = this.transform;

		playerDamage = playerPanel.transform.Find ("DamageText").GetComponent<Text>();
		playerHitChance = playerPanel.transform.Find ("ChanceToHitText").GetComponent<Text> ();
		playerBlockChance = playerPanel.transform.Find ("ChanceToBlockText").GetComponent<Text> ();

		// stat texts
		strengthText = playerPanel.transform.Find("StrengthText").GetComponent<Text>();
		strengthText.text = "Strength: " + strength;

		dexterityText = playerPanel.transform.Find("DexterityText").GetComponent<Text>();
		dexterityText.text = "Dexterity: " + dexterity;

		constitutionText = playerPanel.transform.Find("ConstitutionText").GetComponent<Text>();
		constitutionText.text = "Constitution: " + constitution;

		intelligenceText = playerPanel.transform.Find("IntelligenceText").GetComponent<Text>();
		intelligenceText.text = "Intelligence: " + intelligence;

		wisdomText = playerPanel.transform.Find("WisdomText").GetComponent<Text>();
		wisdomText.text = "Wisdom: " + wisdom;

		charismaText = playerPanel.transform.Find("CharismaText").GetComponent<Text>();
		charismaText.text = "Charisma: " + charisma;

		currentHealth = maxHealth;

		//amountOfTurns = GameObject.Find ("Map").GetComponent<TileMap> ().amountOfTurns;

		updateText ();
		findObjects ();
	}

	void Update(){
		//amountOfTurns = GameObject.Find ("Map").GetComponent<TileMap> ().amountOfTurns;

		if (sword != null) {
			if (playerPos.position.x == sword.transform.position.x && playerPos.position.y == sword.transform.position.y) {
				Destroy (sword);
				//damage += 20;
				inventory.addItem (0);

				updateText ();
			}
		}
		if (arrow != null) {
			if (playerPos.position.x == arrow.transform.position.x && playerPos.position.y == arrow.transform.position.y) {
				Destroy (arrow);
				inventory.addItem (1);
			}
		}
			

		if (arrowSlot.transform.Find ("Arrow")) {
			arrowAmount = arrowSlot.transform.Find ("Arrow").GetComponent<ItemData> ().amount;
			arrowSlot.transform.Find ("Arrow").Find ("StackAmount").GetComponent<Text> ().text = arrowAmount.ToString ();
			updateText ();
		} else {
			arrowAmount = 0;
			updateText ();
		}
		
	}

	public void equip(Item item){
		string itemType = item.ItemType;

		strength += item.Strength;
		dexterity += item.Dexterity;
		constitution += item.Constitution;
		intelligence += item.Intelligence;
		wisdom += item.Wisdom;
		charisma += item.Charisma;

		armor += item.Armor;

		if(item.ItemType.Equals("WeaponSlot")){
			numberOfDice = item.NumberOfDice;
			diceTotalValue = item.DiceTotalValue;
			damage = item.Damage;
		}



		updateText ();

	}
		


	public void unequip(Item item){

		strength -= item.Strength;

		if (item.ItemType.Equals ("WeaponSlot")) {
			numberOfDice = 1;
			diceTotalValue = 2;
			damage = "1d2";
		}

		updateText ();

	}

	/*
	 * this will adjust the players health when taking damage
	 * */
	public int takeDamage(int damage){
		System.Random rand = new System.Random ();
		int randomNumber = rand.Next (1, 100);
		if (randomNumber < chanceToBlock) {
			currentHealth -= damage;
			healthBar.GetComponent<Image> ().fillAmount = ((float)currentHealth / (float)maxHealth);
			healthAmount.text = currentHealth.ToString ();
			return damage;
		}
		return 0;
	}
	/*
	 * this will regain player health given the amount of health to regain
	 * */
	public void regainHealth(int _health){
		if (currentHealth + _health > maxHealth) {
			healthBar.GetComponent<Image> ().fillAmount = maxHealth * .01f;
			currentHealth = maxHealth;
			healthAmount.text = currentHealth.ToString ();
		} else {
			currentHealth += _health;
			healthBar.GetComponent<Image> ().fillAmount = ((float)currentHealth / (float)maxHealth);
			healthAmount.text = currentHealth.ToString ();
		}
	}

	/*
	 * Will attempt to attack the enemy based off hit chance 
	*/
	public int doDamage(){
		System.Random rand = new System.Random ();

		int damage = 0;

		for (int i = 0; i < numberOfDice; i++) {
			int randomDamage = rand.Next (1, diceTotalValue + 1);
			damage += randomDamage;
		}

		int randomNumber = rand.Next (1, 100);
		if (randomNumber < chanceToHit) {
			Debug.Log ("Hit! Rolling " + numberOfDice + "d" + diceTotalValue + " for a total of " + damage + " damage.");
			return damage;
		}
		return 0;
	}
		

	/*
	 * returns true if the player is still alive
	 * */
	public bool alive(){
		if (currentHealth <= 0) {
			return false;
		}
		return true;
	}


	/*
	 * updates status text of player menu
	 */
	void updateText(){
		playerDamage.text = "Damage: " + damage;
		arrowText.text = "X " + arrowAmount;
		playerHitChance.text = "Chance to hit: " + chanceToHit + "%";
		playerBlockChance.text = "Chance to block: " + chanceToBlock + "%";

		strengthText.text = "Strength: " + strength;
		dexterityText.text = "Dexterity: " + dexterity;
		constitutionText.text = "Constitution: " + constitution;
		intelligenceText.text = "Intelligence: " + intelligence;
		wisdomText.text = "Wisdom: " + wisdom;
		charismaText.text = "Charisma: " + charisma;
	}

	/*
	 * tries to find objects on map
	 * */
	public void findObjects(){
		GameObject[] pickupList = GameObject.FindGameObjectsWithTag ("Pickup");
		for (int i = 0; i < pickupList.Length; i++) {
			if(pickupList[i].name.Contains("Sword")){
				sword = pickupList [i];
			}
			if(pickupList[i].name.Contains("Arrow")){
				arrow = pickupList [i];
			}
		}
	}

	/*
	 * returns amount of arrows player currently has
	 * */
	public int getArrows(){
		return arrowAmount;
	}
	/*
	 * increases arrow amount
	 * */
	public void setArrows(int a){
		arrowSlot.transform.Find ("Arrow").GetComponent<ItemData> ().amount += a;
		updateText ();
	}
}