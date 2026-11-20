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
```
