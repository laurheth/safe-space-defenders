using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActionOption : MonoBehaviour, IPointerClickHandler {

    Text txt;
    Unit.Action action;
    ActionMenu actionMenu;
    // Use this for initialization
    private void Awake()
    {
        txt = GetComponent<Text>();
    }
    void Start () {
        actionMenu = transform.parent.gameObject.GetComponent<ActionMenu>();
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

    public void SetText(string str) {
        txt.text = str;
    }

    public void SetAction(Unit.Action act) {
        action = act;
    }

    public Unit.Action GetAction() {
        return action;
    }
}
