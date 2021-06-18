using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace WGPU
{
    public class WgpuNative
    {
        public const string LIBLARY = "./vendor/lib/libwgpu_native.so";
    }

    public class Instance
    {
        public static class Types
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct InstanceDescriptor
            {
                public IntPtr nextInChain;
            }

        }
        static class FFI
        {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void RequestAdapterCallback(IntPtr adapter, IntPtr userdata);

            [DllImport(WgpuNative.LIBLARY)]
            public static extern IntPtr wgpuCreateInstance(IntPtr descriptor);

            [DllImport(WgpuNative.LIBLARY)]
            public static extern void wgpuInstanceRequestAdapter(IntPtr instance, ref RequestAdapterOptions.FFI.Options options, RequestAdapterCallback callback, IntPtr userdata);

            [DllImport(WgpuNative.LIBLARY)]
            public static extern IntPtr wgpuInstanceCreateSurface(IntPtr instance, ref SurfaceDescriptor.FFI.Descriptor descriptor);
        }

        IntPtr Ptr;

        public Instance()
        {
            // ptr = FFI.wgpuCreateInstance(IntPtr.Zero);
            // WGPU creates it's own global instance so we don't have to
            // It may change in future
            this.Ptr = IntPtr.Zero;
        }

        public Instance(IntPtr ptr)
        {
            this.Ptr = ptr;
        }


        public static implicit operator IntPtr(Instance instance) => instance.Ptr;
        public static explicit operator Instance(IntPtr ptr) => new Instance(ptr);


        public Task<Adapter> RequestAdapter(RequestAdapterOptions options)
        {
            var t = new TaskCompletionSource<Adapter>();

            using (var data = options.ToRaw())
            {
                FFI.wgpuInstanceRequestAdapter(Ptr, ref data.GetRef(), (adapter, _) => t.SetResult((Adapter)adapter), IntPtr.Zero);
            }

            return t.Task;
        }

        public Surface CreateSurface(SurfaceDescriptor descriptor)
        {
            using (var data = descriptor.ToRaw())
            {
                return (Surface)(FFI.wgpuInstanceCreateSurface(Ptr, ref data.GetRef()));
            }
        }

    }

}