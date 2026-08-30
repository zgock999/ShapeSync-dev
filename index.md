# ShapeSync チュートリアル & ドキュメント

ようこそ！本ドキュメントは、Unity 向けキャラクター衣装追従ツールセット **ShapeSync（シェイプシンク）** の公式チュートリアルです。

ShapeSync を使うことで、ベースとなるキャラクター（Figure）の体型変化に合わせて、衣装（Outfit）を登録・追従させることができます。

---

## 📦 チュートリアル補助素材

第3章の初期動作確認に使用するアニメーション素材です。

- [CC0Animation.unitypackage](./CC0Animation.unitypackage)（CC0 1.0）

---

## 📖 チュートリアル章構成（目次）

本チュートリアルは以下の全13章で構成されています。初めて導入される方は、第1章から順にお進みください。

| # | 章 | 主な作業 | 状態 |
| :--- | :--- | :--- | :--- |
| 1 | **[第1章: インストール](./installation.html)** | package 導入。前提（Unity version、Graphics API、URP、Asset Serialization Mode）をここでまとめて提示する | 公開中 |
| 2 | **[第2章: 初期 VRM データ生成](./initialvrm.html)** | Base Figure と FBM の VRM を作る | 公開中 |
| 3 | **[第3章: Figure 登録](./figureregistration.html)** | Database 上で Figure を登録し、生成、Scene 配置、初期動作確認を行う | 公開中 |
| 4 | **[第4章: Outfit 登録](./outfitregistration.html)** | 着衣状態の VRM を作成し、Outfit を登録して動作確認を行う | 公開中 |
| 5 | **[第5章: Shape 登録](./shaperegistration.html)** | Morph / Skin / Hair / Outfit Shape を登録して動作確認を行う | 公開中 |
| 6 | **[第6章: 高度な Figure 登録 / PBM](./pbmregistration.html)** | PBM 用 VRM 作成と Database 登録、Morph Shape の再定義と動作確認 | 公開中 |
| 7 | **第7章: 高度な Outfit 登録 / Collection** | 靴着用 VRM の作成と Database 登録、Projection と Collection 設定、動作確認 | 準備中 |
| 8 | **第8章: 高度な Outfit 登録 / Mask** | VRoid Studio 内での靴補正用 Mask 作成、Database への Mask 登録、動作確認 | 準備中 |
| 9 | **第9章: VRM 連携** | 表情と Physics 用 VRM を登録し、生成と動作確認を行う | 準備中 |
| 10 | **第10章: Document 保存** | ここまでの Shape をまとめ、Document を保存する | 準備中 |
| 11 | **第11章: Humanoid Compiler** | 作成した Document から Humanoid を生成する | 準備中 |
| 12 | **第12章: Atlas** | Atlas Editor による設定と Humanoid 再生成 | 準備中 |
| 13 | **第13章: Hot Bake** | Figure / Document / Atlas による Runtime Humanoid 再生 | 準備中 |

---

## 🔗 各章の詳細とリンク

