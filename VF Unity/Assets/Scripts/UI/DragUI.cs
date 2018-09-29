using UnityEngine;
using System.Collections;

public class DragUI : MonoBehaviour {

	public Vector3 offSet;
	private Canvas myCanvas;

	private bool active = false;

	void Start () {
		myCanvas = transform.parent.gameObject.GetComponent<Canvas>();
	}
	// Use this for initialization
	void Update () {
	    Vector2 pos;
         RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out pos);
         transform.position = myCanvas.transform.TransformPoint(pos) + offSet;
		//transform.localPosition = Input.mousePosition - myCanvas.transform.localPosition;
	}

	public void SetActive (bool _active) {
		active = _active;
		gameObject.SetActive(active);
	}

	public bool IsActive () {
		return active;
	}
}
