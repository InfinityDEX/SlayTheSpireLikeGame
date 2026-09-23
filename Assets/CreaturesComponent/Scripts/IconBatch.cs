using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// アイコンバッチクラス
/// 
/// 画面上に表示するアイコン画像と、効果のカウントラベルを
/// 組み合わせたオブジェクト（アイコンバッチ）を定義したクラス
/// </summary>
public class IconBatch : MonoBehaviour
{
   [Header("アイコン画像表示領域")]
   [SerializeField]
   private Image icon;
   
   [Header("効果のカウント表示")]
   [SerializeField]
   private TextMeshProUGUI countView;
   
   /// <summary>
   /// アイコンバッチに紐づけられた
   /// 効果の値（ブロックや筋力等）
   /// </summary>
   public int effectCount = 0;
   
   /// <summary>
   /// 表示するカウンターに（「5x3」のような）倍率の表示が必要な場合
   /// その倍率の値を設定（0ならこの値をバッチ上には表示せず、effectCountの値だけ表示する）
   /// 
   /// 例）攻撃予告のバッチで5ダメージの攻撃を3回という意味で
   /// 「5x3」と表示したい場合、この値に「Y」を設定する
   /// </summary>
   public int ratioCount = 0;

   void Start()
   {
      EffectBatchUpdate();
   }

   void Update()
   {
      EffectBatchUpdate();
   }

   /// <summary>
   /// アイコンバッチの表示内容の更新
   /// </summary>
   private void EffectBatchUpdate()
   {
      // effectCountとratioCountの現在の値を
      // アイコンバッチ実体に反映して表示を更新する
      if (effectCount == 0 && ratioCount == 0)
      {
         countView.text = "";
      }
      else
      {
         countView.text = $"{effectCount}{(ratioCount > 0 ? "×" + ratioCount.ToString() : "")}";
      }
   }

   /// <summary>
   /// アイコンバッチのアイコン画像ソースを登録する
   /// </summary>
   /// <param name="sprite">画像ソース</param>
   public void SetIconSprite(Sprite sprite)
   {
      icon.sprite = sprite;
      icon.color = Color.white;
   }
}
