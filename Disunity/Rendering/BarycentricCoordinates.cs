namespace Disunity.Rendering
{
    public struct BarycentricCoordinates
    {
        private readonly float _a;
        private readonly float _b;
        private readonly float _c;

        public BarycentricCoordinates(float a, float b, float c)
        {
            _a = a;
            _b = b;
            _c = c;
        }

        public float A
        {
            get { return _a; }
        }

        public float B
        {
            get { return _b; }
        }

        public float C
        {
            get { return _c; }
        }
    }
}