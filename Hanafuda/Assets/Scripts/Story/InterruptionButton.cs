using UnityEngine;
using System.Collections;

public class InterruptionButton : MonoBehaviour {

    [SerializeField]
    private StoryManager storyManager;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnInterruptionButton() {
        if (storyManager.isWaitStoryStarted || storyManager.isLineRead || storyManager.isFading) return;
        storyManager.Interruption();
    }
}
