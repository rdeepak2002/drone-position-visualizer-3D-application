using Firesplash.UnityAssets.SocketIO;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


#if HAS_JSON_NET
using Newtonsoft.Json;
#endif

public class MultiSceneExampleScript : MonoBehaviour
{
    public SocketIOCommunicator sioCom;

    [Serializable]
    struct ItsMeData
    {
        public string version;
    }

    [Serializable]
    struct ServerTechData
    {
        public string timestamp;
        public string podName;
    }

    //We are defining a Coroutine to swittch the scene and wait for the load to complete, but this is no requirement.
    //This could also be done in various other ways.
    IEnumerator SwitchSceneAndReply()
    {
        Debug.Log("Coroutine: Now loading the second scene");

        //start switching the scene
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync("MSE-TargetScene");

        //wait until the new scene is fully loaded
        yield return new WaitUntil(() => loadOperation.isDone);

        Debug.Log("Coroutine: The scene has loaded. Emitting next message to the server.");

        //Now send the next package to the server
        ItsMeData me = new ItsMeData()
        {
            version = Application.unityVersion
        };
        sioCom.Instance.Emit("ItsMe", JsonUtility.ToJson(me), false);
    }



    void Start()
    {
        //Make the gameObject carrying this script survive a scene change. This is important!
        DontDestroyOnLoad(gameObject);


        sioCom.Instance.On("connect", (string data) => {
            Debug.Log("LOCAL: Hey, we are connected!");
            sioCom.Instance.Emit("KnockKnock");
        });


        //When the server sends WhosThere, we will switch the scene
        sioCom.Instance.On("WhosThere", (string payload) =>
        {
            Debug.Log("The server asked who we are... Switching scenes and then answering back");

            //This runs the previously defined coroutine
            StartCoroutine(SwitchSceneAndReply());
        });









        //The following code is not related to the scene changing.





        sioCom.Instance.On("Welcome", (string payload) =>
        {
            Debug.Log("Now - after the scene change - the server said hi :)");

            Debug.Log("SERVER: " + payload);

            sioCom.Instance.Emit("Goodbye", "Thanks for talking to me!", true);
        });




        //When the conversation is done, the server will close our connection after we said Goodbye
        sioCom.Instance.On("disconnect", (string payload) => {
            if (payload.Equals("io server disconnect"))
            {
                Debug.Log("Disconnected from server.");
            }
            else
            {
                Debug.LogWarning("We have been unexpectedly disconnected. This will cause an automatic reconnect. Reason: " + payload);
            }
        });

        sioCom.Instance.Connect("https://sio-v4-example.unityassets.i01.clu.firesplash.de", false);
    }
}
