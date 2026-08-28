# ShapeSync チュートリアル & ドキュメント

ようこそ！本ドキュメントは、Unity 向けキャラクター衣装追従ツールセット **ShapeSync（シェイプシンク）** の公式チュートリアルです。

ShapeSync を使うことで、ベースとなるキャラクター（Figure）の体型変化に合わせて、衣装（Outfit）を登録・追従させることができます。

---

## 📖 チュートリアル章構成（目次）

本チュートリアルは以下の全13章で構成されています。初めて導入される方は、第1章から順にお進みください。

| # | 章 | 主な作業 | 状態 |
| :--- | :--- | :--- | :--- |
| 1 | **[第1章: インストール](./installation.html)** | package 導入。前提（Unity version、Graphics API、URP、Asset Serialization Mode）をここでまとめて提示する | 公開中 |
| 2 | **第2章: 初期 VRM データ生成** | Base Figure と FBM の VRM を作る | 準備中 |
| 3 | **第3章: Figure 登録** | Database 上で Figure を登録し、生成、Scene 配置、初期動作確認を行う | 準備中 |
| 4 | **第4章: Outfit 登録** | 着衣状態の VRM を作成し、Outfit を登録して動作確認を行う | 準備中 |
| 5 | **第5章: Shape 層** | Morph / Skin / Hair / Outfit Shape を登録して動作確認を行う | 準備中 |
| 6 | **第6章: 高度な Figure 登録 / PBM** | PBM 用 VRM 作成と Database 登録、Morph Shape の再定義と動作確認 | 準備中 |
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

### 第2章 〜 第13章（準備中）
各章のチュートリアル本文は順次公開予定です。

---

## 💡 はじめに知っておきたい基本用語

* **Figure**: 素体となる3Dキャラクターモデルです。
* **Outfit**: キャラクターが着用する衣装や装飾品です。
* **ShapeSync Core**: 衣装追従の基本機能を提供するコアパッケージです。
* **VRM Integration Companion**: VRM 1.0 形式のアバターモデルで ShapeSync を利用するための拡張パッケージです（オプション）。

---

*© 2026 zgock999. Released under the MIT License.*
