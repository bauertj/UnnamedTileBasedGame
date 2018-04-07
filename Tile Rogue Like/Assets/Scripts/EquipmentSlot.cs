using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {


	private Color32 white, grey;
	private Image image;

	public Item item;
	public Image armorIcon;

	// Use this for initialization
	void Start () {
		image = this.GetComponent<Image> ();
		white = new Color32(255, 255, 255, 255);
		grey = new Color32(205, 205, 205, 255);
	}
		


	public void OnPointerEnter(PointerEventData eventData){
		image.color = grey;
	}

	public void OnPointerExit(PointerEventData eventData){
		image.color = white;
	}


	public bool equipItem(Item _item){
		if (_item.ItemType.Equals (this.name)) {
			item = _item;
			armorIcon.transform.gameObject.SetActive (false);
			return true;
		}
		return false;
	}

	public void unequipItem(){
		armorIcon.transform.gameObject.SetActive (true);
		item = new Item ();
	}

}