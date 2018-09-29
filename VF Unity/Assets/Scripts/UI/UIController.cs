//TODO: Sacar el GraphicValueController y la cámara a una zona común de las dos mantas.
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum PanelState{
	READY = 0,
	CONECTED = 1,
	RECORDING = 2,
	PLAYING = 3,
	DISCONECTED = 4,
	ERROR = 5
}

[RequireComponent(typeof(SimpleCamController))]
public class UIController : MonoBehaviour {

	private PanelState currentState;

	public PanelState CurrentState {
		set {
			currentState = value;
			OnPanelStateChange ();
		}
		get {
			return currentState;
		}
	}

	public GameObject heatmapRawImage;
	public RectTransform matrixPanel;
	public Image circleFillFps;
	public Image circleFillPps;
	public InputField ipText;
	public InputField nameText;
	public InputField portText;
	public Text fpsText;
	public Text eventsText;
	public Text statusText;
	public Text scaleText;
	public bool showFPS = true;

	[SerializeField]
	private Button recButton;
	[SerializeField]
	private Button playButton;
	[SerializeField]
	private Button conectionButton;
	[SerializeField]
	private Button cameraButton;
	[SerializeField]
	private Text switchValuesText;

	private SimpleCamController camController;
	private GameObject cameraPanel;

	[System.NonSerialized]
	public GraphicValuesController gvc;
	[System.NonSerialized]	
	public Heatmap heatmap;

	private bool heatmapActive = false;
	private bool valuesActive = true;

	public bool HeatmapActive {
		get{return heatmapActive;}
		set {
			heatmapActive = value;
			if (heatmapRawImage != null)
				heatmapRawImage.SetActive (heatmapActive);
			else
				Debug.LogWarning ("NO hay ninguna HeatMapImage asignada!");
		}
	}

	GameObject dataObjectUI;

	void Awake() {
		gvc = GetComponentInChildren<GraphicValuesController> ();
		heatmap = FindObjectOfType<Heatmap> ();
		HeatmapActive = false;

		//Persistencia de IP y Puerto
		//ipText.text = PlayerPrefs.GetString("IP");
		//portText.text = PlayerPrefs.GetString("Port");

		try{
			dataObjectUI = FindObjectOfType<DataObject>().gameObject;
		}catch {
			throw new System.IndexOutOfRangeException ("No hay ningún DataObject en la escena!");
		}

	}

	void Start() {

		//WebCam references
		camController = GetComponent<SimpleCamController> ();
		cameraButton.enabled = camController.camAvailable;
		cameraPanel = camController.cameraRawImage.gameObject;
	}


	void Update () {

		if (showFPS)
			//Mostramos la velocidad de los FPS por pantalla. 
			SetFPS (1 / Time.deltaTime);

		if (Input.touchCount == 2)
			GetComponentInChildren<Canvas> ().gameObject.SetActive (true);
	}

	public string GetIP () {
		return ipText.text;
	}

	public string GetName () {
		return nameText.text;
	}

	public int GetPort () {
		return int.Parse (portText.text);
	}

	public void SetCellSize (byte rows, byte cols) {
		//Asignamos el tamaño de cada celda de datos del panel.
		matrixPanel.GetComponent<GridLayoutGroup> ().cellSize = GetCellSize (rows, cols);
	}

	private Vector2 GetCellSize (byte rows, byte cols) {
		
		//Calculamos los anchos y altos adecuados para que la matriz entre en pantalla. 
		float prefH = matrixPanel.rect.height / (rows);
		float prefW = matrixPanel.rect.width / (cols);
		//77
		return new Vector2 (prefW, prefH);
	}

	//Clonamos los objetos de datos necesarios para mostrar la matriz
	public void CreateDataObjects(int dataCount, ref List<Text> textData, ref List<Image> imageData) {

		//Clonamos los objetos de datos... 
		for (int i = 0; i < dataCount; i++) {  
			GameObject newDataObject = Instantiate (dataObjectUI);

			// y los hacemos hijos del matrixPanel.
			newDataObject.transform.SetParent (matrixPanel.transform);

			//Guardamos las referencias a los Text y a los Image en las listas correspondientes
			textData.Add (newDataObject.GetComponentInChildren<Text>());
			imageData.Add (newDataObject.GetComponent<Image>());

		}

		//Ya no necesitamos el objeto inicial, lo destruimos.
		Destroy (dataObjectUI);

	}

	public void SetFPS(float fps) {
		fpsText.text = ((int)fps).ToString () + " fps";
		circleFillFps.fillAmount = fps / 100;
	}

	public void SetEvents(float events) {
		if (eventsText != null && circleFillPps != null) {
			eventsText.text = events.ToString () + " pps";
			circleFillPps.fillAmount = events / 100;
		} else {
			Debug.LogWarning ("No están asignados los elementos UI de pps");
		}
	}

	public void SetState (string newState){
		statusText.text = newState;
	}

	//TODO: Este botón está conectado con el StreamReder, mejor mediante evento.
	public void RecButtonState(bool state){
		var buttonColor = recButton.colors.normalColor;
		buttonColor = state? Color.red : Color.white;
		CurrentState = state ? PanelState.RECORDING : PanelState.CONECTED;
		playButton.interactable = !state;
	}

	public void HeatMapSwitch () {
		HeatmapActive = !HeatmapActive;
	}

	public bool ValuesSwitch () {
		if (valuesActive) {
			valuesActive = false;
			switchValuesText.text = "Show Values";

		} else {
			valuesActive = true;
			switchValuesText.text = "Hide Values";
		}

		return valuesActive;

	}

	public void Exit () {
		Application.Quit ();
	}

	void OnPanelStateChange () {
		switch (CurrentState) {
		case PanelState.READY:
			SetState ("Disconnected");
			statusText.color = Color.gray;

			break;
		case PanelState.CONECTED:
			SetState ("Connected");
			statusText.color = Color.black;
			SavePortIP ();
			break;
		case PanelState.RECORDING:
			SetState ("Recording...");
			statusText.color = Color.red;
			break;
		case PanelState.PLAYING:
			SetState ("Playing...");
			break;
		case PanelState.DISCONECTED:
			statusText.color = Color.gray;
			SetState ("Disconnected");
			SetEvents (0);
			break;
		case PanelState.ERROR:
			statusText.color = Color.red;
			SetState ("ERROR!!!");
			break;
		}
	}

	void SavePortIP () {
		PlayerPrefs.SetString ("IP", ipText.text);
		PlayerPrefs.SetString ("Port", portText.text);		
	}

	public void SwitchCamera() {
		cameraPanel.SetActive (!cameraPanel.activeSelf);
		var buttonColor = cameraButton.colors.normalColor;
		buttonColor = cameraPanel.activeSelf? Color.red : Color.white;
	}
}
