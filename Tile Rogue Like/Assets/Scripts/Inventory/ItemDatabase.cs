using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public class ItemDatabase : MonoBehaviour {

	private List<Item> database = new List<Item>();
	private JsonData itemData;

	void Start(){
		itemData = JsonMapper.ToObject (File.ReadAllText(Application.dataPath + "/StreamingAssets/Items.json"));
		ConstructItemDatabase ();
	}

	public Item fetchItemByID(int id){
		for (int i = 0; i < database.Count; i++) {
			if (database [i].ID == id)
				return database [i];
		}

		return null;
	}

	void ConstructItemDatabase(){
		for (int i = 0; i < itemData.Count; i++) {
			database.Add (new Item ((int)itemData[i]["id"], itemData[i]["name"].ToString(), (int)itemData[i]["stats"]["strength"], (int)itemData[i]["stats"]["dexterity"], 
				(int)itemData[i]["stats"]["constitution"], (int)itemData[i]["stats"]["intelligence"], (int)itemData[i]["stats"]["wisdom"], (int)itemData[i]["stats"]["charisma"],
				itemData[i]["stats"]["damage"].ToString(), (int)itemData[i]["stats"]["armor"], (int)itemData[i]["dice"]["numberOfDice"], (int)itemData[i]["dice"]["diceTotalValue"], itemData[i]["description"].ToString(), 
				(bool)itemData[i]["stackable"], (int)itemData[i]["rarity"], itemData[i]["slug"].ToString(), itemData[i]["buttonType"].ToString(), itemData[i]["itemType"].ToString()
			));
		}
	}
}

public class Item {
	public int ID { get; set; }
	public string Name { get; set; }
	public int Strength { get; set; }
	public int Dexterity { get; set; }
	public int Constitution { get; set; }
	public int Intelligence { get; set; }
	public int Wisdom { get; set; }
	public int Charisma { get; set; }
	public string Damage { get; set; }
	public int Armor { get; set; }
	public int NumberOfDice { get; set; }
	public int DiceTotalValue { get; set; }
	public string Description { get; set; }
	public bool Stackable { get; set; }
	public int Rarity { get; set; }
	public string Slug { get; set; }
	public Sprite Sprite { get; set; }
	public string ButtonType { get; set; }
	public string ItemType { get; set; }

	public Item(int id, string name, int strength, int dexterity, int constitution, int intelligence, int wisdom, int charisma, string damage, int armor, int numberOfDice, int diceTotalValue, string description, bool stackable, int rarity, string slug, string buttonType, string itemType){
		this.ID = id;
		this.Name = name;
		this.Strength = strength;
		this.Dexterity = dexterity;
		this.Constitution = constitution;
		this.Intelligence = intelligence;
		this.Wisdom = wisdom;
		this.Charisma = charisma;
		this.Damage = damage;
		this.Armor = armor;
		this.NumberOfDice = numberOfDice;
		this.DiceTotalValue = diceTotalValue;
		this.Description = description;
		this.Stackable = stackable;
		this.Rarity = rarity;
		this.Slug = slug;
		this.Sprite = Resources.Load<Sprite> ("Sprites/Items/" + slug);
		this.ButtonType = buttonType;
		this.ItemType = itemType;
	}

	public Item(){
		this.ID = -1;
	}
}
