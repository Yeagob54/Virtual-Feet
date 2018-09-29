	
using UnityEngine;

using System.Collections;



public class Teleport : MonoBehaviour
{

	private RaycastHit lastRaycastHit;

	public GameObject teleportMarker;

	public AudioClip audioClip;

	public float range = 1000;

	[Range(1f,100f)]
	public float spedMove = 50f;



	void Start () {

		teleportMarker.gameObject.SetActive (false);

	}

	void Update()

	{

		//Confirmamos teleportación
		if (Input.GetMouseButtonUp(0)){
			if (GetLookedAtObject () != null)
				StartCoroutine (TeleportToLookAt ());

			teleportMarker.SetActive (false);
		}

		if (Input.GetMouseButtonDown (0)) {
			teleportMarker.SetActive (true);
		
		}

		//Mientras elegimos donde mostramos el sistema de partículas
		if (Input.GetMouseButton(0)) {
			GameObject currentTarget = GetLookedAtObject ();
			if (currentTarget != null && currentTarget.tag == "Suelo")
				teleportMarker.transform.position = lastRaycastHit.point + lastRaycastHit.normal;
		}

	}



	private GameObject GetLookedAtObject()

	{

		Vector3 origin = transform.position;

		Vector3 direction = Camera.main.transform.forward;

		if (Physics.Raycast(origin, direction, out lastRaycastHit, range)) //Si es tipo suelo el objeto

		{

			return lastRaycastHit.collider.gameObject;

		}

		else

		{

			return null;

		}

	}



	private IEnumerator TeleportToLookAt()

	{

		Vector3 destinationPoint = lastRaycastHit.point + lastRaycastHit.normal * 1.5f;

		if (audioClip != null)
			AudioSource.PlayClipAtPoint(audioClip, transform.position);

		//Nos movemos hacia el destino
		while (Vector3.Distance (transform.position, destinationPoint) > 0.3f) {
			transform.position = Vector3.MoveTowards (transform.position, destinationPoint, spedMove * Time.deltaTime);
			//Esperamos un prame antes del siguiente movimiento
			yield return null;
		}

		yield return null;


	}




}
