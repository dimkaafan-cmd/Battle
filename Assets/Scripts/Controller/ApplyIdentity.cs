using System.Collections.Generic;
using System.Linq;
using Scripts.Data;
using Scripts.View;
using UnityEngine;

namespace Scripts.Controller{
    public static class ApplyIdentity
    {
        public static void Apply(UnitController unit, UnitIdentity identity,  UnitSettings unitSettings, out float unitMaxSize)
        {
            var gameObject = unit;
            unitMaxSize = 0f;
            if (unitSettings == null)
            {
                return;
            }
            var meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                var meshItem = unitSettings.shapes.FirstOrDefault(it => it.unitShape == identity.shape);
                meshFilter.sharedMesh = meshItem.mesh;
            }

            var scaleItem = unitSettings.sizes.FirstOrDefault(it => it.unitSize == identity.size);
            gameObject.transform.localScale = Vector3.one * scaleItem.scale;

            var colorItem = unitSettings.colors.FirstOrDefault(it => it.unitColor == identity.color);
            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.SetSharedMaterials(new List<Material>{colorItem.material});
                MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
                int fillPropId = Shader.PropertyToID("_FillAmount"); 
                int fillColorPropId = Shader.PropertyToID("_FillColor"); ;
                meshRenderer.GetPropertyBlock(propBlock);

                propBlock.SetFloat(fillPropId, 1f);
                propBlock.SetColor(fillColorPropId, colorItem.color); 
                meshRenderer.SetPropertyBlock(propBlock);
                
                unitMaxSize = Mathf.Max(meshRenderer.bounds.size.x, Mathf.Max(meshRenderer.bounds.size.y, meshRenderer.bounds.size.z));
            }
        }
    }
}