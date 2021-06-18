using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WGPU
{
    public class DeviceDescriptor
    {
        internal class FFI
        {
            [StructLayout(LayoutKind.Sequential)]
            internal struct Extras
            {
                internal ChainedStruct Chain;
                internal uint MaxTextureDimension1D;
                internal uint MaxTextureDimension2D;
                internal uint MaxTextureDimension3D;
                internal uint MaxTextureArrayLayers;
                internal uint MaxBindGroups;
                internal uint MaxDynamicStorageBuffersPerPipelineLayout;
                internal uint MaxStorageBuffersPerShaderStage;
                internal uint MaxStorageBufferBindingSize;
                internal NativeFeature NativeFeatures;

                [MarshalAs(UnmanagedType.LPStr)]
                internal String Label;

                [MarshalAs(UnmanagedType.LPStr)]
                internal String TracePath;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct Descriptor
            {
                internal IntPtr NextInChain;
            }
        }

        public uint MaxTextureDimension1D;
        public uint MaxTextureDimension2D;
        public uint MaxTextureDimension3D;
        public uint MaxBindGroups;
        public uint MaxDynamicStorageBuffersPerPipelineLayout;
        public uint MaxStorageBuffersPerShaderStage;
        public uint MaxStorageBufferBindingSize;
        public String Label;
        public String TracePath;

        internal RawData<FFI.Descriptor> ToRaw()
        {
            var extras = new FFI.Extras
            {
                Chain = new WGPU.ChainedStruct
                {
                    SType = WGPU.SType.DeviceExtras,
                },
                MaxTextureDimension1D = MaxTextureDimension1D,
                MaxTextureDimension2D = MaxTextureDimension2D,
                MaxTextureDimension3D = MaxTextureDimension3D,
                MaxBindGroups = MaxBindGroups,
                MaxDynamicStorageBuffersPerPipelineLayout = MaxDynamicStorageBuffersPerPipelineLayout,
                MaxStorageBuffersPerShaderStage = MaxStorageBuffersPerShaderStage,
                MaxStorageBufferBindingSize = MaxStorageBufferBindingSize,
                Label = Label,
                TracePath = TracePath
            };


            var nextInChain = Marshal.AllocHGlobal(Marshal.SizeOf(extras));
            Marshal.StructureToPtr(extras, nextInChain, false);

            var inner = new FFI.Descriptor
            {
                NextInChain = nextInChain
            };

            return new RawData<FFI.Descriptor>(inner, new[] { (DisposablePtr)nextInChain });
        }
    }
    public class Device
    {
        class FFI
        {
            [DllImport(WgpuNative.LIBLARY, CallingConvention = CallingConvention.Cdecl)]
            internal static extern IntPtr wgpuDeviceCreateSwapChain(IntPtr device, IntPtr surface, ref SwapChainDescriptor.Raw.Descriptor descriptor);

            [DllImport(WgpuNative.LIBLARY, CallingConvention = CallingConvention.Cdecl)]
            internal static extern IntPtr wgpuDeviceCreateCommandEncoder(IntPtr device, ref CommandEncoderDescriptor.FFI.Descriptor descriptor);

            [DllImport(WgpuNative.LIBLARY, CallingConvention = CallingConvention.Cdecl)]
            internal static extern IntPtr wgpuDeviceGetQueue(IntPtr device);

            [DllImport(WgpuNative.LIBLARY, CallingConvention = CallingConvention.Cdecl)]
            internal static extern IntPtr wgpuDeviceCreateShaderModule(IntPtr device, ref ShaderModuleDescriptor.FFI.Descriptor descriptor);

            [DllImport(WgpuNative.LIBLARY, CallingConvention = CallingConvention.Cdecl)]
            internal static extern IntPtr wgpuDeviceCreatePipelineLayout(IntPtr device, ref PipelineLayoutDescriptor.FFI.Descriptor descriptor);

            [DllImport(WgpuNative.LIBLARY, CallingConvention = CallingConvention.Cdecl)]
            internal static extern IntPtr wgpuDeviceCreateRenderPipeline(IntPtr device, ref RenderPipelineDescriptor.FFI.Descriptor descriptor);
        }

        IntPtr Ptr;

        Device(IntPtr ptr)
        {
            Ptr = ptr;
        }

        public static implicit operator IntPtr(Device adapter) => adapter.Ptr;
        public static explicit operator Device(IntPtr ptr) => new Device(ptr);

        public SwapChain CreateSwapChain(Surface surface, SwapChainDescriptor descriptor)
        {
            return (SwapChain)(FFI.wgpuDeviceCreateSwapChain(Ptr, surface, ref descriptor.ToRaw()));
        }

        public CommandEncoder CreateCommandEncoder(CommandEncoderDescriptor descriptor)
        {
            var data = descriptor.ToRaw();
            return (CommandEncoder)(FFI.wgpuDeviceCreateCommandEncoder(Ptr, ref data));
        }

        public CommandEncoder CreateCommandEncoder()
        {
            var descriptor = new CommandEncoderDescriptor();
            var data = descriptor.ToRaw();
            return (CommandEncoder)(FFI.wgpuDeviceCreateCommandEncoder(Ptr, ref data));
        }

        public IntPtr CreateShaderModule(ShaderModuleDescriptor descriptor)
        {
            using (var data = descriptor.ToRaw())
            {
                return FFI.wgpuDeviceCreateShaderModule(Ptr, ref data.GetRef());
            }
        }

        public PipelineLayout CreatePipelineLayout(PipelineLayoutDescriptor descriptor)
        {
            using (var data = descriptor.ToRaw())
            {
                return (PipelineLayout)FFI.wgpuDeviceCreatePipelineLayout(Ptr, ref data.GetRef());
            }
        }

        public IntPtr CreateRenderPipeline(RenderPipelineDescriptor descriptor)
        {
            using (var data = descriptor.ToRaw())
            {
                return FFI.wgpuDeviceCreateRenderPipeline(Ptr, ref data.GetRef());
            }
        }

        public Queue Queue
        {
            get
            {
                return (Queue)FFI.wgpuDeviceGetQueue(Ptr);
            }
        }
    }
}