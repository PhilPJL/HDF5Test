using System;
using System.Runtime.InteropServices;

namespace HDF5Api.Disposables
{
    public class PinnedObject : Disposable
    {
        public GCHandle Pinned { get; private set; }

        public PinnedObject(object objectToPin)
        {
            Pinned = GCHandle.Alloc(objectToPin, GCHandleType.Pinned);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Pinned.IsAllocated)
            {
                Pinned.Free();
            }
        }

        public static unsafe implicit operator void*(PinnedObject pinned)
        {
            return pinned.Pinned.AddrOfPinnedObject().ToPointer();
        }

        public static implicit operator IntPtr(PinnedObject pinned)
        {
            return pinned.Pinned.AddrOfPinnedObject();
        }

        public static implicit operator GCHandle(PinnedObject pinned)
        {
            return pinned.Pinned;
        }
    }
}
