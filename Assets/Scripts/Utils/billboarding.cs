using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class billboarding : MonoBehaviour
{
    Camera mainCam;
    [SerializeField] Vector3 billboardDirection = new Vector3(1, 1, 1);
    // Start is called before the first frame update
    void Start()
    {
        //mainCam = GameObject.Find("Follow Camera").GetComponent<Camera>();
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(mainCam.transform);

        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x * billboardDirection.x,
            transform.rotation.eulerAngles.y * billboardDirection.y,
            transform.rotation.eulerAngles.z * billboardDirection.z);
    }
}
