using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GeneralLibrary;
using GameLibrary;

public class ItemHandler : MonoBehaviour{
#region Inspector
	[Header("Graphical Player Interface")]
	[SerializeField] private TMP_Text itemText;	
	[SerializeField] private RectTransform itemSelect;
	[SerializeField] private Vector2[] itemSelectOffset = new Vector2[2];
	
	[Header("Sounds")]
	[SerializeField] private SoundHandler soundHandler;
	[SerializeField] private AudioClip itemPickup;
	
	[Header("Items")]
	[SerializeField] private Item nothingItem;
	[SerializeField] private ItemSlot[] itemSlots = new ItemSlot[2];
	private int itemSelected;
	
	private static readonly ItemDefinition[] itemDefinitions = {
		new ItemDefinition(ItemType.Nothing, null),
		// new ItemDefinition(ItemType.ChocolateBar, itemFunctions.ChocolateBar),
	};
#endregion

#region MainFunctions
	public void Start(){}
	public void Update(){}
#endregion
	
#region ItemManager
	public void UseItem(ItemSlot slot){
		Item item = slot.item;
		
		if (item.type == ItemType.Nothing){
			return;
		}
		
		foreach(ItemDefinition definition in itemDefinitions){
			if(item.type == definition.type){
				if(definition.function == null){
					return;
				}
				
				definition.function();
				return;
			}
		}
		
		DebugConsole.ThrowError($"No function found for {item.type}.");
	}
	
	public void UpdateSelectionOffset(){
		itemSelect.anchoredPosition = itemSelectOffset[itemSelected];
		UpdateName();
	}
	
	public void ChangeItemSelection(int amount){
		itemSelected += amount;

		if (itemSelected >= itemSelectOffset.Length){
			itemSelected = 0;			
		}
		else if (itemSelected < 0){
			itemSelected = itemSelectOffset.Length - 1;			
		}
		
		UpdateSelectionOffset();
	}

	public void IncreaseItemSelection(){
		ChangeItemSelection(1);
	}

	public void DecreaseItemSelection(){
		ChangeItemSelection(-1);
	}

	public void CollectItem(Item newItem){
		for (int slot = 0; slot < itemSlots.Length; slot++){
			if (itemSlots[slot].item.type == ItemType.Nothing){
				itemSlots[slot].Set(newItem);
				soundHandler.PlaySound(itemPickup);
				GeneralUpdate();
				return;
			}
		}
		
		itemSlots[itemSelected].Set(newItem);
		soundHandler.PlaySound(itemPickup);
		GeneralUpdate();
	}

	public void ResetItem(int selection = 0){
		int clampedSelection = Mathf.Clamp(selection, 0, itemSlots.Length);
		
		itemSlots[clampedSelection].item.Clear();
		itemSlots[clampedSelection].item.Transfer(nothingItem);
		GeneralUpdate();
	}
	
	public void GeneralUpdate(){
		UpdateName();
		UpdateSlotSprites();
	}
	
	public void UpdateSlotSprites(){
		foreach(ItemSlot slot in itemSlots){
			Sprite sprite = slot.item.type == ItemType.Nothing ? nothingItem.sprite : slot.item.sprite;
			
			if(slot.outputTexture.sprite != sprite){
				slot.outputTexture.sprite = sprite;
			}
		}
	}
	
	public void UpdateName(){
		itemText.text = itemSlots[itemSelected].item.name;
	}
#endregion
}