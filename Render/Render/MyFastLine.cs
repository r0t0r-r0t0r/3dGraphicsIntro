﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render
{
    public sealed class MyFastLine
    {
        private readonly double _x0;
        private readonly double _y0;
        private readonly double _x1;
        private readonly double _y1;
        private readonly double _minX;
        private readonly double _minY;
        private readonly double _maxX;
        private readonly double _maxY;

        private readonly double _dx;
        private readonly double _dy;
        private readonly double _c;

        private double _currX;
        private double _currY;

        private double _x;
        private double _y;

        private readonly double _minYdY;

        public MyFastLine(double x0, double y0, double x1, double y1, double minX, double minY, double maxX, double maxY)
        {
            _x0 = x0;
            _y0 = y0;
            _x1 = x1;
            _y1 = y1;
            _minX = minX;
            _minY = minY;
            _maxX = maxX;
            _maxY = maxY;

            _c = -y0*(x1 - x0) + x0*(y1 - y0);
            _dx = y0 - y1;
            _dy = x1 - x0;

            _minYdY = minY*_dy;
            _currX = minX*_dx + _c;
            _currY = _minYdY;

            _x = minX;
            _y = minY;
        }

        public double Value
        {
//            get { return _currX + _currY; }
            get
            {
                var wrong = _currX + _currY;
//                var right = _y*(_x1 - _x0) - _x*(_y1 - _y0) - _y0*(_x1 - _x0) + _x0*(_y1 - _y0);
                return wrong;

            }
        }

        public void StepX()
        {
            _currY = _minYdY;
            _currX += _dx;

//            _x++;
//            _y = _minY;
        }

        public void StepY()
        {
            _currY += _dy;
//            _y++;
        }
    }
}
