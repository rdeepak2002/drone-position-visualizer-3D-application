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
            textmeshPro.text = "(1, 1, 2)";
        }
    }
}
