using Vector3 = UnityEngine.Vector3;

namespace UI
{
    // used to draw quadratic and cubic Bézier curves
    // Wikipedia: https://en.wikipedia.org/wiki/B%C3%A9zier_curve
    // 2D version: https://github.com/wmcnamara/unity-bezier/blob/main/Assets/Bezier/Bezier.cs
    public class Bezier
    {
        private class BezierInternal
        {
            private readonly Vector3 _p1;
            private readonly Vector3 _p2;
            private readonly Vector3 _p3;
            private readonly Vector3 _p4;
            
            internal BezierInternal(Vector3 p1, Vector3 p2, Vector3 p3)
            {
                _p1 = p1;
                _p2 = p2;
                _p3 = p3;
            }
            
            internal BezierInternal(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
            {
                _p1 = p1;
                _p2 = p2;
                _p3 = p3;
                _p4 = p4;
            }

            // get the Quadratic Bezier curve point at time t
            internal Vector3 QuadraticInterp(float t)
            {
                return Vector3.Lerp(
                    Vector3.Lerp(_p1, _p2, t),
                    Vector3.Lerp(_p2, _p3, t),
                    t);
            }
            
            // get the Cubic Bezier curve point at time t
            internal Vector3 CubicInterp(float t)
            {
                Vector3 a = Vector3.Lerp(_p1, _p2, t);
                Vector3 b = Vector3.Lerp(_p2, _p3, t);
                Vector3 c = Vector3.Lerp(_p3, _p4, t);

                return Vector3.Lerp(
                    Vector3.Lerp(a, b, t),
                    Vector3.Lerp(b, c, t),
                    t);
            }
        }
    
        // return `numPoints` of a Quadratic Bezier interpolation
        public static Vector3[] QuadraticInterp(Vector3 p1, Vector3 p2, Vector3 p3, int numPoints)
        {
            Vector3[] res = new Vector3[numPoints];
            BezierInternal bi = new BezierInternal(p1, p2, p3);

            float step = 1f / (numPoints - 1);
            for (int i = 0; i < (numPoints - 1); i++)
            {
                res[i] = bi.QuadraticInterp(step * i);
            }

            res[numPoints - 1]  = p3;

            return res;
        }
        
        // return `numPoints` of a Cubic Bezier interpolation
        public static Vector3[] CubicInterp(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, int numPoints)
        {
            Vector3[] res = new Vector3[numPoints];
            BezierInternal bi = new BezierInternal(p1, p2, p3, p4);

            float step = 1f / (numPoints - 1);
            for (int i = 0; i < (numPoints - 1); i++)
            {
                res[i] = bi.CubicInterp(step * i);
            }

            res[numPoints - 1] = p3;

            return res;
        }
    }
}