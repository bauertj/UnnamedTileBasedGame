using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {
	private Item item;
	private string data;
	private GameObject tooltip;

	private Button equipButton;

	[HideInInspector]
	public GameObject slotData;

	public Inventory inv;

	public int slotID;


	void Start(){
		tooltip = GameObject.Find ("ToolTip");
		tooltip.SetActive (false);
		equipButton = tooltip.transform.Find ("EquipButton").GetComponent<Button> ();
		equipButton.onClick.AddListener (TaskOnClick);
	}

	public void activate(Item _item){
		item = _item;
		
		if (!equipButton.gameObject.activeSelf ) {
			equipButton.gameObject.SetActive (true);
		}

		contructDataString ();



		tooltip.SetActive (true);
	}

	public void deactivate(){
		tooltip.SetActive (false);
	}

	public void contructDataString(){
		Text itemText = tooltip.transform.GetChild (0).GetComponent<Text> ();
		data = item.Name + "\n\n" + item.Description;
		itemText.text = data;

		equipButton.GetComponentInChildren<Text>().text = item.ButtonType;
		if (item.ButtonType.Equals (""))
			equipButton.gameObject.SetActive (false);
	}

	void TaskOnClick(){
		string itemType = item.ItemType;
		if (slotID < inv.slotAmount) {

			if (itemType.Equals ("WeaponSlot")) {
				slotData.transform.SetParent (inv.slots [25].transform);
				slotData.transform.position = inv.slots [25].transform.position;
				slotData.GetComponent<ItemData> ().slotLocation = 25;
				slotData.transform.parent.GetComponent<EquipmentSlot> ().equipItem (item);
			}

			GameObject.Find ("Player").GetComponent<Player> ().equip (this.item);
			GameObject.Find ("Inventory").GetComponent<Inventory> ().removeItemByPosition (slotID);
		} else {
			for (int i = 0; i < inv.items.Count; i++) {
				if (inv.items [i].ID == -1) {
					slotData.transform.parent.GetComponent<EquipmentSlot> ().unequipItem();
					GameObject.Find ("Player").GetComponent<Player> ().unequip (this.item);
					slotData.transform.SetParent (inv.slots [i].transform);
					slotData.transform.position = inv.slots [i].transform.position;
					slotData.GetComponent<ItemData> ().slotLocation = i;
					break;
				}
			}
		}

		tooltip.SetActive (false);
		//slotData.GetComponent<ItemData> ().amount--;

	}
}
