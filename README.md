# Self-Morphing C# Binary

## Proof of concept

...for a C# binary that

### When executed...

- Executes an encrypted payload in memory without drops *(In this example, the payload just displays a MessageBox)*
- All variable names are obfuscated using unreadable characters
- All strings are runtime encrypted with a simple 8-bit xor
- Very hard to analyze when decompiled

### After payload execution...

- The binary re-compiles itself and replaces the original file
- Variable names are obfuscated, using different names after each execution
- Strings are encrypted using a different key
- The payload is re-encrypted using a different key

Since this is proof of concept, no actually malicious payload is included. Dropped files that begin with "Debug_" can be omitted completely, but are written to disk to visualize the build process.

## General concept

Expecting well readable code? Design patterns? Maybe you should stop reading here, because this binary is optimized for maximum obscurity and minimum file size! For this, a custom build process is implemented. When running **Build.exe**, the binary is compiled using CodeDOM for the first time, named **SelfMorphingCSharpBinary.exe**. This binary contains the file **Payload.exe** from the resources of Build.exe. The payload, strings and variable names are encrypted and obfuscated.

For the sake of debugging and visualizing, all generated code files are written to disk.

## In-depth

**Step 1:** Build.exe takes **Stub.cs** from its resources and processes, then compiles it. The original file looks like this:

![](https://bytecode77.com/images/pages/self-morphing-csharp-binary/original.png)

Because the binary needs to obfuscate itself on runtime, the original code must be included as well, somewhat similar to a quine. No managed resources are used, just byte[] literals. Variables that start with "_V_" will be renamed using characters from line 15.

Variable names from the original code, which is included as a byte[] literal, will also be obfuscated. However, alphanumerical characters are used. This is the code that is compiled along with the actual binary:

![](https://bytecode77.com/images/pages/self-morphing-csharp-binary/intermediate.png)

So, the code from the first screenshot is what I work with during development. The code on the second screenshot is the code that is included as a resource... *sigh*... as a byte[] literal in the binary. Now, look at the final code that is used to compile the binary:

![](https://bytecode77.com/images/pages/self-morphing-csharp-binary/obfuscated.png)

This is also what you get when decompiling the binary. The highlighted text, for instance, is a method that decrypts strings. The payload is executed in memory on line 18 using Assembly.Load().Invoke(). For instance, the encrypted string in GetMethod(...) is actually **"Main"**.

**Step 2:** Self-morphing!

So, our well obfuscated binary (screenshot 3) now contains the payload and its own pre-obfuscated code (from screenshot 2), both encrypted using AES and a random key. The key must also be included.

When executed, first the payload is decrypted and executed in memory, displaying a simple message box. Remember, that the payload is a separate and replaceable executable file. Next, the binary takes its original code from the byte[] literal and does basically the same as what our build tool did: Encrypt both payload and its own code, encrypt all strings and obfuscate all variable names. In this instance, the binary has morphed. Every literal and symbol name is encrypted and obfuscated with a different random key each time the binary is executed.

Since we are talking about a "**self**-morphing" binary, the original file should replace itself. For this, we call **cmd** with a command that takes one second, piped by moving the file to the correct place. This may be improved, but serves its purpose for now.

**Step 3:** Looking at the binary when decompiled... Result: Yes, it is obfuscated and very incomprehensive. Note, that there *are* professional obfuscators available. But here, we need a very minimalistic obfuscator that is built in and can re-compile and obfuscate even itself.

![](https://bytecode77.com/images/pages/self-morphing-csharp-binary/decompiled.png)

**Step 4:** Done! Now let's enjoy our binary.

This animation visualizes the executable's source code morphing each time the executable is run.

![](https://bytecode77.com/images/pages/self-morphing-csharp-binary/morphing.gif)

## an explanation of the methods and steps involved in the builder and stub code:

Main method
This is the entry point of the program. It performs the following steps:

Initializes a Random object and a string of characters to be used for obfuscation.
Creates a CodeDomProvider object for compiling C# code.
Sets up CompilerParameters to specify the output assembly, compiler options, and referenced assemblies for the compilation.
Reads in the stub code from the Resources.Stub file and stores it in a string variable.
Replaces occurrences of the string "V" in the stub code with randomly generated variable names, using the GetCleanVariableName method to generate the new names.
Encrypts and compresses the stub code and payload data, and stores the resulting byte arrays in the stub code as array initializers.
Replaces string literals in the stub code with calls to a function that decrypts a string that has been encrypted using a simple substitution cipher.
Renames any remaining occurrences of "V" in the stub code using the GetVariableName method to generate new names.
Compiles the modified stub code using the CodeDomProvider object and the CompilerParameters.
Renames the output assembly to "SelfMorphingCSharpBinary.exe" and replaces the original file with the new one.
GetCleanVariableName method
This method generates a random variable name for use in the modified stub code. It does this by concatenating a random number of randomly chosen lowercase letters from the ObfuscationCharacters string.

GetVariableName method
This method generates a random variable name for use in the modified stub code. It does this by concatenating a random number of randomly chosen lowercase letters from the ObfuscationCharacters string, and appending a randomly chosen number from 0 to 9.

EncryptString method
This method takes a string as input and returns an encrypted version of the string. It does this by replacing each character in the string with a corresponding character from a fixed substitution cipher alphabet.

Encrypt method
This method takes a byte array as input and returns an encrypted version of the array. It does this by XORing each byte in the array with a randomly chosen byte.

Compress method
This method takes a byte array as input and returns a compressed version of the array. It does this using the GZipStream class from the System.IO.Compression namespace.

DecryptString method (in the stub code)
This method takes an encrypted string as input and returns the decrypted version of the string. It does this by replacing each character in the string with a corresponding character from the fixed substitution cipher alphabet that was used to encrypt the string.

Decrypt method (in the stub code)
This method takes an encrypted byte array as input and returns the decrypted version of the array. It does this by XORing each byte in the array with the same randomly chosen byte that was used to encrypt the array.

Decompress method (in the stub code)
This method takes a compressed byte array as input and returns the decompressed version of the array. It does this using the GZipStream class from the System.IO.Compression namespace.
GetPayload method (in the stub code)
This method decompresses and decrypts the payload data that was included in the executable as a byte array. It does this by calling the Decompress and Decrypt methods on the payload data array.

Main method (in the stub code)
This is the entry point of the program. It performs the following steps:

Calls the GetPayload method to retrieve the payload data.
Executes the payload data, which could contain additional code or data that the program will utilize.
## Downloads

[![](http://bytecode77.com/public/fileicons/zip.png) SelfMorphingCSharpBinary 1.0.0.zip](https://downloads.bytecode77.com/SelfMorphingCSharpBinary%201.0.0.zip)
(**ZIP Password:** bytecode77)

## Project Page

[![](https://bytecode77.com/public/favicon16.png) bytecode77.com/self-morphing-csharp-binary](https://bytecode77.com/self-morphing-csharp-binary)
