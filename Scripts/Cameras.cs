using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cameras : MonoBehaviour
{

    Camera[] cameras = new Camera[3];
    public Camera firstPerson, backCam, frontCam; 
    int currentCam = 0;

    public Text plusTecken;

    public Transform arm;
    public GameObject L_axel, R_axel, spine_overdel, huvud_brytning;
    bool changeCamera = false;

    public GameObject f3Info;
    Text fpsText, xyzText;

    // Start is called before the first frame update
    void Start()
    {
        firstPerson = firstPerson.GetComponent<Camera>();
        backCam = backCam.GetComponent<Camera>();
        frontCam = frontCam.GetComponent<Camera>();

        fpsText = f3Info.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
        xyzText = f3Info.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();

        cameras[0] = firstPerson;
        cameras[1] = backCam;
        cameras[2] = frontCam;

        cameras[0].enabled = true;


        //f3Info.GetComponent<RectTransform>().position = new Vector3(f3Info.GetComponent<RectTransform>().rect.width / 2, -1 * (f3Info.GetComponent<RectTransform>().rect.height / 2), 0);
        //Debug.Log("width: " + f3Info.GetComponent<RectTransform>().rect.width);
        //Debug.Log("height: " + f3Info.GetComponent<RectTransform>().rect.height);
        //Debug.Log("pos x: " + f3Info.GetComponent<RectTransform>().rect.x);
        //Debug.Log("pos y: " + f3Info.GetComponent<RectTransform>().rect.y);

        BreakAnimation();
    }

    // Update is called once per frame
    void Update()
    {

        if(f3Info.activeInHierarchy)
        {
            int fps = (int)(1f / Time.deltaTime);
            fpsText.text = fps + " fps";
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            changeCamera = true; 

            currentCam = currentCam + 1;
            if (currentCam == 3)
            {
                currentCam = 0;
            }

            for (int i = 0; i < 3; i++)
            {
                cameras[i].enabled = false;
            }

            cameras[currentCam].enabled = true;

        }
        else
        {
            changeCamera = false; 
        }

        if (Input.GetKeyDown(KeyCode.F3))
            f3Info.SetActive(!f3Info.activeInHierarchy);

        if(cameras[0].enabled)
        {
            
            arm.gameObject.GetComponent<MeshRenderer>().enabled = true;
            plusTecken.enabled = true;

            if (changeCamera)
                BreakAnimation();
        }
        else
        {
            arm.gameObject.GetComponent<MeshRenderer>().enabled = false;
            plusTecken.enabled = false;
            
        }

        if (cameras[1].enabled && changeCamera)
            UnbreakAnimation();

    }

    void FixedUpdate()
    {
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;

        xyzText.text = "XYZ: " + x + " / " + y + " / " + z;

    }

    void BreakAnimation() 
    {
        //GameObject L_parent = new GameObject();
        //GameObject R_parent = new GameObject();

        //L_parent.transform.SetParent(spine_overdel.transform);
        //R_parent.transform.SetParent(spine_overdel.transform);

        //L_axel.transform.SetParent(L_parent.transform);
        //R_axel.transform.SetParent(R_parent.transform);
        //huvud_brytning.transform.SetParent(spine_overdel.transform);
    }

    void UnbreakAnimation() // problem: kan inte gå tillbaka till animation i artmar efteråt ändå
    {
        //GameObject L_parent = L_axel.transform.parent.gameObject;
        //GameObject R_parent = R_axel.transform.parent.gameObject;

        //L_parent.transform.DetachChildren();
        //R_parent.transform.DetachChildren();

        //L_axel.transform.SetParent(spine_overdel.transform);
        //R_axel.transform.SetParent(spine_overdel.transform);
        //huvud_brytning.transform.SetParent(spine_overdel.transform);

        //Destroy(L_parent);
        //Destroy(R_parent);
    }
}
