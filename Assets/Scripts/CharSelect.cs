using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharSelect : MonoBehaviour {
    GameObject NameInputObj;
    GameObject PronounInputObj;
    GameObject ClassInputObj;
    GameObject InfoTextObj;
    GameObject SpriteObj;
    Image img;
    //public string[] classes;
    public Sprite[] sprites;
    InputField Name;
    InputField Pronouns;
    Dropdown Classes;
    Text infotext;
    int morale;
    int move;
    List<Unit.Action> actions;

    //string[]
	// Use this for initialization
	void Start () {
        morale = 8;
        move = 6;
        actions = new List<Unit.Action>();
        NameInputObj = transform.Find("Details/InputFieldName").gameObject;
        PronounInputObj = transform.Find("Details/InputPronouns").gameObject;
        ClassInputObj = transform.Find("Details/InputClass").gameObject;
        InfoTextObj = transform.Find("InfoText").gameObject;
        SpriteObj = transform.Find("BG/CharImg").gameObject;

        img = SpriteObj.GetComponent<Image>();
        Name = NameInputObj.GetComponent<InputField>();
        Pronouns = PronounInputObj.GetComponent<InputField>();
        Classes = ClassInputObj.GetComponent<Dropdown>();
        infotext = InfoTextObj.GetComponent<Text>();

        Name.onValueChanged.AddListener(delegate { ValueUpdate(); });
        Pronouns.onValueChanged.AddListener(delegate { ValueUpdate(); });
        Classes.onValueChanged.AddListener(delegate { ValueUpdate(); });
        ValueUpdate();
        //DontDestroyOnLoad(gameObject);
	}
	
    public void ValueUpdate() {

        /*Debug.Log(Name.text);
        Debug.Log(Pronouns.text);
        Debug.Log(Classes.options[Classes.value].text);*/
        actions.Clear();
        switch (Classes.options[Classes.value].text) {
            default:
            case "Vuvuzelist":
                infotext.text = "Morale: 8\nMove: 6\nSkills:\nVuvuzela blow +4\n" +
                    "Rousing toot +3\nVuvuzela Duelist +6\n";
                img.sprite = sprites[0];
                img.rectTransform.sizeDelta = new Vector3(32, 64);
                morale = 8;
                move = 6;
                actions.Add(new Unit.Action("Move", move, Unit.ActType.Movement, 0,-1,"","",Color.white));
                actions.Add(new Unit.Action("Blow vuvuzela", 6, Unit.ActType.Cone, 4, 6, "EnemyUnit","DOOT!",Color.white));
                actions.Add(new Unit.Action("Rousing toot", 6, Unit.ActType.Cone, 3, 6, "Unit","TOOT TOOT!",Color.cyan));
                actions.Add(new Unit.Action("Vuvuzela Duelist", move, Unit.ActType.Melee, 6, 6, "EnemyUnit","Vuvuzela THWACK!",Color.white));
                break;
            case "Doggo":
                infotext.text = "Morale: 4\nMove: 8\nSkills:\nBork +4\nGrowl +6\nGood dog +10\n";
                img.sprite = sprites[1];
                img.rectTransform.sizeDelta = new Vector3(32, 32);
                morale = 4;
                move = 8;
                actions.Add(new Unit.Action("Move", move, Unit.ActType.Movement, 0,-1,"","",Color.white));
                actions.Add(new Unit.Action("Bork", 0, Unit.ActType.Grenade, 4,10,"Unit","BORK! :D",Color.cyan));
                actions.Add(new Unit.Action("Growl", 10, Unit.ActType.Targetted, 6, -1, "EnemyUnit","GRRR! >:F",Color.white));
                //actions.Add(new Action("Strike a pose", 0, ActType.Grenade, 10, 20));
                break;

        }
//gameObject.Component

    }

    public void ApplyDetails(Unit theunit) {
        theunit.SetDetails(Name.text, Pronouns.text, morale, move, img.sprite, actions);
        Destroy(gameObject);
    }
	// Update is called once per frame
	/*void Update () {
		
	}*/
}
