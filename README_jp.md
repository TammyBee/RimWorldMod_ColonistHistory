# Colonist History
一定間隔で入植者の情報を記録するMODです。  
記録した情報はゲーム内にて表やグラフで確認することが出来ます。

## 機能
- 一定間隔で入植者の各種情報を保存します。
- 「入植者の歴史」タブを追加します。
    - ホーム：手動で入植者の各種情報を保存したり、XMLファイルで出力したりできます。
    - 表：任意の時間の入植者の各種情報を確認できます。
    - グラフ：数値で保存された各種情報を時系列のグラフで確認できます。

### 保存する情報
- スキルレベル(整数)
- スキルレベル(実数)
- 健康状態
- 装備中の武器
- 年齢
- 記録(その他)  *1  
 例: 殺害数、与えたダメージ 等
- 記録(時間)  *1  
 例: 運搬(総時間) 等

*1 MODで追加されたRecordDefにも対応しています。

## 設定
### 軽量セーブモード(実験的)
- 変更があった場合の差分のみを保存することでセーブデータの容量を減らします。