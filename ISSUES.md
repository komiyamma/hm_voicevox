# HmVoiceVox プロジェクトの問題点

このドキュメントは、現在の `HmVoiceVox` プロジェクトにおける既知の問題点や改善点をまとめたものです。

## 1. 機能の未実装・未統合

`README.md`で言及されている中心的な機能のいくつかが、まだ実装されていないか、プログラムに統合されていません。

- **VOICEVOXインストール場所の自動取得**
  - **問題点:** `InstallLocation.cs`にレジストリからインストールパスを取得する `GetVoicevoxInstallLocation` メソッドが存在しますが、`Program.cs`では呼び出されず、ハードコードされたパスが使用されています。
  - **該当箇所:** `src/ConsoleApp14/Program.cs` (L10)

- **話者の動的選択機能**
  - **問題点:** 話者を名前で検索し、そのIDを音声合成に利用する機能が未完成です。`SearchSpeaker.cs`の`GetSpearker`メソッドは常に`1`を返し、`Program.cs`ではその戻り値を使わずにID `2` がハードコードされています。
  - **該当箇所:** `src/ConsoleApp14/Program.cs` (L16), `src/ConsoleApp14/SearchSpeaker.cs` (L90)

- **秀丸エディタとの連携**
  - **問題点:** プロジェクトの最終目標である秀丸エディタとの連携機能が実装されていません。現状では、`Program.cs`内でハードコードされた固定のテキストを読み上げるだけです。

## 2. ハードコードされた値

アプリケーションの柔軟性と移植性を妨げるハードコードされた値が多数存在します。

- **VOICEVOXインストールパス:** `src/ConsoleApp14/Program.cs` L10
- **話者ID:** `src/ConsoleApp14/Program.cs` L16, L29
- **読み上げ対象テキスト:** `src/ConsoleApp14/Program.cs` L17
- **VOICEVOX APIエンドポイント:** `src/ConsoleApp14/Program.cs` L20, L29 および `src/ConsoleApp14/SearchSpeaker.cs` L48
- **出力WAVファイル名:** `src/ConsoleApp14/Program.cs` L42 (`"test.wav"`)
- **Windowsレジストリキー:** `src/ConsoleApp14/InstallLocation.cs` L7 (VOICEVOXの特定バージョンに依存する可能性あり)

## 3. コード品質と設計の問題

コードの保守性や堅牢性に関わる問題です。

- **エラーハンドリングの欠如**
  - **問題点:** `HttpClient`によるAPI通信やファイル保存処理に`try-catch`が存在しません。VOICEVOXエンジンが未起動の場合や、ファイル書き込み権限がない場合にプログラムがクラッシュします。
  - **該当箇所:** `src/ConsoleApp14/Program.cs`

- **不適切な非同期処理**
  - **問題点:** `.Result`を用いて非同期処理を同期的に待機しています。これはUIスレッドでのデッドロックやパフォーマンス低下の原因となります。`await`キーワードの使用が推奨されます。
  - **該当箇所:** `src/ConsoleApp14/Program.cs` (L26), `src/ConsoleApp14/SearchSpeaker.cs` (L53)

- **命名規則の問題**
  - **問題点:** `src/ConsoleApp14/SearchSpeaker.cs`に`GetSpearker`というメソッドが存在します。これは "Speaker" のタイポ（スペルミス）です。
  - **該当箇所:** `src/ConsoleApp14/SearchSpeaker.cs` (L78)

- **責務の分離が不十分**
  - **問題点:** `src/ConsoleApp14/Program.cs`の`Main`メソッドに、APIへのリクエスト、ファイル保存、音声再生といった複数の責務が集中しています。可読性と保守性向上のため、これらのロジックは個別のメソッドに分割するべきです。

- **`HttpClient`インスタンス化方針の不統一**
  - **問題点:** `Program.cs`では`using`ブロックで`HttpClient`を都度生成していますが、`SearchSpeaker.cs`では`static`なインスタンスを再利用しています。アプリケーション全体で方針を統一するべきです（一般的には`static`での再利用が推奨されます）。

## 4. ドキュメントの不整合

- **.NETのバージョン**
  - **問題点:** `README.md`には「.NET 6.0 SDK」が必要と記載されていますが、`ConsoleApp14.csproj`ではターゲットフレームワークが`net8.0-windows`に設定されています。
