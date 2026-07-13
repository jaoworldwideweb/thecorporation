using System;
using UnityEngine;

public class MenuButton : MonoBehaviour{
	[SerializeField] private MenuManager menuManager;
	[SerializeField] private GameObject currentMenu;
	[SerializeField] private GameObject nextMenu;

	public void SwitchMenu(){
		menuManager.SwitchMenus(currentMenu, nextMenu);
	}
}