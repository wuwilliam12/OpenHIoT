// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSetValue.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The externally used Sparkplug B data set value class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.VersionB.Data;

/// <summary>
/// The externally used Sparkplug B data set value class.
/// </summary>
public sealed class DataSetValue : ValueBaseVersionB
{
    /// <summary>
    /// Initializes the DataSetValue
    /// </summary>
    public DataSetValue()
    {
    }

    /// <summary>
    /// Initializes the DataSetValue with the given value with the given type
    /// </summary>
    public DataSetValue(VersionBDataTypeEnum type, object? value)
    {
        this.SetValue(type, value);
    }
}
