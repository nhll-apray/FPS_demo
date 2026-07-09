using UnityEngine;
using UnityEngine.UI;

namespace FpsDemo.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class HitMarkerTriangleGraphic : MaskableGraphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            Rect rect = GetPixelAdjustedRect();
            float middleY = (rect.yMin + rect.yMax) * 0.5f;

            AddVertex(vh, new Vector2(rect.xMin, rect.yMin));
            AddVertex(vh, new Vector2(rect.xMin, rect.yMax));
            AddVertex(vh, new Vector2(rect.xMax, middleY));

            vh.AddTriangle(0, 1, 2);
        }

        private void AddVertex(VertexHelper vh, Vector2 position)
        {
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;
            vertex.position = position;
            vh.AddVert(vertex);
        }
    }
}
