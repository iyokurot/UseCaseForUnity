using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;

public static class BuildClass
{
    [MenuItem("Build/Android")]
    public static void Build()
    {
        Debug.Log ("AndroidBuild...");

        // ビルド対象シーンリスト
        string[] sceneList = {
            "./Assets/Scenes/TitleScene.unity",
            "./Assets/Scenes/SelectMode.unity",
            "./Assets/Scenes/GameScene.unity",
            "./Assets/Scenes/ResultScene.unity",
        };


        // 実行
        // BuildReport errorMessage = BuildPipeline.BuildPlayer(
        //         sceneList,                          //!< ビルド対象シーンリスト
        //         "C:/test_reverse.apk",   //!< 出力先
        //         BuildTarget.Android,      //!< ビルド対象プラットフォーム
        //         BuildOptions.None          //!< ビルドオプション
        // );
         BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = sceneList;
        buildPlayerOptions.locationPathName =  Application.dataPath + "/../../test_reverse_second.apk";
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
         BuildSummary summary =  report.summary;


        // 結果出力
        // if( !string.IsNullOrEmpty( errorMessage ) )
        //     Debug.LogError( "[Error!] " + errorMessage );
        // else
        //     Debug.Log( "[Success!]" );
         if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            Debug.Log(buildPlayerOptions.locationPathName);
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log(summary);
        }
    }
}