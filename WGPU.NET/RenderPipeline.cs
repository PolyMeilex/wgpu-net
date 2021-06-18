using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace WGPU
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BlendComponent
    {
        public BlendFactor SrcFactor;
        public BlendFactor DstFactor;
        public BlendOperation Operation;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct BlendState
    {
        public BlendComponent Color;
        public BlendComponent Alpha;
    }

    public class ColorTargetState
    {
        internal class FFI
        {
            [StructLayout(LayoutKind.Sequential)]
            internal struct State
            {
                IntPtr NextInChain;
                internal TextureFormat Format;
                internal IntPtr Blend;
                internal ColorWriteMask WriteMask;
            }

        }

        public TextureFormat Format;
        public BlendState Blend;
        public ColorWriteMask WriteMask;

        internal RawData<FFI.State> ToRaw()
        {
            var blend = Marshal.AllocHGlobal(Marshal.SizeOf(Blend));
            Marshal.StructureToPtr(Blend, blend, false);

            var descriptor = new FFI.State
            {
                Format = Format,
                Blend = blend,
                WriteMask = WriteMask
            };

            return new RawData<FFI.State>(descriptor, new[] { (DisposablePtr)blend });
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PrimitiveState
    {
        IntPtr NextInChain;
        public PrimitiveTopology Topology;
        public IndexFormat StripIndexFormat;
        public FrontFace FrontFace;
        public CullMode CullMode;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MultisampleState
    {
        IntPtr NextInChain;
        public uint Count;
        public uint Mask;
        public byte AlphaToCoverageEnabled;
    }

    public class FragmentState
    {
        internal class FFI
        {
            [StructLayout(LayoutKind.Sequential)]
            internal struct State
            {
                internal IntPtr NextInChain;
                internal IntPtr Module;

                [MarshalAs(UnmanagedType.LPStr)]
                internal String EntryPoint;
                internal uint TargetCount;
                internal IntPtr Targets;

            }
        }

        public IntPtr Module;
        public String EntryPoint;
        public ColorTargetState[] Targets = new ColorTargetState[] { };


        internal RawData<FFI.State> ToRaw()
        {
            var rawTargets = Targets.Select((ca) => ca.ToRaw()).ToArray();
            var targetsData = rawTargets.Select((ca) => ca.GetData()).ToArray();

            var targetsPtr = ArrayMarshaler.ArrayOfStructToPtr(targetsData);

            var descriptor = new FFI.State
            {
                NextInChain = IntPtr.Zero,
                Module = Module,
                EntryPoint = EntryPoint,
                TargetCount = (uint)Targets.Length,
                Targets = targetsPtr
            };

            return new RawData<FFI.State>(descriptor, rawTargets);
        }
    }

    public class RenderPipelineDescriptor
    {
        public class FFI
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct VertexState
            {
                IntPtr NextInChain;
                public IntPtr Module;

                [MarshalAs(UnmanagedType.LPStr)]
                public String EntryPoint;
                public uint BufferCount;
                public IntPtr Buffers;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct Descriptor
            {
                IntPtr NextInChain;

                [MarshalAs(UnmanagedType.LPStr)]
                internal String Label;
                internal IntPtr Layout;
                internal VertexState Vertex;
                internal PrimitiveState Primitive;
                internal IntPtr DepthStencil;
                internal MultisampleState Multisample;
                internal IntPtr Fragment;
            }
        }

        public String Label;
        public PipelineLayout Layout;
        public FFI.VertexState Vertex;
        public PrimitiveState Primitive;
        public IntPtr DepthStencil;
        public MultisampleState Multisample;
        public FragmentState Fragment;

        internal RawData<FFI.Descriptor> ToRaw()
        {
            var rawFragment = Fragment.ToRaw();
            var fragmentData = rawFragment.GetRef();

            var fragment_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(fragmentData));
            Marshal.StructureToPtr(fragmentData, fragment_ptr, false);

            var descriptor = new FFI.Descriptor
            {
                Label = Label,
                Layout = Layout,
                Vertex = Vertex,
                Primitive = Primitive,
                DepthStencil = DepthStencil,
                Multisample = Multisample,
                Fragment = fragment_ptr
            };

            return new RawData<FFI.Descriptor>(descriptor, new[] { rawFragment });
        }
    }

    public class RenderPipeline
    {
        IntPtr Ptr;

        RenderPipeline(IntPtr ptr)
        {
            Ptr = ptr;
        }

        public static explicit operator RenderPipeline(IntPtr ptr) => new RenderPipeline(ptr);

    }
}

