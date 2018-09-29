using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MouseLabel : DragUI {

	public Text labelText;
	
	// Update is called once per frame
	void SetText (string label) {
		labelText.text = label;	
	}

}
