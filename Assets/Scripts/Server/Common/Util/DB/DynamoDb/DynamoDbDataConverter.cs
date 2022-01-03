using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;
using System.Globalization;
using System.IO;

namespace LGUVirtualOffice
{
	/// <summary>
	/// B:type Binary. For example:"B": "dGhpcyB0ZXh0IGlzIGJhc2U2NC1lbmNvZGVk"
	/// BOOL:type Boolean. For example:"BOOL": true
	/// BS:type Binary Set.For example:"BS": ["U3Vubnk=", "UmFpbnk=", "U25vd3k="]
	/// L:type List. For example:"L": [ {"S": "Cookies"} , {"S": "Coffee"}, {"N", "3.14159"}]
	/// M:type Map. For example:"M": {"Name": {"S": "Joe"}, "Age": {"N": "35"}}
	/// N:type Number. For example:"N": "123.45"
	/// NS:type Number Set. For example:"NS": ["42.2", "-19", "7.5", "3.14"]
	/// NULL:type Null. For example:"NULL": true
	/// S:type String. For example:"S": "Hello"
	/// SS:type String Set. For example:"SS": ["Giraffe", "Hippo" ,"Zebra"]
	/// IsBOOLSet:set to true if the property BOOL is set; false otherwise. 
	/// IsLSet:set to true if the property L is set; false otherwise.
	/// IsMSet:set to true if the property M is set; false otherwise. 
	/// </summary>
	public class DynamoDBDataConverter : Singleton<DynamoDBDataConverter>
	{
		private Dictionary<Type, MethodInfo> methodMap = new Dictionary<Type, MethodInfo>();
		private string convert_time_format = AWSUtil.Instance.GetAWSSetting().dateTime_convert_format;
		private DynamoDBDataConverter()
		{
			Type t = typeof(DynamoDBDataConverter);
			MethodInfo[] methods = t.GetMethods();
			foreach (MethodInfo method in methods)
			{
				if (!method.DeclaringType.Equals(t))
				{
					continue;
				}
				ParameterInfo[] paramArr = method.GetParameters();
				if (paramArr.Length < 2)
				{
					continue;
				}
				methodMap.Add(paramArr[1].ParameterType, method);
			}
		}

		#region public method region
		/// <summary>
		/// T does not include IDictionary type,can suppport List<int>,List<string>,List<float>,List<double>,List<long>
		/// ,List<byte[]>,and custom type(a class for exsample),and unSigned primitive type except char/byte/short
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="document"></param>
		/// <param name="value"></param>
		public void ConvertFromDocument<T>(Document document, ref T value)
		{
			Type t = value.GetType();
			MethodInfo targetMethod;
			if (methodMap.TryGetValue(t.MakeByRefType(), out targetMethod))
			{
				foreach (var item in document.ToAttributeMap())
				{
					object[] paramArr = new object[] { item.Value, value };
					bool invokeResult = (bool)targetMethod.Invoke(this, paramArr);
					//if invoke succeed,then replace the "value" to the real result,otherwise,use the default value;
					if (invokeResult)
					{
						value = (T)paramArr[1];
					}
				}
			}
			else
			{
				Dictionary<string, AttributeValue> attributeValueMap = document.ToAttributeMap();
				PropertyInfo[] propertyArr = t.GetProperties();
				foreach (var property in propertyArr)
				{
					object propertyValue;
					AttributeValue attributeValue;
					if (attributeValueMap.TryGetValue(property.Name, out attributeValue))
					{
						ConvertToObject(attributeValue, out propertyValue);
						property.SetValue(value, propertyValue);
					}
				}
			}
		}

		public Dictionary<string, T> ConvertToDictionary<T>(Document document)
		{
			Dictionary<string, T> value = new Dictionary<string, T>();
			Type t = typeof(T);
			if (document != null && document.Count > 0)
			{
				foreach (var item in document.ToAttributeMap())
				{
					value.Add(item.Key, ConvertFromAttributeValue<T>(item.Value));
				}
			}
			return value;
		}

		public T ConvertFromAttributeValue<T>(AttributeValue param)
		{
			//the Type of a parameter which flags with "out" will end with a "&" character
			MethodInfo targetMethod;
			T value = default(T);
			if (methodMap.TryGetValue(typeof(T).MakeByRefType(), out targetMethod))
			{
				//the invocation must be done like this,otherwise you can not get the result properly.
				object[] paramArr = new object[] { param, value };
				bool invokeResult = (bool)targetMethod.Invoke(this, paramArr);
				//if invoke succeed,then replace the "value" to the real result,otherwise,use the default value;
				if (invokeResult)
				{
					value = (T)paramArr[1];
				}
			}
			return value;
		}

