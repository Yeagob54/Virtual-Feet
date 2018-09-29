using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {

	public Dropdown camerasDropdown;
	public RawImage cameraRawImage;
	WebCamTexture webCamTexture;
	Dictionary <Dropdown.OptionData, WebCamDevice> cameraList = new Dictionary<Dropdown.OptionData, WebCamDevice>();

	// Use this for initialization
	void Start () {

		webCamTexture = new WebCamTexture();

		cameraRawImage.texture = webCamTexture;

		Initialize ();

		camerasDropdown.onValueChanged.AddListener (delegate {DropdownValueChange ();});

		//Inicializamos la cámara con el primer device de la lista
		if (cameraList.Count > 0)
			SetDevice(0);

	}

	void DropdownValueChange () {
		Dropdown.OptionData currentOption;
		currentOption = camerasDropdown.options [camerasDropdown.value];
		SetDevice (currentOption);
	}

	void SetDevice (Dropdown.OptionData option) {
		webCamTexture.deviceName = cameraList [option].name;
		webCamTexture.Play ();
	}

	void SetDevice (int indexDropdown) {
		webCamTexture.deviceName = cameraList [camerasDropdown.options [indexDropdown]].name;
		webCamTexture.Play ();
	}


	void Initialize () {

		//TODO: añadir la opcción OFF
		List <Dropdown.OptionData> listaDropdown = new List<Dropdown.OptionData>();

		foreach (WebCamDevice camDevice in WebCamTexture.devices) {
			Dropdown.OptionData option = new Dropdown.OptionData(camDevice.name);
			listaDropdown.Add(option);
			cameraList.Add(option,camDevice);
		}

		camerasDropdown.ClearOptions ();
		camerasDropdown.AddOptions (listaDropdown);
	}


	// Update is called once per frame
	void Update () {
	
	}
}
