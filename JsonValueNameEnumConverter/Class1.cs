
//using System;
//using System.Text.Json;
//using System.Text.Json.Serialization;

using System;
using System.Text.Json.Serialization;

namespace osexpert.JsonConverters
{

	public class ValueNameEnum<T> where T : struct, Enum
	{
		T _en;
		ValueNameEnum(T en)
		{
			_en = en;
		}

		public int Value => Convert.ToInt32(_en);

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string Name => Enum.IsDefined(typeof(T), _en) ? _en.ToString() : null;
	}


	//	public class Class1
	//	{
	//		public void test()
	//		{
	//			JsonStringEnumConverter
	//		}
	//	}

	//	public class JsonValueStringEnumConverterFactory : JsonConverterFactory
	//	{
	//		public override bool CanConvert(Type typeToConvert)
	//		{
	//			return typeToConvert.IsEnum;
	//		}

	//		public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
	//		{
	//			EnumConverterFactory.Create(typeToConvert);
	//		}

	//	//	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2055:MakeGenericType",
	//	//Justification = "'EnumConverter<T> where T : struct' implies 'T : new()', so the trimmer is warning calling MakeGenericType here because enumType's constructors are not annotated. " +
	//	//"But EnumConverter doesn't call new T(), so this is safe.")]
	//	//	[return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
	//		private static Type GetEnumConverterType(Type enumType) => typeof(EnumConverter<>).MakeGenericType(enumType);
	//	}

	//	public class JsonValueStringEnumConverter<T> : JsonConverter<T> where T : System.Enum
	//	{
	//		public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	//		{
	//			throw new NotImplementedException();
	//		}

	//		public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
	//		{
	//			throw new NotImplementedException();
	//		}
	//	}
}
