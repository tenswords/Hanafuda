using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FlickDicisionManager : SingletonMonoBehaviour<FlickDicisionManager> {

    public STATE state {
        get;
        private set;
    }

    public enum STATE {
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

    public FLICK_DIRECTION flickkDirection {
        get;
        private set;
    }
    public enum FLICK_DIRECTION {
        NONE,
        LEFT,
        RIGHT
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (SceneManager.GetActiveScene().name == SceneName.SceneNameManager.SCENE_NAME_TITLE ||
            SceneManager.GetActiveScene().name == SceneName.SceneNameManager.SCENE_NAME_CHAPTER_SELECT) {

            if (state == STATE.WAIT_INPUT) WaitInput();
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
                if ((Mathf.Abs(pointUpPosition.x - pointDownPosition.x)) >= (Mathf.Abs(pointUpPosition.y - pointDownPosition.y))) {
                    state = STATE.FLICK_ANIMATION;

                    if ((pointUpPosition.x - pointDownPosition.x) < 0) flickkDirection = FLICK_DIRECTION.LEFT;
                    else if ((pointUpPosition.x - pointDownPosition.x) > 0) flickkDirection = FLICK_DIRECTION.RIGHT;

                    Debug.Log("フリック判定 " + flickkDirection);
                }
            }
        }
    }

    /// <summary>
    /// ステータスを設定
    /// </summary>
    /// <param name="state"></param>
    public void SetState(STATE state) {
        this.state = state;
    }

    /// <summary>
    /// フリックの方向ステータスを設定
    /// </summary>
    /// <param name="state"></param>
    public void SetFlickkDirection(FLICK_DIRECTION flickkDirection) {
        this.flickkDirection = flickkDirection;
    }
}
