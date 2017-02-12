using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TitleManager : MonoBehaviour {

    private int CENTER_INDEX;

    private string nextSceneName;
    private GameManager.STATE nextGameMangerState;

    private const string BUTTON_NAME_CHARACTER_INTRODUCTION = "CharacterIntroduction";
    private const string BUTTON_NAME_SETTING = "Setting";
    private const string BUTTON_NAME_STORY = "Story";
    private const string BUTTON_NAME_HANAFUDA_TRAINING = "HanafudaTraining";
    private const string BUTTON_NAME_GALLARY = "Gallary";

    [SerializeField]
    private Button[] selectButtonList;

    private bool onceSettingValue;

    [SerializeField]
    private float moveSpeed;

    private Dictionary<GameObject, bool> isMove_Dic;
    private Dictionary<GameObject, Image> buttonImage_Dic;

    private Dictionary<int, float> nextPositionDistance_Dic;
    private Dictionary<int, int> drawingOrder_Dic;
    private Dictionary<int, Color> color_Dic;
    private Dictionary<int, Vector3> position_Dic;
    private Dictionary<int, Vector3> scale_Dic;

    public Vector3 ON_BUTTON_SCALE_SIZE;

    void Awake() {
        GameManager.Instance.state = GameManager.STATE.TITLE;
        //InterruptionDialogManager.Instance.SetCanvasCamera();
    }

    // Use this for initialization
    void Start() {

        FlickDicisionManager.Instance.SetState(FlickDicisionManager.STATE.WAIT_INPUT);

        CENTER_INDEX = selectButtonList.Length / 2;

        isMove_Dic = new Dictionary<GameObject, bool>();
        buttonImage_Dic = new Dictionary<GameObject, Image>();

        nextPositionDistance_Dic = new Dictionary<int, float>();
        drawingOrder_Dic = new Dictionary<int, int>();
        color_Dic = new Dictionary<int, Color>();
        position_Dic = new Dictionary<int, Vector3>();
        scale_Dic = new Dictionary<int, Vector3>();

        for (int i = 0; i < selectButtonList.Length; i++) {

            var buttonObject = selectButtonList[i].gameObject;
            isMove_Dic.Add(buttonObject, true);

            var image = buttonObject.GetComponent<Image>();
            buttonImage_Dic.Add(buttonObject, image);

            var selectButton = buttonObject.GetComponent<TitleSelectButton>();
            selectButton.RegistStatus(i);

            color_Dic.Add(i, buttonObject.GetComponent<Image>().color);
            position_Dic.Add(i, buttonObject.transform.localPosition);
            scale_Dic.Add(i, buttonObject.transform.localScale);
        }

        drawingOrder_Dic.Add(0, 0);
        drawingOrder_Dic.Add(1, 2);
        drawingOrder_Dic.Add(2, 4);
        drawingOrder_Dic.Add(3, 3);
        drawingOrder_Dic.Add(4, 1);

        onceSettingValue = true;
    }

    // Update is called once per frame
    void Update() {

        if (GameManager.Instance.state == GameManager.STATE.TITLE &&
            FlickDicisionManager.Instance.state == FlickDicisionManager.STATE.FLICK_ANIMATION) {

            //フリックした直後の1回だけ、フリックした方向への情報を設定
            if (onceSettingValue) {
                onceSettingValue = false;
                SortArrayValue();
                SetNextMoveStatus();
            }

            FlickAnimation();
        }
    }

    /// <summary>
    /// 配列の中身をフリックした方向にずらす
    /// </summary>
    private void SortArrayValue() {

        //ボタン押下の可否を設定
        selectButtonList[CENTER_INDEX].enabled = false;

        if (FlickDicisionManager.Instance.flickkDirection == FlickDicisionManager.FLICK_DIRECTION.LEFT) {
            var sortIndex = 0;
            var tmp = selectButtonList[sortIndex];
            for (int i = sortIndex + 1; i < selectButtonList.Length; i++) {
                selectButtonList[sortIndex] = selectButtonList[i];
                sortIndex = i;
            }
            selectButtonList[selectButtonList.Length - 1] = tmp;

        } else if (FlickDicisionManager.Instance.flickkDirection == FlickDicisionManager.FLICK_DIRECTION.RIGHT) {
            var sortIndex = selectButtonList.Length - 1;
            var tmp = selectButtonList[sortIndex];
            for (int i = sortIndex - 1; i >= 0; i--) {
                selectButtonList[sortIndex] = selectButtonList[i];
                sortIndex = i;
            }
            selectButtonList[0] = tmp;
        }

        //描画順の設定
        for (int i = 0; i < selectButtonList.Length; i++) {
            selectButtonList[i].transform.SetSiblingIndex(drawingOrder_Dic[i]);
        }
    }

    /// <summary>
    /// フリックした方向へ移動するための情報を設定
    /// </summary>
    private void SetNextMoveStatus() {

        for (int i = 0; i < selectButtonList.Length; i++) {
            var currentPosition = selectButtonList[i].transform.localPosition;
            var nextMovePosition = position_Dic[i];
            var distance = Vector3.Distance(currentPosition, nextMovePosition);

            //次の座標までの距離を設定
            if (nextPositionDistance_Dic.ContainsKey(i)) {
                nextPositionDistance_Dic[i] = distance;
            } else {
                nextPositionDistance_Dic.Add(i, distance);
            }

            //次の座標まで移動フラグの設定
            if (isMove_Dic.ContainsKey(selectButtonList[i].gameObject)) {
                isMove_Dic[selectButtonList[i].gameObject] = true;
            } else {
                isMove_Dic.Add(selectButtonList[i].gameObject, true);
            }
        }
    }

    /// <summary>
    /// フリックの処理
    /// </summary>
    private void FlickAnimation() {
        var isMove = true;

        for (int i = 0; i < selectButtonList.Length; i++) {
            var button = selectButtonList[i].gameObject;

            if (isMove_Dic[button]) {

                //目標の座標まで動かす
                button.transform.localPosition = Vector3.MoveTowards(button.transform.localPosition,
                                                                 position_Dic[i],
                                                                 Time.deltaTime * moveSpeed);

                //目標までの距離に近づくにつれて、大きさを変える
                var distance = Vector3.Distance(button.transform.localPosition, position_Dic[i]);
                var distanceLeap = Mathf.Lerp(1f, 0f, distance / nextPositionDistance_Dic[i]);
                button.transform.localScale = Vector3.Lerp(button.transform.localScale, scale_Dic[i], distanceLeap);
                buttonImage_Dic[button].color = Color.Lerp(buttonImage_Dic[button].color, color_Dic[i], distanceLeap);

                if (button.transform.localPosition != position_Dic[i]) {
                    isMove = false;
                } else {
                    isMove_Dic[button] = false;
                }

            }
        }

        if (isMove) {
            onceSettingValue = true;
            FlickDicisionManager.Instance.SetState(FlickDicisionManager.STATE.WAIT_INPUT);
            FlickDicisionManager.Instance.SetFlickkDirection(FlickDicisionManager.FLICK_DIRECTION.NONE);

            //ボタン押下の可否を設定
            selectButtonList[CENTER_INDEX].enabled = true;
        }

    }
}
