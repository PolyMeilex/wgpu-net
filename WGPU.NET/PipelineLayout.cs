using System;
using System.Runtime.InteropServices;

namespace WGPU
{
    public class PipelineLayoutDescriptor
    {
        internal class FFI
        {
            [StructLayout(LayoutKind.Sequential)]
            internal struct Descriptor
            {
                internal IntPtr NextInChain;

                [MarshalAs(UnmanagedType.LPStr)]
                internal String Label;
                internal uint BindGroupLayoutCount;
                internal IntPtr BindGroupLayouts;
            }
        }

        public String Label;
        public IntPtr[] BindGroupLayouts = new IntPtr[] { };


        internal RawData<FFI.Descriptor> ToRaw()
        {
            // var rawColorAttachments = BindGroupLayouts.Select((ca) => ca.ToRaw()).ToArray();
            var bindGroupLayouts = ArrayMarshaler.ArrayOfStructToPtr(BindGroupLayouts);

            var descriptor = new FFI.Descriptor
            {
                NextInChain = IntPtr.Zero,
                Label = Label,
                BindGroupLayoutCount = (uint)BindGroupLayouts.Length,
                BindGroupLayouts = bindGroupLayouts,
            };

            return new RawData<FFI.Descriptor>(descriptor, new[] { (DisposablePtr)bindGroupLayouts });
        }
    }

    public class PipelineLayout
    {
        IntPtr Ptr;

        PipelineLayout(IntPtr ptr)
        {
            Ptr = ptr;
        }

        public static implicit operator IntPtr(PipelineLayout adapter) => adapter.Ptr;
        public static explicit operator PipelineLayout(IntPtr ptr) => new PipelineLayout(ptr);
    }

}