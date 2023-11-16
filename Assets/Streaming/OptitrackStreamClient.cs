using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class OptitrackStreamClient : MonoBehaviour
{
    private string server_ipaddress = "172.26.98.224"; // Classroom computer's ipaddress

    private TcpClient socketConnection;     
    private Thread clientReceiveThread;
    GameObject[] optitrackObjects;
    private List<string> message_queue;
    // Start is called before the first frame update
    void Start()
    {
        optitrackObjects = GetOptiTrackObjects();
        message_queue = new List<string>();
        ConnectToTcpServer();
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space)) {
        // }
        if(message_queue.Count > 0){
            var message = message_queue[0];
            message_queue.RemoveAt(0);
            if (message != null){
                SetOptiTrackObjectProperties(message);
                while(message_queue.Count > 10){
                    message_queue.RemoveAt(0);
                }
            }
        }        
    }

    void OnDestroy()
    {
        Debug.Log("Close stream");
        socketConnection.Close();
    }

    private GameObject[] GetOptiTrackObjects() {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("OptiTrack");
        return objects;
    }
    private void ConnectToTcpServer () {        
        try {           
            clientReceiveThread = new Thread (new ThreadStart(ListenForData));          
            clientReceiveThread.IsBackground = true;            
            clientReceiveThread.Start();    
        }       
        catch (Exception e) {           
            Debug.Log("On client connect exception " + e);      
        }   
    }  

    private void ListenForData() {      
        try {           
            socketConnection = new TcpClient(server_ipaddress, 8052);            
            Byte[] bytes = new Byte[1024];             
            while (true) {              
                // Get a stream object for reading              
                using (NetworkStream stream = socketConnection.GetStream()) {                   
                    int length;                     
                    // Read incomming stream into byte arrary.                  
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) {                       
                        var incommingData = new byte[length];                       
                        Array.Copy(bytes, 0, incommingData, 0, length);                         
                        // Convert byte array to string message.                        
                        string serverMessage = Encoding.ASCII.GetString(incommingData);
                        string[] chunks = serverMessage.Split("#splitter#");
                        foreach (string chunk in chunks){
                            if (chunk.Contains("#head#") && chunk.Contains("#tail#")){
                                string message = chunk.Split("#head#")[1].Split("#tail#")[0];
                                message_queue.Add(message);   
                            }
                        }
                        // Debug.Log("server message received as: " + serverMessage);             
                    }               
                }           
            }         
        }         
        catch (SocketException socketException) {             
            Debug.Log("Socket exception: " + socketException);         
        }     
    }

    private void SendMessage() {         
        if (socketConnection == null) {             
            return;         
        }       
        try {           
            // Get a stream object for writing.             
            NetworkStream stream = socketConnection.GetStream();            
            if (stream.CanWrite) {                 
                string clientMessage = "This is a message from one of your clients.";               
                // Convert string message to byte array.                 
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);               
                // Write byte array to socketConnection stream.                 
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);                 
                Debug.Log("Client sent his message - should be received by server");        
            }         
        }       
        catch (SocketException socketException) {             
            Debug.Log("Socket exception: " + socketException);         
        }     
    }

    private void SetOptiTrackObjectProperties(string message){
        string name = MessageParerName(message);
        Vector3 position = MessageParerPosition(message);
        Vector4 rotation = MessageParerRotation(message);
        Quaternion q = new Quaternion(rotation.x,rotation.y,rotation.z,rotation.w);
        GameObject optitrackobject = GameObject.Find(name);
        if (optitrackobject==null){
            return;
        }
        optitrackobject.transform.position = position;
        optitrackobject.transform.rotation = q;
    }

    private string MessageParerName(string message){
        string name = message.Split("(")[0];
        return name;
    }

    private Vector3 MessageParerPosition(string message){
        string position_str = message.Split("(")[1].Split(")")[0];
        string[] position_arr = position_str.Split(",");
        float x = float.Parse(position_arr[0]);
        float y = float.Parse(position_arr[1]);
        float z = float.Parse(position_arr[2]);
        Vector3 position = new Vector3(x,y,z);
        return position;
    }

    private Vector4 MessageParerRotation(string message){
        string rotation_str = message.Split("(")[2].Split(")")[0];
        string[] rotation_arr = rotation_str.Split(",");
        float x = float.Parse(rotation_arr[0]);
        float y = float.Parse(rotation_arr[1]);
        float z = float.Parse(rotation_arr[2]);
        float w = float.Parse(rotation_arr[3]);
        Vector4 rotation = new Vector4(x,y,z,w);
        return rotation;
    }
}
