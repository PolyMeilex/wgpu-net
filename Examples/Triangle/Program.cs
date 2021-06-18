using GLFW;

using System;

using System.Threading.Tasks;

using Serilog;

using WGPU;
using System.Runtime.InteropServices;

namespace Triangle
{
    class Renderer
    {
        Surface Surface;
        Device Device;
        SwapChain SwapChain;
        IntPtr Pipeline;

        public Renderer(Surface surface, Device device)
        {
            Surface = surface;
            Device = device;

            var shaderSrc = System.IO.File.ReadAllText("./Examples/Triangle/shader.wgsl");
            var shader = Device.CreateShaderModule(new ShaderModuleDescriptor(shaderSrc));

            var layout = Device.CreatePipelineLayout(new PipelineLayoutDescriptor());

            var pipelineDescriptor = new RenderPipelineDescriptor
            {
                Layout = layout,
                Vertex = new RenderPipelineDescriptor.FFI.VertexState
                {
                    Module = shader,
                    EntryPoint = "vs_main",
                    BufferCount = 0,
                    Buffers = IntPtr.Zero
                },
                Primitive = new PrimitiveState
                {
                    Topology = PrimitiveTopology.TriangleList,
                    StripIndexFormat = IndexFormat.Undefined,
                    FrontFace = FrontFace.CCW,
                    CullMode = CullMode.None
                },
                Multisample = new MultisampleState
                {
                    Count = 1,
                    Mask = uint.MaxValue,
                    AlphaToCoverageEnabled = 0
                },
                Fragment = new FragmentState
                {
                    Module = shader,
                    EntryPoint = "fs_main",
                    Targets = new[] {
                    new ColorTargetState
                    {
                        Format = WGPU.TextureFormat.BGRA8UnormSrgb,
                        Blend = new BlendState
                        {
                            Color = new BlendComponent
                            {
                                SrcFactor = BlendFactor.One,
                                DstFactor = BlendFactor.Zero,
                                Operation = BlendOperation.Add
                            },
                            Alpha = new BlendComponent
                            {
                                SrcFactor = BlendFactor.One,
                                DstFactor = BlendFactor.Zero,
                                Operation = BlendOperation.Add
                            },
                        },
                        WriteMask = ColorWriteMask.All,
                    }
                }
                },
                DepthStencil = IntPtr.Zero
            };

            Pipeline = device.CreateRenderPipeline(pipelineDescriptor);
        }

        public void InitSwapChain((int, int) size)
        {
            var descriptor = new WGPU.SwapChainDescriptor
            {
                Usage = WGPU.TextureUsage.RenderAttachment,
                Format = WGPU.TextureFormat.BGRA8UnormSrgb,
                Width = (uint)size.Item1,
                Height = (uint)size.Item2,
                PresentMode = WGPU.PresentMode.Fifo
            };

            SwapChain = Device.CreateSwapChain(Surface, descriptor);
        }
        public void Draw()
        {
            using (var texture = SwapChain.GetCurrentTextureView())
            {

                var encoder = Device.CreateCommandEncoder();


                var passDesc = new RenderPassDescriptor
                {
                    ColorAttachments = new[] {
                        new RenderPassColorAttachment {
                            Attachment = texture,
                            ResolveTarget = IntPtr.Zero,
                            LoadOp = WGPU.LoadOp.Clear,
                            StoreOp = WGPU.StoreOp.Store,
                            ClearColor = new Color
                            {
                                R = 0.0,
                                G = 0.0,
                                B = 0.0,
                                A = 1.0
                            }
                        }
                    },
                };

                using (var renderPass = encoder.BeginRenderPass(passDesc))
                {
                    renderPass.SetPipeline(Pipeline);
                    renderPass.Draw(3, 1, 0, 0);
                }

                Device.Queue.Submit(new[] { encoder.Finish() });
            }
        }
    }

    class Program
    {
        static void Loop(Window window, Renderer renderer)
        {
            (int, int) size = (0, 0);
            Glfw.GetWindowSize(window, out size.Item1, out size.Item2);
            renderer.InitSwapChain(size);

            while (!Glfw.WindowShouldClose(window))
            {
                (int, int) newSize = (0, 0);
                Glfw.GetWindowSize(window, out newSize.Item1, out newSize.Item2);

                if (newSize.Item1 != size.Item1 || newSize.Item2 != size.Item2)
                {
                    // Resize swapchain
                    size = newSize;
                    renderer.InitSwapChain(size);
                }

                renderer.Draw();

                Glfw.PollEvents();
            }
        }

        static void InitWgpuLogger()
        {
            WGPULog.SetLevel(WGPULog.Level.Debug);
            WGPULog.SetCallback((level, msg) =>
            {
                var l = level switch
                {
                    WGPULog.Level.Off => Serilog.Events.LogEventLevel.Verbose,
                    WGPULog.Level.Error => Serilog.Events.LogEventLevel.Error,
                    WGPULog.Level.Warn => Serilog.Events.LogEventLevel.Warning,
                    WGPULog.Level.Info => Serilog.Events.LogEventLevel.Information,
                    WGPULog.Level.Debug => Serilog.Events.LogEventLevel.Debug,
                    WGPULog.Level.Trace => Serilog.Events.LogEventLevel.Verbose,
                    _ => Serilog.Events.LogEventLevel.Verbose,
                };

                Log.Write(l, msg);
            });
        }

        async static Task Main(string[] args)
        {
            using var log = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            Log.Logger = log;
            InitWgpuLogger();


            Glfw.WindowHint(Hint.ClientApi, ClientApi.None);
            var window = Glfw.CreateWindow(800, 600, "MyWindowTitle", Monitor.None, Window.None);


            WGPU.SurfaceDescriptor surfaceDescriptor = null;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var x11Display = GLFW.Native.GetX11Display();
                var x11Window = GLFW.Native.GetX11Window(window);
                surfaceDescriptor = WGPU.SurfaceDescriptor.FromX11(x11Display, x11Window);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var hwnd = GLFW.Native.GetWin32Window(window);
                surfaceDescriptor = WGPU.SurfaceDescriptor.FromWindowsHWND(IntPtr.Zero, hwnd);
            }

            var instance = new WGPU.Instance();
            var surface = instance.CreateSurface(surfaceDescriptor);

            var adapterOptions = new WGPU.RequestAdapterOptions
            {
                CompatibleSurface = surface,
                Backend = BackendType.Vulkan
            };

            var adapter = await instance.RequestAdapter(adapterOptions);

            var deviceDescriptor = new WGPU.DeviceDescriptor
            {
                MaxBindGroups = 1
            };
            var device = await adapter.RequestDevice(deviceDescriptor);

            var renderer = new Renderer(surface, device);

            Loop(window, renderer);
        }
    }
}
