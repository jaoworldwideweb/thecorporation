using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuCursorSystem : MonoBehaviour{
	[Header("Cursor")]
	public Texture2D hoverCursor;
	public Vector2 hotspot;
	public CursorMode cursorMode = CursorMode.Auto;

	private HashSet<GameObject> registeredObjects = new HashSet<GameObject>();

	private void Start(){
		RegisterUIElements();
	}

	private void Update(){
		RegisterUIElements();
	}

	private void RegisterUIElements(){
		Selectable[] selectables = GameObject.FindObjectsOfType<Selectable>(true);

		foreach (Selectable selectable in selectables){
			GameObject obj = selectable.gameObject;

			if (registeredObjects.Contains(obj)){
				continue;
			}
			
			AddTrigger(obj);
			registeredObjects.Add(obj);
		}
	}

	private void AddTrigger(GameObject obj){
		EventTrigger trigger = obj.GetComponent<EventTrigger>();
		trigger = obj.AddComponent<EventTrigger>();
		
		bool hasEnter = false;
		bool hasExit = false;

		foreach (EventTrigger.Entry entry in trigger.triggers){
			if (entry.eventID == EventTriggerType.PointerEnter){
				hasEnter = true;				
			}
			if (entry.eventID == EventTriggerType.PointerExit){
				hasExit = true;				
			}
		}

		if (!hasEnter){
			EventTrigger.Entry enterEntry = new EventTrigger.Entry();
			enterEntry.eventID = EventTriggerType.PointerEnter;
			enterEntry.callback.AddListener((data) => OnHover());

			trigger.triggers.Add(enterEntry);
		}
		if (!hasExit){
			EventTrigger.Entry exitEntry = new EventTrigger.Entry();
			exitEntry.eventID = EventTriggerType.PointerExit;
			exitEntry.callback.AddListener((data) => OnExit());

			trigger.triggers.Add(exitEntry);
		}
	}

	private void OnHover(){
		Cursor.SetCursor(hoverCursor, hotspot, cursorMode);
	}

	private void OnExit(){
		Cursor.SetCursor(null, Vector2.zero, cursorMode);
	}
}