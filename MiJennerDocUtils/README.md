# MiJenner.DocUtils
DocUtils offer cross-platform utilities for easy overview documentation of your .cs files. 

The method `WriteDoc(string codePath)` extracts an overview of classes in the file, their constructors with parameters. Further, the properties and fields of each class are listed together with access modifiers. And finally all methods, their access modifiers and their parameters are listed. The output is written to console. 
Often this is too much information, but it is easier to delete the extran information than to write the whole overview manually. 

# Method Signatures 

Below is listed the signatures of the methods: 
```cs
public static void WriteDoc(string codePath)
```

# Examples 
Identify the full path of the source files you want to analyze. 
This can be done from Visual Studio by right clicking and "Copy Full Path". 

Create a console application project. 

Add a using instruction at the top: 
```cs
using MiJenner.DocUtils;
```

Below are examples on how the library can be used: 
```cs 
string codePath = @"TestSourceFile.cs"; 
DocUtils.WriteDoc(codePath);
```

# NuGet package 
There is a NuGet package available for easy usage. 

