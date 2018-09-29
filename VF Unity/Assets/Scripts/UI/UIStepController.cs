using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIStepController : MonoBehaviour {

	public RawImage [] imageMatrixValues;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void SetImageValues (int []values, int max) {

		Color color;
		for (int i = 0; i < values.Length; i++) {
			float value = (float)values [i] / (float)max;
			color = new Color (value, 0, 1f - value);
			imageMatrixValues [i].color = color;
		}

	}
}
