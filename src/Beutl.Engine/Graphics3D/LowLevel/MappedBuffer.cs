using System.Runtime.InteropServices;
using SDL;

namespace Beutl.Graphics3D;

public unsafe class MappedBuffer : IDisposable
{
    internal readonly TransferBuffer _transferBuffer;

    internal MappedBuffer(IntPtr ptr, TransferBuffer transferBuffer)
    {
        Ptr = ptr;
        _transferBuffer = transferBuffer;
    }

    public IntPtr Ptr { get; }

    public Span<byte> Span => new(Ptr.ToPointer(), (int)_transferBuffer.Size);

    public bool Unmapped { get; private set; }

    public Span<T> AsSpan<T>() where T : unmanaged
    {
        return MemoryMarshal.Cast<byte, T>(Span);
    }

    public void Dispose()
    {
        if (Unmapped) return;
        SDL3.SDL_UnmapGPUTransferBuffer(_transferBuffer.Device.Handle, _transferBuffer.Handle);
        Unmapped = true;
    }
}

public unsafe class MappedBuffer<T> : MappedBuffer
    where T : unmanaged
{
    internal MappedBuffer(IntPtr ptr, TransferBuffer<T> transferBuffer)
        : base(ptr, transferBuffer)
    {
    }

    public new Span<T> Span => new(Ptr.ToPointer(), (int)((TransferBuffer<T>)_transferBuffer).ElementCount);

    public new Span<T2> AsSpan<T2>()
        where T2 : unmanaged
    {
        return MemoryMarshal.Cast<T, T2>(Span);
    }
}