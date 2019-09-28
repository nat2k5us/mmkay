// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryUsagePlotViewModel.cs" company="Luminator">
//   Copyright (c) 2017
//   Luminator Technology Group
//   All Rights Reserved
// </copyright>
// <summary>
//   Defines the MemoryUsagePlotViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WpfGtfsRealTime.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using OxyPlot;

    /// <summary>The memory usage plot view model.</summary>
    public class RefreshPlotViewModel
    {
        /// <summary>Gets the title.</summary>
        public string Title { get; private set; }

        /// <summary>Gets or sets the points.</summary>
        public ObservableCollection<DataPoint> Points { get; set; }
       
    }

}
