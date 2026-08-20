using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GeneralLibrary;
using GameLibrary;

public class ItemHandler : MonoBehaviour{
#region Inspector
	[Header("Scripts")]
	public ItemFunctions itemFunctions;
	
	[Header("User Interface")]
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
	private ItemDefinition[] itemDefinitions;
#endregion

#region MainFunctions
	// wow
	public void Awake(){
		itemDefinitions = new ItemDefinition[]{
			new ItemDefinition(ItemType.Nothing, null),
			new ItemDefinition(ItemType.ChocolateBar, itemFunctions.ChocolateBar)
		};
	}
	
	public void Start(){
		itemFunctions = General.GetComponentFromObject<ItemFunctions>("ItemHandler");
		SetAllToNothing();
		SetItemSelection(0);
	}
	
	public void Update(){}
#endregion
	
#region ItemManager
	public void SetAllToNothing(){
		foreach(ItemSlot slot in itemSlots){
			slot.Set(nothingItem);
		}
	}
	
	public void UseItem(){
		Item item = itemSlots[itemSelected].item;
		
		if (item.type == ItemType.Nothing){
			return;
		}
		
		foreach(ItemDefinition definition in itemDefinitions){
			if(item.type == definition.type){
				if(definition.function == null){
					return;
				}
				
				definition.function();
				ResetItem(item);
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
	
	public void SetItemSelection(int slot){
		itemSelected = Mathf.Clamp(slot, 0, itemSlots.Length);
		UpdateSelectionOffset();
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

	public void ResetItem(Item item){
		item.Clear();
		item.Transfer(nothingItem);
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
		itemText.text = $"{itemSlots[itemSelected].item.name.ToLower()}.";
	}
#endregion
}