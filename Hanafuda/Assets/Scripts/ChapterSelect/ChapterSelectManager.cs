using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChapterSelectManager : MonoBehaviour {

    private int currentSelectButtonIndex;
    private int oldCurrentSelectButtonIndex;

    [SerializeField]
    private GameObject[] selectButtonParentList;

    private Button[] selectButtonList;
    
    private const string BUTTON_NAME_TITLE = "Title";
    private const string BUTTON_NAME_CHAPTER_1 = "Chapter1";
    private const string BUTTON_NAME_CHAPTER_2 = "Chapter2";

    private float moveVec;
    private bool isReturnPosition;
    private bool onceSettingValue;

    [SerializeField]
    private float moveSpeed;
    [SerializeField,Header("ボタン間の距離")]
    private float nextPositionDistance;

    [SerializeField]
    private Color alphaColor;

    private Dictionary<GameObject, bool> isMove_Dic;
    private Dictionary<GameObject, Image> buttonImage_Dic;

    private Dictionary<int, float> nextPositionDistance_Dic;
    private Dictionary<int, Vector3> nextPosition_Dic;

    [SerializeField]
    private GameObject viewMainParent;
    [SerializeField]
    private GameObject viewMaskParent;

    [SerializeField]
    private SlidingDoor slidingDoor;

    [SerializeField]
    private GameObject storyTitle;
    [SerializeField]
    private StoryTitle storyTitleChapterImage;
    [SerializeField]
    private Sprite[] storyTitleImageList;

    private bool isStroyTitleAnimation;
    private bool isTap;

    public Vector2 TITLE_BUTTON_SCALE_SIZE;

    // Use this for initialization
    void Start () {

        FlickDicisionManager.Instance.SetState(FlickDicisionManager.STATE.WAIT_INPUT);

        selectButtonList = new Button[selectButtonParentList.Length];

        isMove_Dic = new Dictionary<GameObject, bool>();
        buttonImage_Dic = new Dictionary<GameObject, Image>();

        nextPositionDistance_Dic = new Dictionary<int, float>();
        nextPosition_Dic = new Dictionary<int, Vector3>();

        for (int i = 0; i < selectButtonParentList.Length; i++) {
            var button = selectButtonParentList[i].transform.GetChild(0).GetComponent<Button>();
            selectButtonList[i] = button;

            var buttonObject = selectButtonList[i].gameObject;
            var buttonImage = buttonObject.GetComponent<Image>();
            isMove_Dic.Add(buttonObject, true);
            buttonImage_Dic.Add(buttonObject, buttonImage);

            var selectButton = buttonObject.GetComponent<ChapterSelectButton>();
            selectButton.RegistStatus(i);

            var mirrorSurfaceObject = selectButtonParentList[i].transform.GetChild(1).gameObject;
            var mirrorSurfaceImage = mirrorSurfaceObject.GetComponent<Image>();
            buttonImage_Dic.Add(mirrorSurfaceObject, mirrorSurfaceImage);

        }

        onceSettingValue = true;
        isTap = true;
        currentSelectButtonIndex = 1;
        isStroyTitleAnimation = false;
    }
	
	// Update is called once per frame
	void Update () {


        if (GameManager.Instance.state == GameManager.STATE.CHAPTER_SELECT) {
            switch (FlickDicisionManager.Instance.state) {
                case FlickDicisionManager.STATE.FLICK_ANIMATION:
                    //フリックした直後の1回だけ、フリックした方向への情報を設定
                    if (onceSettingValue) {
                        onceSettingValue = false;
                        SetNextMoveStatus();
                    }

                    FlickAnimation();
                    break;

                case FlickDicisionManager.STATE.BUTTON_INPUT:
                    if (isStroyTitleAnimation) WaitTapInput(); break;
            }
        }
    }

    /// <summary>
    /// フリックの処理
    /// </summary>
    private void FlickAnimation() {
        var isMove = true;

        for (int i = 0; i < selectButtonList.Length; i++) {
            var buttonParent = selectButtonParentList[i];
            var buttonObject = selectButtonList[i].gameObject;
            var mirrorSurfaceObject = buttonParent.transform.GetChild(1).gameObject;

            if (isMove_Dic[buttonObject]) {

                //目標の座標まで動かす
                buttonParent.transform.localPosition = Vector3.MoveTowards(buttonParent.transform.localPosition,
                                                                 nextPosition_Dic[i],
                                                                 Time.deltaTime * moveSpeed);



                //目標までの距離に近づくにつれて、透明度を変更する（透明度を変更いらないかも）
                if (i == currentSelectButtonIndex || i == oldCurrentSelectButtonIndex) {
                    var distance = Vector3.Distance(buttonParent.transform.localPosition, nextPosition_Dic[i]);
                    var distanceLeap = Mathf.Lerp(1f, 0f, distance / nextPositionDistance_Dic[i]);

                    if (i == currentSelectButtonIndex) {
                        buttonImage_Dic[buttonObject].color = Color.Lerp(buttonImage_Dic[buttonObject].color, Color.white, distanceLeap);
                        buttonImage_Dic[mirrorSurfaceObject].color = Color.Lerp(buttonImage_Dic[mirrorSurfaceObject].color, Color.white, distanceLeap);

                    } else if (i == oldCurrentSelectButtonIndex) {
                        buttonImage_Dic[buttonObject].color = Color.Lerp(buttonImage_Dic[buttonObject].color, alphaColor, distanceLeap);
                        buttonImage_Dic[mirrorSurfaceObject].color = Color.Lerp(buttonImage_Dic[mirrorSurfaceObject].color, alphaColor, distanceLeap);
                    }
                }

                if (buttonParent.transform.localPosition != nextPosition_Dic[i]) {
                    isMove = false;
                } else {
                    isMove_Dic[buttonParent] = false;
                }

            }
        }

        if (isMove) {

            if (isReturnPosition) {
                //逆方向に戻るように設定
                moveVec *= -1.0f;
                isReturnPosition = false;
                SetNextStatus();

            } else {

                onceSettingValue = true;
                FlickDicisionManager.Instance.SetState(FlickDicisionManager.STATE.WAIT_INPUT);
                FlickDicisionManager.Instance.SetFlickkDirection(FlickDicisionManager.FLICK_DIRECTION.NONE);
            }
        }
    }

    /// <summary>
    /// フリックした方向へ移動するための情報を設定
    /// </summary>
    private void SetNextMoveStatus() {

        moveVec = 0.0f;

        if (FlickDicisionManager.Instance.flickkDirection == FlickDicisionManager.FLICK_DIRECTION.LEFT) {

            moveVec = -1.0f;
            if (currentSelectButtonIndex == selectButtonList.Length-1) {
                moveVec *= 0.5f;
                isReturnPosition = true;
            }else {
                oldCurrentSelectButtonIndex = currentSelectButtonIndex;

                selectButtonList[currentSelectButtonIndex].enabled = false;
                currentSelectButtonIndex++;
                selectButtonList[currentSelectButtonIndex].enabled = true;
                isReturnPosition = false;
            }

        } else if (FlickDicisionManager.Instance.flickkDirection == FlickDicisionManager.FLICK_DIRECTION.RIGHT) {

            moveVec = 1.0f;
            if (currentSelectButtonIndex == 0) {
                moveVec *= 0.5f;
                isReturnPosition = true;
            }else {
                oldCurrentSelectButtonIndex = currentSelectButtonIndex;

                selectButtonList[currentSelectButtonIndex].enabled = false;
                currentSelectButtonIndex--;
                selectButtonList[currentSelectButtonIndex].enabled = true;
                isReturnPosition = false;
            }

        }

        SetNextStatus();
    }

    /// <summary>
    /// 次の移動先、移動距離、移動状態を設定
    /// </summary>
    private void SetNextStatus() {
        for (int i = 0; i < selectButtonParentList.Length; i++) {
            var buttonParent = selectButtonParentList[i];
            var currentPosition = buttonParent.transform.localPosition;
            var nextMovePosition = currentPosition + new Vector3(moveVec * nextPositionDistance,0.0f,0.0f);
            var distance = Vector3.Distance(currentPosition, nextMovePosition);

            //次の座標を設定
            if (nextPosition_Dic.ContainsKey(i)) {
                nextPosition_Dic[i] = nextMovePosition;
            } else {
                nextPosition_Dic.Add(i, nextMovePosition);
            }

            //次の座標までの距離を設定
            if (nextPositionDistance_Dic.ContainsKey(i)) {
                nextPositionDistance_Dic[i] = distance;
            } else {
                nextPositionDistance_Dic.Add(i, distance);
            }

            //次の座標まで移動フラグの設定
            if (isMove_Dic.ContainsKey(buttonParent)) {
                isMove_Dic[buttonParent] = true;
            } else {
                isMove_Dic.Add(buttonParent, true);
            }

            //移動する前に格納オブジェクトへ移動            
            if (i == currentSelectButtonIndex) {
                buttonParent.transform.SetParent(viewMainParent.transform);
            } else {
                buttonParent.transform.SetParent(viewMaskParent.transform);
            }

        }
    }

    public void SlidingDoorCloseAnimation() {
        slidingDoor.SlidingDoorClose();
    }

    public void SlidingDoorClose() {
        var storyTitleImage = storyTitleChapterImage.GetComponent<Image>();
        storyTitleImage.sprite = storyTitleImageList[GameManager.Instance.storyNo - 1];

        storyTitle.SetActive(true);
        viewMainParent.SetActive(false);
        viewMaskParent.SetActive(false);
    }

    public void SlidingDoorOpen() {
        StartCoroutine(WaitStoryTitleAnimation());
    }
    private IEnumerator WaitStoryTitleAnimation() {
        yield return new WaitForSeconds(0.5f);
        storyTitleChapterImage.PlayViewAnimation();
    }

    public void StoryTitleViewEndAnimation() {
        isStroyTitleAnimation = true;
    }

    private void WaitTapInput() {
        //ストーリータイトルのアニメーション終了後
        //タップしたらストーリーへ
        if (Input.GetMouseButtonUp(0) && isTap) {
            isTap = false;
            storyTitleChapterImage.PlayFadeAnimation();
        }
    }

    public void StoryTitleFadeEndAnimation() {
        FadeManager.Instance.LoadLevel(SceneName.SceneNameManager.SCENE_NAME_STORY, GameManager.Instance.sceneChangeInterval);
    }
}
