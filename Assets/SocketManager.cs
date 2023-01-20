using System;
using Firesplash.UnityAssets.SocketIO;
using UnityEngine;
using System.Collections;

[Serializable]
public class Position
{
    public float x;
    public float y;
    public float z;
}

[Serializable]
public class Orientation
{
    public float x;
    public float y;
    public float z;
    public float w;
}

[Serializable]
public class DeviceData
{
    // {"Position":{"x":0,"y":0,"z":0},"Orientation":{"x":1,"y":0,"z":0,"w":0},"Confidence":"0x1"}
    public Position Position;
    public Orientation Orientation;
    public string Confidence;
}

public class SocketManager : MonoBehaviour {
    SocketIOCommunicator _socket;

    private void Start()
    {
        _socket = GetComponent<SocketIOCommunicator>();
        
        _socket.Instance.On("connect", (payload) =>
        {
            Debug.Log("Connected! Socket ID: " + _socket.Instance.SocketID);
            // uncomment to test sending a few data points
            StartCoroutine(Test());
        });
        
        _socket.Instance.On("disconnect", (payload) =>
        {
            Debug.LogWarning("Disconnected: " + payload);
        });

        _socket.Instance.On("reconnect_attempt", (payload) =>
        {
            Debug.Log("Attempting to reconnect...");
        });

        _socket.Instance.On("reconnect", (payload) =>
        {
            Debug.Log("Reconnected!");
        });
        
        _socket.Instance.On("device-data", (data) =>
        {
            Debug.Log("Received: " + data);
            DeviceData deviceData = JsonUtility.FromJson<DeviceData>(data);
        });

        _socket.Instance.Connect();

        // uncomment to test this script sending data to our server (and handling the receiving of data here)
        // StartCoroutine(Tester());
    }
    
    /**
     * Method to test whether we are able to handle receiving data
     */
    IEnumerator Tester()
    {
        while(true)
        {
            yield return new WaitUntil(() => _socket.Instance.IsConnected());
            // _socket.Instance.Emit("device-1", "test", true);
            _socket.Instance.Emit("device-1", "{\"Position\":{\"x\":0,\"y\":0,\"z\":0},\"Orientation\":{\"x\":0,\"y\":0,\"z\":0,\"w\":0},\"Confidence\":\"0x2\"}", false);
            yield return new WaitForSeconds(3);
        }
    }

    IEnumerator Test()
    {
        yield return new WaitUntil(() => _socket.Instance.IsConnected());
        yield return new WaitForSeconds(1);
        _socket.Instance.Emit("device-1", "{\"Position\":{\"x\":0,\"y\":0,\"z\":0},\"Orientation\":{\"x\":1,\"y\":0,\"z\":0,\"w\":0},\"Confidence\":\"0x1\"}", false);
        yield return new WaitForSeconds(1);
        _socket.Instance.Emit("device-1", "{\"Position\":{\"x\":0,\"y\":0,\"z\":0},\"Orientation\":{\"x\":0,\"y\":1,\"z\":0,\"w\":0},\"Confidence\":\"0x2\"}", false);
        yield return new WaitForSeconds(1);
        _socket.Instance.Emit("device-1", "{\"Position\":{\"x\":0,\"y\":0,\"z\":0},\"Orientation\":{\"x\":0,\"y\":0,\"z\":1,\"w\":0},\"Confidence\":\"0x3\"}", false);
    }
}