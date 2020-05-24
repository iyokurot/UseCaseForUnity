using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;


public class InputController : MonoBehaviour
{

	public enum INPUT_STATE {
		NONE = 0,
		TAP,
		RELEASE,
		HOLD,
		DRAG
	}

	public enum DRAG_STATE {
		NONE = 0,
		UP,
		DOWN,
		RIGHT,
		LEFT,
	}

		
	private Vector3 tapStartPos;
	private Vector3 tapEndPos;
	private INPUT_STATE inputState;
	private DRAG_STATE dragState;

	public List<GameObject> inputEventListener = new List<GameObject>();

	private GameObject stockTapObject;
	private bool disableDrag = false;


	public interface IInputEvent : IEventSystemHandler {
		void OnDrag 	(object dragObj, DRAG_STATE dragState);
		void OnTap 		(object tapObj);
		void OnRelease	(object releaseObj);
	}
		
	// Use this for initialization
	void Start () {
		inputState = INPUT_STATE.NONE;
	}
	
	// Update is called once per frame
	void Update () {
		InputCheck ();

		switch (inputState) {
		case INPUT_STATE.NONE:
			break;
		case INPUT_STATE.TAP:
			TapSendMessage ();
			break;

		case INPUT_STATE.RELEASE:
			ReleaseSendMessage ();
			break;

		case INPUT_STATE.DRAG:
			if(!disableDrag)
				DragSendMessage ();
			break;
		}

//		Debug.LogError (inputState);
		
	}

	/*
	* イベントリスナーセット関数
	*/
	public void SetInputEventListener(GameObject listenObj){
		inputEventListener.Add (listenObj);
	}




	/*
	* 入力のチェック関数
	*/
	void InputCheck(){

		//! ダウンしたのかそうでないのか
		if (Input.GetMouseButtonDown(0)){
			tapStartPos = new Vector3(Input.mousePosition.x,
				Input.mousePosition.y,
				Input.mousePosition.z);
			StockTapObject ();
			inputState = INPUT_STATE.TAP;
			return;
			
		}

		else if (Input.GetMouseButton (0)) {
			tapEndPos = new Vector3(Input.mousePosition.x,
				Input.mousePosition.y,
				Input.mousePosition.z);

			//! 方向チェック
			GetDirection ();
			return;
		}
			


		//! リリースした際の処理
		if (Input.GetMouseButtonUp(0)){
			tapEndPos = new Vector3(Input.mousePosition.x,
				Input.mousePosition.y,
				Input.mousePosition.z);

			inputState = INPUT_STATE.RELEASE;
			disableDrag = false;
			return;
		}

		//! 保持するゲームオブジェクトを破棄
		stockTapObject = null;
		inputState = INPUT_STATE.NONE;
	}


	/*
	* ドラッグ方向のチェック
	*/
	void GetDirection(){


		float dX = tapEndPos.x - tapStartPos.x;
		float dY = tapEndPos.y - tapStartPos.y;

		//! 左右チェック
		if (Mathf.Abs (dY) < Mathf.Abs (dX)) {
			float absdX = Mathf.Abs (dX);
			if (absdX > 15) {
				inputState = INPUT_STATE.DRAG;
				dragState = (Mathf.Sign (dX) > 0) ? DRAG_STATE.RIGHT : DRAG_STATE.LEFT;
			}
		} 

		//! 上下チェック
		else if (Mathf.Abs (dX) < Mathf.Abs (dY)) {
			float absdY = Mathf.Abs (dY);
			if (absdY > 15) {
				inputState = INPUT_STATE.DRAG;
				dragState = (Mathf.Sign (dY) > 0) ? DRAG_STATE.UP : DRAG_STATE.DOWN;
			}
		}

		//! そうで無い場合はホールド
		else {
			inputState = INPUT_STATE.HOLD;
		}
	}

	/*
	* タップだった際にコールバックされる関数
	*/
	void TapSendMessage(){
		tapStartPos = Input.mousePosition;
		Ray ray = Camera.main.ScreenPointToRay(tapStartPos);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit)){
			GameObject obj = hit.collider.gameObject;

			//! イベントリスナーに設定されていたオブジェクトたちに通知
			for (int i = 0; i < inputEventListener.Count; i++) {
				ExecuteEvents.Execute<IInputEvent> (
					target: inputEventListener [i],
					eventData: null,
					functor: (reciever, eventData) => {
						reciever.OnTap (obj as object);
					});
			}
		}
	}

	/*
	* ドラッグだった際にコールバックされる関数
	*/
	void DragSendMessage(){
		//! イベントリスナーに設定されていたオブジェクトたちに通知
		for (int i = 0; i < inputEventListener.Count; i++) {
			ExecuteEvents.Execute<IInputEvent> (
				target: inputEventListener [i],
				eventData: null,
				functor: (reciever, eventData) => {
					reciever.OnDrag (stockTapObject as object, dragState);
				});
		}

		//! ドラッグは一回だけでいい
		stockTapObject = null;
		disableDrag = true;
	}

	/*
	* リリースだった際にコールバックされる関数
	*/
	void ReleaseSendMessage(){
		//! イベントリスナーに設定されていたオブジェクトたちに通知
		for (int i = 0; i < inputEventListener.Count; i++) {
			ExecuteEvents.Execute<IInputEvent> (
				target: inputEventListener [i],
				eventData: null,
				functor: (reciever, eventData) => {
					reciever.OnRelease (stockTapObject as object);
				});
		}
		stockTapObject = null;
	}




	void StockTapObject(){
		tapStartPos = Input.mousePosition;

		Ray ray = Camera.main.ScreenPointToRay(tapStartPos);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit)){
			stockTapObject = hit.collider.gameObject;
		}
	}
}