using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class InventorySlot : MonoBehaviour, IDropHandler {

	private Inventory inv;
	public int id;


	void Start(){
		inv = GameObject.Find ("Inventory").GetComponent<Inventory> ();
	}

	/**
	 * Listener for when an item is dropped on this inventory slot
	 * */
	public void OnDrop(PointerEventData eventData){
		// retrieves the item that is currently being dragged
		ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData> ();
		bool canDrop = true;

		if (inv.slots [id].GetComponent<EquipmentSlot> ()) {
			canDrop = inv.slots [id].GetComponent<EquipmentSlot> ().equipItem(droppedItem.item);
		}

		if (canDrop) {
			// if the slot where the item is dropped is currently empty
			if (inv.items [id].ID == -1) {
				// make the original slot an empty item
				inv.items [droppedItem.slotLocation] = new Item ();
				// update the slot that the item is being dropped to
				inv.items [id] = droppedItem.item;
				// update the slotlocation to the id of the current slot
				droppedItem.slotLocation = id;
			} 
			// if the slot already has an item in it
			else if (droppedItem.slotLocation != id) {
				// get the data of the item in the slot
				Transform item = this.transform.GetChild (0);
				// update the slot location
				item.GetComponent<ItemData> ().slotLocation = droppedItem.slotLocation;
				// update the parent
				item.transform.SetParent (inv.slots [droppedItem.slotLocation].transform);
				// update the position
				item.transform.position = inv.slots [droppedItem.slotLocation].transform.position;

				// update all components of the item being dragged
				droppedItem.slotLocation = id;
				droppedItem.transform.SetParent (this.transform);
				droppedItem.transform.position = this.transform.position;

				inv.items [droppedItem.slotLocation] = item.GetComponent<ItemData> ().item;
				inv.items [id] = droppedItem.item;
			}

		}
	}


}
