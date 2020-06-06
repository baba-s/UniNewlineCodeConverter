# Uni Newline Code Converter

指定されたアセットの改行コードを Win に統一するエディタ拡張

## 使用例

```cs
using Kogane;

public static class Example 
{
	static Example()
	{
		NewlineCodeConverter.Predicate = path =>
		{
			return path.EndsWith( ".spriteatlas" );
		};
	}
}
```