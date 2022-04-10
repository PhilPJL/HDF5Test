﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HDF5Api
{
    public abstract class H5TypeAdapterBase
    {
        protected static readonly ASCIIEncoding Ascii = new();

        // Move to unsafe helper class?
        protected static unsafe void CopyString(string source, byte* destination, int destinationSizeInBytes)
        {
            byte[] sourceBytes = Ascii.GetBytes(source);

            var msg = $"Profile: The provided string '{source}' has length {source.Length} which exceeds the maximum destination length of {destinationSizeInBytes}.";

            if (source.Length > destinationSizeInBytes)
            {
                throw new InvalidOperationException(msg);
            }

            Buffer.MemoryCopy(Marshal.UnsafeAddrOfPinnedArrayElement(sourceBytes, 0).ToPointer(), destination, destinationSizeInBytes, source.Length);
        }
    }

    public abstract class H5TypeAdapter<TInput, TOutput> : H5TypeAdapterBase, IH5TypeAdapter<TInput>
    {
        protected abstract TOutput Convert(TInput source);

        public abstract H5Type GetH5Type();

        public void WriteChunk(Action<IntPtr> write, IEnumerable<TInput> inputRecords)
        {
            var records = inputRecords.Select(Convert).ToArray();

            GCHandle pinnedBuffer = GCHandle.Alloc(records, GCHandleType.Pinned);

            try
            {
                write(pinnedBuffer.AddrOfPinnedObject());
            }
            finally
            {
                pinnedBuffer.Free();
            }
        }
    }
}