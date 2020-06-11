using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Kogane
{
	/// <summary>
	/// アセットインポート時に改行コードを Unix から Win に変換するエディタ拡張 
	/// </summary>
	public sealed class NewlineCodeConverter : AssetPostprocessor
	{
		//================================================================================
		// 定数
		//================================================================================
		private const string WINDOWS = "\r\n";
		private const string UNIX    = "\n";
		private const string MAC     = "\r";

		//================================================================================
		// デリゲート（static）
		//================================================================================
		/// <summary>
		/// 改行コードを変換するアセットかどうかを確認する時に呼び出されます
		/// </summary>
		public static Func<string, bool> Predicate { private get; set; }
		
		//================================================================================
		// イベント（static）
		//================================================================================
		/// <summary>
		/// 変換が完了した時に変換したファイルのパスを取得できます
		/// </summary>
		public static event Action<IReadOnlyList<string>> OnComplete;

		//================================================================================
		// 関数（static）
		//================================================================================
#if UNITY_EDITOR_WIN
		/// <summary>
		/// アセットがインポートされた時に呼び出されます
		/// </summary>
		private static void OnPostprocessAllAssets
		(
			string[] importedAssets,
			string[] deletedAssets,
			string[] movedAssets,
			string[] movedFromAssetPaths
		)
		{
			if ( Predicate == null ) return;

			var list = importedAssets
					.Where( c => Predicate( c ) )
					.ToArray()
				;

			if ( list.Length <= 0 ) return;

			var result = new List<string>();

			for ( var i = 0; i < list.Length; i++ )
			{
				var path    = list[ i ];
				var oldText = File.ReadAllText( path );
				var newText = oldText;

				// 一度 Win -> Unix に変換しないと改行コードの変換が成功しなかった
				newText = newText.Replace( WINDOWS, UNIX );

				newText = newText.Replace( MAC, UNIX );
				newText = newText.Replace( UNIX, WINDOWS );

				// 処理時間削減のため、
				// 改行コードの変換が不要であればファイルの書き込みは行わない
				if ( newText == oldText ) continue;

				File.WriteAllText( path, newText );

				result.Add( path );
			}

			OnComplete?.Invoke( result );
		}
#endif
	}
}