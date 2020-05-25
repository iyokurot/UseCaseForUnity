using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DOTweenSamples : MonoBehaviour {
    public enum Game_State {
        BEFORE,
        GAME,
        OVER,
        CLEAR,
    }
    public Game_State State { get; private set; }
    private float countdown = 0;
    // Start is called before the first frame update
    void Start () {
        //カウントダウンTween
        Sequence startSequence = DOTween.Sequence ();
        startSequence.Append (DOTween.To (
                () => countdown,
                count => countdown = count,
                3.0f,
                3.0f
            ).OnUpdate (() => {
                //countDownText.text = (3 - (int) countdown).ToString ();
                Debug.Log (3 - (int) countdown);
            })
            .OnComplete (
                () => {
                    State = Game_State.GAME;
                    startSequence.Kill ();
                }));

    }

    // Update is called once per frame
    // void Update () {

    // }
}