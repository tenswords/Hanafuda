using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StoryTitle : MonoBehaviour {

    [SerializeField]
    private ChapterSelectManager chapterSelectManager;

    [SerializeField]
    private Vector3 VIEW_TARGET_SCALE_SIZE;
    [SerializeField]
    private Vector3 FADE_TARGET_SCALE_SIZE;

    private Animator animator;

    // Use this for initialization
    void Start() {
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update() {

    }

    public void PlayViewAnimation() {
        StartCoroutine(ViewFade(0.3f,"View"));
    }

    private IEnumerator ViewFade(float interval,string callName) {
        var time = 0.0f;
        var lerp = 0.0f;
        var onceSceneChange = true;

        var image = transform.GetComponent<Image>();

        while (time < interval) {
            time += Time.deltaTime;
            lerp = Mathf.Lerp(0f, 1f, time / interval);

            if(callName == "Fade") {
                if (onceSceneChange && lerp > 0.5f) {
                    onceSceneChange = false;
                    GameManager.Instance.state = GameManager.STATE.STORY;
                    FadeManager.Instance.LoadLevel(SceneName.SceneNameManager.SCENE_NAME_STORY, GameManager.Instance.sceneChangeInterval);
                }
                image.color = Color.Lerp(image.color, Color.clear, lerp);

            } else if(callName == "View"){
                image.color = Color.Lerp(image.color, Color.white, lerp);
            }

            transform.localScale = Vector3.Lerp(transform.localScale, VIEW_TARGET_SCALE_SIZE, lerp);
            yield return 0;
        }
        if (callName == "View") {
            chapterSelectManager.StoryTitleViewEndAnimation();
            VIEW_TARGET_SCALE_SIZE = FADE_TARGET_SCALE_SIZE;
        }
    }

    public void PlayFadeAnimation() {
        StartCoroutine(ViewFade(0.5f, "Fade"));
    }
}
