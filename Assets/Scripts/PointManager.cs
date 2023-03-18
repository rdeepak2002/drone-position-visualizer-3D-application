using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class PointManager : MonoBehaviour
{
    // public Toggle showTextToggle;
    private GameObject showTextToggle;
    private bool showPositionText = false;

    // Start is called before the first frame update
    void Start()
    {
        showTextToggle = GameObject.Find("Show Text Toggle");
        // show position text toggle
        if (showTextToggle == null)
        {
            Debug.Log("Unable to find toggle text object");
        }
        else
        {
            var showPositionPathToggleComponent = showTextToggle.GetComponent<Toggle>();
            showPositionText = showPositionPathToggleComponent.isOn ? true : false;
            showPositionPathToggleComponent.onValueChanged.AddListener(delegate {
                showPositionText = showPositionPathToggleComponent.isOn ? true : false;
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (showTextToggle != null)
        {
            transform.Find("Canvas").gameObject.SetActive(showPositionText);
            GameObject textGameObject = transform.Find("Canvas").Find("Position Text").gameObject;
            TextMeshProUGUI textmeshPro = textGameObject.GetComponent<TextMeshProUGUI>();
            // float x = transform.position.x;
            // float y = transform.position.y;
            // float z = transform.position.z;
            float x = -1 * transform.position.z;
            float y = -1 * transform.position.x;
            float z = transform.position.y;
            textmeshPro.text = "(" + x.ToString("0.00") + ", " + y.ToString("0.00") + ", " + z.ToString("0.00") + ")";
        }
    }
}
