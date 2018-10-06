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
    GameObject SkinObj;
    Image img;
    Image imgdetails;
    //public string[] classes;
    public Sprite[] sprites;
    public Sprite[] detailsprites;
    InputField Name;
    InputField Pronouns;
    Dropdown Classes;
    Slider SkinSlider;
    Text infotext;
    int morale;
    int move;
    int adjacency;
    List<Unit.Action> actions;
    Color whiteskin;
    Color blackskin;

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
        SkinObj = transform.Find("Details/SkinSlider").gameObject;

        img = SpriteObj.GetComponent<Image>();
        imgdetails = transform.Find("BG/CharImg/DetailImg").gameObject.GetComponent<Image>();
        Name = NameInputObj.GetComponent<InputField>();
        Pronouns = PronounInputObj.GetComponent<InputField>();
        Classes = ClassInputObj.GetComponent<Dropdown>();
        infotext = InfoTextObj.GetComponent<Text>();
        SkinSlider = SkinObj.GetComponent<Slider>();
        SkinSlider.value = Random.Range(0f, 1f);

        string[] namelist = { "Andrew", "Adam", "Greg", "Zoe", "Emily", "Evelyn", "Emma", "Grace", "Alexis", "John", "Jennifer","Lauren","Laura","Garfield","Mister","Pupperino","Moira","Alice","Alexanda","Yolande","Nicole","Sarah","David","Richard"};
        string[] pronounlist = { "She/Her", "She/Her", "She/Her", "She/Her", "He/Him", "He/Him", "They/Them", "Ze/Zer", "Xe/Xir", "Ze/Hir", "They/Them", "Xey/Xem", "Fae/Faer", "Ey/Em", "Xe/Xym", "Ze/Zir" };
        Pronouns.text = pronounlist[Random.Range(0, pronounlist.Length)];
        Name.text = namelist[Random.Range(0, namelist.Length)];
        Classes.value = Random.Range(0,Classes.options.Count);
        Name.onValueChanged.AddListener(delegate { ValueUpdate(); });
        Pronouns.onValueChanged.AddListener(delegate { ValueUpdate(); });
        Classes.onValueChanged.AddListener(delegate { ValueUpdate(); });
        SkinSlider.onValueChanged.AddListener(delegate { ValueUpdate(); });

        whiteskin = new Color(1f, 228f/255f, 228f/255f);
        blackskin = new Color(130f/255f,70f/255f,19f/255f);

        ValueUpdate();
        //DontDestroyOnLoad(gameObject);
	}
	
    public void ValueUpdate() {

        /*Debug.Log(Name.text);
        Debug.Log(Pronouns.text);
        Debug.Log(Classes.options[Classes.value].text);*/

        img.color = whiteskin * SkinSlider.value + blackskin * (1 - SkinSlider.value);

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
                actions.Add(new Unit.Action("Blow vuvuzela", 6, Unit.ActType.Cone, 4, 6, "EnemyUnit","DOOT!",Color.white,0,4,true));
                actions.Add(new Unit.Action("Rousing toot", 6, Unit.ActType.Cone, 3, 6, "Unit","TOOT TOOT!",Color.cyan,1,4,true));
                actions.Add(new Unit.Action("Vuvuzela Duelist", move, Unit.ActType.Melee, 6, 6, "EnemyUnit","Vuvuzela THWACK!",Color.white,0,11));
                adjacency = 1;
                break;
            case "Witch":
                infotext.text = "Skills:\nFavourable horoscope +2\n" +
                    "Hex +2\nMagic Glitterbomb +2\n";
                img.sprite = sprites[2];
                img.rectTransform.sizeDelta = new Vector3(32, 64);
                imgdetails.gameObject.SetActive(true);
                imgdetails.sprite = detailsprites[3];
                morale = 6;
                move = 6;
                actions.Add(new Unit.Action("Move", move, Unit.ActType.Movement, 0, -1, "", "", Color.white));
                actions.Add(new Unit.Action("Favourable horoscope", 20, Unit.ActType.Targetted, 2, -1, "Unit", "Blessed!", Color.cyan,3,1));
                actions.Add(new Unit.Action("Hex", 20, Unit.ActType.Targetted, 2, -1, "EnemyUnit", "Hexed!! >:o", Color.white,-3,8));
                actions.Add(new Unit.Action("Magic glitterbomb", 6, Unit.ActType.Grenade, 2, 4, "", "Magic glitter!!", Color.magenta,0,6));
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
                actions.Add(new Unit.Action("Suns out guns out", 10, Unit.ActType.Targetted, 4, -1, "EnemyUnit", "*FLEXES* :)", Color.white,0,10));
                actions.Add(new Unit.Action("Bear hug", move, Unit.ActType.Melee, 4, -1, "Unit", "BEAR HUG!! ^.^", Color.cyan,1,0));
                actions.Add(new Unit.Action("Arm wrestle", move, Unit.ActType.Melee, 6, -1, "EnemyUnit", "Do you even lift, bro :)?", Color.white,0,5));
                adjacency = 1;
                break;
            case "Lumberdyke":
                infotext.text = "Skills:\nCone of sawdust +1\n" +
                    "Chop wood +2\nWood thwack +5\n";
                img.sprite = sprites[0];
                img.rectTransform.sizeDelta = new Vector3(32, 64);
                imgdetails.gameObject.SetActive(true);
                imgdetails.sprite = detailsprites[1];
                morale = 8;
                move = 6;
                actions.Add(new Unit.Action("Move", move, Unit.ActType.Movement, 0, -1, "", "", Color.white));
                actions.Add(new Unit.Action("Cone of sawdust", 6, Unit.ActType.Cone, 1, 6, "EnemyUnit", "Sawdust in eyes!! X.X;;", Color.yellow,-1,9));
                actions.Add(new Unit.Action("Chop wood impressively", 0, Unit.ActType.Grenade, 2, 10, "Unit", "Chop!! ;D", Color.cyan,1,3,true));
                actions.Add(new Unit.Action("Wooden thwack", move, Unit.ActType.Melee, 5, 6, "EnemyUnit", "Wooden THWACK!", Color.white,0,11));
                adjacency = 1;
                break;
            case "Doggo":
                img.color = Color.white;
                infotext.text = "Skills:\nBork +4\nGrowl +4\nGood dog +10\n";
                img.sprite = sprites[1];
                img.rectTransform.sizeDelta = new Vector3(32, 32);
                imgdetails.sprite = null;
                imgdetails.gameObject.SetActive(false);
                morale = 4;
                move = 10;
                actions.Add(new Unit.Action("Move", move, Unit.ActType.Movement, 0,-1,"","",Color.white));
                actions.Add(new Unit.Action("Bork", 0, Unit.ActType.Grenade, 4,10,"Unit","BORK! :D",Color.cyan,1,2,true));
                actions.Add(new Unit.Action("Growl", 10, Unit.ActType.Targetted, 4, -1, "EnemyUnit","GRRR! >:F",Color.white,0,7));
                adjacency = 2;
                //actions.Add(new Action("Strike a pose", 0, ActType.Grenade, 10, 20));
                break;

        }
        infotext.text = "Morale: " + morale + "\nMove: " + move + "\nAdjacency: " + adjacency + "\n" + infotext.text;
//gameObject.Component

    }

    public void ApplyDetails(Unit theunit) {
        theunit.SetDetails(Name.text, Pronouns.text, morale, move, img.sprite, actions,adjacency,imgdetails.sprite,img.color);
        Destroy(gameObject);
    }
	// Update is called once per frame
	/*void Update () {
		
	}*/
}
