// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Luminator">
//   Copyright (c) 2017
//   Luminator Technology Group
//   All Rights Reserved
// </copyright>
// <summary>
//   Interaction logic for App.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace WpfGtfsRealTime
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Methods

        /// <summary>The on startup.</summary>
        /// <param name="e">The e.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            var bs = new Bootstrapper();
            bs.Run();
        }

        #endregion
    }
}