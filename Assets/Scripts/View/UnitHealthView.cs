using UnityEngine;

namespace Scripts.View
{
    public class UnitHealthView : MonoBehaviour 
    {
        private Renderer _renderer;
        private MaterialPropertyBlock _propBlock;
        private int _fillPropId;
        private int _fillColorPropId;

        void Awake() 
        {
            _renderer = GetComponent<Renderer>();
            _propBlock = new MaterialPropertyBlock();
            _fillPropId = Shader.PropertyToID("_FillAmount"); 
            _fillColorPropId = Shader.PropertyToID("_FillColor"); 
        }

        public void UpdateHealth(float health01, Color unitColor) 
        {
            if (_renderer)
            {
                _renderer.GetPropertyBlock(_propBlock);
                _propBlock.SetFloat(_fillPropId, health01);
                _propBlock.SetColor(_fillColorPropId, unitColor); 
                _renderer.SetPropertyBlock(_propBlock);
            }
        }

        public Color GetCurrentColor()
        {
            _renderer.GetPropertyBlock(_propBlock);
            return _propBlock.GetColor(_fillColorPropId);
        }
    }
}