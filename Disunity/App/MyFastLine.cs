using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render
{
    public struct MyFastLine
    {
        private readonly float _dx;
        private readonly float _dy;

        private float _currX;
        private float _currY;

        private readonly float _minXdX;

        public MyFastLine(float x0, float y0, float x1, float y1, float minX, float minY)
        {
            var c = -y0*(x1 - x0) + x0*(y1 - y0);
            _dx = y0 - y1;
            _dy = x1 - x0;

            var minYdY = minY*_dy;
            _minXdX = minX*_dx;
            _currX = minX*_dx;
            _currY = minYdY + c;
        }

        public float Value
        {
            get
            {
                return _currX + _currY;
            }
        }

        public void StepX()
        {
            _currX += _dx;
        }

        public void StepY()
        {
            _currY += _dy;
            _currX = _minXdX;
        }
    }
}
