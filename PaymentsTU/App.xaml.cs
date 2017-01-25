using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using PaymentsTU.Database;
using PaymentsTU.Properties;

namespace PaymentsTU
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			InitializeCultures();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var connectionString = ConfigurationManager.ConnectionStrings["Payments"].ConnectionString;

			try
			{
				if (!DatabaseCreationOperation.IsDatabaseExist(connectionString))
				{
					DatabaseCreationOperation.CreateDatabase(connectionString);
				}
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
			}
			
		}

		private static void InitializeCultures()
		{
			//if (!string.IsNullOrEmpty(Settings.Default.Culture))
			//{
			//	Thread.CurrentThread.CurrentCulture = new CultureInfo(Settings.Default.Culture);
			//}
			//if (!string.IsNullOrEmpty(Settings.Default.UICulture))
			//{
			//	Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Default.UICulture);
			//}

			FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
				XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
		}
	}
}
