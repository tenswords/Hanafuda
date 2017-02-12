using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextFieldImage : MonoBehaviour {

    [SerializeField]
    private StoryManager storyManager;

    [SerializeField]
    private GameObject nextButton;

    private bool isWaitTap;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnPointerUp() {

        if(storyManager.isWaitStoryStarted || storyManager.isLineRead || storyManager.isFading || isWaitTap) return;

        StartCoroutine(WaitTap(0.25f));

        switch (storyManager.state) {
            case StoryManager.STATE.READTEXT:
                storyManager.LineTextShowAll();
                break;

            case StoryManager.STATE.WAIT_NEXT:
                nextButton.SetActive(false);
                storyManager.state = StoryManager.STATE.READTEXT;
                storyManager.CommandNewLine();
                break;
        }

    }

    /// <summary>
    /// 連続でタップされないように制御
    /// </summary>
    private IEnumerator WaitTap(float waitTime) {
        isWaitTap = true;
        yield return new WaitForSeconds(waitTime);
        isWaitTap = false;
    }
}
