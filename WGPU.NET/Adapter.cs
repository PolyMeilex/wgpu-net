using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace WGPU
{
    public class RequestAdapterOptions
    {
        internal class FFI
        {
            [StructLayout(LayoutKind.Sequential)]
            internal struct Extras
            {
                internal ChainedStruct Chain;
                internal BackendType Backend;
            }


            [StructLayout(LayoutKind.Sequential)]
            internal struct Options
            {
                internal IntPtr NextInChain;
                internal IntPtr CompatibleSurface;
            }

        }

        public Surface CompatibleSurface;
        public BackendType Backend;

        internal RawData<FFI.Options> ToRaw()
        {
            var extras = new FFI.Extras
            {
                Chain = new ChainedStruct
                {
                    SType = SType.AdapterExtras
                },
                Backend = Backend
            };

            var nextInChain = Marshal.AllocHGlobal(Marshal.SizeOf(extras));
            Marshal.StructureToPtr(extras, nextInChain, false);

            var inner = new FFI.Options
            {
                NextInChain = nextInChain,
                CompatibleSurface = CompatibleSurface
            };

            return new(inner, new[] { (DisposablePtr)nextInChain });
        }
    }

    public enum AdapterType : uint
    {
        DiscreteGPU = 0x0,
        IntegratedGPU = 0x1,
        CPU = 0x2,
        Unknown = 0x3,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AdapterProperties
    {
        public IntPtr NextInChain;
        public uint DeviceID;
        public uint VendorID;
        public IntPtr Name;
        public IntPtr DriverDescription;
        public AdapterType AdapterType;
        public BackendType BackendType;
    }

    public class Adapter
    {
        class FFI
        {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate void RequestDeviceCallback(IntPtr device, IntPtr userdata);

            [DllImport(WgpuNative.LIBLARY, CallingConvention = CallingConvention.Cdecl)]
            public static extern void wgpuAdapterRequestDevice(IntPtr adapter, ref DeviceDescriptor.FFI.Descriptor descriptor, RequestDeviceCallback callback, IntPtr userdata);

            [DllImport(WgpuNative.LIBLARY, CallingConvention = CallingConvention.Cdecl)]
            public static extern void wgpuAdapterGetProperties(IntPtr adapter, ref AdapterProperties properties);
        }

        IntPtr Ptr;

        Adapter(IntPtr ptr)
        {
            Ptr = ptr;
        }

        public static implicit operator IntPtr(Adapter adapter) => adapter.Ptr;
        public static explicit operator Adapter(IntPtr ptr) => new Adapter(ptr);

        public Task<Device> RequestDevice(DeviceDescriptor descriptor)
        {
            var t = new TaskCompletionSource<Device>();

            using (var data = descriptor.ToRaw())
            {
                FFI.wgpuAdapterRequestDevice(Ptr, ref data.GetRef(), (device, _) => t.SetResult((Device)(device)), IntPtr.Zero);
            }

            return t.Task;
        }

        public AdapterProperties GetProperties()
        {
            var properties = new AdapterProperties();
            FFI.wgpuAdapterGetProperties(Ptr, ref properties);
            return properties;
        }
    }
}