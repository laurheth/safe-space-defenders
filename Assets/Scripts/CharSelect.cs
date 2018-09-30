using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharSelect : MonoBehaviour {
    GameObject NameInputObj;
    GameObject PronounInputObj;
    GameObject ClassInputObj;
    GameObject InfoTextObj;
    InputField Name;
    InputField Pronouns;
    Dropdown Classes;
    Text infotext;
    //string[]
	// Use this for initialization
	void Start () {
        NameInputObj = transform.Find("Details/InputFieldName").gameObject;
        PronounInputObj = transform.Find("Details/InputPronouns").gameObject;
        ClassInputObj = transform.Find("Details/InputClass").gameObject;
        InfoTextObj = transform.Find("InfoText").gameObject;

        Name = NameInputObj.GetComponent<InputField>();
        Pronouns = PronounInputObj.GetComponent<InputField>();
        Classes = ClassInputObj.GetComponent<Dropdown>();
        infotext = InfoTextObj.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
