using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shoot : MonoBehaviour {
	
	public GameObject bulletPrefab;
	public Transform shootPosition;
	public int shootForce= 10000;
	public StreamReader streaming;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (Input.GetButtonDown ("Fire1")) {
			GameObject bullet = Instantiate (bulletPrefab, shootPosition.position, shootPosition.rotation) as GameObject;
			bullet.GetComponent<Rigidbody> ().AddRelativeForce (shootPosition.forward*shootForce);
		}

		//Reiniciamos la escena con Fire1 + Fire2 o conectamos al socket
		if (Input.GetButtonDown ("Fire1") && Input.GetButtonDown ("Fire2")) {
			if (streaming.socketReady)
				SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
			else
				streaming.SocketConnect ();
		}
	}
}
