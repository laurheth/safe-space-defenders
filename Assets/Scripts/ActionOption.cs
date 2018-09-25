using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActionOption : MonoBehaviour, IPointerClickHandler {

    Text txt;
    ActionMenu actionMenu;
	// Use this for initialization
	void Start () {
        actionMenu = transform.parent.gameObject.GetComponent<ActionMenu>();
        txt = GetComponent<Text>();
	}
	
    public void SelectThis() {
        txt.fontStyle=FontStyle.BoldAndItalic;
        txt.color = Color.red;
    }

    public void UnSelectThis()
    {
        txt.fontStyle = FontStyle.Normal;
        txt.color = Color.white;
    }

    public void OnPointerClick(PointerEventData pointerEventData) {
        //Debug.Log(txt.text);
        actionMenu.SelectMe(gameObject);
    }
}
