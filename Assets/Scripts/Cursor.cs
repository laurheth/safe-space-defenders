using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {
    TileGridController gridController;
    public Vector3 offset;
    SpriteRenderer srend;
	// Use this for initialization
	void Start () {
        gridController = transform.parent.gameObject.GetComponent<TileGridController>();
        srend = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
        transform.position=offset+Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition)-offset);
        if (gridController.validPos(transform.position)) {
            srend.color = Color.cyan;
        }
        else {
            srend.color = Color.red;
        }
	}
}
