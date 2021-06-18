using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace WGPU
{
    public class RenderPassColorAttachment
    {
        internal class FFI
        {
            [StructLayout(LayoutKind.Sequential)]
            internal struct Descriptor
            {
                internal IntPtr Attachment;
                internal IntPtr ResolveTarget;
                internal LoadOp LoadOp;
                internal StoreOp StoreOp;
                internal Color ClearColor;

            }
        }

        public IntPtr Attachment;
        public IntPtr ResolveTarget;
        public LoadOp LoadOp;
        public StoreOp StoreOp;
        public Color ClearColor;

        internal FFI.Descriptor ToRaw()
        {
            return new FFI.Descriptor
            {
                Attachment = Attachment,
                ResolveTarget = ResolveTarget,
                LoadOp = LoadOp,
                StoreOp = StoreOp,
                ClearColor = ClearColor
            };
        }
    }

    public class RenderPassDescriptor
    {
        internal class FFI
        {
            [StructLayout(LayoutKind.Sequential)]
            internal partial struct Descriptor
            {
                internal IntPtr NextInChain;

                [MarshalAs(UnmanagedType.LPStr)]
                internal String Label;
                internal uint ColorAttachmentCount;
                // RenderPassColorAttachment[]
                internal IntPtr ColorAttachments;
                internal IntPtr DepthStencilAttachment;
                internal IntPtr OcclusionQuerySet;
            }

        }
        public String Label;
        public RenderPassColorAttachment[] ColorAttachments;
        public IntPtr DepthStencilAttachment;
        public IntPtr OcclusionQuerySet;

        internal RawData<FFI.Descriptor> ToRaw()
        {
            var rawColorAttachments = ColorAttachments.Select((ca) => ca.ToRaw()).ToArray();
            var colorAttachments = ArrayMarshaler.ArrayOfStructToPtr(rawColorAttachments);

            var descriptor = new FFI.Descriptor
            {
                Label = Label,
                ColorAttachmentCount = (uint)ColorAttachments.Length,
                ColorAttachments = colorAttachments,
                DepthStencilAttachment = DepthStencilAttachment,
                OcclusionQuerySet = OcclusionQuerySet
            };

            return new RawData<FFI.Descriptor>(descriptor, new[] { (DisposablePtr)colorAttachments });
        }
    }

    public class RenderPass : IDisposable
    {
        class FFI
        {
            [DllImport(WgpuNative.LIBLARY)]
            internal static extern void wgpuRenderPassEncoderEndPass(IntPtr renderPassEncoder);
            [DllImport(WgpuNative.LIBLARY)]
            internal static extern void wgpuRenderPassEncoderSetPipeline(IntPtr renderPassEncoder, IntPtr pipeline);
            [DllImport(WgpuNative.LIBLARY)]
            internal static extern void wgpuRenderPassEncoderDraw(IntPtr renderPassEncoder, uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance);
        }

        IntPtr Ptr;

        internal RenderPass(IntPtr ptr)
        {
            Ptr = ptr;
        }

        public static implicit operator IntPtr(RenderPass adapter) => adapter.Ptr;
        public static explicit operator RenderPass(IntPtr ptr) => new RenderPass(ptr);

        public void SetPipeline(IntPtr pipeline)
        {
            FFI.wgpuRenderPassEncoderSetPipeline(Ptr, pipeline);
        }

        public void Draw(uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance)
        {
            FFI.wgpuRenderPassEncoderDraw(Ptr, vertexCount, instanceCount, firstVertex, firstInstance);
        }

        void EndPass()
        {
            FFI.wgpuRenderPassEncoderEndPass(Ptr);
        }

        public void Dispose()
        {
            EndPass();
        }
    }
}