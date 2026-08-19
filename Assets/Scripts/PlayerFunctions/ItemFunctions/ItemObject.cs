using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GeneralLibrary;
using GameLibrary;

public class ItemObject : MonoBehaviour{
#region Inspector
	[SerializeField] private Item item;
	private SpriteRenderer billboard;
	private ItemHandler itemHandler;
#endregion

#region MainFunctions
	private void Start(){
		itemHandler = General.GetComponentFromObject<ItemHandler>("ItemHandler");
	}
	
	public void Collect(){
		if(itemHandler == null){
			DebugConsole.ThrowError("No ItemHandler found in scene.");
			return;
		}
		
		itemHandler.CollectItem(item);
	}
#endregion
}