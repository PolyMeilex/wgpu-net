
using System;
using System.Runtime.InteropServices;

namespace WGPU
{
    internal class DisposablePtr : IDisposable
    {
        private IntPtr Ptr;

        public void Dispose()
        {
            if (Ptr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(Ptr);
                Ptr = IntPtr.Zero;
            }
        }

        public static implicit operator IntPtr(DisposablePtr raw) => raw.Ptr;
        public static explicit operator DisposablePtr(IntPtr ptr) => new DisposablePtr
        {
            Ptr = ptr
        };

        ~DisposablePtr()
        {
            Dispose();
        }
    }
    internal class RawData<T> : IDisposable
    {
        private IDisposable[] ToDispose = new IDisposable[0];

        private T Data;

        internal RawData(T data, IDisposable[] toDispose)
        {
            Data = data;
            ToDispose = toDispose;
        }

        internal T GetData()
        {
            return Data;
        }

        internal ref T GetRef()
        {
            return ref Data;
        }

        public void Dispose()
        {
            foreach (var ptr in ToDispose)
            {
                ptr.Dispose();
            }
            ToDispose = new IDisposable[0];
        }

        public static explicit operator T(RawData<T> raw) => raw.Data;

        ~RawData()
        {
            Dispose();
        }
    }

    public static class ArrayMarshaler
    {
        public static IntPtr ArrayOfStructToPtr<T>(T[] array)
        {
            if (array.Length > 0)
            {
                var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(array[0]) * array.Length);

                for (int i = 0; i < array.Length; i++)
                {
                    var s = array[i];
                    Marshal.StructureToPtr(s, ptr + i * Marshal.SizeOf(s), false);
                }

                return ptr;
            }
            else
                return IntPtr.Zero;
        }
    }
}