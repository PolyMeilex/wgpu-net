using System;
using System.Runtime.InteropServices;

namespace WGPU
{
    public class ShaderModuleDescriptor
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

            [StructLayout(LayoutKind.Sequential)]
            internal struct WGSLDescriptor
            {
                internal ChainedStruct Chain;

                [MarshalAs(UnmanagedType.LPStr)]
                internal String Source;

                internal WGSLDescriptor(String source)
                {
                    Chain = new ChainedStruct
                    {
                        Next = IntPtr.Zero,
                        SType = SType.ShaderModuleWGSLDescriptor
                    };
                    Source = source;
                }
            }
        }


        FFI.WGSLDescriptor Descriptor;

        public String Label;

        public ShaderModuleDescriptor(String source)
        {
            Descriptor = new FFI.WGSLDescriptor(source);
        }

        internal RawData<FFI.Descriptor> ToRaw()
        {
            var nextInChain = Marshal.AllocHGlobal(Marshal.SizeOf(Descriptor));
            Marshal.StructureToPtr(Descriptor, nextInChain, false);

            var rootDescriptor = new FFI.Descriptor
            {
                Label = Label,
                NextInChain = nextInChain,
            };

            return new RawData<FFI.Descriptor>(rootDescriptor, new[] { (DisposablePtr)nextInChain });
        }
    }



}