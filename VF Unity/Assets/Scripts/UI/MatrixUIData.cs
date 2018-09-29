using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(UIController))]
public class MatrixUIData : MonoBehaviour {

	//Conexion
	public StreamReader streamReader;

	//Visualization UI
	UIController panelDataController;
	[System.NonSerialized]
	public List<Text> textData = new List<Text> ();
	private List<Image> imageData = new List<Image> ();
	public bool showValues = true;

	//Callibration
	private Matrix calibration;
	public byte calibrationConunt;  //Numero de lecturas para establecer un calibrado de mínimos (o matriz de ruido)
	private byte currentCalibration = 0;
	private bool calibreateOn = false;

	//Data management
	public byte ROWS = 8;
	public byte COLS = 15; 
	[System.NonSerialized]
	public Matrix dataMatrix;
	float scale = 1; // x2 o /2 up down

	//TODO: esto temporalmente aquí!
	//VR Mode
	public bool vrMode;

	// Use this for initialization
	void Start () {
		//Inicializamos la matriz de calibrado, con sus valores al máximo, para hacer el calibrado de mínimos
		calibration = new Matrix (ROWS, COLS, 255);

		StreamReader.OnDataRecived += MatrixRecived;

		if (!vrMode) {
			//Hacemos referencia al panel de datos: UICOntroller
			panelDataController = GetComponent<UIController> ();

			//Recorremos todos los paneles de visualización de datos que tengamos. 
			//Asignamos el tamaño de cada celda en pantalla, 
			panelDataController.SetCellSize (ROWS, COLS);

			//Creamos los objetos de datos necesarios para mostrar la matriz, para los dos paneles.
			panelDataController.CreateDataObjects (ROWS * COLS, ref textData, ref imageData);

			//TOOD: mejor informar por evento...
			panelDataController.CurrentState = PanelState.READY;

			//Mostramos la posición de cada sensor mientras no hemos conectado
			Matrix.TextListPositions (ref textData);
		}


	}
	
	void MatrixRecived () {
		//Si no estamos calibrando mostramos la matriz 
		if (!Calibrating ()) {
			dataMatrix = new Matrix (streamReader.GetStream(), ROWS, COLS);

			//Mostramos HeatMap y/o valores / colroes
			if (!vrMode && !HeatMapping (dataMatrix)) {
				//Mostramos los datos de la matriz en los objetos de texto
				if (showValues)
					dataMatrix.ToTextList (ref textData, scale);

				//Ajustamos el color de los objetos Imagen
				dataMatrix.ToImageList (ref imageData, scale);
			}
		}
	}

	//Metodo para encender o apagar los valores numéricos.
	public void SwitchValues (){
		bool state = panelDataController.ValuesSwitch();
		foreach (Text text in textData) {
			text.enabled = state;
		}
	}

	//Poner esto en la zona de botones TODO pendientes del listener
	public void IncScale (bool positive) {
		scale *= positive ? 2f : 0.5f;
		panelDataController.scaleText.text = scale.ToString() + ":1";
	}

	public void SetCalibrate (bool value) {
		calibreateOn = value;

	}

	//Calibrado de mínimos
	bool Calibrating () {
		if (calibreateOn && calibrationConunt > currentCalibration) {
			//Sobrecarga para calibrar la matriz
			calibration -= new Matrix (streamReader.GetStream(), ROWS, COLS);
			currentCalibration++;
			return true;
		}
		return false;
	}

	//Refresco del HeatMapping
	bool HeatMapping (Matrix dataMatrix) {
		if (panelDataController.HeatmapActive) {
			//iniciamos el refresco del heatmap
			panelDataController.heatmap.Refresh (dataMatrix,scale);
			//Desactivamos el panel de datos.
			if (panelDataController.matrixPanel.gameObject.activeSelf)
				panelDataController.matrixPanel.gameObject.SetActive (false);
			return true;
		}
		//Activamos el panel de datos.
		if (!panelDataController.matrixPanel.gameObject.activeSelf)
			panelDataController.matrixPanel.gameObject.SetActive (true);
		return false;
	}
}
