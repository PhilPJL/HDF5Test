using System.Runtime.CompilerServices;

namespace HDF5Api
{
    public static class H5ThrowExtensions
    {
        public static void ThrowIfNotValid(this H5Handle handle, [CallerMemberName] string methodName = null)
        {
            ThrowIfNotValid(handle.Handle, methodName ?? handle.GetType().Name);
        }

        public static void ThrowIfNotValid(this Handle handle, [CallerMemberName] string methodName = null)
        {
            if (handle <= 0)
            {
                if (string.IsNullOrWhiteSpace(methodName))
                {
                    throw new HDF5Exception($"Bad handle {handle}.");
                }
                else
                {
                    throw new HDF5Exception($"Bad handle {handle} when {methodName}.");
                }
            }
        }

        public static void ThrowIfError(this int err, [CallerMemberName] string methodName = null)
        {
            if (err < 0)
            {
                if (string.IsNullOrWhiteSpace(methodName))
                {
                    throw new HDF5Exception($"Error: {err}.");
                }
                else
                {
                    throw new HDF5Exception($"Error {err} calling: {methodName}.");
                }
            }
        }
    }
}
