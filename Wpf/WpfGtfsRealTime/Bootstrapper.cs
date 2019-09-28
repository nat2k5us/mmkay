// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bootstrapper.cs" company="Luminator">
//   Copyright (c) 2017
//   Luminator Technology Group
//   All Rights Reserved
// </copyright>
// <summary>
//   Defines the Bootstrapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WpfGtfsRealTime
{
    using System.Windows;

    using Microsoft.Practices.Unity;

    using Prism.Unity;

    using WpfGtfsRealTime.Views;

    /// <summary>The bootstrapper.</summary>
    public class Bootstrapper : UnityBootstrapper
    {
        #region Methods

        /// <summary>The create shell.</summary>
        /// <returns>The <see cref="DependencyObject"/>.</returns>
        protected override DependencyObject CreateShell()
        {
            return this.Container.Resolve<MainWindow>();
        }

        /// <summary>The initialize shell.</summary>
        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        #endregion
    }
}