using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextFieldImage : MonoBehaviour {

    [SerializeField]
    private StoryManager storyManager;

    [SerializeField]
    private GameObject nextButton;

    private bool isWaitTap;

    private bool isPress;
    private bool isLongPress;
    private float pressTime;
    [SerializeField]
    private float LONG_PRESS_TIME;

    [SerializeField]
    private GameObject textField_NextButton;

    [SerializeField]
    private Image skippingButton;
    private bool isSkipFlush;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (isPress) {
            pressTime += Time.deltaTime;
            if (pressTime > LONG_PRESS_TIME) {
                isLongPress = true;
                if(textField_NextButton.gameObject.activeInHierarchy) textField_NextButton.gameObject.SetActive(false);

                //タップ待ちの状態で長押しした場合、
                if (storyManager.state == StoryManager.STATE.WAIT_NEXT) {
                    if (nextButton.activeInHierarchy) nextButton.SetActive(false);

                    storyManager.state = StoryManager.STATE.READTEXT;
                    storyManager.CommandNewLine();
                }
            }
        }else {
            isLongPress = false;
            if (skippingButton.gameObject.activeInHierarchy) skippingButton.gameObject.SetActive(false);
        }

        if (isLongPress && !isSkipFlush) StartCoroutine(SkipImageFlush());
	}

    private IEnumerator SkipImageFlush() {
        isSkipFlush = true;

        var interval = 0.75f;
        skippingButton.gameObject.SetActive(true);
        yield return new WaitForSeconds(interval);

        skippingButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(interval);

        isSkipFlush = false;
    }


    public void OnPointerDown() {
        isPress = true;
    }

    public void OnPointerUp() {

        pressTime = 0.0f;
        isPress = false;

        if (storyManager.isWaitStoryStarted || storyManager.isLineRead || storyManager.isFading || isWaitTap) return;

        StartCoroutine(WaitTap(0.25f));

        switch (storyManager.state) {
            case StoryManager.STATE.READTEXT:
                if(!isLongPress) storyManager.LineTextShowAll();
                break;

            case StoryManager.STATE.WAIT_NEXT:
                nextButton.SetActive(false);
                storyManager.state = StoryManager.STATE.READTEXT;
                AudioManager.Instance.PlaySE(AudioName.AudioNameManager.SE_SE_NEXTBUTTON);
                AudioManager.Instance.PlaySE(AudioName.AudioNameManager.SE_SE_NEXTBUTTON);
                storyManager.CommandNewLine();
                break;
        }
        isLongPress = false;

        if(skippingButton.gameObject.activeInHierarchy) skippingButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// 連続でタップされないように制御
    /// </summary>
    private IEnumerator WaitTap(float waitTime) {
        isWaitTap = true;
        yield return new WaitForSeconds(waitTime);
        isWaitTap = false;
    }

    public bool GetIsLongPress() {
        return isLongPress;
    }
}
