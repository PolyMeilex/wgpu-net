using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WGPU
{
    public class CommandBufferDescriptor
    {
        public class FFI
        {
            [StructLayout(LayoutKind.Sequential)]
            internal struct Descriptor
            {
                internal IntPtr NextInChain;

                [MarshalAs(UnmanagedType.LPStr)]
                internal String Label;
            }

        }

        public String Label;

        internal FFI.Descriptor ToRaw()
        {
            return new FFI.Descriptor
            {
                Label = Label
            };
        }
    }
    public class CommandBuffer
    {
        IntPtr Ptr;

        CommandBuffer(IntPtr ptr)
        {
            Ptr = ptr;
        }

        public static implicit operator IntPtr(CommandBuffer adapter) => adapter.Ptr;
        public static explicit operator CommandBuffer(IntPtr ptr) => new CommandBuffer(ptr);
    }
}