using UnityEngine;

public abstract class PhysicsObject : MonoBehaviour{
#region Inspector
	[SerializeField] private bool isPhysicsOn = true;
	[SerializeField] private bool canBlockNPCs = true;
	[SerializeField] private Weight objectWeight = Weight.FiveKilograms;
	private Rigidbody objectRigidbody;
	
	public enum Weight{
		FiveKilograms = 5,
		TenKilograms = 10,
		TwentyKilograms = 20,
	};
#endregion

#region MainFunctions
	protected virtual void Start(){
		objectRigidbody = GetComponent<Rigidbody>();
		
		if (objectRigidbody == null){
			Debug.LogWarning($"{gameObject.name} does not have a Rigidbody.");
			return;
		}
		
		SetPhysics(isPhysicsOn);
	}
	
	protected virtual void OnTriggerEnter(Collider other){
		if(other.CompareTag("Player")){
			OnPlayerTouching();
		}
	}
#endregion

#region API
	/// <summary>
	/// Called when the player or a NPC drops a object.
	/// </summary>
	public virtual void OnObjectDrop(){
		SetPhysics(true);
	}
	
	/// <summary>
	/// Called when the player or a NPC picks up a object.
	/// </summary>
	public virtual void OnObjectPickup(){
		if (objectRigidbody == null){
			return;
		}
		
		objectRigidbody.isKinematic = false;
		objectRigidbody.useGravity = false;
		
		objectRigidbody.linearDamping = 5f;
		objectRigidbody.angularDamping = 5f;
	}

	/// <summary>
	/// Called when the player touches the object.
	/// </summary>
	public virtual void OnPlayerTouching(){}

	/// <summary>
	/// Returns the object's weight.
	/// </summary>
	public float GetObjectWeight(){
		return (float)objectWeight;
	}
	
	/// <summary>
	/// Returns whether the object can block NPCs.
	/// </summary>
	public bool CanBlockNPCs(){
		return canBlockNPCs;
	}
	
	/// <summary>
	/// Returns the Rigidbody attached to this object.
	/// </summary>
	public Rigidbody GetRigidbody(){
		return objectRigidbody;
	}
#endregion

#region MainPhysicsFunctions
	/// <summary>
	/// Throws the object using an impulse force.
	/// </summary>
	public virtual void ThrowObject(Vector3 direction, float force){
		SetPhysics(true);
		
		if (objectRigidbody == null){
			return;
		}
		
		objectRigidbody.linearDamping = 0.05f;
		objectRigidbody.angularDamping = 0.05f;
		
		objectRigidbody.AddForce(direction.normalized * force, ForceMode.Impulse);
	}

	/// <summary>
	/// Moves the object toward a target position.
	/// </summary>
	public virtual void MoveObject(Vector3 targetPosition, float force){
		if (objectRigidbody == null){
			return;
		}
		
		Vector3 direction = targetPosition - objectRigidbody.worldCenterOfMass;
		objectRigidbody.AddForce(direction * force, ForceMode.Acceleration);
	}

	/// <summary>
	/// Enables or disables the object's physics.
	/// </summary>
	public virtual void SetPhysics(bool state){
		isPhysicsOn = state;

		if (objectRigidbody == null){
			return;
		}
		
		objectRigidbody.useGravity = state;
		objectRigidbody.isKinematic = !state;
		
		if (state){
			objectRigidbody.linearDamping = 0.05f;
			objectRigidbody.angularDamping = 0.05f;
		}
	}
#endregion
}
