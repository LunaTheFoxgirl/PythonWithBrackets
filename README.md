# PythonWithBrackets
A transpiler that transpiles a python variant (pywb) into python. (The code is ugly, i know.)

## Key differences
 * string literals are always unicode unless specified otherwise (prefix s)
 * BRACKETS! :D
 * Statements ends with semicolons.
 * func instead of def
 * throw instead of raise
 * construct instead of __init__
 * iterate instead of __iter__
 * new instead of __new__
 * this insteaad of self
 * C style multiline comments
 * true instead of True
 * false instead of False
 
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
