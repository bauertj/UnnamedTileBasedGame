using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
	GameObject inventoryPanel;
	GameObject slotPanel;
	GameObject equipmentPanel;

	ItemDatabase database;
	public GameObject inventorySlot;
	public GameObject inventoryItem;

	[HideInInspector]
	public int slotAmount;
	int equipmentSlots;
	public List<Item> items = new List<Item>();
	public List<GameObject> slots = new List<GameObject>();

	void Start(){
		database = GetComponent<ItemDatabase> ();
		slotAmount = 20;
		equipmentSlots = 10;
		inventoryPanel = GameObject.Find ("InventoryPanel");
		slotPanel = GameObject.Find ("SlotPanel");
		equipmentPanel = GameObject.Find ("EquipmentPanel");

		for (int i = 0; i < slotAmount; i++) {
			items.Add (new Item ());
			slots.Add (Instantiate(inventorySlot));
			slots [i].GetComponent<InventorySlot> ().id = i;
			slots [i].transform.SetParent (slotPanel.transform);
		}

		for (int i = 0; i < equipmentPanel.transform.childCount; i++) {
			slots.Add (equipmentPanel.transform.GetChild (i).gameObject);
			slots [i + slotAmount].GetComponent<InventorySlot> ().id = i + slotAmount;
		}

		for (int i = slotAmount; i < slotAmount + equipmentSlots; i++) {
			items.Add (new Item ());
		}

		inventoryPanel.SetActive (false);

		addItem (0);
	}

	public void addItem(int id){
		Item itemToAdd = database.fetchItemByID (id);

		if (itemToAdd.Stackable && checkInInventory (itemToAdd)) {
			for (int i = 0; i < items.Count; i++) {
				if (items [i].ID == itemToAdd.ID) {
					ItemData data = null;
					foreach(Transform child in slots[i].transform){
						if (child.GetComponent<ItemData> ()) {
							data = child.GetComponent<ItemData> ();
							break;
						}
					}
					if (data.amount == 0)
						data.amount++;
					data.amount++;
					data.transform.GetChild (0).GetComponent<Text> ().text = data.amount.ToString ();
					break;
				}
			}
		} else {
			for (int i = 0; i < items.Count; i++) {
				if (items [i].ID == -1) {
					items [i] = itemToAdd;
					GameObject itemObject = Instantiate (inventoryItem);
					itemObject.GetComponent<ItemData> ().item = itemToAdd;
					itemObject.GetComponent<ItemData> ().slotLocation = i;
					itemObject.transform.SetParent (slots [i].transform);
					itemObject.transform.position = slots[i].transform.position;
					itemObject.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0,0);
					itemObject.GetComponent<Image> ().sprite = itemToAdd.Sprite;
					itemObject.name = itemToAdd.Name;
					break;
				}
			}
		}
	}

	bool checkInInventory(Item item){
		for (int i = 0; i < items.Count; i++) {
			if (items [i].ID == item.ID) {
				return true;
			}
		}

		return false;
	}

	public void removeItemByPosition(int position){
		
		items [position] = new Item ();
			
	}

	public void removeItemByID(int id){
		for (int i = 0; i < items.Count; i++) {
			if(items[i].ID == id){
				if (items [i].Stackable) {
					
					ItemData data = slots [i].transform.GetChild (0).GetComponent<ItemData> ();
					data.amount--;
					data.transform.GetChild (0).GetComponent<Text> ().text = data.amount.ToString ();
					break;
				}
					
			}
		}
	}
}
