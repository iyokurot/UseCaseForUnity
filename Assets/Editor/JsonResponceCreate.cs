using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// JSONパース用クラスの自動生成エディター拡張
/// </summary>
public class JsonResponceCreate : EditorWindow {
    /// <summary>
    /// 作成クラス名
    /// </summary>
    private string className = default;

    /// <summary>
    /// メンバ変数のリスト
    /// </summary>
    /// <typeparam name="ObjectClass"></typeparam>
    /// <returns></returns>
    [SerializeField]
    private List<ObjectClass> objectList = new List<ObjectClass> ();

    /// <summary>
    /// １メンバ変数のパラメータ
    /// </summary>
    [System.Serializable]
    public class ObjectClass {
        /// <summary>
        /// 説明文
        /// </summary>
        public string summary;

        /// <summary>
        /// 型
        /// </summary>
        public string type;

        /// <summary>
        /// 変数名
        /// </summary>
        public string name;
    }

    /// <summary>
    /// 保存先のパスを変更するか
    /// </summary>
    private bool isChangePath = false;

    /// <summary>
    /// 変更したパス
    /// </summary>
    private string originalPath = "Assets/Scripts";

    [MenuItem ("JSON/ResponceScriptCreator")]
    public static void ShowWindow () {
        EditorWindow.GetWindow (typeof (JsonResponceCreate));
    }

    /// <summary>
    /// GUIの生成
    /// </summary>
    void OnGUI () {
        var serializedObject = new SerializedObject (this);
        serializedObject.Update ();

        GUILayout.Label ("Json受け取りクラスの作成", EditorStyles.boldLabel);
        className = EditorGUILayout.TextField ("クラス名", className);
        EditorGUILayout.HelpBox ("先頭は必ず大文字に変換されます", MessageType.Info);

        GUILayout.Label ("メンバ変数", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox ("summaryに変数の説明を明記\nTypeで型を指定\nNameで変数名を明記", MessageType.Info);
        EditorGUILayout.HelpBox ("変数名の先頭は必ず小文字に変換され、先頭大文字のプロパティが生成されます", MessageType.Info);
        EditorGUILayout.PropertyField (serializedObject.FindProperty ("objectList"), true);
        serializedObject.ApplyModifiedProperties ();

        GUILayout.Label ("Assets/Scripts直下にcs作成", EditorStyles.boldLabel);
        isChangePath = GUILayout.Toggle (isChangePath, "生成先のパスを変更");
        if (isChangePath) {
            GUILayout.Label ("project直下以降のパス", EditorStyles.boldLabel);
            originalPath = EditorGUILayout.TextField ("生成先のパス", originalPath);
        }
        if (GUILayout.Button ("Class Create")) {
            OnClickCreate ();
        }
    }

    /// <summary>
    /// クラスファイル生成ボタン押下時の関数
    /// </summary>
    private void OnClickCreate () {
        // usingの記述
        string classStr = "using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\n\n";
        classStr += "[System.Serializable]\n";

        // 空欄の除外
        string rewriteClassName = className.Replace (" ", "");
        // クラス名のnullチェック
        if (rewriteClassName == "") {
            Debug.LogError ("クラス名が空です");
            return;
        }
        rewriteClassName = rewriteClassName[0].ToString ().ToUpper () + rewriteClassName.Substring (1);
        classStr += "public class " +
            rewriteClassName +
            " {\n";
        List<string> nameList = new List<string> ();
        List<string> propertyList = new List<string> ();

        // メンバ変数の作成
        foreach (var obj in objectList) {
            // summaryの記述
            classStr += $"/// <summary>\n/// {obj.summary}\n/// </summary>\n";
            classStr += "[SerializeField, HideInInspector]\n";

            // 型のnullチェック
            if (obj.type == "") {
                Debug.LogError ("メンバ変数型が空です");
                return;
            }

            // 名前のnullチェック
            if (obj.name == "") {
                Debug.LogError ("メンバ変数名が空です");
                return;
            }
            // 空欄を削除したメンバ変数名
            string rewriteMemberName = obj.name.Replace (" ", "");

            // 先頭を必ず小文字にする
            string lowerMemberName = rewriteMemberName[0].ToString ().ToLower () + rewriteMemberName.Substring (1);

            // _を取り除き次の文字を大文字にする
            string[] memberNameArray = lowerMemberName.Split ('_');

            // 先頭を必ず大文字にする
            string upperMemberName = "";
            for (int i = 0; i < memberNameArray.Length; i++) {
                string str = memberNameArray[i];
                if (str != "") {
                    str = str[0].ToString ().ToUpper () + str.Substring (1);
                    upperMemberName += str;
                }
            }

            // private変数
            classStr += "private " + obj.type + " " + lowerMemberName + " = default;\n";

            // 対応するgetプロパティ
            classStr += $"public {obj.type} {upperMemberName} => {lowerMemberName};\n\n";
            nameList.Add (lowerMemberName);
            propertyList.Add (upperMemberName);
        }
        if (!CheckParams (nameList)) {
            Debug.LogError ("メンバ変数名が被っています");
            return;
        }
        if (!CheckParams (propertyList)) {
            Debug.LogError ("プロパティ名が被っています");
            return;
        }
        classStr += "}";
        Debug.Log (classStr);

        // 同名ファイルの存在確認
        string path = "";

        // 保存先のパスを変更するか
        if (isChangePath) {
            path = $"./{originalPath}/{rewriteClassName}.cs";
        } else {
            path = $"./Assets/Scripts/{rewriteClassName}.cs";
        }

        // 同名ファイルが存在するか
        if (File.Exists (path)) {
            Debug.LogError ("同名クラスファイルが存在します");
            return;
        }

        // クラスファイルの作成
        try {
            using (FileStream fs = File.Create (path));
        } catch (Exception e) {
            Debug.LogError ("ファイルを作成できませんでした");
            return;
        }

        Encoding enc = Encoding.GetEncoding ("UTF-8");
        // ファイルの書き込み
        try {
            using (StreamWriter writer = new StreamWriter (path, false, enc)) {
                writer.WriteLine (classStr);
            }
        } catch (Exception e) {
            Debug.LogError ("ファイルに書き込みが失敗しました");
            return;
        }
        Debug.Log ("create class succeed!");
    }

    /// <summary>
    /// メンバ変数名のかぶりチェック
    /// </summary>
    /// <returns></returns>
    private bool CheckParams (List<string> nameList) {
        for (int i = 0; i < nameList.Count; i++) {
            string name = nameList[i];
            for (int j = i + 1; j < nameList.Count; j++) {
                if (name == nameList[j]) {
                    return false;
                }
            }
        }
        return true;
    }
}