using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectButton : MonoBehaviour {

    private const int CENTER_BUTTON_INDEX = 2;

    [SerializeField]
    private Button[] selectButtonList;

    private STATE state;
    private enum STATE {
        WAIT_INPUT,
        FLICK_ANIMATION,
        BUTTON_INPUT
    }

    [SerializeField, Header("フリック判定距離")]
    private float FLICK_DISTANCE;

    [SerializeField, Header("フリック判定時間")]
    private float FLICK_DECISION_TIME;

    private Vector3 pointDownPosition;
    private Vector3 pointUpPosition;
    private float pointDownTime;
    private float pointUpTime;

    private FLICK_DIRECTION flickkDirection;
    private enum FLICK_DIRECTION {
        NONE,
        LEFT,
        RIGHT
    }

    [SerializeField]
    private float rotationSpeed;
    private Dictionary<int, float> nextPositionDistance_Dic;
    private Dictionary<GameObject, bool> isRotation_Dic;
    private Dictionary<GameObject, Image> buttonImage_Dic;

    private Dictionary<int,int> drawingOrder_Dic;
    private Dictionary<int, Color> color_Dic;
    private Dictionary<int, Vector3> position_Dic;
    private Dictionary<int, Vector3> scale_Dic;


    // Use this for initialization
    void Start() {
        nextPositionDistance_Dic = new Dictionary<int, float>();
        isRotation_Dic = new Dictionary<GameObject, bool>();
        buttonImage_Dic = new Dictionary<GameObject, Image>();

        drawingOrder_Dic = new Dictionary<int, int>();
        color_Dic = new Dictionary<int, Color>();
        position_Dic = new Dictionary<int, Vector3>();
        scale_Dic = new Dictionary<int, Vector3>();

        for (int i = 0; i < selectButtonList.Length; i++) {

            isRotation_Dic.Add(selectButtonList[i].gameObject, true);

            var image = selectButtonList[i].GetComponent<Image>();
            buttonImage_Dic.Add(selectButtonList[i].gameObject, image);

            color_Dic.Add(i, selectButtonList[i].GetComponent<Image>().color);
            position_Dic.Add(i, selectButtonList[i].transform.localPosition);
            scale_Dic.Add(i, selectButtonList[i].transform.localScale);

        }

        drawingOrder_Dic.Add(0, 0);
        drawingOrder_Dic.Add(1, 2);
        drawingOrder_Dic.Add(2, 4);
        drawingOrder_Dic.Add(3, 3);
        drawingOrder_Dic.Add(4, 1);
    }

    // Update is called once per frame
    void Update() {
        switch (state) {
            case STATE.WAIT_INPUT:
                WaitInput();
                break;
            case STATE.FLICK_ANIMATION:
                FlickAnimation();
                break;
            case STATE.BUTTON_INPUT:
                break;

        }


    }

    /// <summary>
    /// ユーザーの入力待ち
    /// </summary>
    private void WaitInput() {

        if (Input.GetMouseButtonDown(0)) {
            pointDownPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pointDownTime = Time.time;

        }
        if (Input.GetMouseButtonUp(0)) {
            pointUpPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pointUpTime = Time.time;

            var flickdistance = Vector3.Distance(pointDownPosition, pointUpPosition);

            //Debug.Log("flickdistance " + flickdistance);
            if (flickdistance > FLICK_DISTANCE && (pointUpTime - pointDownTime) < FLICK_DECISION_TIME) {
                //横方向へのフリックの場合
                if ((Mathf.Abs(pointUpPosition.x - pointDownPosition.x)) <= (Mathf.Abs(pointUpPosition.y - pointDownPosition.y))) {
                    Debug.Log("フリック判定");
                    state = STATE.FLICK_ANIMATION;

                    if ((pointUpPosition.x - pointDownPosition.x) < 0) flickkDirection = FLICK_DIRECTION.LEFT;
                    else if ((pointUpPosition.x - pointDownPosition.x) > 0) flickkDirection = FLICK_DIRECTION.RIGHT;

                    SortArrayValue();
                    SetNextMoveStatus();
                }
            }
        }
    }

    /// <summary>
    /// 配列の中身をフリックした方向にずらす
    /// </summary>
    private void SortArrayValue() {
        if (flickkDirection == FLICK_DIRECTION.LEFT) {
            var sortIndex = 0;
            var tmp = selectButtonList[sortIndex];
            for (int i = sortIndex + 1; i < selectButtonList.Length; i++) {
                selectButtonList[sortIndex] = selectButtonList[i];
                sortIndex = i;
            }
            selectButtonList[selectButtonList.Length - 1] = tmp;

        } else if (flickkDirection == FLICK_DIRECTION.RIGHT) {
            var sortIndex = selectButtonList.Length - 1;
            var tmp = selectButtonList[sortIndex];
            for (int i = sortIndex - 1; i >= 0; i--) {
                selectButtonList[sortIndex] = selectButtonList[i];
                sortIndex = i;
            }
            selectButtonList[0] = tmp;
        }

        //描画順の設定
        for (int i=0;i<selectButtonList.Length;i++) {
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
            }else { 
                nextPositionDistance_Dic.Add(i, distance);
            }

            //次の座標まで移動している稼動のフラグの設定
            if (isRotation_Dic.ContainsKey(selectButtonList[i].gameObject)) {
                isRotation_Dic[selectButtonList[i].gameObject] = true;
            } else {
                isRotation_Dic.Add(selectButtonList[i].gameObject,true);
            }
        }
    }

    /// <summary>
    /// 全ボタンを回転させる
    /// </summary>
    private void FlickAnimation() {
        var isRotation = true;

        for (int i = 0; i < selectButtonList.Length; i++) {
            var button = selectButtonList[i].gameObject;

            if (isRotation_Dic[button]) {

                //目標の座標まで動かす
                button.transform.localPosition = Vector3.MoveTowards(button.transform.localPosition,
                                                                 position_Dic[i],
                                                                 Time.deltaTime * rotationSpeed);

                //目標までの距離に近づくにつれて、大きさを変える
                var distance = Vector3.Distance(button.transform.localPosition, position_Dic[i]);
                var distanceLeap = Mathf.Lerp(1f, 0f, distance / nextPositionDistance_Dic[i]);
                button.transform.localScale = Vector3.Lerp(button.transform.localScale, scale_Dic[i], distanceLeap);
                buttonImage_Dic[button].color = Color.Lerp(buttonImage_Dic[button].color, color_Dic[i], distanceLeap);



                if (button.transform.localPosition != position_Dic[i]) {
                    isRotation = false;
                } else {
                    isRotation_Dic[button] = false;
                }

            }
        }

        if (isRotation) {
            state = STATE.WAIT_INPUT;
            flickkDirection = FLICK_DIRECTION.NONE;
        }
        
    }
    
    public void OnStoryButton() {
        FadeManager.Instance.LoadLevel(SceneName.SceneNameManager.SCENE_NAME_STORY);
    }


}
