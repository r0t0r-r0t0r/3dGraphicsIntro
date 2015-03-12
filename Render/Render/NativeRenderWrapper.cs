using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Render
{
    public static class NativeRenderWrapper
    {
        [DllImport("NativeRender.dll", EntryPoint = "?fnNativeRender@@YAHXZ")]
        public static extern int fnNativeRender();

        [DllImport("NativeRender.dll", EntryPoint = "?GetIntensity@@YAMHMMM@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern float GetIntensity(int packedNormal, float lightX, float lightY, float lightZ);
    }
}
