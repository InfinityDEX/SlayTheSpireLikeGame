// Note：時短の為AIで生成。詳細を確認できていない為、不具合が発生する可能性あり。

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// UILineRenderer に「線の累積距離」をUV.xとして設定する
    /// 破線表示用のUILineRenderer。
    ///
    /// Shader Graph側では UV.x を線の距離として扱える。
    ///
    /// 注意:
    /// - BezierMode は None 推奨
    /// - ImproveResolution は None 推奨
    /// </summary>
    [AddComponentMenu("UI/Extensions/Primitives/Dashed UILineRenderer")]
    public class DashedUILineRenderer : UILineRenderer
    {
        private struct PathSegment
        {
            public Vector2 start;
            public Vector2 end;

            /// <summary>
            /// この線分の開始位置までの累積距離
            /// </summary>
            public float distance;

            /// <summary>
            /// 線分自身の長さ
            /// </summary>
            public float length;

            public PathSegment(
                Vector2 start,
                Vector2 end,
                float distance)
            {
                this.start = start;
                this.end = end;
                this.distance = distance;
                this.length = Vector2.Distance(start, end);
            }
        }

        private readonly List<PathSegment> pathSegments = new();
        private readonly List<UIVertex> vertexStream = new();

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            // まず通常のUILineRendererにメッシュを作らせる
            base.OnPopulateMesh(vh);

            if (vh.currentVertCount == 0)
            {
                return;
            }

            // BezierやResolutionを使用すると、
            // UILineRenderer内部でPointsが別の形に変換されるため、
            // このクラスでは正確な距離UVを作れない。
            if (BezierMode != BezierType.None)
            {
                return;
            }

            if (ImproveResolution != ResolutionMode.None)
            {
                return;
            }

            BuildPathSegments();

            if (pathSegments.Count == 0)
            {
                return;
            }

            // VertexHelperから現在の頂点を取得
            vertexStream.Clear();
            vh.GetUIVertexStream(vertexStream);

            // 各頂点のUV.xを「線の始点からの距離」に変更
            for (int i = 0; i < vertexStream.Count; i++)
            {
                UIVertex vertex = vertexStream[i];

                float distance = GetDistanceAlongPath(vertex.position);

                // U = 累積距離
                // V = 元のV値を維持
                vertex.uv0 = new Vector2(
                    distance,
                    vertex.uv0.y
                );

                vertexStream[i] = vertex;
            }

            // UVを書き換えた頂点を再設定
            vh.Clear();
            vh.AddUIVertexTriangleStream(vertexStream);
        }

        /// <summary>
        /// UILineRendererのPointsから、
        /// 「線分 + 累積距離」の情報を作る。
        /// </summary>
        private void BuildPathSegments()
        {
            pathSegments.Clear();

            // Segmentsが設定されている場合
            if (Segments != null && Segments.Count > 0)
            {
                foreach (Vector2[] segment in Segments)
                {
                    BuildPathForPoints(segment);
                }

                return;
            }

            // 通常のPoints
            if (Points != null && Points.Length >= 2)
            {
                BuildPathForPoints(Points);
            }
        }

        /// <summary>
        /// 1本のPolylineからPathSegmentを作る。
        /// </summary>
        private void BuildPathForPoints(Vector2[] points)
        {
            if (points == null || points.Length < 2)
            {
                return;
            }

            // UILineRendererと同じ座標変換を行う。
            float sizeX = !RelativeSize ? 1f : rectTransform.rect.width;
            float sizeY = !RelativeSize ? 1f : rectTransform.rect.height;

            float offsetX = -rectTransform.pivot.x * sizeX;
            float offsetY = -rectTransform.pivot.y * sizeY;

            Vector2 ConvertToLocal(Vector2 point)
            {
                return new Vector2(
                    point.x * sizeX + offsetX,
                    point.y * sizeY + offsetY
                );
            }

            // Line List の場合
            if (LineList)
            {
                for (int i = 1; i < points.Length; i += 2)
                {
                    Vector2 start = ConvertToLocal(points[i - 1]);
                    Vector2 end = ConvertToLocal(points[i]);

                    AddPathSegment(start, end, 0f);
                }

                return;
            }

            // 通常の連続したLine
            List<Vector2> convertedPoints = new(points.Length);

            foreach (Vector2 point in points)
            {
                convertedPoints.Add(ConvertToLocal(point));
            }

            // Closed Lineの場合、
            // UILineRendererと同様に始点を最後へ追加。
            if (LineClosed &&
                convertedPoints.Count > 2 &&
                convertedPoints[0] != convertedPoints[^1])
            {
                convertedPoints.Add(convertedPoints[0]);
            }

            float cumulativeDistance = 0f;

            for (int i = 1; i < convertedPoints.Count; i++)
            {
                Vector2 start = convertedPoints[i - 1];
                Vector2 end = convertedPoints[i];

                float length = Vector2.Distance(start, end);

                if (length <= Mathf.Epsilon)
                {
                    continue;
                }

                pathSegments.Add(
                    new PathSegment(
                        start,
                        end,
                        cumulativeDistance
                    )
                );

                cumulativeDistance += length;
            }
        }

        /// <summary>
        /// PathSegmentを追加。
        /// LineListの場合は各線ごとに距離を0から開始する。
        /// </summary>
        private void AddPathSegment(
            Vector2 start,
            Vector2 end,
            float distance)
        {
            float length = Vector2.Distance(start, end);

            if (length <= Mathf.Epsilon)
            {
                return;
            }

            pathSegments.Add(
                new PathSegment(
                    start,
                    end,
                    distance
                )
            );
        }

        /// <summary>
        /// 任意の頂点位置が、
        /// 線の始点から何UI単位の位置にあるかを求める。
        /// </summary>
        private float GetDistanceAlongPath(Vector2 position)
        {
            float closestDistanceSqr = float.MaxValue;
            float resultDistance = 0f;

            foreach (PathSegment segment in pathSegments)
            {
                Vector2 direction = segment.end - segment.start;

                float directionSqrMagnitude = direction.sqrMagnitude;

                if (directionSqrMagnitude <= Mathf.Epsilon)
                {
                    continue;
                }

                // 線分上の最近点を求める
                float t = Vector2.Dot(
                    position - segment.start,
                    direction
                ) / directionSqrMagnitude;

                t = Mathf.Clamp01(t);

                Vector2 closestPoint =
                    segment.start + direction * t;

                float distanceSqr =
                    (position - closestPoint).sqrMagnitude;

                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;

                    resultDistance =
                        segment.distance +
                        segment.length * t;
                }
            }

            return resultDistance;
        }
    }
}