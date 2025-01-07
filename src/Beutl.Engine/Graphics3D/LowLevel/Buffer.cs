using SDL;

namespace Beutl.Graphics3D;

public unsafe class Buffer<T> : Buffer 
    where T : unmanaged
{
    internal Buffer(Device device, SDL_GPUBuffer* handle, BufferCreateInfo createInfo, uint elementCount)
        : base(device, handle, createInfo)
    {
        ElementCount = elementCount;
    }

    public uint ElementCount { get; }

    public int ElementSize => sizeof(T);

    public static implicit operator BufferBinding<T>(Buffer<T> buffer) => new(buffer);
}

public unsafe class Buffer : GraphicsResource
{
    private readonly BufferCreateInfo _createInfo;

    internal Buffer(Device device, SDL_GPUBuffer* handle, BufferCreateInfo createInfo) : base(device)
    {
        Handle = handle;
        _createInfo = createInfo;
    }

    public BufferUsageFlags Usage => _createInfo.Usage;

    public uint Size => _createInfo.Size;

    internal SDL_GPUBuffer* Handle { get; }

    public static Buffer<T> Create<T>(
        Device device,
        BufferUsageFlags usageFlags,
        uint elementCount) where T : unmanaged
    {
        var createInfo = new BufferCreateInfo
        {
            Usage = usageFlags,
            Size = (uint)sizeof(T) * elementCount
        };

        var nativeInfo = createInfo.ToNative();
        var handle = SDL3.SDL_CreateGPUBuffer(device.Handle, &nativeInfo);
        if (handle == null)
        {
            throw new InvalidOperationException(SDL3.SDL_GetError());
        }

        return new Buffer<T>(device, handle, createInfo, elementCount);
    }

    public static Buffer Create(
        Device device,
        in BufferCreateInfo createInfo)
    {
        var nativeInfo = createInfo.ToNative();
        var handle = SDL3.SDL_CreateGPUBuffer(device.Handle, &nativeInfo);
        if (handle == null)
        {
            throw new InvalidOperationException(SDL3.SDL_GetError());
        }

        return new Buffer(device, handle, createInfo);
    }

    public static implicit operator BufferBinding(Buffer buffer) => new(buffer);

    protected override void Dispose(bool disposing)
    {
        SDL3.SDL_ReleaseGPUBuffer(Device.Handle, Handle);
    }
}
