using SDL;

namespace Beutl.Graphics3D;

public readonly record struct BufferBinding(Buffer Buffer, uint Offset = 0)
{
    internal unsafe SDL_GPUBufferBinding ToNative()
    {
        return new SDL_GPUBufferBinding { buffer = Buffer != null ? Buffer.Handle : null, offset = Offset };
    }
}

public readonly record struct BufferBinding<T>(Buffer<T> Buffer, uint Offset = 0)
    where T : unmanaged
{
    internal unsafe SDL_GPUBufferBinding ToNative()
    {
        return new SDL_GPUBufferBinding { buffer = Buffer != null ? Buffer.Handle : null, offset = Offset };
    }
}
