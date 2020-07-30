using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonTestController : MonoBehaviour
{
    [SerializeField]
    private GameObject parentObject;
    [SerializeField]
    private int stage_id = 0;
    
    private List<JsonObject> jsonList = new List<JsonObject>();
    public void CreateJsonData()
    {
        
        // 子オブジェクトを全て取得する
        foreach (Transform childTransform in parentObject.transform)
        {
            // Baseくらすから値を取得
            Base baseObj=childTransform.gameObject.GetComponent<Base>();
            //Base baseObj = new Base();
            JsonObject json = new JsonObject(
                baseObj.gimmick_id,
                baseObj.gimmick_type_id,
                stage_id,
                baseObj.pos_x,
                baseObj.pos_y,
                baseObj.start_pos_x,
                baseObj.start_pos_y,
                baseObj.end_pos_x,
                baseObj.end_pos_y,
                baseObj.time,
                baseObj.delay,
                baseObj.rotation,
                baseObj.beat_score);
                jsonList.Add(json);
        }
        BaseJson basJ = new BaseJson();
        basJ.stage_gimmicks = jsonList.ToArray();
        string postJson = JsonUtility.ToJson(basJ);
        Debug.Log(postJson);
    }
}

[System.Serializable]
public class JsonObject{
    public int gimmick_id =0;
    public int gimmick_type_id = 0;
    public int stage_id = 0;
    public float pos_x = 0;
    public float pos_y = 0;

    public float start_pos_x = 0;
    public float start_pos_y = 0;
    public float end_pos_x = 0;
    public float end_pos_y = 0;

    public float time = 0;
    public float delay = 0;

    public float rotation =0;
    public int beat_score = 0;
    public JsonObject(
        int gimmick_id,
        int gimmick_type_id,
        int stage_id,
        float pos_x,
        float pos_y,
        float start_pos_x,
        float start_pos_y,
        float end_pos_x,
        float end_pos_y,
        float time,
        float delay,
        float rotation,
        int beat_score
        ){
            this.gimmick_id = gimmick_id;
            this.gimmick_type_id = gimmick_type_id;
            this.stage_id = stage_id;
        this.pos_x = pos_x;
        this.pos_y = pos_y;
        this.start_pos_x = start_pos_x;
        this.start_pos_y = start_pos_y;
        this.end_pos_x = end_pos_x;
        this.end_pos_y = end_pos_y;
        this.time = time;
        this.delay = delay;
        this.rotation = rotation;
        this.beat_score = beat_score;
    }
}

[System.Serializable]
public class BaseJson{
    public JsonObject[] stage_gimmicks;
}
