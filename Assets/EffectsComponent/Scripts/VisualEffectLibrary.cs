using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class VisualEffectLibrary : MonoBehaviour
{
    // カードとIDを組み合わせた構造体
    [System.Serializable]
    public struct EffectWithID
    {
        public int effectId;
        public GameObject effectPrefab;
    }

    public List<EffectWithID> effects;

    /// <summary>
    /// 指定したeffectIdを持つエフェクトPrefabを返す。存在しない場合はnull。
    /// </summary>
    public GameObject GetEffectById(int effectId)
    {
        foreach (var effect in effects)
        {
            if (effect.effectId == effectId)
            {
                return effect.effectPrefab;
            }
        }
        return null;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(VisualEffectLibrary))]
    public class EffectLibraryEditor : Editor
    {
        private SerializedProperty effectsProp;

        private void OnEnable()
        {
            effectsProp = serializedObject.FindProperty("effects");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("エフェクト一覧", EditorStyles.boldLabel);

            // ヘッダー
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("ID", GUILayout.Width(40));
            GUILayout.Label("Effect Prefab", GUILayout.ExpandWidth(true));
            GUILayout.Label("", GUILayout.Width(60)); // for 上に追加
            GUILayout.Label("", GUILayout.Width(60)); // for 下に追加
            GUILayout.Label("", GUILayout.Width(40)); // for 削除
            EditorGUILayout.EndHorizontal();

            int insertIndexAbove = -1;
            int insertIndexBelow = -1;
            int deleteIndex = -1;

            for (int i = 0; i < effectsProp.arraySize; i++)
            {
                SerializedProperty effectWithIDProp = effectsProp.GetArrayElementAtIndex(i);
                SerializedProperty effectIdProp = effectWithIDProp.FindPropertyRelative("effectId");
                SerializedProperty effectPrefabProp = effectWithIDProp.FindPropertyRelative("effectPrefab");

                EditorGUILayout.BeginHorizontal();

                // IDをラベルで表示(変更不可)
                EditorGUILayout.LabelField(effectIdProp.intValue.ToString(), GUILayout.Width(40));

                effectPrefabProp.objectReferenceValue = EditorGUILayout.ObjectField(
                    effectPrefabProp.objectReferenceValue,
                    typeof(GameObject),
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
            if (GUILayout.Button("Add Effect At End"))
            {
                // Insert new element at the end of the array
                effectsProp.InsertArrayElementAtIndex(effectsProp.arraySize);
                // 新しい要素の値を初期化
                if (effectsProp.arraySize > 0)
                {
                    SerializedProperty newElement = effectsProp.GetArrayElementAtIndex(effectsProp.arraySize - 1);
                    newElement.FindPropertyRelative("effectPrefab").objectReferenceValue = null;
                    newElement.FindPropertyRelative("effectId").intValue = effectsProp.arraySize - 1;
                }
            }

            // 行削除処理
            if (deleteIndex != -1)
            {
                effectsProp.DeleteArrayElementAtIndex(deleteIndex);
                // IDを振り直し
                for (int j = 0; j < effectsProp.arraySize; j++)
                {
                    SerializedProperty effectWithID = effectsProp.GetArrayElementAtIndex(j);
                    effectWithID.FindPropertyRelative("effectId").intValue = j;
                }
            }

            // 上に挿入処理
            if (insertIndexAbove != -1)
            {
                effectsProp.InsertArrayElementAtIndex(insertIndexAbove);
                SerializedProperty newElement = effectsProp.GetArrayElementAtIndex(insertIndexAbove);
                newElement.FindPropertyRelative("effectPrefab").objectReferenceValue = null;

                // IDを振り直し
                for (int j = 0; j < effectsProp.arraySize; j++)
                {
                    SerializedProperty effectWithID = effectsProp.GetArrayElementAtIndex(j);
                    effectWithID.FindPropertyRelative("effectId").intValue = j;
                }
            }

            // 下に挿入処理
            if (insertIndexBelow != -1)
            {
                effectsProp.InsertArrayElementAtIndex(insertIndexBelow + 1);
                SerializedProperty newElement = effectsProp.GetArrayElementAtIndex(insertIndexBelow + 1);
                newElement.FindPropertyRelative("effectPrefab").objectReferenceValue = null;

                // IDを振り直し
                for (int j = 0; j < effectsProp.arraySize; j++)
                {
                    SerializedProperty effectWithID = effectsProp.GetArrayElementAtIndex(j);
                    effectWithID.FindPropertyRelative("effectId").intValue = j;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
