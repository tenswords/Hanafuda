using UnityEngine;
using System.Collections;

public class CardEffect : MonoBehaviour {

    private Animation anim;

    void OnEnable() {
        if(anim == null)anim = GetComponent<Animation>();
        anim.Play();
    }
}
