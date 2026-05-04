using UnityEngine;

public class FacultyTriggerScript : MonoBehaviour
{
	private void Start()
	{
		hitBox = GetComponent<BoxCollider>();
	}
	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.CompareTag("Player")) //If it is a player
		{
			return;
		}
	}
	public PlayerScript ps;
	private BoxCollider hitBox;
}
