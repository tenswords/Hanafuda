using UnityEngine;
using System.Collections;
using TAG;

public class CardTouchManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonUp(0)) {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var collider = Physics2D.OverlapPoint(touchPosition);
            Debug.Log("collider " + collider);
            if (collider) {

                //タッチした場所のカードのタグがプレイヤーの手札だったら、手札のカードを上に上げる処理
                if(collider.tag == TagManager.TAG_PLAYER_HAND) {



                    var selectCard = collider.GetComponent<Card>();


                }else {//プレイヤー以外のところをタッチした場合、手札のカードの中に上にあがっているものがあれば下げる

                }


            }
        }

        if (Input.touchCount > 0) {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var collider = Physics2D.OverlapPoint(touchPosition);
            Debug.Log("collider " + collider);
        }
	}
}
