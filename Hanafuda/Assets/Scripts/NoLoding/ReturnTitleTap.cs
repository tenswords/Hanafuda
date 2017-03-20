using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ReturnTitleTap : MonoBehaviour {

    private float time;
    [SerializeField]
    private Text text;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        time += Time.deltaTime;
        if(time > 0.5f) {
            time = 0.0f;
            if (text.enabled) text.enabled = false;
            else text.enabled = true;
        }
	}
}
