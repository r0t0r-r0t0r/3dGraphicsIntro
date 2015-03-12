// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the NATIVERENDER_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// NATIVERENDER_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef NATIVERENDER_EXPORTS
#define NATIVERENDER_API __declspec(dllexport)
#else
#define NATIVERENDER_API __declspec(dllimport)
#endif

// This class is exported from the NativeRender.dll
class NATIVERENDER_API CNativeRender {
public:
	CNativeRender(void);
	// TODO: add your methods here.
};

extern NATIVERENDER_API int nNativeRender;

NATIVERENDER_API int fnNativeRender(void);

NATIVERENDER_API float GetIntensity(int packedNormal, float lightX, float lightY, float lightZ);
