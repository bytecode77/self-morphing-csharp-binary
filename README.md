# Self-Morphing C# Binary

## Proof of concept

...for a C# binary that

* When executed
  * Executes an encrypted payload in memory without drops *(In this example, an executable displaying a MessageBox)*
  * All variable names are obfuscated using unreadable characters
  * All strings are runtime encrypted with a simple 8 bit xor
  * Very hard to analyze when decompiled, however, getting the original payload by decompilation and debugging is possible
* After payload execution
  * The binary will re-compile itself and replace the original file
  * Again, all variable names are obfuscated, using different names each time
  * Strings are encrypted differently each time as well
  * The payload is also re-encrypted using a different key

Since this is proof of concept, no actually malicious payload is included. Dropped files that begin with "Debug_" can be omitted completely, but are written to disk to visualize the build process.

## How it works in general

Expecting well readable code? Design patterns? Maybe you should stop reading
here, because this binary is optimized for maximum obscurity, minimum file size
and security software evasion! For this, a custom build process is implemented.
When running **Build.exe**, the binary is compiled using CodeDOM for the first
time and placed in the same directory, named **SelfMorphingCSharpBinary.exe**.
This binary contains the file **Payload.exe** from the resources of Build.exe.
The payload, strings and variable names are encrypted and obfuscated.

For debugging purposes, all generated code files, even from the encrypted binary
itself, are written to disk, however, it is not required, because code can
usually be compiled in memory using CodeDOM.

## In-depth

**Step 1:** Build.exe takes **Stub.cs** from its resources and processes, then
compiles it. The original file looks like this:

[![](http://bytecode77.com/images/sites/hacking/poc/self-morphing-csharp-binary/original.png)](http://bytecode77.com/images/sites/hacking/poc/self-morphing-csharp-binary/original.png)

Because the binary needs to obfuscate itself on runtime, the original code has
to be included as well, somewhat similar to a quine. No managed resources are
used, just `byte[]` literals. Variables that start with `"_V_"` will be renamed
using characters from line 15.

Variable names of the original code which is included as a `byte[]` literal,
will also be obfuscated. However, alphanumerical characters are used. This is
the code that is compiled along with the actual binary:

[![](http://bytecode77.com/images/sites/hacking/poc/self-morphing-csharp-binary/intermediate.png)](http://bytecode77.com/images/sites/hacking/poc/self-morphing-csharp-binary/intermediate.png)

So, the code from the first screenshot is what I work with during development.
The code on the second screenshot is the code that is included as a resource...
\*sigh\*... as a `byte[]` literal in the binary. Now, look at the final code
that is used to compile the binary:

[![](http://bytecode77.com/images/sites/hacking/poc/self-morphing-csharp-binary/obfuscated.png)](http://bytecode77.com/images/sites/hacking/poc/self-morphing-csharp-binary/obfuscated.png)

This is also what you get when decompiling the binary. The highlighted text, for
instance, is a method that decrypts strings. The payload is executed in memory
in line 18 using Assembly.Load() and then Invoke(). For instance, the encrypted
string in `GetMethod(...)` is actually `"Main".`

**Step 2:** Self-morphing!

So our well obfuscated binary (screenshot 3) now contains the payload and its
own pre-obfuscated code (from screenshot 2), both encrypted using AES and a
random key. *(Note: From a cryptographical perspective, this is rather an
encoding than an actual encryption, because the key is also contained in the
binary.)*

When executed, first the payload is decrypted and executed in memory, displaying
a simple message box. Remember, that this is a separate and replaceable
executable file. Next, the binary takes its original code from the byte[]
literal and does basically the same as what our build tool did: Encrypt both
payload and its own code, encrypt all strings and obfuscate all variable names.
Then we have a fresh binary that, from a heuristic point of view, is not similar
to the previous one! Every literal and symbol name is encrypted and obfuscated
with a different random key each time the binary is executed.

Since we are talking about a "**self**-morphing" binary, the original file has
to replace itself. For this, we call **cmd** with a command that takes one
second, piped by moving the file to the correct place. This may be improved, but
serves our purpose for now.

**Step 3:** Looking at the binary when decompiled... Result: Yes, it is
obfuscated and very hard to debug. Note, that there *are* professional
obfuscators available. But here, we need a very minimalistic obfuscator that is
built in and can re-compile and obfuscate even itself.

[![](http://bytecode77.com/images/sites/hacking/poc/self-morphing-csharp-binary/decompiled.png)](http://bytecode77.com/images/sites/hacking/poc/self-morphing-csharp-binary/decompiled.png)

**Step 4:** Done! Now let's enjoy our binary. It is alive! And it wants to stay!

This animation visualizes the executable's source code morphing each time the
executable is run.

[![](http://bytecode77.com/images/sites/hacking/poc/self-morphing-csharp-binary/morphing.gif)](http://bytecode77.com/images/sites/hacking/poc/self-morphing-csharp-binary/morphing.gif)

## Downloads

[![](https://bytecode77.com/images/shared/fileicons/zip.png) SelfMorphingCSharpBinary 1.0.0 Binaries.zip](https://bytecode77.com/downloads/hacking/poc/SelfMorphingCSharpBinary%201.0.0%20Binaries.zip)

## Project Page

[![](https://bytecode77.com/images/shared/favicon16.png) bytecode77.com/hacking/poc/self-morphing-csharp-binary](https://bytecode77.com/hacking/poc/self-morphing-csharp-binary)