		//name of the attribute in the table must be the same as name of the property in the custom type
		public T ConvertToCustomType<T>(Document document) where T : class, new()
		{
			if (document == null || document.Count <= 0)
			{
				return null;
			}
			Type t = typeof(T);
			PropertyInfo[] propertyArr = t.GetProperties();
			ConstructorInfo[] constructorArr = t.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
			ConstructorInfo constructor = Array.Find(constructorArr, c => c.GetParameters().Length == 0);
			T instance = constructor.Invoke(null) as T;
			Dictionary<string, AttributeValue> attributeValueMap = document.ToAttributeMap();
			object value;
			foreach (PropertyInfo property in propertyArr)
			{
				AttributeValue attributeValue;
				if (attributeValueMap.TryGetValue(property.Name, out attributeValue))
				{
					ConvertToObject(attributeValue, out value);
					property.SetValue(instance, value);
				}
			}
			return instance;
		}

		public bool ConvertToObject(AttributeValue attributeValue, out object value)
		{
			if (attributeValue.IsBOOLSet)
			{
				value = attributeValue.BOOL;
				return true;
			}
			if (attributeValue.IsLSet)
			{
				List<object> resultList = new List<object>();
				foreach (var item in attributeValue.L)
				{
					object innerValue;
					ConvertToObject(item, out innerValue);
					resultList.Add(innerValue);
				}
				value = resultList;
				return true;
			}
			if (attributeValue.IsMSet)
			{
				Dictionary<string, object> result = new Dictionary<string, object>();
				foreach (var item in attributeValue.M)
				{
					object innerValue;
					ConvertToObject(item.Value, out innerValue);
					result.Add(item.Key, innerValue);
				}
				value = result;
				return true;
			}
			if (!string.IsNullOrEmpty(attributeValue.N))
			{
				value = double.Parse(attributeValue.N);
				return true;
			}
			if (!string.IsNullOrEmpty(attributeValue.S))
			{
				value = attributeValue.S;
				return true;
			}
			if (attributeValue.NS != null && attributeValue.NS.Count > 0)
			{
				List<double> resultList = new List<double>();
				foreach (var item in attributeValue.NS)
				{
					resultList.Add(double.Parse(item));
				}
				value = resultList;
				return true;
			}
			if (attributeValue.SS != null && attributeValue.SS.Count > 0)
			{
				value = attributeValue.SS;
				return true;
			}
			if (attributeValue.B != null)
			{
				int count = int.Parse(attributeValue.B.Length.ToString());
				byte[] b = new byte[count];
				attributeValue.B.Read(b, 0, count);
				value = b;
				return true;
			}
			if (attributeValue.BS != null)
			{
				List<byte[]> resultList = new List<byte[]>();
				foreach (var item in attributeValue.BS)
				{
					resultList.Add(item.GetBuffer());
				}
				value = resultList;
				return true;
			}
			value = null;
			return false;
		}

		public bool ConvertToBool(AttributeValue attributeValue)
		{
			if (attributeValue.NULL || !attributeValue.IsBOOLSet)
			{
				return false;
			}
			return attributeValue.BOOL;
		}
		public bool ConvertToInt(AttributeValue attributeValue, out int value)
		{
			if (attributeValue.NULL || string.IsNullOrEmpty(attributeValue.N))
			{
				value = default(int);
				return false;
			}
			value = Convert.ToInt32(attributeValue.N);
			return true;
		}

		public bool ConvertToLong(AttributeValue attributeValue, out long value)
		{
			if (attributeValue.NULL || string.IsNullOrEmpty(attributeValue.N))
			{
				value = default(long);
				return false;
			}
			value = Convert.ToInt64(attributeValue.N);
			return true;
		}
		public bool ConvertToDouble(AttributeValue attributeValue, out double value)
		{
			if (attributeValue.NULL || string.IsNullOrEmpty(attributeValue.N))
			{
				value = default(double);
				return false;
			}
			value = Convert.ToDouble(attributeValue.N);
			return true;
		}
		public bool ConvertToFloat(AttributeValue attributeValue, out float value)
		{
			if (attributeValue.NULL || string.IsNullOrEmpty(attributeValue.N))
			{
				value = default(float);
				return false;
			}
			value = Convert.ToSingle(attributeValue.N);
			return true;
		}
		public bool ConvertToDateTime(AttributeValue attributeValue, out DateTime value)
		{
			if (attributeValue.NULL || string.IsNullOrEmpty(attributeValue.N))
			{
				value = default(DateTime);
				return false;
			}
			value = Convert.ToDateTime(attributeValue.S);
			return true;
		}
		public bool CovertToString(AttributeValue attributeValue, out string value)
		{
			if (attributeValue.NULL)
			{
				value = null;
				return false;
			}
			value = attributeValue.S;
			return true;
		}
		public bool ConvertToStringList(AttributeValue attributeValue, out List<string> value)
		{
			if (attributeValue.NULL)
			{
				value = null;
				return false;
			}
			value = attributeValue.SS;
			return true;
		}

		public bool ConvertToIntList(AttributeValue attributeValue, out List<int> value)
		{
			if (!attributeValue.NULL && (attributeValue.NS != null || attributeValue.IsLSet))
			{
				List<int> resultList = new List<int>();
				foreach (var item in attributeValue.NS)
				{
					resultList.Add(Convert.ToInt32(item));
				}
				value = resultList;
				return true;
			}
			value = null;
			return false;
		}

