using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

public class DinamicTCPTerrain : MonoBehaviour {
	const byte ROWS = 10; 
	const byte COLS = 12;
	const int READ_BUFFER_SIZE = ROWS*COLS;

	public float sensitivity;
	public int port = 23;
	public string ip = "192.168.10.3";

	private byte[] readBuffer = new byte[READ_BUFFER_SIZE];
	private Terrain terrain;
	private int terrainWidth, terrainLength;
	private TcpClient client;
	private float[,] data;

	// Use this for initialization
	void Start () {
		terrain = GetComponent<Terrain> ();
		terrainWidth = terrain.terrainData.heightmapWidth;
		terrainLength = terrain.terrainData.heightmapHeight;
		data = new float[terrainWidth, terrainLength];

		client = new TcpClient(ip, port);
		client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(DoRead), null);

	}
	
	// Update is called once per frame
	void Update () {
		GenerateTerrain(data);
	}

	
	private void DoRead(IAsyncResult ar)
		{ 
			int BytesRead;
			string strMessage=string.Empty;
			try
			{
				// Finish asynchronous read into readBuffer and return number of bytes read.
				BytesRead = client.GetStream().EndRead(ar);
				if (BytesRead < 1) 
				{
					// if no bytes were read server has close.  
					print("Disconnected");
					return;
				}
				
				strMessage = Encoding.ASCII.GetString(readBuffer, 0, BytesRead - 2);

				if (Input.GetMouseButton(0)){
					print (strMessage);
					StrToHeights(strMessage);
				}

				//GenerateTerrain();

				// Start a new asynchronous read into readBuffer.
				client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(DoRead), null);
 
			} 
			catch
			{
				print("Disconnected");
			}
		}

	float[,] StrToHeights (string strData) {

		string [] splitedData = strData.Split(',');

		int chunkWidth =  (int) (terrainWidth / COLS);
		int chunkLength =  (int) (terrainLength / ROWS);

		for (int i = 0; ROWS > i; i++) {
			for (int j = 0; COLS > j; j++) {

				for ( int k = i*chunkWidth; k < (i+1)*chunkWidth; k++) {
					for (int l = j*chunkLength; l < (j+1)*chunkLength; l++){
						//QUITAR TRY!!!	
						try {
							//Añadimos los datos 
							data[k,l] = Int32.Parse(splitedData[i*ROWS + j]) * sensitivity; 
						}catch
						{}
					}
				}
			}
		}


		return data;
		
	}
	void GenerateTerrain(float[,] heights)
	{

		terrain.terrainData.SetHeights(0, 0, heights );
	}

}
