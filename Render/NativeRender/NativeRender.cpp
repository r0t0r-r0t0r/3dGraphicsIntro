// NativeRender.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "NativeRender.h"


// This is an example of an exported variable
NATIVERENDER_API int nNativeRender=0;

// This is an example of an exported function.
NATIVERENDER_API int fnNativeRender(void)
{
	return 42;
}

NATIVERENDER_API float GetIntensity(int packedNormal, float lightX, float lightY, float lightZ)
{
	return 0;
}

// This is the constructor of a class that has been exported.
// see NativeRender.h for the class definition
CNativeRender::CNativeRender()
{
	return;
}

struct Color
{
	union
	{
		struct
		{
			unsigned char b, g, r, a;
		};
		unsigned int argb;
	};

	Color()
	{
	}

	Color(unsigned int col)
	{
		argb = col;
	}
};
