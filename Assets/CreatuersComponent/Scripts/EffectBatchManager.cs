using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EffectBatchManager : MonoBehaviour
{
    [Header("Batch Group")]
    [SerializeField]
    private GameObject batchGroup;

    [Header("バッチプレハブ")]
    [SerializeField]
    private EffectBatch batchPrefab;

    [System.Serializable]
    public struct IconEntry
    {
        public int iconId;
        public Sprite iconSprite;
    }

    [Header("アイコンリスト")]
    [SerializeField]
    public List<IconEntry> iconList = new List<IconEntry>();
    
    /// <summary>
    /// 新しいバッチを追加。
    /// </summary>
    /// <param name="batchId">バッチ画像のID</param>
    /// <returns>生成したエフェクトバッチオブジェクト本体</returns>
    public EffectBatch GenerateEffectBatch(int iconId)
    {

        // iconIdに該当するSpriteを探す
        Sprite iconSprite = null;
        foreach (var entry in iconList)
        {
            if (entry.iconId == iconId)
            {
                iconSprite = entry.iconSprite;
                break;
            }
        }

        // 該当するIDが見つからなかったら
        if(iconSprite == null)
        {
            Debug.LogWarning($"ID[{iconId}]のバッチ画像は登録されていません。");
            // 何も生成せずに終了
            return null;
        }

        EffectBatch effectBatch = Instantiate(batchPrefab, batchGroup.transform);
        // Spriteを渡してセット
        effectBatch.SetIconSprite(iconSprite);
        return effectBatch;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(EffectBatchManager))]
public class EffectBatchManagerEditor : Editor
{
    private SerializedProperty iconListProp;

    // --- ユーティリティメソッド ---
    // iconList以外のSerializeField/publicなフィールドをデフォルトレンダリングで描画
    void DrawDefaultInspectorExceptIconList()
    {
        // すべてのフィールドを取得
        var obj = serializedObject.targetObject;
        var type = obj.GetType();

        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;
        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false;

            // "iconList"はカスタム表示のためスキップ
            if (iterator.propertyPath == "iconList")
                continue;

            // "m_Script"は常に最上部・自動で描画されるので必要なければ除外（Unity既定）
            if (iterator.propertyPath == "m_Script")
                continue;

            EditorGUILayout.PropertyField(iterator, true);
        }
    }

    private void OnEnable()
    {
        iconListProp = serializedObject.FindProperty("iconList");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // すべての[SerializeField]やpublic変数をInspector上部にデフォルトレンダリングで表示
        DrawDefaultInspectorExceptIconList();


        EditorGUILayout.LabelField("アイコンリスト", EditorStyles.boldLabel);

        // ヘッダー
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("ID", GUILayout.Width(40));
        GUILayout.Label("Icon Image", GUILayout.ExpandWidth(true));
        GUILayout.Label("", GUILayout.Width(25)); // for 上に追加
        GUILayout.Label("", GUILayout.Width(25)); // for 下に追加
        GUILayout.Label("", GUILayout.Width(25)); // for 削除
        EditorGUILayout.EndHorizontal();

        int insertIndexAbove = -1;
        int insertIndexBelow = -1;
        int deleteIndex = -1;

        for (int i = 0; i < iconListProp.arraySize; i++)
        {
            SerializedProperty iconEntryProp = iconListProp.GetArrayElementAtIndex(i);
            SerializedProperty iconIdProp = iconEntryProp.FindPropertyRelative("iconId");
            SerializedProperty iconSpriteProp = iconEntryProp.FindPropertyRelative("iconSprite");

            EditorGUILayout.BeginHorizontal();

            // IDをラベルで表示(変更不可)
            EditorGUILayout.LabelField(iconIdProp.intValue.ToString(), GUILayout.Width(40));

            iconSpriteProp.objectReferenceValue = EditorGUILayout.ObjectField(
                iconSpriteProp.objectReferenceValue,
                typeof(Sprite),
                true,
                GUILayout.ExpandWidth(true)
            );

            // 上に追加ボタン
            if (GUILayout.Button("上に追加", GUILayout.Width(60)))
            {
                insertIndexAbove = i;
            }
            // 下に追加ボタン
            if (GUILayout.Button("下に追加", GUILayout.Width(60)))
            {
                insertIndexBelow = i;
            }
            // 削除ボタン
            if (GUILayout.Button("削除", GUILayout.Width(40)))
            {
                deleteIndex = i;
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("新規アイコン追加"))
        {
            insertIndexBelow = iconListProp.arraySize - 1;
        }

        // 行削除処理
        if (deleteIndex != -1)
        {
            iconListProp.DeleteArrayElementAtIndex(deleteIndex);
            // IDを振り直し
            for (int j = 0; j < iconListProp.arraySize; j++)
            {
                SerializedProperty cardWithID = iconListProp.GetArrayElementAtIndex(j);
                cardWithID.FindPropertyRelative("iconId").intValue = j;
            }
        }

        // 上に挿入処理
        if (insertIndexAbove != -1)
        {
            iconListProp.InsertArrayElementAtIndex(insertIndexAbove);
            SerializedProperty newElement = iconListProp.GetArrayElementAtIndex(insertIndexAbove);
            newElement.FindPropertyRelative("iconSprite").objectReferenceValue = null;

            // IDをすべて振り直し (0, 1, 2,...)
            for (int j = 0; j < iconListProp.arraySize; j++)
            {
                SerializedProperty cardWithID = iconListProp.GetArrayElementAtIndex(j);
                cardWithID.FindPropertyRelative("iconId").intValue = j;
            }
        }

        // 下に挿入処理
        if (insertIndexBelow != -1)
        {
            iconListProp.InsertArrayElementAtIndex(insertIndexBelow + 1);
            SerializedProperty newElement = iconListProp.GetArrayElementAtIndex(insertIndexBelow + 1);
            newElement.FindPropertyRelative("iconSprite").objectReferenceValue = null;

            // IDをすべて振り直し (0, 1, 2,...)
            for (int j = 0; j < iconListProp.arraySize; j++)
            {
                SerializedProperty cardWithID = iconListProp.GetArrayElementAtIndex(j);
                cardWithID.FindPropertyRelative("iconId").intValue = j;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
