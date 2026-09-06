using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public struct MapGridWithID
{
    public int id;
    public MapGridInfo info;
}

[CreateAssetMenu(fileName = "Map Data", menuName = "Create Map /MapGrid Data List")]
public class MapGridInfoList : ScriptableObject
{
    public List<MapGridWithID> mapGridInfos;
    public MapGridInfo startGridInfo;
    public MapGridInfo bossGridInfo;

    /// <summary>
    /// 指定したidを持つMapGridInfoを返す。存在しない場合はnull。
    /// </summary>
    public MapGridInfo GetMapGridInfoById(int id)
    {
        foreach (var mapGridWithID in mapGridInfos)
        {
            if (mapGridWithID.id == id)
            {
                return mapGridWithID.info;
            }
        }
        return null;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MapGridInfoList))]
    public class MpaGridInfoListEditor : Editor
    {
        private SerializedProperty mapGridInfosProp;
        private SerializedProperty startGridInfoProp;
        private SerializedProperty bossGridInfoProp;

        private void OnEnable()
        {
            mapGridInfosProp = serializedObject.FindProperty("mapGridInfos");
            startGridInfoProp = serializedObject.FindProperty("startGridInfo");
            bossGridInfoProp = serializedObject.FindProperty("bossGridInfo");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            /// スタートマス情報
            EditorGUILayout.LabelField("スタートマス情報", EditorStyles.boldLabel);
            startGridInfoProp.objectReferenceValue = EditorGUILayout.ObjectField(
                startGridInfoProp.objectReferenceValue,
                typeof(MapGridInfo),
                false,
                GUILayout.ExpandWidth(true)
            );
            
            /// ボスマス情報
            EditorGUILayout.LabelField("ボスマス情報", EditorStyles.boldLabel);
            bossGridInfoProp.objectReferenceValue = EditorGUILayout.ObjectField(
                bossGridInfoProp.objectReferenceValue,
                typeof(MapGridInfo),
                false,
                GUILayout.ExpandWidth(true)
            );

            /// マップグリッド一覧
            EditorGUILayout.LabelField("マップグリッド一覧", EditorStyles.boldLabel);
            // ヘッダー
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("ID", GUILayout.Width(40));
            GUILayout.Label("Map Grid Info", GUILayout.ExpandWidth(true));
            GUILayout.Label("", GUILayout.Width(60)); // for 上に追加
            GUILayout.Label("", GUILayout.Width(60)); // for 下に追加
            GUILayout.Label("", GUILayout.Width(40)); // for 削除
            EditorGUILayout.EndHorizontal();

            int insertIndexAbove = -1;
            int insertIndexBelow = -1;
            int deleteIndex = -1;

            for (int i = 0; i < mapGridInfosProp.arraySize; i++)
            {
                SerializedProperty mapGridInfoWithIDProp = mapGridInfosProp.GetArrayElementAtIndex(i);
                SerializedProperty infoIdProp = mapGridInfoWithIDProp.FindPropertyRelative("id");
                SerializedProperty mapGridInfoProp = mapGridInfoWithIDProp.FindPropertyRelative("info");

                EditorGUILayout.BeginHorizontal();

                // IDをラベルで表示(変更不可)
                EditorGUILayout.LabelField(infoIdProp.intValue.ToString(), GUILayout.Width(40));

                mapGridInfoProp.objectReferenceValue = EditorGUILayout.ObjectField(
                    mapGridInfoProp.objectReferenceValue,
                    typeof(MapGridInfo),
                    false,
                    GUILayout.ExpandWidth(true)
                );

                if (GUILayout.Button("上に追加", GUILayout.Width(60)))
                {
                    insertIndexAbove = i;
                }
                if (GUILayout.Button("下に追加", GUILayout.Width(60)))
                {
                    insertIndexBelow = i;
                }
                if (GUILayout.Button("削除", GUILayout.Width(40)))
                {
                    deleteIndex = i;
                }
                EditorGUILayout.EndHorizontal();
            }

            // 末尾に追加ボタン
            if (GUILayout.Button("Add Map Grid Info At End"))
            {
                // Insert new element at the end of the array
                mapGridInfosProp.InsertArrayElementAtIndex(mapGridInfosProp.arraySize);
                // 新しい要素の値を初期化
                if (mapGridInfosProp.arraySize > 0)
                {
                    SerializedProperty newElement = mapGridInfosProp.GetArrayElementAtIndex(mapGridInfosProp.arraySize - 1);
                    newElement.FindPropertyRelative("info").objectReferenceValue = null;
                    newElement.FindPropertyRelative("id").intValue = mapGridInfosProp.arraySize - 1;
                }
            }

            // 行削除処理
            if (deleteIndex != -1)
            {
                mapGridInfosProp.DeleteArrayElementAtIndex(deleteIndex);
                // IDを振り直し
                for (int j = 0; j < mapGridInfosProp.arraySize; j++)
                {
                    SerializedProperty effectWithID = mapGridInfosProp.GetArrayElementAtIndex(j);
                    effectWithID.FindPropertyRelative("id").intValue = j;
                }
            }

            // 上に挿入処理
            if (insertIndexAbove != -1)
            {
                mapGridInfosProp.InsertArrayElementAtIndex(insertIndexAbove);
                SerializedProperty newElement = mapGridInfosProp.GetArrayElementAtIndex(insertIndexAbove);
                newElement.FindPropertyRelative("info").objectReferenceValue = null;

                // IDを振り直し
                for (int j = 0; j < mapGridInfosProp.arraySize; j++)
                {
                    SerializedProperty effectWithID = mapGridInfosProp.GetArrayElementAtIndex(j);
                    effectWithID.FindPropertyRelative("id").intValue = j;
                }
            }

            // 下に挿入処理
            if (insertIndexBelow != -1)
            {
                mapGridInfosProp.InsertArrayElementAtIndex(insertIndexBelow + 1);
                SerializedProperty newElement = mapGridInfosProp.GetArrayElementAtIndex(insertIndexBelow + 1);
                newElement.FindPropertyRelative("info").objectReferenceValue = null;

                // IDを振り直し
                for (int j = 0; j < mapGridInfosProp.arraySize; j++)
                {
                    SerializedProperty effectWithID = mapGridInfosProp.GetArrayElementAtIndex(j);
                    effectWithID.FindPropertyRelative("id").intValue = j;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    } 
#endif
}