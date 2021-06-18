using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace WGPU
{
    public class Queue
    {
        class FFI
        {
            [DllImport(WgpuNative.LIBLARY)]
            internal static extern void wgpuQueueSubmit(IntPtr queue, uint commandCount, IntPtr[] commands);
        }

        IntPtr Ptr;

        Queue(IntPtr ptr)
        {
            Ptr = ptr;
        }

        public static implicit operator IntPtr(Queue adapter) => adapter.Ptr;
        public static explicit operator Queue(IntPtr ptr) => new Queue(ptr);

        public void Submit(IntPtr[] commands)
        {
            FFI.wgpuQueueSubmit(Ptr, (uint)commands.Length, commands);
        }
    }
}