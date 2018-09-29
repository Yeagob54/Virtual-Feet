using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DragImage : DragUI {

	public Image image;


	public void SetImage (Image _image) {
		image.overrideSprite  = _image.overrideSprite;	
	}
}