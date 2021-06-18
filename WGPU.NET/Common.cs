using System;
using System.Runtime.InteropServices;

namespace WGPU
{
    public class WGPULog
    {
        public enum Level : uint
        {
            Off = 0x0,
            Error = 0x1,
            Warn = 0x2,
            Info = 0x3,
            Debug = 0x4,
            Trace = 0x5,
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LogCallback(Level level, [MarshalAs(UnmanagedType.LPStr)] string msg);

        [DllImport(WgpuNative.LIBLARY, EntryPoint = "wgpuSetLogCallback", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCallback(LogCallback callback);

        [DllImport(WgpuNative.LIBLARY, EntryPoint = "wgpuSetLogLevel", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetLevel(Level level);
    }


    public enum SType : uint
    {
        Invalid = 0,
        SurfaceDescriptorFromMetalLayer = 1,
        SurfaceDescriptorFromWindowsHWND = 2,
        SurfaceDescriptorFromXlib = 3,
        SurfaceDescriptorFromCanvasHTMLSelector = 4,
        ShaderModuleSPIRVDescriptor = 5,
        ShaderModuleWGSLDescriptor = 6,
        DeviceExtras = 0x60000001,
        AdapterExtras = 0x60000002,
    }

    [Flags]
    public enum NativeFeature : uint
    {
        TEXTURE_ADAPTER_SPECIFIC_FORMAT_FEATURES = 0x10000000
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ChainedStruct
    {
        // ChainedStruct*
        public IntPtr Next;
        public SType SType;

    }

    public enum TextureUsage : uint
    {
        None = 0x00000000,
        CopySrc = 0x00000001,
        CopyDst = 0x00000002,
        Sampled = 0x00000004,
        Storage = 0x00000008,
        RenderAttachment = 0x00000010,
    }

    public enum PresentMode : uint
    {
        Immediate = 0x0,
        Mailbox = 0x1,
        Fifo = 0x2,
    }

    public enum BackendType : uint
    {
        Null = 0x0,
        D3D11 = 0x1,
        D3D12 = 0x2,
        Metal = 0x3,
        Vulkan = 0x4,
        OpenGL = 0x5,
        OpenGLES = 0x6,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Color
    {
        public double R;
        public double G;
        public double B;
        public double A;
    }

    public enum LoadOp : uint
    {
        Clear = 0x0,
        Load = 0x1,
    }

    public enum StoreOp : uint
    {
        Store = 0x0,
        Clear = 0x1,
    }

    public enum PrimitiveTopology : uint
    {
        PointList = 0x0,
        LineList = 0x1,
        LineStrip = 0x2,
        TriangleList = 0x3,
        TriangleStrip = 0x4,
    }

    public enum IndexFormat : uint
    {
        Undefined = 0x0,
        Uint16 = 0x1,
        Uint32 = 0x2,
    }

    public enum FrontFace : uint
    {
        CCW = 0x0,
        CW = 0x1,
    }

    public enum CullMode : uint
    {
        None = 0x0,
        Front = 0x1,
        Back = 0x2,
    }

    public enum ColorWriteMask : uint
    {
        None = 0x0,
        Red = 0x1,
        Green = 0x2,
        Blue = 0x4,
        Alpha = 0x8,
        All = 0xf,
    }
    public enum BlendFactor : uint
    {
        Zero = 0x0,
        One = 0x1,
        SrcColor = 0x2,
        OneMinusSrcColor = 0x3,
        SrcAlpha = 0x4,
        OneMinusSrcAlpha = 0x5,
        DstColor = 0x6,
        OneMinusDstColor = 0x7,
        DstAlpha = 0x8,
        OneMinusDstAlpha = 0x9,
        SrcAlphaSaturated = 0xa,
        BlendColor = 0xb,
        OneMinusBlendColor = 0xc,
    }

    public enum BlendOperation : uint
    {
        Add = 0x0,
        Subtract = 0x1,
        ReverseSubtract = 0x2,
        Min = 0x3,
        Max = 0x4,
    }

    public enum TextureFormat : uint
    {
        Undefined = 0x0,
        R8Unorm = 0x1,
        R8Snorm = 0x2,
        R8Uint = 0x3,
        R8Sint = 0x4,
        R16Uint = 0x5,
        R16Sint = 0x6,
        R16Float = 0x7,
        RG8Unorm = 0x8,
        RG8Snorm = 0x9,
        RG8Uint = 0xa,
        RG8Sint = 0xb,
        R32Float = 0xc,
        R32Uint = 0xd,
        R32Sint = 0xe,
        RG16Uint = 0xf,
        RG16Sint = 0x10,
        RG16Float = 0x11,
        RGBA8Unorm = 0x12,
        RGBA8UnormSrgb = 0x13,
        RGBA8Snorm = 0x14,
        RGBA8Uint = 0x15,
        RGBA8Sint = 0x16,
        BGRA8Unorm = 0x17,
        BGRA8UnormSrgb = 0x18,
        RGB10A2Unorm = 0x19,
        RG11B10Ufloat = 0x1a,
        RGB9E5Ufloat = 0x1b,
        RG32Float = 0x1c,
        RG32Uint = 0x1d,
        RG32Sint = 0x1e,
        RGBA16Uint = 0x1f,
        RGBA16Sint = 0x20,
        RGBA16Float = 0x21,
        RGBA32Float = 0x22,
        RGBA32Uint = 0x23,
        RGBA32Sint = 0x24,
        Depth32Float = 0x25,
        Depth24Plus = 0x26,
        Depth24PlusStencil8 = 0x27,
        Stencil8 = 0x28,
        BC1RGBAUnorm = 0x29,
        BC1RGBAUnormSrgb = 0x2a,
        BC2RGBAUnorm = 0x2b,
        BC2RGBAUnormSrgb = 0x2c,
        BC3RGBAUnorm = 0x2d,
        BC3RGBAUnormSrgb = 0x2e,
        BC4RUnorm = 0x2f,
        BC4RSnorm = 0x30,
        BC5RGUnorm = 0x31,
        BC5RGSnorm = 0x32,
        BC6HRGBUfloat = 0x33,
        BC6HRGBFloat = 0x34,
        BC7RGBAUnorm = 0x35,
        BC7RGBAUnormSrgb = 0x36,
    }

}