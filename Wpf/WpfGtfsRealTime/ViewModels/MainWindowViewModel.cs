// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="Luminator">
//   Copyright (c) 2017
//   Luminator Technology Group
//   All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace WpfGtfsRealTime.ViewModels
{
    using System;

    using NLog;

    using Prism.Mvvm;

    /// <summary>The main window view model.</summary>
    public class MainWindowViewModel : BindableBase, IDisposable
    {
        #region Static Fields

        /// <summary>The logger.</summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        public MainWindowViewModel()
        {
            this.PredictionWindowViewModel = new PredictionWindowViewModel();
            this.ServiceAlertsViewModel = new ServiceAlertsViewModel();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the prediction window view model.</summary>
        public PredictionWindowViewModel PredictionWindowViewModel { get; set; }

        /// <summary>Gets or sets the service alerts view model.</summary>
        public ServiceAlertsViewModel ServiceAlertsViewModel { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
        }

        #endregion
    }
}