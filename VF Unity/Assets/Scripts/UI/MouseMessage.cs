using UnityEngine;
using System.Collections;

public class MouseMessage : MonoBehaviour {

	public string message;
	public Vector3 offSet;

	public MouseLabel mouseLabel; 

	public void RollOver () {
		if (message != "") {
			mouseLabel.SetActive(true);
			mouseLabel.labelText.text = message;
			mouseLabel.offSet = offSet;
		}
			
	}

	public void RollOut () {
		mouseLabel.SetActive(false);
	}
}
