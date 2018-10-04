﻿using System.Collections;
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
    Image imgdetails;
    //public string[] classes;
    public Sprite[] sprites;
    public Sprite[] detailsprites;
    InputField Name;
    InputField Pronouns;
    Dropdown Classes;
    Text infotext;
    int morale;
    int move;
    int adjacency;
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
        imgdetails = transform.Find("BG/CharImg/DetailImg").gameObject.GetComponent<Image>();
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
                infotext.text = "Skills:\nVuvuzela blow +4\n" +
                    "Rousing toot +3\nVuvuzela Duelist +6\n";
                img.sprite = sprites[0];
                img.rectTransform.sizeDelta = new Vector3(32, 64);
                imgdetails.gameObject.SetActive(true);
                imgdetails.sprite = detailsprites[0];
                morale = 8;
                move = 6;
                actions.Add(new Unit.Action("Move", move, Unit.ActType.Movement, 0,-1,"","",Color.white));
                actions.Add(new Unit.Action("Blow vuvuzela", 6, Unit.ActType.Cone, 4, 6, "EnemyUnit","DOOT!",Color.white));
                actions.Add(new Unit.Action("Rousing toot", 6, Unit.ActType.Cone, 3, 6, "Unit","TOOT TOOT!",Color.cyan));
                actions.Add(new Unit.Action("Vuvuzela Duelist", move, Unit.ActType.Melee, 6, 6, "EnemyUnit","Vuvuzela THWACK!",Color.white));
                adjacency = 1;
                break;
            case "Witch":
                infotext.text = "Skills:\nFavourable horoscope +2 +BLESSED\n" +
                    "Hex +2 +CURSED\nMagic Glitterbomb +2\n";
                img.sprite = sprites[2];
                img.rectTransform.sizeDelta = new Vector3(32, 64);
                imgdetails.gameObject.SetActive(true);
                imgdetails.sprite = detailsprites[3];
                morale = 6;
                move = 6;
                actions.Add(new Unit.Action("Move", move, Unit.ActType.Movement, 0, -1, "", "", Color.white));
                actions.Add(new Unit.Action("Favourable horoscope", 20, Unit.ActType.Targetted, 2, -1, "Unit", "Blessed!", Color.cyan,2));
                actions.Add(new Unit.Action("Hex", 20, Unit.ActType.Targetted, 2, -1, "EnemyUnit", "Hexed!! >:o", Color.white,-2));
                actions.Add(new Unit.Action("Magic glitterbomb", 6, Unit.ActType.Grenade, 2, 3, "Unit", "Magic glitter!!", Color.magenta));
                //actions.Add(new Unit.Action("Blow vuvuzela", 6, Unit.ActType.Cone, 4, 6, "EnemyUnit", "DOOT!", Color.white));
                //actions.Add(new Unit.Action("Rousing toot", 6, Unit.ActType.Cone, 3, 6, "Unit", "TOOT TOOT!", Color.cyan));
                //actions.Add(new Unit.Action("Vuvuzela Duelist", move, Unit.ActType.Melee, 6, 6, "EnemyUnit", "Vuvuzela THWACK!", Color.white));
                adjacency = 1;
                break;
            case "Bear":
                infotext.text = "Skills:\nSuns out guns out +4\n" +
                    "Bear hug +4\nArm wrestle +6\n";
                img.sprite = sprites[0];
                img.rectTransform.sizeDelta = new Vector3(32, 64);
                imgdetails.gameObject.SetActive(true);
                imgdetails.sprite = detailsprites[2];
                morale = 10;
                move = 6;
                actions.Add(new Unit.Action("Move", move, Unit.ActType.Movement, 0, -1, "", "", Color.white));
                actions.Add(new Unit.Action("Suns out guns out", 10, Unit.ActType.Targetted, 4, -1, "EnemyUnit", "*FLEXES* :)", Color.white));
                actions.Add(new Unit.Action("Bear hug", move, Unit.ActType.Melee, 4, -1, "EnemyUnit", "BEAR HUG!! ^.^", Color.cyan));
                actions.Add(new Unit.Action("Arm wrestle", move, Unit.ActType.Melee, 6, -1, "EnemyUnit", "Do you even lift, bro :)?", Color.white));
                adjacency = 1;
                break;
            case "Lumberdyke":
                infotext.text = "Skills:\nCone of sawdust +1\n" +
                    "Chop wood +3\nWood thwack +5\n";
                img.sprite = sprites[0];
                img.rectTransform.sizeDelta = new Vector3(32, 64);
                imgdetails.gameObject.SetActive(true);
                imgdetails.sprite = detailsprites[1];
                morale = 8;
                move = 6;
                actions.Add(new Unit.Action("Move", move, Unit.ActType.Movement, 0, -1, "", "", Color.white));
                actions.Add(new Unit.Action("Cone of sawdust", 6, Unit.ActType.Cone, 1, 6, "EnemyUnit", "Sawdust in eyes!! X.X;;", Color.yellow,-1));
                actions.Add(new Unit.Action("Chop wood impressively", 0, Unit.ActType.Grenade, 3, 10, "Unit", "Chop!! ;D", Color.cyan));
                actions.Add(new Unit.Action("Wooden thwack", move, Unit.ActType.Melee, 5, 6, "EnemyUnit", "Wooden THWACK!", Color.white));
                adjacency = 1;
                break;
            case "Doggo":
                infotext.text = "Skills:\nBork +4\nGrowl +4\nGood dog +10\n";
                img.sprite = sprites[1];
                img.rectTransform.sizeDelta = new Vector3(32, 32);
                imgdetails.gameObject.SetActive(false);
                morale = 4;
                move = 10;
                actions.Add(new Unit.Action("Move", move, Unit.ActType.Movement, 0,-1,"","",Color.white));
                actions.Add(new Unit.Action("Bork", 0, Unit.ActType.Grenade, 4,10,"Unit","BORK! :D",Color.cyan));
                actions.Add(new Unit.Action("Growl", 10, Unit.ActType.Targetted, 4, -1, "EnemyUnit","GRRR! >:F",Color.white));
                adjacency = 2;
                //actions.Add(new Action("Strike a pose", 0, ActType.Grenade, 10, 20));
                break;

        }
        infotext.text = "Morale: " + morale + "\nMove: " + move + "\nAdjacency: " + adjacency + "\n" + infotext.text;
//gameObject.Component

    }

    public void ApplyDetails(Unit theunit) {
        theunit.SetDetails(Name.text, Pronouns.text, morale, move, img.sprite, actions,adjacency,imgdetails.sprite);
        Destroy(gameObject);
    }
	// Update is called once per frame
	/*void Update () {
		
	}*/
}
