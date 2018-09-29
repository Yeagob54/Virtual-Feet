using UnityEngine;
using System.Collections;
using System.IO;
using System.Net.Sockets;
 

public class StreamReader : MonoBehaviour {

	//Conection 
	internal bool socketReady = false;
     TcpClient mySocket;
     NetworkStream theStream;
	System.IO.StreamReader theReader;
	//BinaryReader binaryReader;
	public bool autoConect;
	public string ip;
	public string name;
	public int port;

	//Sistema de eventos
	public delegate void NewDataRecived();
	public static event NewDataRecived OnDataRecived;
	[System.NonSerialized]
	public int eventCount;
	float lastTime;

	//Recording System
	bool recording = false;
	StreamWriter writer;
	string lastFileName;


	string strMessage = string.Empty;

	// Use this for initialization
	void Start () {
		print ("Ruta de guardado de datos: " +Application.persistentDataPath);

		//Activamos el evento de llegada de datos
		OnDataRecived += DataRecived;

		if (autoConect) 
			//Iniciamos la conexión y comenzamos a mostrar datos
			SocketConnect();
	}


	// Update is called once per frame
	void Update () {
		
		if (socketReady) {
			//Guardamos lalíena del streaming en strMessage 
			strMessage = ReadSocket();

			//Si llega una matriz válisa
			if (strMessage != null) {
				
				//On recording: escribimos en archivo
				if (recording)
					WriteToFile(strMessage);

				//Lanzamos el evento de lectura de datos.
				OnDataRecived();
			}
		}
	}

	public string GetStream() {
		return strMessage;
	}



	void WriteToFile (string line) {
		string timeValue = System.DateTime.Now.Hour.ToString () +
		                   System.DateTime.Now.Minute.ToString () +
		                   System.DateTime.Now.Second.ToString () +
		                   System.DateTime.Now.Millisecond.ToString ();

		writer.WriteLine(line + "/" + timeValue);
	}
	
	public void StartRecording() {
		if (recording) {
			recording = false;
			writer.Flush();
			writer.Close();
			Application.OpenURL (lastFileName);
		} else{
			string filename = "\\" + System.DateTime.Now.ToString ().Replace ("/", "-").Replace (":", "_");
			writer = new StreamWriter (Application.persistentDataPath + filename + ".spm");
			recording = true;
			lastFileName = Application.persistentDataPath + filename + ".spm";
		}

		//TODO: Pasar como evento.
		UIController panelData = FindObjectOfType<UIController> ();
		panelData.RecButtonState (recording);
	}

	//Este método es llamado desde el botón de conexión, TODO: deberíamos crear un lístener al botón. Para hacer el UIController independiente. 
	public void SocketConnect () {
		
		if (socketReady)
			CloseSocket ();
		else {
			//Si no se asigna una IP en el inspector, se busca un UIController para pedírsela
			if (ip.Equals (string.Empty)) {
				UIController panelData = FindObjectOfType<UIController> ();
				ip = panelData.GetIP ();
				port = panelData.GetPort ();
				name = panelData.GetName ();
				panelData.CurrentState = PanelState.CONECTED;
			}

			//TODO:Try y mostrar error en panel de conexión.
			print ("conectando a: " + ip + " : " +port);
			mySocket = new TcpClient (ip, port);
			theStream = mySocket.GetStream ();
			theReader = new  System.IO.StreamReader (theStream);
			//binaryReader = new BinaryReader (theStream);
			socketReady = true;

			//TODO: Lanzar eventos...
		}
	}
    
	string ReadSocket () {
		if (theStream.DataAvailable)
			return theReader.ReadLine ();
		
		return null;
	}


	void CloseSocket(){
 
        if (!socketReady)
            return;

		theReader.Close();
        mySocket.Close();
        socketReady = false;

		//TODO: Lanzar eventos...
		FindObjectOfType<UIController> ().CurrentState = PanelState.DISCONECTED;
	
    }

	void DataRecived () {
		if (1 <= lastTime) { 

			//TODO: Lanzar eventos...
			FindObjectOfType<UIController> ().SetEvents (eventCount);
			eventCount = 0;
			lastTime = 0;
		} else {
			lastTime += Time.deltaTime;
			eventCount++;
		}
	
	}

}