### [第1章: インストール](./installation.html)（公開中）
* ShapeSync の導入に必要な動作環境（Unity 6 / URP / DX12 等）
* OpenUPM レジストリおよび NuGetForUnity の設定
* 必須パッケージ（R3、ShapeSync Core）のインストール手順
* プロジェクトの初期設定（Color Space: Linear、Asset Serialization Mode: Mixed）
* オプション: VRM 連携パッケージ（UniVRM / Companion）の導入
* **[よくあるトラブルと解決策（トラブルシューティング）](./installation.html#6-よくあるトラブルと解決策トラブルシューティング)**

### [第2章: 初期 VRM データ生成](./initialvrm.html)（公開中）
* VRoid Studio を使用した Base VRM（`BasicFemale`）および FBM VRM（`SampleI`）の準備
* 同一トポロジ維持のための髪型・衣装・アクセサリーの全除去
* VRM 1.0 エクスポートとポリゴン・マテリアル・ボーン数の一致確認
* VRoid Studio サンプルモデルの利用規約について
* **[よくあるトラブルと解決策（トラブルシューティング）](./initialvrm.html#6-よくあるトラブルと解決策トラブルシューティング)**

### [第3章: Figure 登録](./figureregistration.html)（公開中）
* ShapeSync Editor の起動と Database（`.prefab`）の作成
* Figure セクションでの Base VRM（`BasicFemale.vrm`）登録
* Materials セクションでの 9 件の Material Entry 命名
* FBMs セクションでの FBM 軸（`SampleI` / `SampleI.vrm`）登録（`Import All Materials and Textures` 有効化）
* Generation セクションでの Figure 生成（Generate）
* Scene 配置と CC0 Animation（`Walking.controller`）再生下での FBM weight 動作確認
* **[よくあるトラブルと解決策（トラブルシューティング）](./figureregistration.html#8-よくあるトラブルと解決策トラブルシューティング)**

### [第4章: Outfit 登録](./outfitregistration.html)（公開中）
* VRoid Studio を使用したカスタムヘア（`Hair1`）およびプリセットドレス（`Dress1`）の Base / FBM 用 VRM 準備
* ShapeSync Editor の Outfits セクション（Mesh Outfits）での `Hair1` / `Dress1` 登録
* Mesh Outfit Materials でのマテリアル分類（`Include` / `Exclude`）
* FBMs セクションでの `SampleI` 軸用 VRM 割り当て
* Generation セクションでの Outfit Prefab 出力（Generate）
* Scene 配置した Figure への `OutfitAttacher` 設定と Play Mode での着脱・体型変形追従確認
* **[よくあるトラブルと解決策（トラブルシューティング）](./outfitregistration.html#7-よくあるトラブルと解決策トラブルシューティング)**

### [第5章: Shape 登録](./shaperegistration.html)（公開中）
* VRoid Studio からのストッキングテクスチャ書き出し（`Stocking.png`）と Unity での `Alpha is Transparency` 設定
* Outfits セクションでの Material Outfit（`Stocking`）登録（テクスチャ名 `Body` 割り当て）
* Shapes セクションでの各種 Shape 登録（Morph: `morphSampleI`、Hair: `hairSampleI`、Skin: `skinSampleI`、Outfit: `outfitSampleI`）
* Generation セクションでの Shape Template アセット群およびカタログの出力（Generate）
* Figure の `ShapeDirector` への Template 登録と Play Mode（`Walking.controller`）での連動制御・動作確認
* **[よくあるトラブルと解決策（トラブルシューティング）](./shaperegistration.html#10-よくあるトラブルと解決策トラブルシューティング)**

### [第6章: 高度な Figure 登録 / PBM](./pbmregistration.html)（公開中）
* VRoid Studio を使用した局所体型変形（`BreastSize`）の Base / FBM 用 `.vroid` データ作成
* 素体用（`BreastSizeBasicFemale.vrm`, `BreastSizeSampleI.vrm`）および衣装用（`Dress1BreastSizeBasicFemale.vrm`, `Dress1BreastSizeSampleI.vrm`）の PBM VRM エクスポート
* Figure セクションでの PBM（`BreastSize`）登録と Base / FBM VRM 割り当て
* Outfits セクションでの Dress への PBM 追従設定（`Follow BreastSize`）
* Shapes セクションでの Morph Shape（`morphSampleI`）上書き再定義（`SampleI = 1`, `BreastSize = 0.8`）
* Generation セクションでの再 Generate 実行
* Figure の `ShapeDirector` による Play Mode での PBM 胸部連動変形の動作確認
* **[よくあるトラブルと解決策（トラブルシューティング）](./pbmregistration.html#10-よくあるトラブルと解決策トラブルシューティング)**

### 第7章 〜 第13章（準備中）
各章のチュートリアル本文は順次公開予定です。

---

## 💡 はじめに知っておきたい基本用語

* **Figure**: 素体となる3Dキャラクターモデルです。
* **Outfit**: キャラクターが着用する衣装や装飾品です。
* **ShapeSync Core**: 衣装追従の基本機能を提供するコアパッケージです。
* **VRM Integration Companion**: VRM 1.0 形式のアバターモデルで ShapeSync を利用するための拡張パッケージです（オプション）。

---

*© 2026 zgock999. Released under the MIT License.*
