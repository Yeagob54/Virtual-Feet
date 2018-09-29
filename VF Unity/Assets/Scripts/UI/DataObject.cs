using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DataObject : MonoBehaviour {

	public static List<DataObject> actuveValues = new List<DataObject> ();

	GraphicValuesController gvc;
	Text dataValue;
	bool showGraphiData;
	byte activeIndex = 255;
	static byte currentIndex =0;

	// Use this for initialization
	void Start () {
		gvc = GetComponentInParent<UIController> ().gvc;
		dataValue = GetComponentInChildren<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (showGraphiData) 
			gvc.values[activeIndex] = int.Parse(dataValue.text);

	}

	public void StartStopVisualization () {
		showGraphiData = !showGraphiData;

		//Comenzamos a mostrar el valor
		if (showGraphiData) {
			//Solo si hay líneas disponibles
			if (DataObject.actuveValues.Count < gvc.values.Length) {
				activeIndex = (byte)DataObject.actuveValues.Count;
				DataObject.actuveValues.Add (this);
			} else {
				showGraphiData = false;
			}
		} else {
			DataObject.actuveValues.Remove (this);
			activeIndex = 255;
		}
		//En cualquier caso aplicamos el color correspondiente
		dataValue.color = showGraphiData ? gvc.GetColorline(activeIndex): Color.black;

	}
}
