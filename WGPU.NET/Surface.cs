using System;
using System.Runtime.InteropServices;

namespace WGPU
{
    public class SurfaceDescriptor
    {
        internal class FFI
        {
            [StructLayout(LayoutKind.Sequential)]
            internal struct Descriptor
            {
                internal IntPtr NextInChain;

                [MarshalAs(UnmanagedType.LPStr)]
                internal String Label;
            }

            internal interface PlatformSpecific { }

            [StructLayout(LayoutKind.Sequential)]
            internal struct DescriptorFromMetalLayer : PlatformSpecific
            {
                internal ChainedStruct Chain;
                internal IntPtr Layer;
            }


            [StructLayout(LayoutKind.Sequential)]
            internal struct DescriptorFromWindowsHWND : PlatformSpecific
            {
                internal ChainedStruct Chain;
                internal IntPtr HInstance;
                internal IntPtr HWND;

            }


            [StructLayout(LayoutKind.Sequential)]
            internal struct DescriptorFromXlib : PlatformSpecific
            {
                internal ChainedStruct Chain;
                internal IntPtr Display;
                internal uint Window;
            }
        }
        public String Label;

        FFI.PlatformSpecific PlatformSpecific;

        SurfaceDescriptor(FFI.PlatformSpecific desc)
        {
            PlatformSpecific = desc;
        }

        static public SurfaceDescriptor FromX11(IntPtr display, IntPtr window)
        {
            var desc = new FFI.DescriptorFromXlib
            {
                Chain = new ChainedStruct
                {
                    SType = SType.SurfaceDescriptorFromXlib
                },
                Display = display,
                Window = (uint)window
            };

            return new SurfaceDescriptor(desc);
        }

        static public SurfaceDescriptor FromWindowsHWND(IntPtr hinstance, IntPtr hwnd)
        {
            var desc = new FFI.DescriptorFromWindowsHWND
            {
                Chain = new ChainedStruct
                {
                    SType = SType.SurfaceDescriptorFromWindowsHWND
                },
                HInstance = hinstance,
                HWND = hwnd
            };

            return new SurfaceDescriptor(desc);
        }

        internal RawData<FFI.Descriptor> ToRaw()
        {
            var nextInChain = Marshal.AllocHGlobal(Marshal.SizeOf(PlatformSpecific));
            Marshal.StructureToPtr(PlatformSpecific, nextInChain, false);

            var inner = new FFI.Descriptor
            {
                NextInChain = nextInChain,
                Label = Label,
            };

            return new RawData<FFI.Descriptor>(inner, new[] { (DisposablePtr)nextInChain });
        }
    }

    public class Surface
    {
        public class Types
        {
        }

        class FFI
        {
            [DllImport(WgpuNative.LIBLARY, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void WgpuSurfaceGetPreferredFormat(IntPtr surface, IntPtr adapter, IntPtr callback, IntPtr userdata);
        }

        IntPtr Ptr;

        Surface(IntPtr ptr)
        {
            Ptr = ptr;
        }

        public static implicit operator IntPtr(Surface adapter) => adapter.Ptr;
        public static explicit operator Surface(IntPtr ptr) => new Surface(ptr);

        public void GetPreferredFormat(IntPtr adapter)
        {
            // FFI.WgpuSurfaceGetPreferredFormat(Ptr, adapter, () => { }, IntPtr.Zero);
        }
    }
}