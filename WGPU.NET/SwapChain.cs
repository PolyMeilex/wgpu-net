using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WGPU
{
    public class SwapChainDescriptor
    {
        public class Raw
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct Descriptor
            {
                public IntPtr nextInChain;
                [MarshalAs(UnmanagedType.LPStr)]
                public String label;
                public TextureUsage usage;
                public TextureFormat format;
                public uint width;
                public uint height;
                public PresentMode presentMode;
            }
        }

        public String Label
        {
            get
            {
                return Inner.label;
            }
            set
            {
                Inner.label = value;
            }
        }

        public TextureUsage Usage
        {
            get
            {
                return Inner.usage;
            }
            set
            {
                Inner.usage = value;
            }
        }

        public TextureFormat Format
        {
            get
            {
                return Inner.format;
            }
            set
            {
                Inner.format = value;
            }
        }

        public uint Width
        {
            get
            {
                return Inner.width;
            }
            set
            {
                Inner.width = value;
            }
        }

        public uint Height
        {
            get
            {
                return Inner.height;
            }
            set
            {
                Inner.height = value;
            }
        }

        public PresentMode PresentMode
        {
            get
            {
                return Inner.presentMode;
            }
            set
            {
                Inner.presentMode = value;
            }
        }

        Raw.Descriptor Inner = new Raw.Descriptor();

        // List<IntPtr> pointers = new List<IntPtr>();

        public SwapChainDescriptor()
        {
        }

        public SwapChainDescriptor(Raw.Descriptor descriptor)
        {
            Inner = descriptor;
        }


        internal ref Raw.Descriptor ToRaw()
        {
            // inner.nextInChain = Marshal.AllocHGlobal(Marshal.SizeOf(extras));
            // Marshal.StructureToPtr(extras, inner.nextInChain, false);

            // pointers.Add(inner.nextInChain);
            return ref Inner;
        }

        ~SwapChainDescriptor()
        {
            // pointers.ForEach(ptr => Marshal.FreeHGlobal(ptr));
        }
    }
    public class SwapChain
    {
        public class Types
        {


        }

        class FFI
        {
            [DllImport(WgpuNative.LIBLARY)]
            public static extern IntPtr wgpuSwapChainGetCurrentTextureView(IntPtr swapChain);

            [DllImport(WgpuNative.LIBLARY)]
            public static extern void wgpuSwapChainPresent(IntPtr swapChain);

        }

        IntPtr Ptr;

        SwapChain(IntPtr ptr)
        {
            Ptr = ptr;
        }

        public static implicit operator IntPtr(SwapChain adapter) => adapter.Ptr;
        public static explicit operator SwapChain(IntPtr ptr) => new SwapChain(ptr);

        public SwapChainFrame GetCurrentTextureView()
        {
            return new SwapChainFrame(FFI.wgpuSwapChainGetCurrentTextureView(Ptr), this);
        }

        internal void Present()
        {
            FFI.wgpuSwapChainPresent(Ptr);
        }
    }

    public class SwapChainFrame : IDisposable
    {
        IntPtr Ptr;
        SwapChain SwapChain;

        internal SwapChainFrame(IntPtr ptr, SwapChain swapChain)
        {
            Ptr = ptr;
            SwapChain = swapChain;
        }

        public static implicit operator IntPtr(SwapChainFrame adapter) => adapter.Ptr;

        public void Dispose()
        {
            SwapChain.Present();
        }
    }
}