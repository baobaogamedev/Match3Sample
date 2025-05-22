using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    public class ColorPiece : MonoBehaviour
    {
        [System.Serializable]
        public struct ColorSprite
        {
            public ColorType color;
            public Sprite sprite;
        }

        [SerializeField]
        private ColorSprite[] colorSprites;

        private ColorType _color;
        public ColorType Color
        {
            get => _color;
            set => SetColor(value);
        }

        public int NumColors => colorSprites.Length;

        private SpriteRenderer _spriteRenderer;
        private readonly Dictionary<ColorType, Sprite> _colorSpriteDict = new();

        private void Awake()
        {
            _spriteRenderer = transform.Find("piece")?.GetComponent<SpriteRenderer>();
            if (_spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer for 'piece' not found.");
                return;
            }

            foreach (var colorSprite in colorSprites)
            {
                if (!_colorSpriteDict.ContainsKey(colorSprite.color))
                {
                    _colorSpriteDict[colorSprite.color] = colorSprite.sprite;
                }
                else
                {
                    Debug.LogWarning($"Duplicate color entry: {colorSprite.color}");
                }
            }
        }

        public void SetColor(ColorType newColor)
        {
            _color = newColor;

            if (_colorSpriteDict.TryGetValue(newColor, out var sprite))
            {
                _spriteRenderer.sprite = sprite;
            }
            else
            {
                Debug.LogWarning($"Sprite for color {newColor} not found.");
            }
        }
    }
}
