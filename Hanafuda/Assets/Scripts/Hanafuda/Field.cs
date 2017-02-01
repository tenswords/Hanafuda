using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Field : MonoBehaviour {

    [SerializeField]
    private FieldManager fieldManager;
    [SerializeField]
    private CardManager cardManager;

    private Dictionary<GameObject, float> handCardTargetMaxDistance_Dic = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, Vector3> handCardTargePosition_Dic = new Dictionary<GameObject, Vector3>();

    public Dictionary<int, List<GameObject>> fieldCard_Dic = new Dictionary<int, List<GameObject>>();
    public Dictionary<int, Vector3> fieldPosition = new Dictionary<int, Vector3>();

    private const float adjustX = 1.55f;
    private const float adjustY = 2.05f;
    private Vector3 fieldFirstPosition = new Vector3(-2.325f, 1.55f, 0.0f);
    [HideInInspector]
    public Vector3 cardScale = new Vector3(1.0f, 1.0f, 1.0f);
    private Quaternion turnCardRotation = Quaternion.Euler(new Vector3(0.0f, 359.9f, 0.0f));

    private const int FIELD_COUNT = 12;

    private int addCardIndex;

    [HideInInspector]
    public int selectCardIndex;

    public List<int> getCardPutIndexList = new List<int>();
    public List<int> nonGetCardPutIndexList = new List<int>();

    // Use this for initialization
    void Start () {
        //場のイメージ
        //  8  0  1  2  3  9
        // 10  4  5  6  7 11
        for (int i = 0; i < FIELD_COUNT; i++) {

            fieldCard_Dic.Add(i,new List<GameObject>());

            var pos = fieldFirstPosition;
            if (i < 8) {
                pos.x += (i % 4) * adjustX;
                pos.y -= (i / 4) * adjustY;

            } else {

                switch (i) {
                    case 8:
                        pos.x -= adjustX;
                        break;
                    case 9:
                        pos.x += 4 * adjustX;
                        break;
                    case 10:
                        pos.x -= adjustX;
                        pos.y -= adjustY;
                        break;
                    case 11:
                        pos.x += 4 * adjustX;
                        pos.y -= adjustY;
                        break;
                }
            }
            fieldPosition.Add(i, pos);
        }
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

        for (int i = 0; i < fieldCard_Dic.Count; i++) {
            if (fieldCard_Dic[i].Count != 0) {
                var targetCard = fieldCard_Dic[i][0];

                //目標の座標まで動かす
                targetCard.transform.position = Vector3.MoveTowards(targetCard.transform.position,
                                                                handCardTargePosition_Dic[targetCard],
                                                                Time.deltaTime * fieldManager.handCardSpeed);

                //目標までの距離に近づくにつれて、大きさを変える
                var distance = Vector3.Distance(targetCard.transform.position, handCardTargePosition_Dic[targetCard]);
                var distanceLeap = Mathf.Lerp(1f, 0f, distance / handCardTargetMaxDistance_Dic[targetCard]);
                targetCard.transform.localScale = Vector3.Lerp(targetCard.transform.localScale, cardScale, distanceLeap);

                //一定距離まで移動したら、回転させる
                //if (distanceLeap >= fieldManager.handCardCenterDistanePer) {

                    //card.transform.rotation = Quaternion.RotateTowards(card.transform.rotation,
                    //                                                    Quaternion.identity,
                    //                                                    Time.deltaTime * 250);
                    var spriteRenderer = targetCard.GetComponent<SpriteRenderer>();
                    var card = targetCard.GetComponent<Card>();

                    targetCard.transform.rotation = Quaternion.Slerp(targetCard.transform.rotation, turnCardRotation, distanceLeap);

                    //本当は中間まで回転したらの割合でやりたい
                    if (targetCard.transform.rotation.eulerAngles.y >= 270.0f && spriteRenderer.sprite != card.image[0]) {
                        spriteRenderer.sprite = card.image[0];
                    }
                //}
            }
        }
    }

    /// <summary>
    /// ゲーム開始時の場に追加される処理
    /// </summary>
    public void AddCard(GameObject card) {
        var pos = fieldPosition[addCardIndex];
        var maxDistance = Vector3.Distance(card.transform.position, pos);

        //var cardList = new List<GameObject>();
        //cardList.Add(card);
        //fieldCard_Dic[addCardIndex] = cardList;
        fieldCard_Dic[addCardIndex].Add(card);
        addCardIndex++;

        handCardTargetMaxDistance_Dic.Add(card, maxDistance);
        handCardTargePosition_Dic.Add(card, pos);
        card.transform.parent = transform;

        //タグと描画順を変更
        cardManager.SetCardTag(card, TAG.TagManager.FIELD_CARD);
        cardManager.SetCardSortingLayer(card, SortingLayer.SortingLayerManager.FIELD_CARD);
    }

    /// <summary>
    /// 指定されたカードが含まれている場のカードリストを削除
    /// </summary>
    /// <returns></returns>
    public void RemoveCard(GameObject card) {

        foreach (var data in fieldCard_Dic) {
            //if (data.Value[0] == card) {
            //    fieldCard_Dic[data.Key].Remove(card);
            //    break;
            //}

            if (data.Value.Contains(card)) {
                fieldCard_Dic[data.Key].Remove(card);
                break;
            }

        }
    }

    /// <summary>
    /// ゲーム開始時の手札が指定されている座標まで全て移動しきっているかを取得
    /// </summary>
    public bool GetIsHandOutMovement() {

        foreach (var card in fieldCard_Dic[0]) {
            if (card.transform.position != handCardTargePosition_Dic[card]) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 指定したカードで場のカードを取れるかどうかを設定
    /// </summary>
    public void SetGetCardPutIndexList(GameObject deckCard){
        var d_Card = deckCard.GetComponent<Card>();

        getCardPutIndexList.Clear();
        nonGetCardPutIndexList.Clear();

        for (int i=0;i<fieldCard_Dic.Count;i++) {

             if (fieldCard_Dic[i].Count != 0) {
                var card = fieldCard_Dic[i][0];
                var f_Card = card.GetComponent<Card>();

                if (i != selectCardIndex) {
                    if (d_Card.month == f_Card.month) {
                        getCardPutIndexList.Add(i);
                    } else {
                        nonGetCardPutIndexList.Add(i);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 場に置ける場所のリストを取得
    /// </summary>
    /// <returns></returns>
    public List<int> GetPutField() {
        var getIsPutList = new List<int>();

        for (int i=0;i<FIELD_COUNT;i++) {
            if (fieldCard_Dic[i].Count == 0) {
                getIsPutList.Add(i);
            }
        }
        return getIsPutList;
    }

    /// <summary>
    /// 場のカードを選択した時に、そのカードに対応した番号を設定
    /// </summary>
    /// <param name="card"></param>
    public void SetSelectCardIndex(GameObject card){

        foreach (var data in fieldCard_Dic) {

            //if (data.Value[0] == card) {
            //    selectCardIndex = data.Key;
            //    break;
            //}

            if (data.Value.Contains(card)) {
                selectCardIndex = data.Key;
                break;
            }
        }
    }
    /// <summary>
    /// 場のカードを選択した時に、そのカードに対応した番号を設定
    /// </summary>
    /// <param name="index"></param>
    public void SetSelectCardIndex(int index) {
        selectCardIndex = index;
    }
}
