using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    [SerializeField]
    private CardManager cardManager;
    [SerializeField]
    private FieldManager fieldManager;
    [SerializeField]
    private Field field;

    private List<GameObject> hand = new List<GameObject>();
    private Dictionary<GameObject, float> handCardTargetMaxDistance_Dic = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, Vector3> handCardTargePosition_Dic = new Dictionary<GameObject, Vector3>();

    private const float adjust = 1.925f;
    private Vector3 fieldFirstPosition = new Vector3(-6.7375f, -3.5f, 0.0f);
    private Vector3 cardScale = new Vector3(1.25f,1.25f,1.0f);

    //手札のカードで場のカードを取れるカードリスト
    private Dictionary<GameObject, List<GameObject>> getCardList_Dic = new Dictionary<GameObject, List<GameObject>>();
    private Dictionary<GameObject, List<GameObject>> nonGetCardList_Dic = new Dictionary<GameObject, List<GameObject>>();

    private bool onceGetCardList;

    private Color CARD_EFFECT_MAX_ALFA = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    private Color CARD_EFFECT_MIN_ALFA = new Color(1.0f, 1.0f, 1.0f, 0.2f);

    // Use this for initialization
    void Start () {
        onceGetCardList = false;
    }

    // Update is called once per frame
    void Update() {
        switch (fieldManager.state) {
            case FieldManager.STATE.CARD_HAND_OUT: CardHandOut(); break;
            case FieldManager.STATE.WAIT_SELECT_CARD: Game(); break;

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
                if (hand[i].transform.rotation.eulerAngles.y >= 270.0f) {
                    //if (rotationLeap >= fieldManager.handCardCenterDistanePer) {
                    spriteRenderer.sprite = card.image[0];
                }
            }
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
        handCardTargePosition_Dic.Add(card,pos);
        card.transform.parent = transform;

        //タグと描画順を変更
        cardManager.SetCardTag(card, TAG.TagManager.PLAYER_HAND);
        cardManager.SetCardSortingLayer(card,SortingLayer.SortingLayerManager.PLAYER_HAND);
        cardManager.SetCardSortingLayer(card.transform.GetChild(0).gameObject, SortingLayer.SortingLayerManager.PLAYER_HAND);
    }

    /// <summary>
    /// ゲーム開始時の手札が指定されている座標まで全て移動しきっているかを取得
    /// </summary>
    public bool GetIsHandOutMovement() {

        foreach (var card in hand) {
            if(card.transform.position != handCardTargePosition_Dic[card]) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// ゲーム中の処理
    /// </summary>
    private void Game() {

        //プレイヤーのターンだったら
        if (fieldManager.tunePlayer == FieldManager.TURNPLAYER.PLAYER) {

            //まだ取得可能リストを取得していなかったら、取得する
            if (!onceGetCardList) {
                onceGetCardList = true;
                SetGetCardList();
            }
        }
    }

    /// <summary>
    /// 自分のターン時にカードを出す処理
    /// </summary>
    public void PutDownCard() {
    }

    /// <summary>
    /// 手札管理の初期化（エフェクトや取得可能リストを消去）
    /// </summary>
    private void MyHandInitialize() {
    }

    /// <summary>
    /// 自分のターン時に手札と場のカードから同じ月（花）の取得可能リストと取得不可能リストを設定する
    /// </summary>
    /// <returns></returns>
    private void SetGetCardList() {

        Debug.Log("Player SetGetCardList");
        for (int i = 0; i < hand.Count; i++) {
            var myCard = hand[i].GetComponent<Card>();

            var getCardList = new List<GameObject>();
            var nonGetCardList = new List<GameObject>();
            for (int j = 0; j < field.fieldCard.Count; j++) {
                var fieldCard = field.fieldCard[j].GetComponent<Card>();

                if (myCard.month == fieldCard.month) {
                    getCardList.Add(field.fieldCard[j]);
                } else {
                    nonGetCardList.Add(field.fieldCard[j]);
                }
            }

            if (getCardList.Count > 0) {
                getCardList_Dic.Add(hand[i], getCardList);
                hand[i].transform.GetChild(0).gameObject.SetActive(true);
            }

            if (nonGetCardList.Count > 0) {
                nonGetCardList_Dic.Add(hand[i], nonGetCardList);
            }
        }
    }




    /// <summary>
    /// 指定したカードで場のカードを取れるカードリスト
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetGetCardList_Dic(GameObject card) {
        return getCardList_Dic[card];
    }

    /// <summary>
    /// 指定したカードで場のカードを1枚以上取れるかどうかを取得(true:取れる false:取れない)
    /// </summary>
    /// <returns></returns>
    public bool IsGetCard(GameObject card) {
        return getCardList_Dic.ContainsKey(card);
    }


    /// <summary>
    /// 指定したカードで場のカードを取れないカードリスト
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetNonGetCardList_Dic(GameObject card) {
        return nonGetCardList_Dic[card];
    }
}
