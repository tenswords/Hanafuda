using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    [SerializeField]
    private CardManager cardManager;
    [SerializeField]
    private FieldManager fieldManager;
    [SerializeField]
    private CardTouchManager cardTouchManager;

    [SerializeField]
    private Field field;

    //得点
    //public int score;
    //役成立済みリスト
    public Dictionary<string,int> flushList = new Dictionary<string, int>();
    ////役成立済みリスト
    //public List<string> establishRole_FlushNameList;
    ////役成立済みリスト
    //public List<int> establishRole_FlushScoreList;

    [SerializeField]
    private List<GameObject> hand = new List<GameObject>();
    private Dictionary<GameObject, float> handCardTargetMaxDistance_Dic = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, Vector3> handCardTargePosition_Dic = new Dictionary<GameObject, Vector3>();

    private const float adjust = 1.925f;
    private Vector3 handCardFirstPosition = new Vector3(-6.7375f, -3.5f, 0.0f);
    private Vector3 cardScale = new Vector3(1.25f,1.25f,1.0f);

    private Quaternion TURN_CARD_ROTATION = Quaternion.Euler(new Vector3(0.0f, 359.9f, 0.0f));

    //手札のカードで場のカードを取れるカードリスト
    public Dictionary<GameObject, List<int>> getCardList_Dic = new Dictionary<GameObject, List<int>>();
    public Dictionary<GameObject, List<int>> nonGetCardList_Dic = new Dictionary<GameObject, List<int>>();

    private bool onceGetCardList;

    private Color CARD_EFFECT_MAX_ALFA = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    private Color CARD_EFFECT_MIN_ALFA = new Color(1.0f, 1.0f, 1.0f, 0.2f);

    public void Initialize() {
        onceGetCardList = false;
        cardTouchManager.Initialize();
    }

    // Use this for initialization
    void Start () {
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

            //目標の座標まで動かす
            hand[i].transform.position = Vector3.MoveTowards(hand[i].transform.position,
                                                             handCardTargePosition_Dic[hand[i]],
                                                             Time.deltaTime * fieldManager.handCardSpeed);

            //目標までの距離に近づくにつれて、大きさを変える
            var distance = Vector3.Distance(hand[i].transform.position, handCardTargePosition_Dic[hand[i]]);
            var distanceLeap = Mathf.Lerp(1f, 0f, distance / handCardTargetMaxDistance_Dic[hand[i]]);
            hand[i].transform.localScale = Vector3.Lerp(hand[i].transform.localScale, cardScale, distanceLeap);

            //一定距離まで移動したら、回転させる
            //if (distanceLeap >= fieldManager.handCardCenterDistanePer) {

            //    hand[i].transform.rotation = Quaternion.Slerp(hand[i].transform.rotation, Quaternion.identity, distanceLeap);

            //    //本当は中間まで回転したらの割合でやりたい
            //    if (hand[i].transform.rotation.eulerAngles.y >= 270.0f) {
            //        var spriteRenderer = hand[i].GetComponent<SpriteRenderer>();
            //        var card = hand[i].GetComponent<Card>();

            //        spriteRenderer.sprite = card.image[0];
            //    }
            //}

            var spriteRenderer = hand[i].GetComponent<SpriteRenderer>();
            var card = hand[i].GetComponent<Card>();

            hand[i].transform.rotation = Quaternion.Slerp(hand[i].transform.rotation, TURN_CARD_ROTATION, distanceLeap);

            //本当は中間まで回転したらの割合でやりたい
            if (hand[i].transform.rotation.eulerAngles.y >= 270.0f && spriteRenderer.sprite != card.image[0]) {
                spriteRenderer.sprite = card.image[0];
            }



        }
    }

    /// <summary>
    /// ゲーム開始時の手札が追加される処理
    /// </summary>
    public void AddCard(GameObject card) {

        var pos = handCardFirstPosition;
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
    /// 手札からカードを削除
    /// </summary>
    public void RemoveCard(GameObject card) {
        hand.Remove(card);
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
        if (fieldManager.turnPlayer == FieldManager.TURNPLAYER.PLAYER) {

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

        getCardList_Dic.Clear();
        nonGetCardList_Dic.Clear();

        //Debug.Log("Player SetGetCardList");
        for (int i = 0; i < hand.Count; i++) {
            var myCard = hand[i].GetComponent<Card>();

            var getCardList = new List<int>();
            var nonGetCardList = new List<int>();
            for (int j = 0; j < field.fieldCard_Dic.Count; j++) {
                if (field.fieldCard_Dic[j].Count != 0) {
                    var fieldCard = field.fieldCard_Dic[j][0].GetComponent<Card>();

                    if (myCard.month == fieldCard.month) {
                        getCardList.Add(j);
                    } else {
                        nonGetCardList.Add(j);
                    }
                }
            }

            if (getCardList.Count > 0) {
                getCardList_Dic.Add(hand[i], getCardList);

                var cardEffect = hand[i].transform.GetChild(0).gameObject;
                cardManager.SetCardEffectIsActive(cardEffect, true);
            }

            if (nonGetCardList.Count > 0) {
                nonGetCardList_Dic.Add(hand[i], nonGetCardList);
            }
        }
    }

    /// <summary>
    /// 成立済みリストの中に光系の役があるかチェックし、ある場合、その役を取得
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public string GetFlushListHikari() {
        //foreach (var establishRole in establishRole_FlushList) {
        //    if (establishRole.Contains("光")) {
        //        return establishRole;
        //    }
        //}
        //return "";

        
        foreach (var establishRoleData in flushList) {
            if (establishRoleData.Key.Contains("光")) {
                return establishRoleData.Key;
            }
        }
        return "";
    }

    /// <summary>
    /// 現在のトータルスコアを取得
    /// </summary>
    /// <returns></returns>
    public int GetTotalScore() {
        var score = 0;
        foreach (var data in flushList) {
            score += flushList[data.Key];
        }
        return score;
    }

    public Dictionary<string ,int> GetFlushList() {
        return flushList;
    }

    public void SetFlushList(string[] flushSplit) {
        for (int i = 0; i < flushSplit.Length - 1; i++) {

            var data = flushSplit[i].Split("、"[0]);
            var flushText = data[0];
            var flushScore = data[1];

            flushList.Add(flushText, int.Parse(flushScore));
        }
    }

    /// <summary>
    /// 手札の枚数を取得
    /// </summary>
    /// <returns></returns>
    public int GetHandCardCount() {
        return hand.Count;
    }
}
