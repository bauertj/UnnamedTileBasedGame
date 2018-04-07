using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemData : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerClickHandler {
	public Item item;
	public int amount;

	public int slotLocation;

	private Inventory inv;
	private Vector2 offset;
	private Tooltip tooltip;
	private Transform originalParent;

	void Start(){
		// finds inventory in the editor
		inv = GameObject.Find ("Inventory").GetComponent<Inventory> ();
		// gets the tooltip class from the inventory
		tooltip = inv.GetComponent<Tooltip> ();
		amount = 1;
	}
		
	void Update(){
		if (amount == 0) {
			this.transform.parent.GetComponent<EquipmentSlot> ().unequipItem ();
			inv.items [this.transform.parent.GetComponent<InventorySlot> ().id] = new Item ();
			Destroy (this.gameObject);
		}
	}

	public void OnPointerDown(PointerEventData eventData){
		// as long as there is an item in the current slot
		if (item != null) {
			// set the original parent
			originalParent = this.transform.parent;



			// set an offset to adjust the position of the item relative to the mouse
			offset = eventData.position - new Vector2 (this.transform.position.x, this.transform.position.y);
			// sets the parent of the item to allow hovering over the UI
			this.transform.SetParent (this.transform.parent.parent.parent);
			this.transform.position = eventData.position - offset;
			// blocks raycasts
			GetComponent<CanvasGroup> ().blocksRaycasts = false;
			// activate tooltip for this item
			tooltip.activate (item);
			tooltip.slotData = this.gameObject;
			tooltip.slotID = this.slotLocation;
		}
	}

	public void OnPointerUp(PointerEventData eventData){
		this.transform.SetParent (originalParent);
		this.transform.position = originalParent.transform.position;
		GetComponent<CanvasGroup> ().blocksRaycasts = true;
	}

	public void OnDrag(PointerEventData eventData){
		if (item != null) {


			this.transform.position = eventData.position - offset;

		}
	}

	public void OnEndDrag(PointerEventData eventData){
		if (originalParent.GetComponent<EquipmentSlot> ()) {
			originalParent.GetComponent<EquipmentSlot> ().unequipItem ();
			GameObject.Find ("Player").GetComponent<Player> ().unequip (this.item);
		}
		this.transform.SetParent (inv.slots[slotLocation].transform);
		if (this.transform.parent.GetComponent<EquipmentSlot> ()) {
			this.transform.parent.GetComponent<EquipmentSlot> ().equipItem (this.item);
			GameObject.Find ("Player").GetComponent<Player> ().equip (this.item);
		}

		this.transform.position = inv.slots[slotLocation].transform.position;
		GetComponent<CanvasGroup> ().blocksRaycasts = true;

		tooltip.deactivate ();
	}

	public void OnPointerEnter(PointerEventData eventData){
		//tooltip.activate (item);
	}

	public void OnPointerExit(PointerEventData eventData){
		//tooltip.deactivate ();
	}

	public void OnPointerClick(PointerEventData eventData){
		tooltip.activate (item);
		tooltip.slotData = this.gameObject;
		tooltip.slotID = this.slotLocation;
	}
		
}
