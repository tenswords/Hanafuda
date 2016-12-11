using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cpu : MonoBehaviour {

    [SerializeField]
    private FieldManager fieldManager;

    private Dictionary<GameObject, float> handCardTargetMaxDistance_Dic = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, Vector3> handCardTargePosition_Dic = new Dictionary<GameObject, Vector3>();
    private List<GameObject> hand = new List<GameObject>();

    private const float adjust = 1.16f;
    private Vector3 fieldFirstPosition = new Vector3(-4.06f, 4.0f, 0.0f);
    private Vector3 cardScale = new Vector3(0.75f, 0.75f, 1.0f);

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update () {
        switch (fieldManager.state) {
            case FieldManager.STATE.CARD_HAND_OUT: CardHandOut(); break;
        }
    }

    /// <summary>
    /// 配られているときの処理
    /// </summary>
    private void CardHandOut() {

        for (int i = 0; i < hand.Count; i++) {

            hand[i].transform.position = Vector3.MoveTowards(hand[i].transform.position,
                                                             handCardTargePosition_Dic[hand[i]],
                                                             Time.deltaTime * fieldManager.handCardSpeed);

            //目標までの距離に近づくにつれてだんだん大きく
            var distance = Vector3.Distance(hand[i].transform.position, handCardTargePosition_Dic[hand[i]]);
            var distanceLeap = Mathf.Lerp(1f, 0f, distance / handCardTargetMaxDistance_Dic[hand[i]]);
            hand[i].transform.localScale = cardScale * distanceLeap;
        }

    }
    /// <summary>
    /// ゲーム開始時の手札が追加される処理
    /// </summary>
    public void AddCard(GameObject card) {
        var pos = fieldFirstPosition;
        pos.x += adjust * hand.Count;

        var maxDistance = Vector3.Distance(card.transform.position, pos);

        hand.Add(card);
        handCardTargetMaxDistance_Dic.Add(card, maxDistance);
        handCardTargePosition_Dic.Add(card, pos);
        card.transform.parent = transform;
    }

    ///// <summary>
    ///// ゲーム開始時の手札が指定されている座標まで全て移動しきっているかを取得
    ///// </summary>
    public bool GetIsHandOutMovement() {

        foreach (var card in hand) {
            if (card.transform.position != handCardTargePosition_Dic[card]) {
                return false;
            }
        }
        return true;

    }

    /// <summary>
    /// 自分のターン時にカードを出す処理
    /// </summary>
    public void PutDownCard() {
    }
}
