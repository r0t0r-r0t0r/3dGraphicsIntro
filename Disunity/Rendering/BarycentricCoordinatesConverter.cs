namespace Disunity.Rendering
{
    public struct BarycentricCoordinatesConverter
    {
        private readonly float _ka;
        private readonly float _la;
        private readonly float _m;

        private readonly float _kb;
        private readonly float _lb;
        private readonly float _n;

        public BarycentricCoordinatesConverter(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            var a = 1/((y1 - y3)*(x2 - x3) - (x1 - x3)*(y2 - y3));
            var b = 1/((y2 - y1)*(x3 - x1) - (x2 - x1)*(y3 - y1));

            _ka = (x2 - x3)*a;
            _la = (y2 - y3)*a;
            _m = x3*_la - y3*_ka;

            _kb = (x3 - x1) * b;
            _lb = (y3 - y1) * b;
            _n = x1 * _lb - y1 * _kb;
        }

        public BarycentricCoordinates Convert(float x, float y)
        {
            var lambda1 = y*_ka - x*_la + _m;
            var lambda2 = y*_kb - x*_lb + _n;
            var lambda3 = 1 - lambda1 - lambda2;
            return new BarycentricCoordinates(lambda1, lambda2, lambda3);
        }
    }
}