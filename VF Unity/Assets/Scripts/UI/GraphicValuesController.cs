using UnityEngine;
using System.Collections;

public class GraphicValuesController : MonoBehaviour {


	Transform pivot;
	LineRenderer []lines;
	[System.NonSerialized]
	public int []values;
	public float factor = 0.1f;
	int []countVertexLine;

	[SerializeField]
	GameObject lineaH;
	[SerializeField]
	GameObject lineaV;

	// Use this for initialization
	void Start () {
		pivot = transform.GetChild (0);
		lines = GetComponentsInChildren<LineRenderer> ();
		values = new int[lines.Length];
		countVertexLine = new int[lines.Length];

		lineaH.SetActive (true);
		lineaV.SetActive (true);
	}
	
	// Update is called once per frame
	void Update () {
		//El pivot está siempre en movimiento
		pivot.Translate(Time.deltaTime,0,0);
	
		for (int i = 0; i < lines.Length; i++) {
			if (values[i] > 0) {
				countVertexLine[i]++;
				lines [i].SetVertexCount (countVertexLine[i]);
				Vector3 newPos = pivot.position + Vector3.up * values[i] * factor;
				lines [i].SetPosition (countVertexLine[i] - 1, newPos);
			}
		}
	}

	public Color GetColorline(byte index){
		return lines [index].material.GetColor("_TintColor");
	}
}