		public bool ConvertToLongList(AttributeValue attributeValue, out List<long> value)
		{
			if (!attributeValue.NULL && attributeValue.NS != null)
			{
				List<long> resultList = new List<long>();
				foreach (var item in attributeValue.NS)
				{
					resultList.Add(Convert.ToInt64(item));
				}
				value = resultList;
				return true;
			}
			value = null;
			return false;
		}
		public bool ConvertToDoubleList(AttributeValue attributeValue, out List<double> value)
		{
			if (!attributeValue.NULL && attributeValue.NS != null)
			{
				List<double> resultList = new List<double>();
				foreach (var item in attributeValue.NS)
				{
					resultList.Add(Convert.ToDouble(item));
				}
				value = resultList;
				return true;
			}
			value = null;
			return false;
		}
		public bool ConvertToFloatList(AttributeValue attributeValue, out List<float> value)
		{
			if (!attributeValue.NULL && attributeValue.NS != null)
			{
				List<float> resultList = new List<float>();
				foreach (var item in attributeValue.NS)
				{
					resultList.Add(Convert.ToSingle(item));
				}
				value = resultList;
				return true;
			}
			value = null;
			return false;
		}

		public bool ConvertToByteArray(AttributeValue attributeValue, out byte[] value)
		{
			if (attributeValue.NULL || attributeValue.B == null)
			{
				value = null;
				return false;
			}
			int count = int.Parse(attributeValue.B.Length.ToString());
			byte[] b = new byte[count];
			attributeValue.B.Read(b, 0, count);
			value = b;
			return true;
		}

		public bool ConvertToByteArrayList(AttributeValue attributeValue, out List<byte[]> value)
		{
			if (attributeValue.NULL || attributeValue.BS == null)
			{
				value = null;
				return false;
			}
			value = new List<byte[]>();
			foreach (var item in attributeValue.BS)
			{
				value.Add(item.GetBuffer());
			}
			return true;
		}

		public bool ConvertToDictionary(AttributeValue attributeValue, out Dictionary<string, object> value)
		{
			if (attributeValue.NULL || !attributeValue.IsMSet)
			{
				value = null;
				return false;
			}
			value = new Dictionary<string, object>();
			foreach (var item in attributeValue.M)
			{
				object innerValue;
				ConvertToObject(item.Value, out innerValue);
				value.Add(item.Key, innerValue);
			};
			return true;
		}

		public string ConverteDateTimeToString(DateTime time)
		{
			return time.ToString(convert_time_format);
		}

		public DateTime ConvertStringToDateTime(string dateTime)
		{
			if (DateTime.TryParseExact(dateTime, convert_time_format, CultureInfo.InvariantCulture,
				DateTimeStyles.AssumeUniversal, out DateTime result))
			{
				return result;
			}
			throw new Exception("the format of dateTime string is not correct!");
		}

		/// <summary>
		/// convert form C# type to DynamoDBEntry
		/// </summary>
		/// <param name="value">primitive type,List,Dictionary(key will be convert to string)</param>
		/// <returns></returns>
		public DynamoDBEntry BuildDynamoDBEntry(object value)
		{
			if (value == null)
			{
				return new DynamoDBNull();
			}
			Type type = value.GetType();
			if (type.IsPrimitive && type.IsValueType)
			{
				return BuildFromPrimitive(value);
			}
			else if (type.IsValueType && type.Equals(typeof(DateTime)))
			{
				return Convert.ToDateTime(value);
			}
			else
			{
				return BuildFromReferenceType(value);
			}

		}
		#endregion

		#region private method region
		/// <summary>
		/// Convert from C# primitive type to DynamoDBEntry
		/// </summary>
		/// <param name="value">primitive type value</param>
		/// <returns></returns>
		private DynamoDBEntry BuildFromPrimitive(object value)
		{
			IConvertible ivalue = (IConvertible)value;
			switch (ivalue.GetTypeCode())
			{
				case TypeCode.Boolean:
					return new DynamoDBBool(Convert.ToBoolean(value));
				case TypeCode.Byte:
					return Convert.ToByte(value);
				case TypeCode.Double:
					return Convert.ToDouble(value);
				case TypeCode.Int64:
					return Convert.ToInt64(value);
				case TypeCode.Single:
					return Convert.ToSingle(value);
				default:
					return Convert.ToInt32(value);
			}
		}

		private DynamoDBEntry BuildFromReferenceType(object value)
		{
			if (value is string)
			{
				return Convert.ToString(value);
			}
			else if (value is byte[])
			{
				return new MemoryStream(value as byte[]);
			}
			else if (value is IList)
			{
				DynamoDBList list = new DynamoDBList();
				IList ivalue = value as IList;
				foreach (var item in ivalue)
				{
					list.Add(BuildDynamoDBEntry(item));
				}
				return list;
			}
			else if (value is IDictionary)
			{
				Document doc = new Document();
				IDictionary ivalue = value as IDictionary;
				foreach (var item in ivalue.Keys)
				{
					doc[item.ToString()] = BuildDynamoDBEntry(ivalue[item]);
				}
				return doc;
			}
			else
			{
				return null;
			}
		}
		#endregion
	}
}