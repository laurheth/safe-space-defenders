using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Startup : MonoBehaviour {
    bool preparetoswitch;
    float switchcount;
	// Use this for initialization
	void Start () {
        preparetoswitch = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.anyKey)
        {
            preparetoswitch = true;
        }
        if (preparetoswitch) {
            switchcount += Time.deltaTime;
        }
        if (switchcount>3) {
            SceneManager.LoadScene("TeamSelect");
        }
	}
}
