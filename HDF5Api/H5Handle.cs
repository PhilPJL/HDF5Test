global using HDF.PInvoke;
global using Handle = System.Int64;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HDF5Api
{
    public class H5AttributeHandle : H5Handle
    {
        public H5AttributeHandle(Handle handle) : base(handle, H5A.close) { }
    }

    public class H5PropertyListHandle : H5Handle
    {
        public H5PropertyListHandle(Handle handle) : base(handle, H5P.close) { }
    }

    public class H5TypeHandle : H5Handle
    {
        public H5TypeHandle(Handle handle) : base(handle, H5T.close) { }
        private H5TypeHandle(Handle handle, Func<Handle, int> closer) : base(handle, closer) { }

        public static H5TypeHandle WrapNative(Handle handle)
        {
            return new H5TypeHandle(handle, NullCloser);
        }
    }

    public class H5FileHandle : H5LocationHandle
    {
        public H5FileHandle(Handle handle) : base(handle, H5F.close) { }
    }

    public class H5GroupHandle : H5LocationHandle
    {
        public H5GroupHandle(Handle handle) : base(handle, H5G.close) { }
    }

    public abstract class H5LocationHandle : H5Handle
    {
        protected H5LocationHandle(Handle handle, Func<Handle, int> closer) : base(handle, closer) { }
    }

    public class H5DataSetHandle : H5Handle
    {
        public H5DataSetHandle(Handle handle) : base(handle, H5D.close) { }
    }

    public class H5SpaceHandle : H5Handle
    {
        public H5SpaceHandle(Handle handle) : base(handle, H5S.close) { }
    }

    /// <summary>
    /// Base class for H5 handles.
    /// </summary>
    /// <remarks>
    /// Used to properly dispose H5 handles by calling the appropriate H5'X'.Close() method.
    /// </remarks>
    public abstract class H5Handle : Disposable
    {
#if DEBUG
        public static Dictionary<Handle, string> Handles { get; private set; } = new Dictionary<Handle, string>();
#endif

        // Null-op for wrapped native handles
        protected static internal int NullCloser(Handle _)
        {
            Debug.WriteLine($"*not* Closing native handle {_}");
            return 0;
        }

        /// <summary>
        /// Func used to close the handle
        /// </summary>
        private Func<Handle, int> CloseHandle { get; }

        /// <summary>
        /// The int32/int64 handle returned by the H5 API
        /// </summary>
        public Handle Handle { get; private set; }

        /// <summary>
        /// The invalid handle value (there may be a value for this in P.Invoke)
        /// </summary>
        public const Handle InvalidHandleValue = -1;

        internal bool IsDisposed => Handle <= 0;

        protected H5Handle(Handle handle, Func<Handle, int> closer)
        {
            Debug.WriteLine($"Opened handle {GetType().Name}: {handle}");

            handle.ThrowIfNotValid("H5Handle-ctor");
            Handle = handle;
            CloseHandle = closer;

#if DEBUG
            lock (this)
            {
                Handles.Add(handle, Environment.StackTrace);
            }
#endif
        }

        public static implicit operator Handle(H5Handle h)
        {
            return h.Handle;
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                Debug.WriteLine($"Closing handle {GetType().Name}: {Handle}");

                lock (this)
                {
                    var err = CloseHandle(Handle);

#if DEBUG
                    Handles.Remove(Handle);
#endif

                    Handle = InvalidHandleValue;
                    err.ThrowIfError($"Closing handle for {GetType().Name}");
                }
            }
        }
    }
}
