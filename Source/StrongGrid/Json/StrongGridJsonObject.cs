using StrongGrid.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace StrongGrid.Json
{
	[JsonConverter(typeof(StrongGridJsonObjectConverter))]
	internal class StrongGridJsonObject
	{
		private readonly IDictionary<string, object> _properties = new Dictionary<string, object>();

		public IReadOnlyDictionary<string, object> Properties
		{
			get { return new ReadOnlyDictionary<string, object>(_properties); }
		}

		public StrongGridJsonObject()
		{
		}

		public void AddProperty<T>(string propertyName, Parameter<T> parameter, bool ignoreIfDefault = true)
		{
			if (ignoreIfDefault && !parameter.HasValue) return;
			this.AddProperty(propertyName, parameter.Value, false);
		}

		public void AddProperty<T>(string propertyName, T value, bool ignoreIfDefault = true)
		{
			switch (value)
			{
				case string stringValue:
					if (ignoreIfDefault && string.IsNullOrEmpty(stringValue)) return;
					AddDeepProperty(propertyName, stringValue);
					break;

				case Enum enumValue:
					AddDeepProperty(propertyName, enumValue.ToEnumString());
					break;

				case IEnumerable enumerableValue:
					if (ignoreIfDefault)
					{
						if (enumerableValue == null) return;

						// Non-generic IEnumerable does not natively provide a way to check if it contains any items.
						// Therefore, we need to figure it out ourselves
						var countainsAtLeastOneItem = false;
						foreach (var item in enumerableValue)
						{
							countainsAtLeastOneItem = true;
							break;
						}

						if (!countainsAtLeastOneItem) return;
					}

					AddDeepProperty(propertyName, value);
					break;

				default:
					if (ignoreIfDefault && EqualityComparer<T>.Default.Equals(value, default)) return;
					AddDeepProperty(propertyName, value);
					break;
			}
		}

		private void AddDeepProperty<T>(string propertyName, T value)
		{
			var separatorLocation = propertyName.IndexOf('/');

			if (separatorLocation == -1)
			{
				_properties.Add(propertyName, value);
			}
			else
			{
				var name = propertyName.Substring(0, separatorLocation);
				var childrenName = propertyName.Substring(separatorLocation + 1);

				var properties = _properties.Where(kvp => kvp.Key.Equals(name, StringComparison.OrdinalIgnoreCase));
				if (!properties.Any())
				{
					var propertyValue = new StrongGridJsonObject();
					propertyValue.AddDeepProperty(childrenName, value);
					_properties.Add(name, propertyValue);
				}
				else
				{
					var propertyValue = properties.Single().Value as StrongGridJsonObject;
					propertyValue.AddDeepProperty(childrenName, value);
				}
			}
		}
	}
}
