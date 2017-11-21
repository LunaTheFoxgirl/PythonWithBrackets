# PythonWithBrackets
A transpiler that transpiles a python variant (pywb) into python. (The code is ugly, i know.)

## Key differences
 * string literals are always unicode unless specified otherwise (prefix s)
 * BRACKETS! :D
 * Statements ends with semicolons.
 * func instead of def
 * throw instead of raise
 * construct instead of \_\_init\_\_
 * iterate instead of \_\_iter\_\_
 * new instead of \_\_new\_\_
 * this insteaad of self
 * C style multiline comments
 * true instead of True
 * false instead of False
 * empty classes/functions/etc will automatically get a placeholder to prevent compilation errors.
 * conditions use ()
 * fixes `from __future__ import braces`
### Code requirements
Code written in pythonwb has only 2 requirements, everything else is syntax sugar.
 * Semicolons end statements
 * curlybrackets define indentation.
 * Use () for conditions.
 
 ## Example code
 ```Csharp
 class MyClass {
	func construct(this) {
		this.MyMember = "Example";
	}

	func GetMember(this) {
		return this.MyMember;
	}
}

print("Hello, world!");
t = MyClass();
print(t.GetMember());
```

### Example code (minimal pythonwb)
 ```Csharp
 class MyClass {
	def __init__(self) {
		self.MyMember = "Example";
	}

	def GetMember(self) {
		return self.MyMember;
	}
}

print("Hello, world!");
t = MyClass();
print(t.GetMember());
```
