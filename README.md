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

## Downloads

[![](http://bytecode77.com/public/fileicons/zip.png) SelfMorphingCSharpBinary 1.0.0.zip](https://bytecode77.com/downloads/SelfMorphingCSharpBinary%201.0.0.zip)

## Project Page

[![](https://bytecode77.com/public/favicon16.png) bytecode77.com/self-morphing-csharp-binary](https://bytecode77.com/self-morphing-csharp-binary)