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
    public GameObject linePrefab;
    private GameObject tracePathToggle;
    private GameObject showTextToggle;
    public bool sendTestData = false;
    private bool tracePath = false;
    private bool showPositionText = false;

    private GameObject startMarkerGameObject = null;
    private GameObject endMarkerGameObject = null;
    private int count = 0;

    private void Start()
    {
        showTextToggle = GameObject.Find("Show Text Toggle");
        tracePathToggle = GameObject.Find("Trace Path Toggle");

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
            {
                if (tracePath)
                {
                    bool canDrawLine = false;
                    Vector3 oldPosition = new Vector3();
                    if (endMarkerGameObject != null)
                    {
                        oldPosition = endMarkerGameObject.transform.position;
                        canDrawLine = true;
                    } 
                    else if (startMarkerGameObject != null)
                    {
                        oldPosition = startMarkerGameObject.transform.position;
                        canDrawLine = true;
                    }
                    if (canDrawLine)
                    {
                        var lineDirection = position - oldPosition;
                        var middlePosition = oldPosition + 0.5f * lineDirection;
                        var rot = Quaternion.LookRotation(lineDirection, Vector3.up);
                        GameObject lineGameObject = Instantiate(linePrefab, middlePosition, rot);
                        float scaleFactor = 1.0f;
                        lineGameObject.transform.Find("GameObject").transform.localScale = new Vector3(0.1f, lineDirection.magnitude / scaleFactor, 0.1f);
                    }
                }
            }
            // destroy old end marker
            if (endMarkerGameObject != null)
            {
                // if (!tracePath)
                // {
                //     Destroy(endMarkerGameObject);
                // }
                // Destroy(endMarkerGameObject);
            }
            // create instance of the point
            GameObject newlySpawnedObject = null;
            if (startMarkerGameObject == null)
            {
                startMarkerGameObject = Instantiate(startPointPrefab, position, orientation);
                newlySpawnedObject = startMarkerGameObject;
            }
            else if (endMarkerGameObject == null)
            {
                endMarkerGameObject = Instantiate(endPointPrefab, position, orientation);
                newlySpawnedObject = endMarkerGameObject;
            }
            else
            {
                newlySpawnedObject = endMarkerGameObject;
                newlySpawnedObject.transform.position = position;
                newlySpawnedObject.transform.rotation = orientation;
            }
            // parent point to map objects to keep scene organized
            newlySpawnedObject.transform.parent = mapObjects.transform;
            // set color of new point
            if (deviceData.Confidence.Equals("0x1"))
            {
                newlySpawnedObject.transform.Find("Mesh").GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
                // newlySpawnedObject.transform.Find("Mesh_Alt/Root/Cube/Cube_0").GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
                // Transform result = newlySpawnedObject.transform.Find("Root/Cube/Cube_0");
                // if (result)
                // {
                //     newlySpawnedObject.transform.Find("Mesh/Root/Cube/Cube_0").GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
                // }
                // newlySpawnedObject.transform.Find("Mesh/Root/Cube/Cube_0").GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
                // newlySpawnedObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
            }
            else if (deviceData.Confidence.Equals("0x2"))
            {
                newlySpawnedObject.transform.Find("Mesh").GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);

                // newlySpawnedObject.transform.Find("Cube_0").GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);
                // newlySpawnedObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);
            }
            else if (deviceData.Confidence.Equals("0x3"))
            {
                newlySpawnedObject.transform.Find("Mesh").GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);

                // newlySpawnedObject.transform.Find("Cube_0").GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
                // newlySpawnedObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
            }
            else
            {
                newlySpawnedObject.transform.Find("Mesh").GetComponent<MeshRenderer>().material.SetColor("_Color", Color.black);

                // newlySpawnedObject.transform.Find("Cube_0").GetComponent<MeshRenderer>().material.SetColor("_Color", Color.black);
                // newlySpawnedObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", Color.black);
            }
            count++;
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