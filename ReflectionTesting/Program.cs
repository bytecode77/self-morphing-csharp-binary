using System;
using System.CodeDom.Compiler;

namespace ReflectionTesting
{
	public static class Program
	{
		public static void Main()
		{
			int a1 = Environment.TickCount;
			Random b1 = new Random(a1);

			dynamic a2 = GetProperty("System.Environment", "TickCount");
			dynamic b2 = GetConstructor("System.Random", new Type[] { GetType("System.Int32") }, new object[] { a2 });

			CodeDomProvider c1 = CodeDomProvider.CreateProvider("CSharp");
			dynamic c2 = InvokeMethod("System.CodeDom.Compiler.CodeDomProvider", "CreateProvider", new Type[] { GetType("System.String") }, new object[] { "CSharp" });
		}

		static dynamic GetConstructor(string type, Type[] types, object[] parameters)
		{
			return Type.GetType(type).GetConstructor(types).Invoke(parameters);
		}
		static dynamic GetProperty(string type, string property)
		{
			return Type.GetType(type).GetProperty(property).GetMethod.Invoke(null, new object[0]);
		}
		static dynamic InvokeMethod(string type, string method, Type[] types, object[] parameters)
		{
			return Type.GetType(type).GetMethod(method, System.Reflection.BindingFlags.Static).Invoke(null, parameters);
		}
		static Type GetType(string name)
		{
			return Type.GetType(name);
		}
	}
}