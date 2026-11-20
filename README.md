# ウマ娘 切れ者カウンター

ウマ娘 プリティーダービーの「切れ者」まわりのイベントを手動で記録・集計する Windows デスクトップアプリです。
ウィンドウは最前面に固定でき、位置は自由に動かせます（位置は次回起動時に復元）。

## 対応環境

- Windows 10 / 11
- ゲーム: DMM GAMES 版 / Steam 版（ゲームとは連携せず、横に並べて使う想定）

## 機能

| 記録ボタン | 内容 |
| --- | --- |
| 育成開始 | 育成回数 |
| 勝負服 / 固有 切れ者イベント | 成功 / 失敗 |
| 共通 切れ者イベント | 成功 / 失敗 |
| レース後 切れ者イベント | 成功 / 失敗 |
| 恋愛相談 | 成功 / 失敗 |
| 覚醒Lv7 切れ者イベント | 発生（+1） |

- **統計（累計）**: 勝負服/固有・共通・レース後・合計（この3種）の発生・成功・成功率・未取得連続、恋愛相談の発生・成功・成功率、覚醒Lv7 の発生
- **未取得連続**: 前回の成功から連続で失敗した回数（育成をまたいで数える）
- **切れ者取得ログ**: 取得日時・育成番号・種類（何回目で取得したか）
- **取り消し**: 直前の記録を1件取り消し
- **トレーナーアビリティタイマー**: 「開始」で12分のカウントダウン。0になるとチャイムが鳴り、赤く表示される。動作中・終了後の「再開始」で12分から再スタート、「リセット」で停止
- **省スペースモード**: 「省スペース」ボタンで、育成開始・取り消し・タイマー・記録ボタンだけの小さい表示（高さ約1/3）に切り替え。「通常表示」で戻す。状態は次回起動時も維持
- データの保存先: `%LOCALAPPDATA%\UmaKiremonoCounter\data.json`

## 開発環境

- .NET SDK 11.0（`global.json` で 11.0.100 以降に固定）
- C# / WPF（MVVM）

```
src/UmaKiremonoCounter/
  Models/      保存データ・記録
  Services/    保存処理・集計・View 向けサービスのインターフェース
  ViewModels/  ViewModel
  Views/       画面（コードビハインドは InitializeComponent のみ）と WPF 依存のサービス実装
  App.xaml.cs  View と ViewModel の組み立て
```

## ビルド・実行

```powershell
dotnet build
dotnet run --project src/UmaKiremonoCounter
dotnet test
```

## ダウンロード

[Releases](https://github.com/sojuenke-cmyk/umamusume-kiremono-counter/releases) から `UmaKiremonoCounter-vX.Y.Z-win-x64.zip` をダウンロードし、展開した `UmaKiremonoCounter.exe` を実行します（.NET のインストールは不要）。

### アンチウイルス・SmartScreen について

この exe にはコード署名がありません。また、起動時に中身を一時フォルダへ展開する形式のため、
アンチウイルスソフトに誤検知（ブロック・隔離・警告）される可能性が高いです。

- **SmartScreen**（「Windows によって PC が保護されました」）が表示された場合は、「詳細情報」→「実行」で起動できます。
- アンチウイルスの反応が邪魔な場合は、`UmaKiremonoCounter.exe`（または展開したフォルダ）を**除外リストに追加**してください。
  - Windows セキュリティ（Microsoft Defender）の場合:
    「ウイルスと脅威の防止」→「ウイルスと脅威の防止の設定」の「設定の管理」→「除外」の「除外の追加または削除」→「除外の追加」→「ファイル」または「フォルダー」
  - その他のアンチウイルスソフトは、それぞれの除外（例外）設定から追加してください。

除外に追加するのは、このリポジトリの [Releases](https://github.com/sojuenke-cmyk/umamusume-kiremono-counter/releases) から入手したファイルだけにしてください。

## リリース手順

`v` で始まるタグを push すると、GitHub Actions がリリースビルドを作成して Release に添付します。

```powershell
git tag v0.1.0
git push origin v0.1.0
```

ローカルで同じ配布用ビルドを作る場合:

```powershell
dotnet publish src/UmaKiremonoCounter -p:PublishProfile=win-x64
```
