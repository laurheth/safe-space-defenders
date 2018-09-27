using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitCanvas : MonoBehaviour {
    Text txt;
    GameObject txtobj;
    int maxMP;
    int currentMP;
    List<GameObject> MaxPips;
    List<GameObject> CurrentPips;
	// Use this for initialization
	void Start () {
        txtobj = transform.Find("Text").gameObject;
        txt = txtobj.GetComponent<Text>();
        txt.text = "";
	}

    public void SetMaxMP(int MP) {
        maxMP = MP;
        MaxPips = new List<GameObject>();
        CurrentPips = new List<GameObject>();
        GameObject maxpipbar = transform.Find("HealthBar_Max").gameObject;
        GameObject currentpipbar = transform.Find("HealthBar_Current").gameObject;
        MaxPips.Add(maxpipbar.transform.Find("MaxPip").gameObject);
        CurrentPips.Add(currentpipbar.transform.Find("CurrentPip").gameObject);
        if (MP>1) {
            for (int i = 1; i < MP;i++) {
                MaxPips.Add(Instantiate(MaxPips[0], maxpipbar.transform));
                CurrentPips.Add(Instantiate(CurrentPips[0], currentpipbar.transform));
            }
        }
    }

    public void SetCurrentMP(int MP) {
        for (int i = 0; i < CurrentPips.Count;i++) {
            if (i<MP) {
                CurrentPips[i].SetActive(true);
            }
            else {
                CurrentPips[i].SetActive(false);
            }
        }
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
