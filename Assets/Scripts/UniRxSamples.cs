using System.Collections;
using System.Collections.Generic;

//UniRxのimport
using System; //Timer
using UniRx;
using UniRx.Triggers; //UpdateAsObservable
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UniRxSamples : MonoBehaviour {
    // Start is called before the first frame update
    private Subject<string> subject = new Subject<string> ();

    void Start () {
        //OnNextで呼び出し
        //subject.OnNext ("event");
        subject.Subscribe (str => {
                    //event
                },
                error => {
                    //error
                },
                () => {
                    //OnComplete
                })
            .AddTo (this); //scriptと同時破棄

    }

    void UpdateAlter () {
        //GameObjectに基づいたUpdate
        this.UpdateAsObservable ()
            //例；スペースが押されたとき
            .Where (_ => Input.GetKeyDown (KeyCode.Space))
            //例；駆動後、500msは無視
            .ThrottleFirst (TimeSpan.FromMilliseconds (500), Scheduler.MainThread)
            .Subscribe (_ => {
                //
            });
    }

    void BufferAndThrottleSample () {
        //Buffer & Throttle　二度押し検知
        //Buffer : Observableにメッセージを蓄える
        //Throttle : 最後の一つを流す
        var pushObservable = this.UpdateAsObservable ()
            .Where (_ => Input.GetKeyDown (KeyCode.Space));
        pushObservable.Buffer (pushObservable.Throttle (TimeSpan.FromMilliseconds (500)))
            .Where (x => x.Count == 2)
            .Subscribe (_ => {
                //do
            });
    }

    [SerializeField]
    Button button;
    void PushTwiceSample () {
        //にどおしボタン編
        button.OnClickAsObservable ()
            .Buffer (2)
            .Subscribe (_ => {
                //do
            });
    }

    void NoLinkUpdate () {
        //紐づけなしUpdate
        //停止=>手動orAddTo
        Observable.EveryUpdate ()
            .Subscribe (_ => {
                //
            })
            .AddTo (this);
    }

    ReactiveProperty<int> property;
    void ValueChecker () {
        //値監視、変動処理
        int number = 0;
        property = new ReactiveProperty<int> (number);
        property.Subscribe (renewNumber => {
            //numberの変動時Event
        });
    }

    void TimerEventDelay () {
        //TimerEvent ex.3s遅延
        Observable.Timer (TimeSpan.FromMilliseconds (3000))
            .Subscribe (delay => {
                //
            });
    }

    //シーン遷移での値受け渡し
    void SceneChange () {
        SceneManager.LoadSceneAsync ("[シーン名]").AsObservable ()
            .Subscribe (_ => {
                //scene先のControllerなど
                //var scene = FindObjectOfType<Controller> () as Controller;
                //ControllerのObjectに代入　など
            });
    }
}