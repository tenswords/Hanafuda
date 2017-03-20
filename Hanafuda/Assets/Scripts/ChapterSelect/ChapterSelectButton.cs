using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChapterSelectButton : MonoBehaviour {

    [SerializeField]
    private ChapterSelectManager chapterSelectManager;

    [SerializeField]
    private GameObject mirrorSurfaceObject;

    private int index;
    private string nextSceneName;
    private GameManager.STATE nextGameMangerState;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void RegistStatus(int index) {
        this.index = index;

        switch (index) {
            case 0:
                nextSceneName = SceneName.SceneNameManager.SCENE_NAME_TITLE;
                nextGameMangerState = GameManager.STATE.TITLE;
                break;

            default:
                nextSceneName = SceneName.SceneNameManager.SCENE_NAME_CHAPTER_SELECT;
                nextGameMangerState = GameManager.STATE.CHAPTER_SELECT;
                break;
        }
    }

    public void OnSelectButton() {

        if (FlickDicisionManager.Instance.state == FlickDicisionManager.STATE.WAIT_INPUT) {
            FlickDicisionManager.Instance.SetState(FlickDicisionManager.STATE.BUTTON_INPUT);

            if (nextSceneName == SceneName.SceneNameManager.SCENE_NAME_TITLE) {
                //タイトルを選んだ場合
                AudioManager.Instance.StopBGM(0.5f);

                mirrorSurfaceObject.SetActive(false);
                StartCoroutine(OnTitleButton(1.0f));

            } else {
                //ストーリーの各章を選んだ場合
                GameManager.Instance.storyNo = index;
                chapterSelectManager.SlidingDoorCloseAnimation();
            }
        }
    }

    private IEnumerator OnTitleButton(float interval) {
        var time = 0.0f;
        var lerp = 0.0f;
        var rect = transform.GetComponent<RectTransform>();

        while (time < interval) {
            time += Time.deltaTime;
            lerp = Mathf.Lerp(0f, 1f, time / interval);
            rect.sizeDelta = Vector2.Lerp(rect.sizeDelta, chapterSelectManager.TITLE_BUTTON_SCALE_SIZE, lerp);

            yield return 0;
        }

        GameManager.Instance.state = nextGameMangerState;
        FadeManager.Instance.LoadLevel(nextSceneName, GameManager.Instance.sceneChangeInterval);
    }
}
