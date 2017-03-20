using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ImageManager : MonoBehaviour {

    private Image image;
    [SerializeField]
    private Sprite[] arraySprite;

    private Dictionary<string,Sprite> spriteDic;

	// Use this for initialization
	void Start () {
        image = GetComponent<Image>();

        foreach (var sprite in arraySprite) {
            spriteDic.Add(sprite.name, sprite);
        }

    }
	
	// Update is called once per frame
	void Update () {
	
	}
    public void SetImage(string spriteName) {
        image.sprite = spriteDic[spriteName];
    }
}
