using System;
using System.Configuration;
using System.IO;
using PaymentsTU.Properties;

namespace PaymentsTU
{
	internal sealed class PortableSettings
	{
		public static void WritePortableSettings()
		{
			var path = AppDomain.CurrentDomain.BaseDirectory;
			var config = ConfigurationManager.OpenExeConfiguration(Path.Combine(path, "paymentstu.exe"));

			var properties = Settings.Default.Properties;

			foreach (SettingsProperty property in properties)
			{
				if (property.IsReadOnly) continue;

				var setting = config.AppSettings.Settings[property.Name];

				if (setting != null)
				{
					if (property.SerializeAs == SettingsSerializeAs.String)
						setting.Value = SerializePropertyAsString(property.PropertyType, Settings.Default[property.Name]);
				}
				else
				{
					if (property.SerializeAs == SettingsSerializeAs.String)
						config.AppSettings.Settings.Add(property.Name, SerializePropertyAsString(property.PropertyType, Settings.Default[property.Name]));
				}
			}

			config.Save();
		}

		public static void ReadPortableSettings()
		{
			Settings.Default.Upgrade();

			var path = AppDomain.CurrentDomain.BaseDirectory;
			var config = ConfigurationManager.OpenExeConfiguration(Path.Combine(path, "paymentstu.exe"));

			var properties = Settings.Default.Properties;

			foreach (SettingsProperty property in properties)
			{
				if (property.IsReadOnly) continue;

				var setting = config.AppSettings.Settings[property.Name];

				if (setting != null)
				{
					Settings.Default[property.Name] = DeserializeProperty(property, setting.Value);
				}
			}
		}

		private static string SerializePropertyAsString(Type type, object value)
		{
			if (type == typeof(DateTime))
			{
				return ((DateTime)value).ToString("O");
			}

			return value.ToString();
		}

		private static object DeserializeProperty(SettingsProperty property, string value)
		{
			if (property.SerializeAs == SettingsSerializeAs.String)
			{
				if (property.PropertyType == typeof(DateTime))
				{
					return DateTime.Parse(value);
				}
			}

			return value;
		}
	}
}