	
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct Matrix{

	// ******** PROPIEDADES *********

	//- Const -
	//Margen de calibración para mantas de 10X12
	internal const byte OFFSET = 9; 

	//- Private -
	private int count;
	private byte rows, cols;

	//- Public -
	public byte [] matrixData;

	public int Count { get { return count; } }
	
	public byte Rows { get {return rows; }}

	public byte Cols  { get { return cols; } }


	//******** CONSTRUCTORES *********

	//Recibiendo un array de Bytes
	public Matrix (byte[] data, byte _rows, byte _cols) {
		
		//Establecemos cantidad de filas, columnas y total de sensores 
		rows = _rows;
		cols = _cols;
		count = rows * cols;

		//Inicializamos la matriz de datos.
		matrixData = new byte[count] ;

		//Asignamos los datos al array matrixData.
		for (byte i=0, j=0; i< count; i++, j++) {
			matrixData [i] = data [i];

		}
	}

	//Constructor básíco recibiendo un total de filas y columnas y un valor inicial opcional
	public Matrix (byte _rows, byte _cols, byte initValue = 0) {

		int _count = _rows * _cols;

		count = _count;
		rows = _rows;
		cols = _cols;

		matrixData = new byte[_rows * _cols] ;

		//Inicializamos la matriz a cero
		for (int i = 0; i < _count; i++ )
			matrixData[i] = initValue;
		
	}

	//Recibiendo una cadena de texto, e indicando el separador, opcional
	public Matrix (string data, byte _rows, byte _cols, char separator = ',') {

		//Establecemos cantidad de filas, columnas y total de sensores 
		rows = _rows;
		cols = _cols;
		count = rows * cols;

		matrixData = new byte[count+1];
		//data = data.Trim(' ','\n','\r');
		//Debug.Log("Total data:" + data.Split (separator).Length);
		string [] totalData =  new string[count];
		try {
			//Debug.Log("TOtal valores entrando: " + data.Split (separator).Length);
			totalData = data.Split (separator);
			}catch{ Debug.Log ("ERROR, totalData: " + data);}

		//Asignamos los datos al array matrixData.
		int j =0;
		int i=0;
		foreach (string singleData in totalData) {

			if (ValueInRange(i) ){
				
			try{
				matrixData [j] = byte.Parse (singleData);

			}catch (System.Exception e){
					Debug.LogError ("Matrix error[" + j + "]: " + e.Message + " / Data: " + singleData);
					matrixData [j] = 0;
			}
				j++;
			}//else Debug.Log ("Valor descartado: " +i + " Valor: " +singleData );
			i++;
		}
		
	}

	internal bool ValueInRange(int i){
		string data = i.ToString();
		return i < 160;
		//Rango dentro del total de valores y fuera de los márgenes de calibrado
		return  i > OFFSET && i < 160 && i%10 != 8 && i%10 != 9;
	}

	//TODO: DEPRECATED - ReCIBIENDO ARRAY DE BYTE Y CALIBRADO 
	public Matrix (byte[] data, byte _rows, byte _cols, Matrix calibration) {
		
		rows = _rows;
		cols = _cols;
		count = rows * cols;

		//Hay una secuencia concreta en la lectura de las columnas para extraer los valores ordenados.
		//byte[] colSecuence= new byte[]{8,9,10,11,12,13,14,15,7,6,5,4,3,2,1,0};

		matrixData = new byte[count] ;


		for (byte i=0, j=0; i< count; i++, j++) {
			matrixData [i] = data [i];

//				int newValue = data [i] - calibration.matrixData [i];
//				if (newValue < 0)
//					matrixData [i] = 0;
//				else
//					//TODO: esta conversión NO PUEDE ser asi...
//					matrixData [i] = byte.Parse (newValue.ToString());
//			}
			
		}
	}

	//Constructor básíco pasando un array de byte y un tamaño total
	public Matrix (byte[] data, int size) {

		rows = 1;
		cols = 0;
		count = size;

		matrixData = new byte[count] ;

		for (byte i=0, j=0; i< count; i++, j++) {
			matrixData [i] = data [i];
		}

	}
		
	//**************************************
	//******** METODOS Y FUNCIONES *********
	//**************************************

	//29/09/16: 

	//Obtener la Magnitud de submatrices 
	public float GetMagnitud(int posIni, int posEnd) {
		float magnitud = 0;
		int posAct = posIni;

		//Recorremos la matriz acumulando los valores.
		while (posAct < posEnd) {
			magnitud += matrixData [posAct++]; 
		}

		//Dividimos el total obtenido entre el numero de valores.
		magnitud /= (posEnd - posIni);

		return magnitud;
	}

	//Obtener la Magnitud de la matriz completa.
	public float GetMagnitud() {
		float magnitud = 0;
		byte posAct = 0;

		//Recorremos la matriz acumulando los valores.
		while (posAct < count) {
			magnitud += matrixData [posAct++]; 
		}

		//Dividimos el total obtenido entre el numero de valores.
		magnitud /= count;

		return magnitud;
	}

	// Anterior:

	//Reseteamos la matrix completa a un valor determinado
	public void Reset (byte value = 0) {
		for (int i = 0; i < count; i++ )
			matrixData[i] = value;
	}

	public bool ToArray2D (ref byte [,] array2D, byte rows, byte cols) {
		int i = 0;

		//array2D = new int [rows,cols];

		for (byte col = 0; col < cols; col++) {
			for (byte row = 0; row < rows; row++) {
				try {
					array2D[col,row] = matrixData[i];
					i++;
					}catch{
						return false;
					} 
			}
		}

		return i == count? true : false; 
	}

	public bool ToIntArray2D (ref int [,] array2D, byte rows, byte cols, float scale) {
		int i = 0;

		//array2D = new int [rows,cols];

		//TODO: hay un +4 en cols para compensar el heatmap, que desconozco porque.
		for (int col = 0; col < cols+4; col++) {
			for (int row = 0; row < rows; row++) {
				try {
					array2D[Mathf.Clamp(col-4,0,cols+4),row] = (int)(matrixData[i] * scale);
					i++;
				}catch{
					return false;
				} 
			}
		}

		return i == count? true : false; 
	}

	public byte Get2DValue (byte row, byte col) {
		return matrixData[row*rows + col];
	}

	//Devolveremos un string con la secuencia de datos separados por comas
	public string ToString (string separator = ",") {
		
		string result = "";

		for (int i = 0; i < count; i++ ) {
			result += matrixData[i].ToString();
			//Si no es el último valor, separamos los valores con el separador definido
			result += i < count-1? separator : ""; 
		}

		return result;
	}



	//Devolveremos un array de objetos Text de la UI
	public void ToTextList(ref List<Text> result, float scale ) {
		for (int i = 0; i < count; i++ ) {
			result[i].text = ((int)(matrixData[i] * scale)).ToString();
		}
	}

	//Modificamos la lista de imágenes que nos pasan por referencia
	public void ToImageList(ref List<Image> result, float scale) {
				
		for (int i = 0; i < count; i++ ) {
//TODO: Refacto plis
			Vector3 rgb = Heatmap.instance.Jet (0, 255 * scale, matrixData [i]);
			result [i].color = new Color (rgb.x, rgb.y, rgb.z);//Matrix.GetColorValue(matrixData[i], scale);
		}
	}


	//Obtnemos las magnitudes parciales de la matriz actual (inicialmente de la mitad y de la otra mitad)
	public int [] GetMagnitudes (int divisions) {

		int [] magnitudes = new int[divisions]; 

		//Si hay datos
		if (matrixData != null)
			magnitudes  = CalculateMagnitudes ( divisions);

		return magnitudes;

	}

	//CAlculamos las magnitudes resultantes de dividir la matriz en secciones verticales
	public int [] CalculateMagnitudes (int pieces) {
		//Sumatorio de magnitudes parciales
		int parcial = 0;

		int posAct = 0;
		int endPosition = 0;
		int sizePiece = Mathf.RoundToInt (matrixData.Length / pieces);
		int posArray = 0;

		//Array de magnitudes a devolver.
		int [] magnitudes = new int[pieces];


		//Recorremos todo el array de datos
		while (posAct < matrixData.Length-1) {
			endPosition += sizePiece;

			//Recorremos la matriz acumulando los valores.
			while (posAct < endPosition) {
				try {
					parcial += matrixData [posAct++]; 
				} 
				catch {}
			}

			//Y añadimos el valor medio de cada parcial al array de magnitudes
			magnitudes[posArray++] = parcial / sizePiece;
			parcial = 0;

		}

		return magnitudes;	
	}

	//************************************************
	//******** METODOS Y FUNCIONES ESTATICOS *********
	//************************************************

	public static Color GetColorValue(float value, float scale = 1) {
		value = Matrix.Map (value*scale, 0, 255*scale, 0, 1);
		//float flip = (1 - (value/ 255*scale));
		//return new Color (value/ 255* scale, 0, flip);
		float flip = (1 - value);

		return new Color (value, 0, flip);
	}

	public static int GetStrCount (string strData) {
		
		string []strMatrixData =  strData.Split(',');
		
		return strMatrixData.Length;
	}

	public static float Map (float value, float minIn, float maxIn, float minOut, float maxOut) {
		return (value - minIn) * (maxOut - minOut) / (maxIn - minIn) + minOut;
	}


	//Devolveremos un array de objetos Text de la UI
	public static void TextListPositions(ref List<Text> result) {
		for (int i = 0; i < result.Count; i++ ) {
			result[i].text = i.ToString();
		}

			
	}


	public static int [] StrToMagnitudes (string data, int pieces) {
		//Array de valores spliteados
		string[] totalData = data.Split (',');

		//Sumatorio de magnitudes parciales
		int parcial = 0;


		int posAct = 0, 
			end = 0, 
			sizePiece = Mathf.RoundToInt( totalData.Length/pieces),
			posArray = 0;

		//Array de magnitudes a devolver.
		int [] magnitudes = new int[pieces];


		//Recorremos todo el array de datos
		while (posAct < totalData.Length-1) {
			end += sizePiece;

			//Recorremos la matriz acumulando los valores.
			while (posAct < end) {
				try {
					parcial += int.Parse (totalData [posAct++]); 
				} 
				catch {}
			}

			//Y añadimos el valor medio de cada parcial al array de magnitudes
			magnitudes[posArray++] = parcial / sizePiece;
			parcial = 0;

		}

		return magnitudes;	
	}


	public static float StrToMagnitude (string data, int ini, int end) {
		float magnitud = 0;
		int posAct = ini;

		string[] totalData = data.Split (',');

		//Recorremos la matriz acumulando los valores.
		while (posAct < end) {
			try{
				magnitud += float.Parse(totalData[posAct++]); 
			}catch(KeyNotFoundException e){}
		}

		//Dividimos el total obtenido entre el numero de valores.
		magnitud /= (end - ini);

		return magnitud;	
	}

	// bicubic interpolation of 2D array.
	public static int[,] Interp2d(int[,] x, int xScale, int yScale)
	{
			
		//Matriz de pixels de salida
		int [,] out_interp2d = new int[xScale,yScale]; 

		int nrowx = x.GetLength(0);
		int ncolx = x.GetLength(1);

		int nrowy = out_interp2d.GetLength(0);
		int ncoly = out_interp2d.GetLength(1);

		int x1, x2, x3, x4, y1, y2, y3, y4;
		float v1, v2, v3, v4, v;
		float xx, yy, p, q;

		for (int i = 0; i < nrowy; i++)
		{
			for (int j = 0; j < ncoly; j++)
			{
				xx = (float)(ncolx*j)/(float)(ncoly);
				yy = (float)(nrowx*i)/(float)(nrowy);

				x2 = (int)xx;
				x1 = x2 - 1;
				if (x1 < 0) x1 = 0;
				x3 = x2 + 1;
				if (x3 >= ncolx) x3 = ncolx - 1;
				x4 = x2 + 2;
				if (x4 >= ncolx) x4 = ncolx - 1;
				p = xx - x2;

				y2 = (int)yy;
				y1 = y2 - 1;
				if (y1 < 0) y1 = 0;
				y3 = y2 + 1;
				if (y3 >= nrowx) y3 = nrowx - 1;
				y4 = y2 + 2;
				if (y4 >= nrowx) y4 = nrowx - 1;
				q = yy - y2;

				v1 = cubicci(x[y1,x1], x[y1,x2], x[y1,x3], x[y1,x4], p);
				v2 = cubicci(x[y2,x1], x[y2,x2], x[y2,x3], x[y2,x4], p);
				v3 = cubicci(x[y3,x1], x[y3,x2], x[y3,x3], x[y3,x4], p);
				v4 = cubicci(x[y4,x1], x[y4,x2], x[y4,x3], x[y4,x4], p);

				v = cubicci(v1, v2, v3, v4, q);

				if (v < 0) v = 0; // to avoid negative value.

				out_interp2d[i,j] = (int)v;
			}
		}

		return out_interp2d;
	}

	// cubic Convolution Interpolation - called by Interp2d().
	public static float cubicci(float v1, float v2, float v3, float v4, float d)
	{
		float v, p1, p2, p3, p4;

		p1 = v2;
		p2 = -v1 + v3;
		p3 = 2*(v1-v2) + v3 - v4;
		p4 = -v1 + v2 - v3 + v4;

		v = p1 + d*(p2 + d*(p3 + d*p4));

		return v;
	}

	//*******************************************
	//******** OPERADORES SOBRECARGADOS *********
	//*******************************************

	//Con el operador - haremos el calibrado de mínimos
	public static Matrix operator - (Matrix a, Matrix b) {
		
		//Conservamos la dimension y el tamaño de la matriz a
		Matrix result = new Matrix (a.rows, a.cols);

		for (int i = 0; i < a.count; i++ )
			result.matrixData[i] = a.matrixData[i] < b.matrixData[i]? a.matrixData[i]: b.matrixData[i];

		return result;
	}

	//Con el operador + haremos el calibrado de máximos
	public static Matrix operator + (Matrix a, Matrix b) {

		//Conservamos la dimension y el tamaño de la matriz a
		Matrix result = new Matrix (a.rows, a.cols);

		for (int i = 0; i < a.count; i++ )
			result.matrixData[i] = a.matrixData[i] > b.matrixData[i]? a.matrixData[i]: b.matrixData[i];

		return result;
	}




}