using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
Inventorio básico. Tiene una clase Item abajo y definís qué items existen en el mundo en el itemCatalogue.
Podés agregar items al inventorio con AddItem() y quitarlos con RemoveItem()
*/
public class itemManagerScript : MonoBehaviour
{
    public static itemManagerScript itemMan;
    public Dictionary<string, int> inventory;

    public Item[] ItemSpriteList;
    public Dictionary<string, Item> itemCatalogue; // itemname + description

    void Awake(){
        if (itemMan == null){
			DontDestroyOnLoad(this.gameObject);
			itemMan = this;
			inventory = new Dictionary<string, int>();

			itemCatalogue = new Dictionary<string, Item>();
			foreach (Item o in ItemSpriteList){
				itemCatalogue.Add(o.name, o);
			}
		} else {
			Destroy(gameObject);
			return;
		}
	}

    void Start(){
        //AddItem("Bread", 99);
        //AddItem("Onion", 15);
        //AddItem("Cuddles", 1);
        //AddItem("Pikachu", 27);
    }

    public List<string> GetItemList(){
        return new List<string>(inventory.Keys);
    }

    public bool HasItem(string itemname){
        return (inventory.ContainsKey(itemname) && inventory[itemname] > 0);
    }

    public int GetItemCount(string itemname){
        return (inventory.ContainsKey(itemname) ? inventory[itemname] : 0);
    }

    public Sprite GetItemSprite(string itemname){
        return (itemCatalogue.ContainsKey(itemname) ? itemCatalogue[itemname].sprite : null);
    }

    public string GetItemDescription(string itemname){
        string description = "It's a " + itemname + "! What else do you want me to say?";
        if (itemCatalogue.ContainsKey(itemname)){
            description = itemCatalogue[itemname].description;
        }
        return description;
    }

    public void AddItem(string itemname, int number=1){
        number = Mathf.Max(1, Mathf.Min(number, 999));

        if (inventory.ContainsKey(itemname)){
            inventory[itemname] = Mathf.Min(inventory[itemname] + 1, 999);
        } else {

            inventory.Add(itemname, number);
        }
    }

    public void RemoveItem(string itemname, int number=1){
        number = Mathf.Max(1, Mathf.Min(number, 999));

        if (inventory.ContainsKey(itemname)){
            inventory[itemname] -= number;
            if (inventory[itemname] < 1){
                inventory.Remove(itemname);
            }
        }
    }
}

[System.Serializable]
public class Item{
    public string name;
    public string description;
    public Sprite sprite;

    public Item(string n, string desc, Sprite spr){
        name = n;
        description = desc;
        sprite = spr;
    }
};
