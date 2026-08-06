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
   
   public int effectCount;

   // Start is called before the first frame update
   void Start()
   {
      countView.text = $"{effectCount}";
   }

   // Update is called once per frame
   void Update()
   {
      countView.text = $"{effectCount}";
   }

   public void SetIconSprite(Sprite sprite)
   {
      icon.sprite = sprite;
      icon.color = Color.white;
   }
}
