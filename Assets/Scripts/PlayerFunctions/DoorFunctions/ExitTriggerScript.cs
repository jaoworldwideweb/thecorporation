using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTriggerScript : MonoBehaviour{
	[Header("Main")]
	[SerializeField] private GameControllerScript gameController;
	[SerializeField] private string resultsScene;
	[SerializeField] private string secretScene;	
	
	private void OnTriggerEnter(Collider other){
		if (gameController.collectedBoxes >= gameController.maxBoxes && other.tag == "Player"){
			SceneManager.LoadScene(resultsScene);
		}
	}
}
