using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ThrowObjectScript : MonoBehaviour
{
    // object is initially 'carried' by player
    private bool isHeld = true;
    private Rigidbody rbody;
    private Transform userCam;
    private Text objInfo;
    private Dictionary<string, float> massOps;

    private Vector3 getThrowForce ()
    {
        return (userCam.up * 150f) + (userCam.forward * 350f);
    }

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        rbody.isKinematic = true;
        userCam = GameObject.Find("FlyCam").transform;
        transform.parent = userCam;

        massOps = new Dictionary<string, float>();
        massOps.Add("+", 0.1f);
        massOps.Add("=", 0.1f);
        massOps.Add("-", -0.1f);

        objInfo = GameObject.Find("Mass").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isHeld)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                print("BOOP");
                transform.parent = null;
                isHeld = false;
                rbody.isKinematic = false;
                rbody.AddForce(getThrowForce());
            }
            else if (Input.anyKey)
            {
                if (massOps.ContainsKey(Input.inputString))
                {
                    if (Input.inputString == "-" && rbody.mass > 0.5f)
                    {
                        rbody.mass += massOps[Input.inputString];
                    } else if ((Input.inputString == "=" || Input.inputString == "+") && rbody.mass < 25f)
                    {
                        rbody.mass += massOps[Input.inputString];
                    }
                    objInfo.text = "Mass Multiplier: " + rbody.mass;
                }
            }
        }
    }
}
