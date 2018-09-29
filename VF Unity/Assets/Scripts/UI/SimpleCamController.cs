using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SimpleCamController : MonoBehaviour {
	
	public RawImage cameraRawImage;
	[System.NonSerialized]
	public bool camAvailable ;
	WebCamTexture webCamTexture;

	// Use this for initialization
	void Awake () {
		webCamTexture = new WebCamTexture();
		cameraRawImage.texture = webCamTexture;

		Initialize ();
	}

	void Initialize () {

		if (WebCamTexture.devices.Length > 0) {
			webCamTexture.deviceName = WebCamTexture.devices [0].name;
			webCamTexture.Play ();
			camAvailable = true;
		} else
			camAvailable = false;
	}

}
