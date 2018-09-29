
using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class Heatmap : MonoBehaviour
{

	public Color []rangos;
	public RawImage targetImage;

	[SerializeField]
	int detailScale = 8;

	int xScale, yScale;
	Texture2D texture;
	RawImage raw;

	public static Heatmap instance;
	void Awake () {
		instance = this;
	}
	public void Refresh(Matrix dataMatrix, float scale)
    {
		if (dataMatrix.Rows == 0)
			Debug.LogError("ROws y COls de la matrix no inicializadas!!");
		
		if (texture == null) {

			xScale = detailScale * dataMatrix.Rows;
			yScale = detailScale * dataMatrix.Cols;
			// Create a new texture RGB (8 bit without alpha) and no mipmaps
			texture = new Texture2D (xScale, yScale, TextureFormat.RGB24, false);
			raw = targetImage;
			raw.texture = texture;
		}

	    // 2D Array values
		int [,] sensorValues = new int [dataMatrix.Rows,dataMatrix.Cols];
		dataMatrix.ToIntArray2D(ref sensorValues, dataMatrix.Rows, dataMatrix.Cols, scale);
		// 2D printable values
		int [,] printValues = Matrix.Interp2d(sensorValues,xScale,yScale);
		//
		for (int x = 0; x < xScale; x++) {
			for (int y = yScale-1, i = 0; y > 0 ; y--, i++) {
				Vector3 rgb = Jet (0, 255, printValues [x, i]);
				texture.SetPixel(x, y, new Color (rgb.x, rgb.y, rgb.z));//rgb.z
			}
		}

		// Apply all SetPixel calls
		texture.Apply();
    }


	public Vector3 Jet (float min, float max, float x) {
		float r, g, b;
		float dv;

		r = 1;
		g = 1;
		b = 1;

		if (x < min) x = min;
		if (x > max) x = max;
		dv = max - min;
		int sizeRanges= (int)dv / rangos.Length;
		
		for (int i = 1; i <= rangos.Length; i++){
			if (x >= sizeRanges*(i-1) && x <= sizeRanges*(i)){
				r = rangos[i-1].r;
				b = rangos[i-1].b;
				g = rangos[i-1].g; 
			}
		}
/*
		if (x < (min + 0.2f*dv))
		{
			r = 1 + 5 * (min * dv - x) / dv;
			g = 1;
			b = 1;
		} else if (x < (min + 0.4f * dv))
		{
			r = 0;
			b = 1 + 5 * (min + 0.2f * dv - x) / dv;
			g = 1;
		} else if (x < (min + 0.6f * dv))
		{
			r = 5 * (x - min - 0.4f * dv) / dv;
			g = 1;
			b = 0;
		} else if (x < (min + 0.8f * dv))
		{
			r = 1;
			g = 1 + 2.5f * (min + 0.8f * dv - x) / dv;
			b = 0;
		} else
		{
			r =1;
			b = 0;
			g = 1 + 5 * (min + 0.8f * dv - x) / dv;
		}
*/
		Vector3 rgb = new Vector3(r,g,b);
		return rgb;
	}

	//Old Version2!!
//	Vector3 Jet (float min, float max, float x) {
//		float r, g, b;
//		float dv;
//
//		r = 1;
//		g = 1;
//		b = 1;
//
//		if (x < min) x = min;
//		if (x > max) x = max;
//		dv = max - min;
//
//		if (x < (min + 0.25f*dv))
//		{
//			r = 0;
//			g = 4 * (x - min) / dv;
//		} else if (x < (min + 0.5f * dv))
//		{
//			r = 0;
//			b = 1 + 4 * (min + 0.25f * dv - x) / dv;
//		} else if (x < (min + 0.75f * dv))
//		{
//			r = 4 * (x - min - 0.5f * dv) / dv;
//			b = 0;
//		} else
//		{
//			g = 1 + 4 * (min + 0.75f * dv - x) / dv;
//			b = 0;
//		}
//
//		Vector3 rgb = new Vector3(r,g,b);
//		return rgb;
//	}

}