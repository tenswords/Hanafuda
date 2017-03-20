using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TitleSelectButton : MonoBehaviour {

    [SerializeField]
    private TitleManager titleManager;

    //[SerializeField]
    //private InterruptionDialogManager interruptionDialog;

    private string nextSceneName;
    //private GameManager.STATE nextGameMangerState;

    // Use this for initialization
    void Start() {
    }
    // Update is called once per frame
    void Update() {
    }

    public void RegistStatus(int index) {
        switch (index) {
            case 0:
                nextSceneName = SceneName.SceneNameManager.SCENE_NAME_TEST;
                //nextGameMangerState = GameManager.STATE.CHAPTER_SELECT;
                break;
            case 1:
                nextSceneName = SceneName.SceneNameManager.SCENE_NAME_TEST;
                //nextGameMangerState = GameManager.STATE.CHAPTER_SELECT;
                break;

            case 2:
                nextSceneName = SceneName.SceneNameManager.SCENE_NAME_CHAPTER_SELECT;
                //nextGameMangerState = GameManager.STATE.CHAPTER_SELECT;
                break;

            case 3:
                nextSceneName = SceneName.SceneNameManager.SCENE_NAME_TEST;
                //nextGameMangerState = GameManager.STATE.CHAPTER_SELECT;
                break;

            case 4:
                nextSceneName = SceneName.SceneNameManager.SCENE_NAME_TEST;
                //nextGameMangerState = GameManager.STATE.CHAPTER_SELECT;
                break;

        }
    }

    public void OnSelectButton() {
        if (FlickDicisionManager.Instance.state == FlickDicisionManager.STATE.WAIT_INPUT) {
            FlickDicisionManager.Instance.SetState(FlickDicisionManager.STATE.BUTTON_INPUT);

            AudioManager.Instance.PlaySE(AudioName.AudioNameManager.SE_SE_BUTTON);

            StartCoroutine(ButtonScaling(0.5f));

            ////チャプター選択の場合
            //if (nextSceneName == SceneName.SceneNameManager.SCENE_NAME_CHAPTER_SELECT) {

            //    //セーブデータがある場合、保存されたデータを読み込む
            //    if (SaveLoadManager.Instance.CheckHasSaveData()) {
            //        InterruptionDialogManager.Instance.SwitchStatus(InterruptionDialogManager.STATE.Resumption);
            //        Debug.Log("ダイアログ表示");

            //    } else {
            //        StartCoroutine(ButtonScaling(0.5f));
            //    }
            //}
        }
    }

    private IEnumerator ButtonScaling(float interval) {
        var time = 0.0f;
        var lerp = 0.0f;
        var onceSceneChange = true;

        var image = transform.GetComponent<Image>();

        AudioManager.Instance.StopBGM(0.5f);

        while (time < interval) {
            time += Time.deltaTime;
            lerp = Mathf.Lerp(0f, 1f, time / interval);

            if (onceSceneChange && lerp > 0.5f) {
                onceSceneChange = false;
                //GameManager.Instance.state = nextGameMangerState;
                //FadeManager.Instance.LoadLevel(nextSceneName, GameManager.Instance.sceneChangeInterval);

                titleManager.SetTextImageIsActive(gameObject);
                //チャプター選択の場合
                if (nextSceneName == SceneName.SceneNameManager.SCENE_NAME_CHAPTER_SELECT) {

                    //セーブデータがある場合、保存されたデータを読み込む
                    if (SaveLoadManager.Instance.CheckHasSaveData()) {

                        InterruptionDialogManager.Instance.SwitchStatus(InterruptionDialogManager.STATE.RESUMPTION);

                    }else {

                        FadeManager.Instance.LoadLevel(nextSceneName, GameManager.Instance.sceneChangeInterval);
                    }

                }else {
                    FadeManager.Instance.LoadLevel(nextSceneName, GameManager.Instance.sceneChangeInterval);

                }
            }
            image.color = Color.Lerp(image.color, Color.clear, lerp);
            transform.localScale = Vector3.Lerp(transform.localScale, titleManager.ON_BUTTON_SCALE_SIZE, lerp);
            yield return 0;
        }
    }
}
