# スレスパもどき

技術力向上の為に作成した、Slay the Spireを参考にしたデッキ構築型ローグライクゲームのプロジェクトです。

## 操作方法

基本的にマウスで操作します。

マップだけは左右キーで移動、Spaceで移動先決定です。

※実装時期のズレにより他の操作方法と統一できていない為、マウス操作の追加、またはキー操作を削除してマウス操作に置き換えることで全操作をマウスで行えるようにする予定です。

## 使用技術・ツール

- Unity 6.6 / C#
- Visual Studio
- Claude、Cursor（AIの利用については下記参照）
- Wwise ※
- VContainer ※

※公開中のビルド（Releases）には未反映

## AIの利用について

設計・実装力を身につけるため、ゲームの根本の仕組みに関わるコードは自分で設計・実装しています。

AIは、技術力の向上に直接結びつかない作業や、1人では実施が難しい作業の効率化に限って使っています。

| AIに任せない（自分で行う） | AIで効率化する |
|---|---|
| バトル進行処理 | テストコードの追加（序盤は自分で作成） |
| カードの効果処理システム | コーディングに関する調査・質問（公式ドキュメントや動作で確認） |
| デッキ管理・マップ管理 | 作業スケジュールの管理 |
| セーブデータ管理 | コードレビュー |
| AIのレビューで出た指摘の修正 | クラス図などの図の作成 |

### AIで作成したコードの見分け方
- AIで作成したファイルは、ファイル先頭に「このファイルはAIで作成」と記載しています。
- Claudeが関わったコミットには `Co-Authored-By: Claude` を付け、AIが書いた範囲をコミットメッセージに記載しています（2026年11月から）。
- 上記以外のコードは自分で作成したものです。

検索例：
```
git log --grep="Co-Authored-By: Claude"
git grep "AIで作成"
```

## 使用素材

### イラスト
- [かわいいフリー素材集 いらすとや](https://www.irasutoya.com/)
- [SILHOUETTE ILLUST](https://www.silhouette-illust.com/)

### 背景
- [空想曲線](https://kopacurve.blog.fc2.com/)

### BGM
- [フリーBGM・音楽素材MusMus](https://musmus.main.jp)
- [魔王魂®](https://maou.audio/)

### 効果音
- [効果音ラボ](https://soundeffect-lab.info)
- [OtoLogic](https://otologic.jp/)
- [効果音工房](https://umipla.com/)

### フォント
- [DotGothic16](https://fonts.google.com/specimen/DotGothic16)
