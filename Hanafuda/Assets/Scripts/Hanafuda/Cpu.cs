using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cpu : MonoBehaviour {

    [SerializeField]
    private CardManager cardManager;
    [SerializeField]
    private FieldManager fieldManager;
    [SerializeField]
    private Field field;

    //得点
    public int score;

    private Dictionary<GameObject, float> handCardTargetMaxDistance_Dic = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, Vector3> handCardTargePosition_Dic = new Dictionary<GameObject, Vector3>();
    private List<GameObject> hand = new List<GameObject>();

    private const float adjust = 1.16f;
    private Vector3 handCardFirstPosition = new Vector3(-4.06f, 4.0f, 0.0f);
    private Vector3 cardScale = new Vector3(0.75f, 0.75f, 1.0f);

    //手札のカードで場のカードを取れるカードリスト
    public Dictionary<GameObject, List<int>> getCardList_Dic = new Dictionary<GameObject, List<int>>();
    public Dictionary<GameObject, List<int>> nonGetCardList_Dic = new Dictionary<GameObject, List<int>>();

    public void Initialize() {
    }

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update () {
        switch (fieldManager.state) {
            case FieldManager.STATE.CARD_HAND_OUT: CardHandOut(); break;
            case FieldManager.STATE.WAIT_SELECT_CARD: Game(); break;
            case FieldManager.STATE.WAIT_FIELD_SELECT_CARD: WaitFieldSelectCard(); break;
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
            hand[i].transform.localScale = Vector3.Lerp(hand[i].transform.localScale,cardScale,distanceLeap);


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
        handCardTargePosition_Dic.Add(card, pos);
        card.transform.parent = transform;
        
    }

    /// <summary>
    /// 手札からカードを削除
    /// </summary>
    public void RemoveCard(GameObject card) {
        hand.Remove(card);
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
    /// ゲーム中の処理
    /// </summary>
    private void Game() {

        //COMのターンだったら
        if (fieldManager.turnPlayer == FieldManager.TURNPLAYER.COM) {

            //取得可能リストを設定する
            SetGetCardList();

            SelectPutDownCard();


        }
    }

    /// <summary>
    /// 自分のターン時に手札と場のカードから同じ月（花）の取得可能リストと取得不可能リストを設定する
    /// </summary>
    /// <returns></returns>
    private void SetGetCardList() {

        getCardList_Dic.Clear();
        nonGetCardList_Dic.Clear();

        for (int j = 0; j < field.fieldCard_Dic.Count; j++) {
            Debug.Log("field.fieldCard_Dic[j].Count" + field.fieldCard_Dic[j].Count);
        }

        Debug.Log("CPU SetGetCardList");
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
            }

            if (nonGetCardList.Count > 0) {
                nonGetCardList_Dic.Add(hand[i], nonGetCardList);
            }
        }
    }

    /// <summary>
    /// 自分のターン時にカードを出す処理
    /// </summary>
    private void SelectPutDownCard() {
        //とりあえず手札の中からランダムでカードを出す
        var randomHandIndex = Random.Range(0, hand.Count);
        var selectCard = hand[randomHandIndex];

        var s_Card = selectCard.GetComponent<Card>();

        int putIndex = -1;
        //ランダムで選ばれた手札のselectCardカードで場のカードが取れるかどうかをチェック
        for (int i=0;i<field.fieldCard_Dic.Count;i++) {
            if (field.fieldCard_Dic[i].Count > 0) {
                var card = field.fieldCard_Dic[i][0];
                var f_Card = card.GetComponent<Card>();

                if (s_Card.month == f_Card.month) {
                    putIndex = i;
                    break;
                }
            }
        }

        //ランダムで選んだカードと同じ月（花）の場カードが場になかった場合、空いている場所をランダム選ぶ
        if (putIndex == -1) {
            //fieldの各場所を調べて、何もカードが置かれていないところから、ランダムで選んで場に出す
            var isPutList = field.GetPutField();
            putIndex = isPutList[Random.Range(0, isPutList.Count)];

            field.fieldCard_Dic[putIndex].Add(selectCard);

        } else {

            field.SetSelectCardIndex(putIndex);
            fieldManager.getCardList.Add(selectCard);

            //場の対応した番号にあるカードを全て取得リストに追加
            for (int i = 0; i < field.fieldCard_Dic[field.selectCardIndex].Count; i++) {
                fieldManager.getCardList.Add(field.fieldCard_Dic[field.selectCardIndex][i]);
            }
        }

        selectCard.transform.parent = field.transform;
        cardManager.SetCardTag(selectCard, TAG.TagManager.FIELD_CARD);
        cardManager.SetCardSortingLayer(selectCard, SortingLayer.SortingLayerManager.FIELD_CARD);
        cardManager.SetCardOrderInLayer(selectCard, 3);

        fieldManager.SetMoveCardStatus(selectCard,
                                        field.fieldPosition[putIndex],
                                        Vector3.zero,
                                        field.cardScale,
                                        FieldManager.STATE.TURN_PLAYER_SELECT_CARD);

        RemoveCard(selectCard);
    }

    /// <summary>
    /// デッキから場に出るときに取得できるカードを選択する処理
    /// </summary>
    private void WaitFieldSelectCard() {



        //とりあえずランダムでカードを選ぶ
        var randomIndex = Random.Range(0, field.getCardPutIndexList.Count);
        fieldManager.FieldSelectCard(randomIndex);
    }


}
