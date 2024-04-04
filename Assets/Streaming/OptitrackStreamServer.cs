using System;
using System.Collections; 
using System.Collections.Generic;
using System.Linq;
using System.Net; 
using System.Net.Sockets; 
using System.Text; 
using System.Threading; 
using UnityEngine;  

public class OptitrackStreamServer : MonoBehaviour
{
    public int port = 0;
    private TcpListener tcpListener;
    private Thread tcpListenerThread;
    private List<TcpClient> connectedTcpClient;
    
    GameObject[] optitrackObjects;
    // Start is called before the first frame update
    void Start()
    {
        if (port == 0)
        {
            Debug.LogWarning("Port is 0. Are you sure it got set correctly");
        }
        optitrackObjects = GetOptiTrackObjects();
        connectedTcpClient = new List<TcpClient>();
        tcpListenerThread = new Thread (new ThreadStart(ListenForIncommingRequests));       
        tcpListenerThread.IsBackground = true;      
        tcpListenerThread.Start();
        Debug.Log("Opening new socket. IP: " + GetLocalIPv4() + " Port: " + port.ToString());
    }
    public string GetLocalIPv4()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        return host.AddressList[host.AddressList.Length - 1].ToString();
        
    }

    // Update is called once per frame
    void FixedUpdate()
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
            tcpListener = new TcpListener(IPAddress.Parse("0.0.0.0"), port);          
            tcpListener.Start();              
            Debug.Log("Server is listening");
            while(true){
                connectedTcpClient.Add(tcpListener.AcceptTcpClient());
            }
        }       
        catch (SocketException socketException) {
            Debug.LogError("SocketException " + socketException.ToString());
            Debug.Log("This was probably because there's a socket open on this port. Try changing the port number.");
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
