using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateObjects : MonoBehaviour
{
    public Transform playerCam;

    // options for different objects that can be thrown
    public GameObject cube;
    public GameObject sphere;
    public GameObject cylinder;
    public GameObject rectangle;

    private Dictionary<string, GameObject> options;
    private Vector3 spawnPosition;

    private void Start()
    {
        // Create lookup for all object options
        options = new Dictionary<string, GameObject>();
        options.Add("c", cube);
        options.Add("p", sphere);
        options.Add("y", cylinder);
        options.Add("r", rectangle);
    }

    void Update()
    {
        if(Input.anyKeyDown)
        {
            string keyPressed = Input.inputString;            

            if (options.ContainsKey(keyPressed))
            {
                // calculate spawn position
                Vector3 spawnAdd = (playerCam.forward * 1.5f) + (playerCam.up * -0.6f);
                Vector3 spawnPosition = playerCam.position + spawnAdd;                                
                Instantiate(options[keyPressed], spawnPosition, Quaternion.identity);
                GameObject.Find("Mass").GetComponent<Text>().text = "Mass Multiplier: " + options[keyPressed].GetComponent<Rigidbody>().mass;
            } 
            else if (keyPressed == "q")
            {
                Application.Quit();
            }
        }
    }
}
