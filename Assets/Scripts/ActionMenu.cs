using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMenu : MonoBehaviour {
    GameObject selected;
    public GameObject actionprefab;
    List<GameObject> options;
    Cursor cursor;
    bool newselection;
    public GameObject cursobj;
	// Use this for initialization
	void Start () {
        newselection = false;
        selected = null;
        options = new List<GameObject>();
        cursor = cursobj.GetComponent<Cursor>();
        UpdateList();
	}

    void UpdateList() {
        options.Clear();
        foreach (Transform child in transform) {
            options.Add(child.gameObject);
        }
    }

    public void ClearOptions() {
        foreach (GameObject obj in options)
        {
            obj.transform.parent = null;
            Destroy(obj);
        }
        selected = null;
        options.Clear();
    }

    public void DefineOptions(GameObject thisunit) {

        ClearOptions();
        newselection = true;
        Unit unit = thisunit.GetComponent<Unit>();
        foreach (Unit.Action act in unit.actions) {
            GameObject newoption=Instantiate(actionprefab, transform);
            newoption.GetComponent<ActionOption>().SetText("> "+act.GetName());
            newoption.GetComponent<ActionOption>().SetAction(act);
        }
        UpdateList();
    }

    private void Update()
    {
        if ((newselection || Input.GetButtonDown("Fire2")) && options.Count>0) {
            newselection = false;
            if (selected == null || !options.Contains(selected)) {
                selected = options[0];
            }
            else {
                int i = options.IndexOf(selected);
                if (i==options.Count-1) {
                    selected = options[0];
                }
                else {
                    selected = options[i + 1];
                }
            }
            SelectMe(selected);
        }
    }
    public void SelectMe(GameObject option) {
        if (option == null) { return; }
        foreach (GameObject opt in options) {
            opt.GetComponent<ActionOption>().UnSelectThis();
        }
        selected = option;
        selected.GetComponent<ActionOption>().SelectThis();
        cursor.SetAction(selected.GetComponent<ActionOption>().GetAction());
    }
}
