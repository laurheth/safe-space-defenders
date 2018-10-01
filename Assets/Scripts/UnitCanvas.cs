using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitCanvas : MonoBehaviour {
    Text txt;
    Text resistText;
    GameObject txtobj;
    int maxMP;
    int currentMP;
    bool started;
    List<GameObject> MaxPips;
    //List<GameObject> CurrentPips;
    // Use this for initialization
    private void Awake()
    {
        //MaxPips = new List<GameObject>();
        setup();
        //CurrentPips = new List<GameObject>();
    }
    void setup() {
        if (!started)
        {
            started = true;
            MaxPips = new List<GameObject>();
        }
    }
    void Start () {
        txtobj = transform.Find("Text").gameObject;
        txt = txtobj.GetComponent<Text>();
        txt.text = "";
        resistText = transform.Find("ResistText").gameObject.GetComponent<Text>();
        resistText.GetComponent<Text>().fontStyle=FontStyle.Bold;
	}

    public void SetMaxMP(int MP) {
        setup();
        maxMP = MP;
        GameObject maxpipbar = transform.Find("HealthBar_Max").gameObject;
        //GameObject currentpipbar = transform.Find("HealthBar_Current").gameObject;
        if (MaxPips.Count == 0)
        {
            MaxPips.Add(maxpipbar.transform.Find("MaxPip").gameObject);
        }
        //CurrentPips.Add(currentpipbar.transform.Find("CurrentPip").gameObject);
        if (MP>1) {
            for (int i = 1; i < MP;i++) {
                MaxPips.Add(Instantiate(MaxPips[0], maxpipbar.transform));
                //CurrentPips.Add(Instantiate(CurrentPips[0], currentpipbar.transform));
            }
        }
        for (int i = 0; i < MaxPips.Count;i++) {
            if (i<MP) {
                MaxPips[i].SetActive(true);
            }
            else {
                MaxPips[i].SetActive(false);
            }
        }
        SetCurrentMP(MP);
    }

    public void SetCurrentMP(int MP) {
        for (int i = 0; i < maxMP;i++) {
            if (i<=MP) {
                MaxPips[i].GetComponent<Image>().color = Color.white;
                MaxPips[i].transform.SetAsFirstSibling();
                //CurrentPips[i].SetActive(true);
            }
            else {
                MaxPips[i].GetComponent<Image>().color = Color.black;
                //CurrentPips[i].SetActive(false);
            }
        }
    }

    public void SetResist(int resist) {
        string newtext="";
        int i;
        if (resist>0) {
            resistText.color = Color.cyan;
            for (i = 0; i < resist;i++) {
                newtext += "^";
            }
        }
        else if (resist<0) {
            resistText.color = Color.red;
            for (i = 0; i < Mathf.Abs(resist); i++)
            {
                newtext += "v";
            }
        }
        resistText.text = newtext;
    }

    public IEnumerator DamageAnimation(int dmg) {
        if (dmg >= 0) { 
            txt.color = Color.cyan;
            txt.text = "+";
        }
        else {
            txt.color = new Color(1,0.6f,0);
            txt.text = "-";
        }
        float position = 0;
        txt.text += dmg.ToString();
        //txt.rectTransform.localPosition = Vector2.up * position;
        while (position<100) {
            position += Time.deltaTime * 100;
            txt.rectTransform.localPosition = Vector2.up * position;
            yield return null;
        }
        txt.text = "";
        yield return null;
    }
}
