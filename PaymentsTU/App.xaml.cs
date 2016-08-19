using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using PaymentsTU.Database;

namespace PaymentsTU
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
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
	}
}
