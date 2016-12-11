using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    [SerializeField]
    private FieldManager fieldManager;

    private Dictionary<GameObject, float> handCardTargetMaxDistance_Dic = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, Vector3> handCardTargePosition_Dic = new Dictionary<GameObject, Vector3>();
    private List<GameObject> hand = new List<GameObject>();

    private const float adjust = 1.925f;
    private Vector3 fieldFirstPosition = new Vector3(-6.7375f, -3.5f, 0.0f);
    private Vector3 cardScale = new Vector3(1.25f,1.25f,1.0f);

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < hand.Count; i++) {

            hand[i].transform.position = Vector3.MoveTowards(hand[i].transform.position,
                                                             handCardTargePosition_Dic[hand[i]],
                                                             Time.deltaTime * fieldManager.handCardSpeed);

            //目標までの距離に近づくにつれてだんだん大きく
            var distance = Vector3.Distance(hand[i].transform.position, handCardTargePosition_Dic[hand[i]]);
            var distanceLeap = Mathf.Lerp(1f, 0f, distance / handCardTargetMaxDistance_Dic[hand[i]]);
            hand[i].transform.localScale = cardScale * distanceLeap;

            //一定距離まで移動したら、回転させる
            if (distanceLeap >= fieldManager.handCardCenterDistanePer) {
            //if (posLeap > fieldManager.handCardMoveDistaneRotPer) {
                var spriteRenderer = hand[i].GetComponent<SpriteRenderer>();
                var card = hand[i].GetComponent<Card>();

                hand[i].transform.rotation = Quaternion.RotateTowards(hand[i].transform.rotation,
                                                                      Quaternion.identity,
                                                                      Time.deltaTime * 250);

                //var rotationLeap = Mathf.Lerp(1f, 0f, distance / handCardTargetMaxDistance_Dic[hand[i]]);
                //LerpAngle(float a, float b, float t);

                //var rotationLeap = Mathf.LerpAngle(1f, 0f, hand[i].transform.rotation.eulerAngles.y / 360.0f);
                //Debug.Log("rotationLeap" + rotationLeap);

                //本当は中間まで回転したらの割合でやりたい
                if(hand[i].transform.rotation.eulerAngles.y >= 270.0f) {
                //if (rotationLeap >= fieldManager.handCardCenterDistanePer) {
                    spriteRenderer.sprite = card.image[0];
                }
            }
        }
    }

    ///// <summary>
    ///// ゲーム開始時の手札が追加される処理
    ///// </summary>
    public void AddCard(GameObject card) {

        var pos = fieldFirstPosition;
        pos.x += adjust * hand.Count;
        
        var maxDistance = Vector3.Distance(card.transform.position, pos);

        hand.Add(card);
        handCardTargetMaxDistance_Dic.Add(card, maxDistance);
        handCardTargePosition_Dic.Add(card,pos);
        card.transform.parent = transform;
    }

    /// <summary>
    /// 自分のターン時にカードを出す処理
    /// </summary>
    public void PutDownCard() {
    }
}
