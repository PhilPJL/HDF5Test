using HDF.PInvoke;
using HDF5Api;
using PulseData.TvlAlt;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    /// <summary>
    /// A type converter for <see cref="RawRecord"/>.
    /// </summary>
    public sealed class RawRecordAdapter : H5TypeAdapter<RawRecord, RawRecordAdapter.SRawRecord>
    {
        private RawRecordAdapter() { }

        protected override SRawRecord Convert(RawRecord source)
        {
            return new SRawRecord
            {
                Id = source.Id,
                MeasurementId = source.MeasurementId,
                Timestamp = source.Timestamp.ToOADate(),
                Thickness = source.Thickness ?? double.NaN,
                IntervalId = source.IntervalId ?? 0,
                ProfileDeviation = source.ProfileDeviation ?? double.NaN,
                ProfileHeight = source.ProfileHeight ?? double.NaN,
                ZPosition = source.ZPosition ?? double.NaN,
                PulseOffset = source.PulseOffset ?? double.NaN,
                ReferenceOffset = source.ReferenceOffset ?? double.NaN
            };
        }

        public override H5Type GetH5Type()
        {
            return H5Type
                .CreateCompoundType<SRawRecord>()
                .Insert<SRawRecord>(nameof(SRawRecord.Id), H5T.NATIVE_INT64)
                .Insert<SRawRecord>(nameof(SRawRecord.Timestamp), H5T.NATIVE_DOUBLE)
                .Insert<SRawRecord>(nameof(SRawRecord.MeasurementId), H5T.NATIVE_INT32)
                .Insert<SRawRecord>(nameof(SRawRecord.Thickness), H5T.NATIVE_DOUBLE)
                .Insert<SRawRecord>(nameof(SRawRecord.ProfileDeviation), H5T.NATIVE_DOUBLE)
                .Insert<SRawRecord>(nameof(SRawRecord.ProfileHeight), H5T.NATIVE_DOUBLE)
                .Insert<SRawRecord>(nameof(SRawRecord.ZPosition), H5T.NATIVE_DOUBLE)
                .Insert<SRawRecord>(nameof(SRawRecord.IntervalId), H5T.NATIVE_INT64)
                .Insert<SRawRecord>(nameof(SRawRecord.PulseOffset), H5T.NATIVE_DOUBLE)
                .Insert<SRawRecord>(nameof(SRawRecord.ReferenceOffset), H5T.NATIVE_DOUBLE);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SRawRecord
        {
            public long Id;
            public double Timestamp;
            public int MeasurementId;
            public double Thickness;
            public double ProfileDeviation;
            public double ProfileHeight;
            public double ZPosition;
            public long IntervalId;
            public double PulseOffset;
            public double ReferenceOffset;
        }

        public static IH5TypeAdapter<RawRecord> Default { get; } = new RawRecordAdapter();
    }
}
