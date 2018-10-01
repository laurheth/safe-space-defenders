using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharSelectButton : MonoBehaviour {
    List<CharSelect> chars;
	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(transform.parent.gameObject);
        chars = new List<CharSelect>();
        foreach (GameObject thischar in GameObject.FindGameObjectsWithTag("CharSelector")) {
            chars.Add(thischar.GetComponent<CharSelect>());
        }
        GetComponent<Button>().onClick.AddListener(delegate { StartCoroutine(ReadyToGo()); });
	}
	
	// Update is called once per frame
	/*void Update () {
		
	}*/
    public IEnumerator ReadyToGo() {
        // Detach things so they don't get deleted
        /*transform.parent = null;
        foreach (CharSelect thischar in chars) {
            thischar.gameObject.transform.parent = null;
        }*/
        // Load main scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainGame");

        while (!asyncLoad.isDone) {
            yield return null;
        }
        //int i = 0;
        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
        for (int i = 0; i < Mathf.Min(chars.Count,units.Length);i++) {
            Debug.Log(i);
            chars[i].ApplyDetails(units[i].GetComponent<Unit>());
            //Destroy(chars[i].gameObject);
        }
        Destroy(transform.parent.gameObject);
    }
}
