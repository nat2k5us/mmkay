// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PredictionWindow.xaml.cs" company="Luminator">
//   Copyright (c) 2017
//   Luminator Technology Group
//   All Rights Reserved
// </copyright>
// <summary>
//   Interaction logic for PredictionWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WpfGtfsRealTime.Views
{
    using System.Windows;
    using System.Windows.Controls;

    using MahApps.Metro.Controls;

    using WpfGtfsRealTime.Interfaces;

    /// <summary>
    ///     Interaction logic for PredictionWindow.xaml
    /// </summary>
    public partial class PredictionWindow : UserControl
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PredictionWindow"/> class.</summary>
        public PredictionWindow()
        {
            this.InitializeComponent();
        }

        #endregion
    }
}