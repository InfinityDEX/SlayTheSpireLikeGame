using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconBatch : MonoBehaviour
{
   [Header("アイコン")]
   [SerializeField]
   private Image icon;
   
   [Header("効果のカウント表示")]
   [SerializeField]
   private TextMeshProUGUI countView;
   
   public int effectCount = 0;
   
   public int ratioCount = 0;

   // Start is called before the first frame update
   void Start()
   {
      IconUpdate();
   }

   // Update is called once per frame
   void Update()
   {
      IconUpdate();
   }

   private void IconUpdate()
   {
      if (effectCount == 0 && ratioCount == 0)
      {
         countView.text = "";
      }
      else
      {
         countView.text = $"{effectCount}{(ratioCount > 0 ? "×" + ratioCount.ToString() : "")}";
      }
   }

   public void SetIconSprite(Sprite sprite)
   {
      icon.sprite = sprite;
      icon.color = Color.white;
   }
}
