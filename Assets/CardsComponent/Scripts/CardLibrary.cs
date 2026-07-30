using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// 全てのカードの情報を管理するクラス
[CreateAssetMenu(fileName = "AllCardData", menuName = "Create Card Library Data")]
public class CardLibrary : ScriptableObject
{
    // カードとIDを組み合わせた構造体
    [System.Serializable]
    public struct CardWithID
    {
        public int cardId;
        public CardData cardData;
    }

    [Header("カード一覧")]
    public List<CardWithID> cardDatas;
#if UNITY_EDITOR

[CustomEditor(typeof(CardLibrary))]
public class CardLibraryEditor : Editor
{
    private SerializedProperty cardDatasProp;

    private void OnEnable()
    {
        cardDatasProp = serializedObject.FindProperty("cardDatas");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("カード一覧", EditorStyles.boldLabel);

        // ヘッダー
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("ID", GUILayout.Width(40));
        GUILayout.Label("Card Data", GUILayout.ExpandWidth(true));
        GUILayout.Label("", GUILayout.Width(25)); // for 上に追加
        GUILayout.Label("", GUILayout.Width(25)); // for 下に追加
        GUILayout.Label("", GUILayout.Width(25)); // for 削除
        EditorGUILayout.EndHorizontal();

        int insertIndexAbove = -1;
        int insertIndexBelow = -1;
        int deleteIndex = -1;

        for (int i = 0; i < cardDatasProp.arraySize; i++)
        {
            SerializedProperty cardWithIDProp = cardDatasProp.GetArrayElementAtIndex(i);
            SerializedProperty cardIdProp = cardWithIDProp.FindPropertyRelative("cardId");
            SerializedProperty cardDataProp = cardWithIDProp.FindPropertyRelative("cardData");

            EditorGUILayout.BeginHorizontal();

            // IDをラベルで表示(変更不可)
            EditorGUILayout.LabelField(cardIdProp.intValue.ToString(), GUILayout.Width(40));

            cardDataProp.objectReferenceValue = EditorGUILayout.ObjectField(
                cardDataProp.objectReferenceValue,
                typeof(CardData),
                false,
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

        // 末尾に追加ボタン
        if (GUILayout.Button("Add Card At End"))
        {
            insertIndexBelow = cardDatasProp.arraySize - 1;
        }

        // 行削除処理
        if (deleteIndex != -1)
        {
            cardDatasProp.DeleteArrayElementAtIndex(deleteIndex);
            // IDを振り直し
            for (int j = 0; j < cardDatasProp.arraySize; j++)
            {
                SerializedProperty cardWithID = cardDatasProp.GetArrayElementAtIndex(j);
                cardWithID.FindPropertyRelative("cardId").intValue = j;
            }
        }

        // 上に挿入処理
        if (insertIndexAbove != -1)
        {
            cardDatasProp.InsertArrayElementAtIndex(insertIndexAbove);
            SerializedProperty newElement = cardDatasProp.GetArrayElementAtIndex(insertIndexAbove);
            newElement.FindPropertyRelative("cardData").objectReferenceValue = null;

            // IDをすべて振り直し (0, 1, 2,...)
            for (int j = 0; j < cardDatasProp.arraySize; j++)
            {
                SerializedProperty cardWithID = cardDatasProp.GetArrayElementAtIndex(j);
                cardWithID.FindPropertyRelative("cardId").intValue = j;
            }
        }

        // 下に挿入処理
        if (insertIndexBelow != -1)
        {
            cardDatasProp.InsertArrayElementAtIndex(insertIndexBelow + 1);
            SerializedProperty newElement = cardDatasProp.GetArrayElementAtIndex(insertIndexBelow + 1);
            newElement.FindPropertyRelative("cardData").objectReferenceValue = null;

            // IDをすべて振り直し (0, 1, 2,...)
            for (int j = 0; j < cardDatasProp.arraySize; j++)
            {
                SerializedProperty cardWithID = cardDatasProp.GetArrayElementAtIndex(j);
                cardWithID.FindPropertyRelative("cardId").intValue = j;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
}
