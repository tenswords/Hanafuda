using UnityEngine;
using System.Collections;

public class CardTouchManager : MonoBehaviour {

    [SerializeField]
    private CardManager cardManager;
    [SerializeField]
    private FieldManager fieldManager;

    [SerializeField]
    private Player player;


    private Vector3 EMPHASIZE_SCALE = new Vector3(1.5f, 1.5f, 1.0f);
    private Vector3 EMPHASIZE_ROTATION = new Vector3(0.0f,0.0f,-7.5f);


    private GameObject selectCard;

    private GameObject oldSelectCard;
    private Vector3 oldCardScale;
    private Vector3 oldCardRotation;


    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        switch (fieldManager.state) {
            case FieldManager.STATE.WAIT_SELECT_CARD: WaitSelectCard(); break;
        }
	}

    private void WaitSelectCard(){

        if (Input.GetMouseButtonUp(0)) {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var collider = Physics2D.OverlapPoint(touchPosition);
            Debug.Log("collider " + collider);

            //場のカードを取れないカードを選択している用のコライダー
            var layerMask = 1 << LayerMask.NameToLayer("FieldSpace");
            var fieldSpaceCllider = Physics2D.OverlapPoint(touchPosition, layerMask);

            //fieldSpaceClliderがnullではない場合、"FieldSpace"をタッチした
            if (fieldSpaceCllider) {

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
        cardManager.SetCardTag(selectCard,TAG.TagManager.PLAYER_HAND_FORE);
        cardManager.SetCardSortingLayer(selectCard, SortingLayer.SortingLayerManager.PLAYER_HAND_FORE);
        cardManager.SetCardSortingLayer(selectCard.transform.GetChild(0).gameObject, SortingLayer.SortingLayerManager.PLAYER_HAND_FORE);

        //場のカードの取れないカードを暗くさせる
        var nonGetcardList = player.GetNonGetCardList_Dic(selectCard);
        fieldManager.SetFieldCardColor(nonGetcardList, false);

        //場のカードの取れるカードが1枚もない場合、場のコライダーをON
        //1枚以上ある場合、場のコライダーをOFF
        if (!player.IsGetCard(selectCard)) fieldManager.SetFieldColliderActive(true);
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

            var oldCardList = player.GetNonGetCardList_Dic(oldSelectCard);
            fieldManager.SetFieldCardColor(oldCardList, true);
        }
    }

    /// <summary>
    /// 選択しているカードがある状態で場のカードをタッチしたときに、同じ月（花）ならば取る処理
    /// </summary>
    private void SelectFieldCard(GameObject fieldCard) {
        var s_card = selectCard.GetComponent<Card>();
        var f_card = fieldCard.GetComponent<Card>();

        if (s_card.month == f_card.month) {
            fieldManager.SetMoveCardStatus(selectCard, fieldCard.transform.position,FieldManager.STATE.TURN_PLAYER_SELECT_CARD);
        }
    }
}
