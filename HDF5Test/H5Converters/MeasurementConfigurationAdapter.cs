using HDF.PInvoke;
using HDF5Api;
using PulseData.TvlSystem;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    /// <summary>
    /// A type converter for <see cref="MeasurementConfiguration"/>.
    /// </summary>
    public sealed class MeasurementConfigurationAdapter : H5TypeAdapter<MeasurementConfiguration, MeasurementConfigurationAdapter.SMeasurementConfiguration>
    {
        private const int NameLength = 50;
        private const int DescriptionLength = 255;
        private const int ModuleNameLength = 255;
        private const int ScannerNameLength = 255;
        private const int ScannerConfigurationLength = 8000;
        private const int SessionKeyLength = 32;

        private MeasurementConfigurationAdapter() { }

        protected override SMeasurementConfiguration Convert(MeasurementConfiguration source)
        {
            var result = new SMeasurementConfiguration
            {
                Id = source.Id,
                Timestamp = source.Timestamp.ToOADate()
            };

            unsafe
            {
                CopyString(source.Name, result.Name, NameLength);
                CopyString(source.Description, result.Description, DescriptionLength);
                CopyString(source.ModuleName, result.ModuleName, ModuleNameLength);
                CopyString(source.ScannerName, result.ScannerName, ScannerNameLength);
                CopyString(source.ScannerConfiguration, result.ScannerConfiguration, ScannerConfigurationLength);
                CopyString(source.SessionKey, result.SessionKey, SessionKeyLength);
            }

            return result;
        }

        public override H5Type GetH5Type()
        {
            using var nameStringType = H5Type.CreateFixedLengthStringType(NameLength);
            using var descriptionStringType = H5Type.CreateFixedLengthStringType(DescriptionLength);
            using var moduleNameStringType = H5Type.CreateFixedLengthStringType(ModuleNameLength);
            using var scannerNameStringType = H5Type.CreateFixedLengthStringType(ScannerNameLength);
            using var scannerConfigurationStringType = H5Type.CreateFixedLengthStringType(ScannerConfigurationLength);
            using var sessionKeyStringType = H5Type.CreateFixedLengthStringType(SessionKeyLength);

            return H5Type
                .CreateCompoundType<SMeasurementConfiguration>()
                .Insert<SMeasurementConfiguration>(nameof(SMeasurementConfiguration.Id), H5T.NATIVE_INT64)
                .Insert<SMeasurementConfiguration>(nameof(SMeasurementConfiguration.Timestamp), H5T.NATIVE_DOUBLE)
                .Insert<SMeasurementConfiguration>(nameof(SMeasurementConfiguration.Name), nameStringType)
                .Insert<SMeasurementConfiguration>(nameof(SMeasurementConfiguration.Description), descriptionStringType)
                .Insert<SMeasurementConfiguration>(nameof(SMeasurementConfiguration.ModuleName), moduleNameStringType)
                .Insert<SMeasurementConfiguration>(nameof(SMeasurementConfiguration.ScannerName), scannerNameStringType)
                .Insert<SMeasurementConfiguration>(nameof(SMeasurementConfiguration.ScannerConfiguration), scannerConfigurationStringType)
                .Insert<SMeasurementConfiguration>(nameof(SMeasurementConfiguration.SessionKey), sessionKeyStringType);
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct SMeasurementConfiguration
        {
            public long Id;
            public double Timestamp;
            public fixed byte Name[NameLength];
            public fixed byte Description[DescriptionLength];
            public fixed byte ModuleName[ModuleNameLength];
            public fixed byte ScannerName[ScannerNameLength];
            public fixed byte ScannerConfiguration[ScannerConfigurationLength];
            public fixed byte SessionKey[SessionKeyLength];
        }

        public static IH5TypeAdapter<MeasurementConfiguration> Default { get; } = new MeasurementConfigurationAdapter();
    }
}
