using System;
using System.Runtime.InteropServices;

namespace WGPU
{
    public class CommandEncoderDescriptor
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
        }
        public String Label;
        public CommandEncoderDescriptor()
        {
        }

        internal FFI.Descriptor ToRaw()
        {
            return new FFI.Descriptor
            {
                Label = Label
            };
        }
    }

    public class CommandEncoder
    {
        class FFI
        {
            [DllImport(WgpuNative.LIBLARY, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr wgpuCommandEncoderBeginRenderPass(IntPtr commandEncoder, ref RenderPassDescriptor.FFI.Descriptor descriptor);

            [DllImport(WgpuNative.LIBLARY, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr wgpuCommandEncoderFinish(IntPtr commandEncoder, ref CommandBufferDescriptor.FFI.Descriptor descriptor);
        }

        IntPtr Ptr;

        CommandEncoder(IntPtr ptr)
        {
            Ptr = ptr;
        }

        public static implicit operator IntPtr(CommandEncoder adapter) => adapter.Ptr;
        public static explicit operator CommandEncoder(IntPtr ptr) => new CommandEncoder(ptr);

        public RenderPass BeginRenderPass(RenderPassDescriptor descriptor)
        {
            using (var data = descriptor.ToRaw())
            {
                return new RenderPass(FFI.wgpuCommandEncoderBeginRenderPass(Ptr, ref data.GetRef()));
            }
        }

        public IntPtr Finish(CommandBufferDescriptor descriptor)
        {
            var data = descriptor.ToRaw();
            return FFI.wgpuCommandEncoderFinish(Ptr, ref data);
        }

        public IntPtr Finish()
        {
            var descriptor = new CommandBufferDescriptor();
            var data = descriptor.ToRaw();
            return FFI.wgpuCommandEncoderFinish(Ptr, ref data);
        }
    }
}