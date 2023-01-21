using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float scrollScale = 100.0f;
    private float mouseMoveScale = 20.0f;
    private float mouseRotateScale = 200.0f;
    private Vector2 mousePosition;
    
    // Start is called before the first frame update
    void Start()
    {
        mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 mouseDeltaPosition = newMousePosition - mousePosition;
        mousePosition = newMousePosition;
        // dragging with left click moves camera laterally
        if (Input.GetMouseButton(0))
        {
            this.GetComponent<Transform>().position += -1 * this.GetComponent<Transform>().right * mouseDeltaPosition.x * mouseMoveScale * Time.deltaTime;
            this.GetComponent<Transform>().position += -1 * this.GetComponent<Transform>().up * mouseDeltaPosition.y * mouseMoveScale * Time.deltaTime;
        }
        // scroll moves camera back and forth
        this.GetComponent<Transform>().position += this.GetComponent<Transform>().forward * Input.mouseScrollDelta.y * scrollScale * Time.deltaTime;
        // dragging with right click rotates camera
        if (Input.GetMouseButton(1))
        {
            float x = -1 * mouseDeltaPosition.x * mouseRotateScale * Time.deltaTime;
            float y = mouseDeltaPosition.y * mouseRotateScale * Time.deltaTime;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x + y, transform.eulerAngles.y + x, 0);
        }
    }
}
