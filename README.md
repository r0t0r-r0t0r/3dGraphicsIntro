# Disunity
Disunity is software real-time 3d renderer inspired by this tutorial: https://github.com/ssloy/tinyrenderer/wiki

Rendering is performed to System.Drawing.Bitmap instance. That instance is shown then by System.Windows.Forms.PictureBox.
It is reasonably fast due to some optimizations:
- Unsafe operations for reading and writing to bitmap
- Frame splitted into parts. These parts are rendered then separately in multithreaded way
- Nearly zero memory allocations while rendering. All necessary allocations done during loading phase

**Note**: App is working way more faster in Release build than in Debug
