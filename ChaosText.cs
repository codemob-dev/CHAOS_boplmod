using TMPro;
using UnityEngine;
using System;

namespace CHAOS
{
    class ChaosText : MonoBehaviour
    {
        void Awake()
        {
            tmpUGUI.fontSize = fontSize;
        }
        void Update()
        {
            tmpUGUI.fontSize = fontSize;
            var timeTo = (float)(animationFinish - DateTime.Now).TotalSeconds;
            if (timeTo > 0) {
                var deltaX = animateTo.x - position.x;
                var deltaY = animateTo.y - position.y;
                var percentMove = Time.deltaTime / timeTo;
                position.x += deltaX * percentMove;
                position.y += deltaY * percentMove;
            } else
            {
                position = animateTo;
            }
            rectTransform.anchoredPosition = position;
        }
        public void Remove()
        {
            animateTo -= new Vector2(leftSeparation + tmpUGUI.preferredWidth, 0);
            animationFinish = DateTime.Now + TimeSpan.FromSeconds(.5);
            Destroy(this, .5f);
        }
        public void Move(int amt)
        {
            animateTo += new Vector2(0, amt * -separation);
            animationFinish = DateTime.Now + TimeSpan.FromSeconds(.5);
        }
        public Vector2 position;
        public RectTransform rectTransform;
        public TextMeshProUGUI tmpUGUI;

        public Vector2 animateTo;
        public DateTime animationFinish;

        public static int separation = 15;
        public static int leftSeparation = 4;
        public static int fontSize = 26;
        internal static ChaosText CreateNew(string text, Vector2? previousPosition = null)
        {
            Vector2 newPos;
            if (previousPosition.HasValue)
            {
                newPos = previousPosition.Value + new Vector2(0, -separation);
            }
            else
            {
                newPos = new Vector2(leftSeparation, -35);
            }
            
            // create the GameObject and set the parent to the canvas
            GameObject textGO = new GameObject($"chaos info text \"{text}\"");
            textGO.SetActive(false);
            ChaosText chaosText = textGO.AddComponent<ChaosText>();
            chaosText.position = newPos;

            RectTransform rectTransform = textGO.AddComponent<RectTransform>();
            chaosText.rectTransform = rectTransform;
            var textComponent = textGO.AddComponent<TextMeshProUGUI>();
            chaosText.tmpUGUI = textComponent;
            textGO.transform.SetParent(CHAOS.overlayCanvas.transform);

            textGO.SetActive(true);


            textComponent.text = text;

            // change settings
            textComponent.font = LocalizedText.localizationTable.GetFont(Settings.Get().Language, false);
            textComponent.color = Color.Lerp(Color.blue, Color.black, 0.6f);
            textComponent.fontSize = fontSize;

            // Allow the text to be clicked through
            textComponent.raycastTarget = false;

            // Align to bottom right
            textComponent.alignment = TextAlignmentOptions.TopLeft;

            rectTransform.pivot = new Vector2(0f, 0f);
            rectTransform.sizeDelta = new Vector2(1200, 0);
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(0f, 1f);

            chaosText.animateTo = chaosText.position;
            chaosText.position -= new Vector2(leftSeparation + textComponent.preferredWidth, 0);
            chaosText.animationFinish = DateTime.Now + TimeSpan.FromSeconds(.5);

            return chaosText;
        }
    }
}