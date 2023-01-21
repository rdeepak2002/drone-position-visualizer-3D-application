using System;
using Firesplash.UnityAssets.SocketIO;
using UnityEngine;
using UnityEngine.UI;
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
    public GameObject startPointPrefab;
    public GameObject endPointPrefab;
    public Toggle tracePathToggle;
    public Toggle showTextToggle;
    public bool sendTestData = false;
    private bool tracePath = false;
    private bool showPositionText = false;

    private GameObject startMarkerGameObject = null;
    private GameObject endMarkerGameObject = null;

    private void Start()
    {
        // trace path toggle
        var tracePathToggleComponent = tracePathToggle.GetComponent<Toggle>();
        tracePath = tracePathToggleComponent.isOn ? true : false;
        tracePathToggleComponent.onValueChanged.AddListener(delegate {
            tracePath = tracePathToggleComponent.isOn ? true : false;
        });
        
        // show position text toggle
        var showPositionPathToggleComponent = showTextToggle.GetComponent<Toggle>();
        showPositionText = showPositionPathToggleComponent.isOn ? true : false;
        showPositionPathToggleComponent.onValueChanged.AddListener(delegate {
            showPositionText = showPositionPathToggleComponent.isOn ? true : false;
        });

        GameObject mapObjects = GameObject.Find("MapObjects");

        _socket = GetComponent<SocketIOCommunicator>();
        
        _socket.Instance.On("connect", (payload) =>
        {
            Debug.Log("Connected! Socket ID: " + _socket.Instance.SocketID);
            if (sendTestData)
            {
                StartCoroutine(Test());
            }
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
            // uncomment to log data received from socket server
            // Debug.Log("Received: " + data);
            // parse data from server to object
            DeviceData deviceData = JsonUtility.FromJson<DeviceData>(data);
            var position = new Vector3(deviceData.Position.x, deviceData.Position.y, deviceData.Position.z);
            var orientation = new Quaternion(deviceData.Orientation.x, deviceData.Orientation.y,
                deviceData.Orientation.z, deviceData.Orientation.w);
            // destroy old end marker
            if (endMarkerGameObject != null && startMarkerGameObject != endMarkerGameObject)
            {
                if (!tracePath)
                {
                    Destroy(endMarkerGameObject);
                }
            }
            // create instance of the point
            if (startMarkerGameObject == null)
            {
                endMarkerGameObject = Instantiate(startPointPrefab, position, orientation);
            }
            else
            {
                endMarkerGameObject = Instantiate(endPointPrefab, position, orientation);
            }
            // parent point to map objects to keep scene organized
            endMarkerGameObject.transform.parent = mapObjects.transform;
            // set color of new point
            if (deviceData.Confidence.Equals("0x1"))
            {
                endMarkerGameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
            }
            else if (deviceData.Confidence.Equals("0x2"))
            {
                endMarkerGameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);
            }
            else if (deviceData.Confidence.Equals("0x3"))
            {
                endMarkerGameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
            }
            else
            {
                endMarkerGameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", Color.black);
            }
            // set start marker
            if (startMarkerGameObject == null)
            {
                startMarkerGameObject = endMarkerGameObject;
            }
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
        _socket.Instance.Emit("device-1", "{\"Position\":{\"x\":1,\"y\":0,\"z\":0},\"Orientation\":{\"x\":0,\"y\":0,\"z\":0,\"w\":0},\"Confidence\":\"0x1\"}", false);
        yield return new WaitForSeconds(1);
        _socket.Instance.Emit("device-1", "{\"Position\":{\"x\":0,\"y\":2,\"z\":0},\"Orientation\":{\"x\":0,\"y\":0,\"z\":0,\"w\":0},\"Confidence\":\"0x2\"}", false);
        yield return new WaitForSeconds(1);
        _socket.Instance.Emit("device-1", "{\"Position\":{\"x\":0,\"y\":0,\"z\":3},\"Orientation\":{\"x\":0,\"y\":0,\"z\":0,\"w\":0},\"Confidence\":\"0x3\"}", false);
        yield return new WaitForSeconds(1);
        _socket.Instance.Emit("device-1", "{\"Position\":{\"x\":1.1,\"y\":1.2,\"z\":1.3},\"Orientation\":{\"x\":0,\"y\":0,\"z\":0,\"w\":0},\"Confidence\":\"0x3\"}", false);
        yield return new WaitForSeconds(1);
        _socket.Instance.Emit("device-1", "{\"Position\":{\"x\":1.2,\"y\":1.2,\"z\":1.3},\"Orientation\":{\"x\":0,\"y\":0,\"z\":0,\"w\":0},\"Confidence\":\"0x3\"}", false);
        yield return new WaitForSeconds(1);
        _socket.Instance.Emit("device-1", "{\"Position\":{\"x\":1.3,\"y\":1.2,\"z\":1.3},\"Orientation\":{\"x\":0,\"y\":0,\"z\":0,\"w\":0},\"Confidence\":\"0x3\"}", false);
        yield return new WaitForSeconds(1);
        _socket.Instance.Emit("device-1", "{\"Position\":{\"x\":1.4,\"y\":1.2,\"z\":1.3},\"Orientation\":{\"x\":0,\"y\":0,\"z\":0,\"w\":0},\"Confidence\":\"0x3\"}", false);
        yield return new WaitForSeconds(1);
        _socket.Instance.Emit("device-1", "{\"Position\":{\"x\":1.5,\"y\":1.2,\"z\":1.3},\"Orientation\":{\"x\":0,\"y\":0,\"z\":0,\"w\":0},\"Confidence\":\"0x3\"}", false);
        yield return new WaitForSeconds(1);
        _socket.Instance.Emit("device-1", "{\"Position\":{\"x\":1.6,\"y\":1.2,\"z\":1.3},\"Orientation\":{\"x\":0,\"y\":0,\"z\":0,\"w\":0},\"Confidence\":\"0x3\"}", false);
    }
}