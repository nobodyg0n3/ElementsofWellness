using System;
using System.Collections; 
using System.Collections.Generic; 
using System.Net; 
using System.Net.Sockets; 
using System.Text; 
using System.Threading; 
using UnityEngine;  

public class OptitrackStreamServer : MonoBehaviour
{
    private TcpListener tcpListener;
    private Thread tcpListenerThread;
    private List<TcpClient> connectedTcpClient;
    GameObject[] optitrackObjects;
    // Start is called before the first frame update
    void Start()
    {
        optitrackObjects = GetOptiTrackObjects();
        connectedTcpClient = new List<TcpClient>();
        tcpListenerThread = new Thread (new ThreadStart(ListenForIncommingRequests));       
        tcpListenerThread.IsBackground = true;      
        tcpListenerThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        SendOptiTrackObjectInfo();
    }
    private GameObject[] GetOptiTrackObjects() {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("OptiTrack");
        return objects;
    }
    private void ListenForIncommingRequests () {        
        try {           
            // Create listener on localhost port 8052.          
            tcpListener = new TcpListener(IPAddress.Parse("0.0.0.0"), 8052);          
            tcpListener.Start();              
            Debug.Log("Server is listening");
            while(true){
                connectedTcpClient.Add(tcpListener.AcceptTcpClient());
            }
        }       
        catch (SocketException socketException) {           
            Debug.Log("SocketException " + socketException.ToString());         
        }     
    }   

    private void SendMessage(string serverMessage) {
        if (connectedTcpClient.Count == 0) {             
            return;         
        }       
        
        try {
            foreach (TcpClient client in connectedTcpClient){
                // Get a stream object for writing.
                try{
                    Debug.Log("How many people connected? "+connectedTcpClient.Count);
                    NetworkStream stream = client.GetStream();          
                    if (stream.CanWrite) {                 
                        // string serverMessage = "This is a message from your server.";      
                        // Convert string message to byte array.                 
                        byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);               
                        // Write byte array to socketConnection stream.           
                        stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);               
                        // Debug.Log("Server sent his message - should be received by client");           
                    } 
                }
                catch (Exception e){
                    // release non-connected socket client
                    for (int i = 0; i < connectedTcpClient.Count; i++){
                        if (client == connectedTcpClient[i]){
                            connectedTcpClient.RemoveAt(i);
                            break;
                        }
                    }
                    // Debug.Log("Any exception: "+ e);
                } 
                
            }      
        }       
        catch (SocketException socketException) {             
            Debug.Log("Socket exception: " + socketException);         
        }   
    }

    private void SendOptiTrackObjectInfo(){
        foreach (GameObject optitrackobject in optitrackObjects){
            string name = optitrackobject.name;
            string position = optitrackobject.transform.position.ToString();
            string rotation = optitrackobject.transform.rotation.ToString();
            string message = "#head#"+name+position+rotation+"#tail#"+"#splitter#";
            SendMessage(message);
        }
    }
}
