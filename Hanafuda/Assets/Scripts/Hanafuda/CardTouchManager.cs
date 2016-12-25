using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardTouchManager : MonoBehaviour {

    [SerializeField]
    private CardManager cardManager;
    [SerializeField]
    private FieldManager fieldManager;

    [SerializeField]
    private Player player;
    [SerializeField]
    private Field field;

    private Vector3 EMPHASIZE_SCALE = new Vector3(1.5f, 1.5f, 1.0f);
    private Vector3 EMPHASIZE_ROTATION = new Vector3(0.0f, 0.0f, -7.5f);

    private GameObject selectCard;

    private GameObject oldSelectCard;
    private Vector3 oldCardScale;
    private Vector3 oldCardRotation;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        switch (fieldManager.state) {
            case FieldManager.STATE.WAIT_SELECT_CARD: WaitSelectCard(); break;
            case FieldManager.STATE.WAIT_FIELD_SELECT_CARD: WaitFieldSelectCard(); break;
        }
    }

    /// <summary>
    /// プレイヤーのターンのときに手札から選択する処理
    /// </summary>
    private void WaitSelectCard() {

        if (Input.GetMouseButtonUp(0)) {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var collider = Physics2D.OverlapPoint(touchPosition);
            Debug.Log("collider " + collider);

            //場のカードを取れないカードを選択している用のコライダー
            var layerMask = 1 << LayerMask.NameToLayer("FieldSpace");
            //var fieldSpaceCllider = Physics2D.OverlapPoint(touchPosition, layerMask);

            var isFieldSpace = false;
            var fieldSpaceCllider = Physics2D.OverlapPointAll(touchPosition, layerMask);
            foreach (var col in fieldSpaceCllider) {
                if (col.tag == TAG.TagManager.FIELD_SPACE) {
                    isFieldSpace = true;
                }
            }

            //isFieldSpaceがtrueではない場合、"FieldSpace"をタッチした
            if (isFieldSpace) {

                //fieldのisputがfalseになっているところから、ランダムで選んで場に出す
                var isPutList = field.GetPutField();
                var putIndex = isPutList[Random.Range(0, isPutList.Count)];

                field.SetSelectCardIndex(putIndex);

                fieldManager.SetMoveCardStatus(selectCard,
                               field.fieldPosition[putIndex],
                               Vector3.zero,
                               field.cardScale,
                               FieldManager.STATE.TURN_PLAYER_SELECT_CARD);

                //場のカードの色を明るくする
                foreach (var index in player.nonGetCardList_Dic[selectCard]) {
                    fieldManager.SetFieldCardColor(field.fieldCard_Dic[index], true);
                }

                //カードエフェクトを消す
                foreach (var data in player.getCardList_Dic) {
                    var cardEffect = data.Key.transform.GetChild(0).gameObject;
                    cardManager.SetCardEffectIsActive(cardEffect, false);
                }

                //選択したカードを場に追加
                var _selectCard = new List<GameObject>();
                _selectCard.Add(selectCard);
                field.fieldCard_Dic[putIndex] = _selectCard;

                selectCard.transform.parent = field.transform;
                cardManager.SetCardTag(selectCard, TAG.TagManager.FIELD_CARD);
                cardManager.SetCardSortingLayer(selectCard, SortingLayer.SortingLayerManager.FIELD_CARD_FORE);

                selectCard = null;
                fieldManager.SetFieldColliderActive(false);

            } else {

                if (collider) {

                    switch (collider.tag) {
                        case TAG.TagManager.PLAYER_HAND_FORE:
                            //タッチした場所のカードのタグが選択されているプレイヤーの手札の場合
                            //選択されているカードをキャンセルする
                            ReturnOldSelectCard();
                            selectCard = null;
                            fieldManager.SetFieldColliderActive(false);
                            break;

                        case TAG.TagManager.PLAYER_HAND:
                            //タッチした場所のカードのタグがプレイヤーの手札の場合
                            //強調されているカードがあれば、元に戻し新たに選択したカードを強調させる
                            SelectPlayerHandCard(collider.gameObject);
                            break;

                        case TAG.TagManager.FIELD_CARD:
                            //タッチした場所のカードのタグが場のカードの場合
                            //プレイヤーのカードが選択されている、かつ
                            //選択されているカードでそのカードが取れるなら、カードを取る処理
                            //それ以外では何もしない
                            SelectFieldCard(collider.gameObject);

                            //デッキから場にカードが出る時に、プレイヤーが選択する処理

                            break;

                        default:
                            //取ったカードリスト
                            break;
                    }
                }
            }
        }

        if (Input.touchCount > 0) {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var collider = Physics2D.OverlapPoint(touchPosition);
            Debug.Log("collider " + collider);
        }

    }

    /// <summary>
    /// プレイヤーの手札のカードをタッチした時の処理
    /// </summary>
    private void SelectPlayerHandCard(GameObject selectCard) {

        this.selectCard = selectCard;

        //1つ前に選択していたカードを元に戻す
        ReturnOldSelectCard();

        //今回選択したカードを保存しておく
        oldSelectCard = selectCard;
        oldCardScale = selectCard.transform.localScale;
        oldCardRotation = selectCard.transform.rotation.eulerAngles;

        //今回選択したカードに対しての処理
        selectCard.transform.localScale = EMPHASIZE_SCALE;
        selectCard.transform.rotation = Quaternion.Euler(EMPHASIZE_ROTATION);
        cardManager.SetCardTag(selectCard, TAG.TagManager.PLAYER_HAND_FORE);
        cardManager.SetCardSortingLayer(selectCard, SortingLayer.SortingLayerManager.PLAYER_HAND_FORE);
        cardManager.SetCardSortingLayer(selectCard.transform.GetChild(0).gameObject, SortingLayer.SortingLayerManager.PLAYER_HAND_FORE);

        //場の取れないカードを暗くさせる
        foreach (var index in player.nonGetCardList_Dic[selectCard]) {
            fieldManager.SetFieldCardColor(field.fieldCard_Dic[index], false);
        }

        //場のカードの取れるカードが1枚もない場合、場のコライダーをON
        //1枚以上ある場合、場のコライダーをOFF
        if (!player.getCardList_Dic.ContainsKey(selectCard)) fieldManager.SetFieldColliderActive(true);
        else fieldManager.SetFieldColliderActive(false);

    }

    /// <summary>
    /// 1つ前に選択していたカードを元に戻す処理
    /// </summary>
    private void ReturnOldSelectCard() {
        if (oldSelectCard != null) {
            oldSelectCard.transform.localScale = oldCardScale;
            oldSelectCard.transform.rotation = Quaternion.Euler(oldCardRotation);
            cardManager.SetCardTag(oldSelectCard, TAG.TagManager.PLAYER_HAND);
            cardManager.SetCardSortingLayer(oldSelectCard, SortingLayer.SortingLayerManager.PLAYER_HAND);
            cardManager.SetCardSortingLayer(oldSelectCard.transform.GetChild(0).gameObject, SortingLayer.SortingLayerManager.PLAYER_HAND);

            foreach (var index in player.nonGetCardList_Dic[oldSelectCard]) {
                fieldManager.SetFieldCardColor(field.fieldCard_Dic[index], true);
            }

        }
    }

    /// <summary>
    /// 選択しているカードがある状態で場のカードをタッチしたときに、同じ月（花）ならば取る処理
    /// </summary>
    private void SelectFieldCard(GameObject fieldCard) {
        var s_Card = selectCard.GetComponent<Card>();
        var f_Card = fieldCard.GetComponent<Card>();

        if (s_Card.month == f_Card.month) {
            field.SetSelectCardIndex(fieldCard);

            fieldManager.SetMoveCardStatus(selectCard,
                                           fieldCard.transform.position,
                                           Vector3.zero,
                                           field.cardScale,
                                           FieldManager.STATE.TURN_PLAYER_SELECT_CARD);

            fieldManager.getCardList.Add(selectCard);

            //場の対応した番号にあるカードを全て取得リストに追加
            for (int i = 0; i < field.fieldCard_Dic[field.selectCardIndex].Count; i++) {
                fieldManager.getCardList.Add(field.fieldCard_Dic[field.selectCardIndex][i]);
            }

            //場のカードの色を明るくする
            foreach (var index in player.nonGetCardList_Dic[selectCard]) {
                fieldManager.SetFieldCardColor(field.fieldCard_Dic[index], true);
            }

            //カードエフェクトを消す
            foreach (var data in player.getCardList_Dic) {
                var cardEffect = data.Key.transform.GetChild(0).gameObject;
                cardManager.SetCardEffectIsActive(cardEffect, false);
            }

            selectCard.transform.parent = field.transform;
            cardManager.SetCardTag(selectCard, TAG.TagManager.FIELD_CARD);
            cardManager.SetCardSortingLayer(selectCard, SortingLayer.SortingLayerManager.FIELD_CARD_FORE);
        }
    }

    /// <summary>
    /// デッキから場に出るときに取得できるカードを選択する処理
    /// </summary>
    private void WaitFieldSelectCard() {

        if (Input.GetMouseButtonUp(0)) {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var collider = Physics2D.OverlapPoint(touchPosition);
            Debug.Log("collider " + collider);

            if (collider) {

                switch (collider.tag) {

                    case TAG.TagManager.FIELD_CARD:
                        fieldManager.FieldSelectCard(collider.gameObject);
                    break;

                    default:
                        //取ったカードリスト
                        break;
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
