﻿using System;

namespace RealTimeGraphX
{
    /// <summary>
    /// Represents a graph data series.
    /// </summary>
    /// <seealso cref="IGraphComponent" />
    public interface IGraphDataSeries : IGraphComponent
    {
        /// <summary>
        /// Gets or sets the series name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the stroke thickness.
        /// </summary>
        float StrokeThickness { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to fill the series.
        /// </summary>
        bool UseFill { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this series should be visible.
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// Gets the current value.
        /// </summary>
        object CurrentValue { get; set; }
    }
}
