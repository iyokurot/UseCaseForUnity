using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Move : MonoBehaviour {
    public float speed = 6.0F; //歩行速度
    public float jumpSpeed = 8.0F; //ジャンプ力
    public float gravity = 20.0F; //重力の大きさ

    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    private float h, v;

    [SerializeField]
    Button cursolButton;

    Vector3 startPos = Vector3.zero;
    float dx = 0;

    // Use this for initialization
    void Start () {
        controller = GetComponent<CharacterController> ();
        cursolButton.onClick.AddListener (OnClickCursol);

        EventTrigger trigger = cursolButton.GetComponent<EventTrigger> ();
        EventTrigger.Entry entry = new EventTrigger.Entry ();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener ((eventDate) => { OnClickCursol (); });
        trigger.triggers.Add (entry);

        EventTrigger.Entry entrydown = new EventTrigger.Entry ();
        entrydown.eventID = EventTriggerType.Drag;
        entrydown.callback.AddListener ((eventDate) => { OnDragCursol (); });
        trigger.triggers.Add (entrydown);

        EventTrigger.Entry entryUp = new EventTrigger.Entry ();
        entryUp.eventID = EventTriggerType.PointerUp;
        entryUp.callback.AddListener ((eventDate) => { OnCursolUp (); });
        trigger.triggers.Add (entryUp);
    } //Start()

    // Update is called once per frame
    void Update () {

        h = Input.GetAxis ("Horizontal"); //左右矢印キーの値(-1.0~1.0)
        v = Input.GetAxis ("Vertical"); //上下矢印キーの値(-1.0~1.0)

        if (controller.isGrounded) {
            // moveDirection = new Vector3 (dx, 0, 0);
            // moveDirection = transform.TransformDirection (moveDirection);
            // moveDirection *= speed;
            if (Input.GetButton ("Jump"))
                moveDirection.y = jumpSpeed;
        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move (moveDirection * Time.deltaTime);

    } //Update()

    private void OnClickCursol () {
        //
        startPos = new Vector3 (Input.mousePosition.x,
            Input.mousePosition.y,
            Input.mousePosition.z);
        Debug.Log (startPos);
    }

    private void OnDragCursol () {
        Vector3 touchStartPos = new Vector3 (Input.mousePosition.x,
            Input.mousePosition.y,
            Input.mousePosition.z);
        float x = startPos.x - touchStartPos.x;
        float y = startPos.y - touchStartPos.y;
        //Debug.Log ("x:" + x + "  y:" + y);
        MoveSide (touchStartPos);
    }

    private void MoveSide (Vector3 pos) {
        float x = startPos.x - pos.x;
        if (x > 50.0f) {
            x = 50.0f;
        } else if (x < -50.0f) {
            x = -50.0f;
        }
        x = x / 50.0f;
        dx = x;
        moveDirection = new Vector3 (x, 0, 0);
        moveDirection = transform.TransformDirection (moveDirection);
        moveDirection *= speed;
        //controller.Move (moveDirection * Time.deltaTime);
    }

    private void OnCursolUp () {
        moveDirection = Vector3.zero;
        // dx = 0;
    }
}