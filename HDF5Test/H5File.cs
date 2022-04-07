﻿using HDF.PInvoke;
using Handle = System.Int64;

namespace HDF5Test
{
    public class H5File : H5FileHandle
    {
        private H5File(Handle handle) : base(handle) { }

        /// <summary>
        /// Create a DataSet in this file
        /// </summary>
        public H5DataSet CreateDataSet(string name, H5TypeHandle typeId, H5SpaceHandle spaceId, H5PropertyListHandle propertyListId)
        {
            return H5DataSet.Create(this, name, typeId, spaceId, propertyListId);
        }

        /// <summary>
        /// Create a Group in this file
        /// </summary>
        public H5Group CreateGroup(string name)
        {
            return H5Group.Create(this, name);
        }

        #region Factory methods
        /// <summary>
        /// Create a file
        /// </summary>
        public static H5File Create(string name, uint flags = H5F.ACC_TRUNC)
        {
            // TODO: open/create etc
            Handle h = H5F.create(name, flags);
            AssertHandle(h);
            return new H5File(h);
        }
        #endregion
    }
}
