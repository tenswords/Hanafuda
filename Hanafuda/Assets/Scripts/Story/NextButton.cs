using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NextButton : MonoBehaviour {

    private Image image;
    private Sprite[] sprites = new Sprite[2];

    [SerializeField]
    private Sprite uramen;

	// Use this for initialization
	void Start () {
        image = GetComponent<Image>();
        sprites[0] = image.sprite;
        sprites[1] = uramen;
    }
	
	// Update is called once per frame
	void Update () {
	
	}


    public void ImageChange(int index) {
        image.sprite = sprites[index];

        if (index == 0) {
            transform.rotation = Quaternion.identity;
        }
    }
}
