# goToBuchNavi (ARNavigation)

このリポジトリは [岡山大学附属図書館](https://www.lib.okayama-u.ac.jp/) の中央図書館において，目的の本を容易に見つけられるように AR ナビゲーションを行うことを目的としたものである．` Assets/Scripts/Navigation.cs ` と `Assets/Scripts/Scraping.cs` を変更し，岡山大学附属図書館以外で利用することも可能である．

## 概要

屋内では GPS の精度が著しく低下する．また，屋内は屋外よりも細かな通路が多い．そのため，誤差の出る GPS 以外の方法で位置情報を取得する必要がある．その時，AR マーカやビーコン，Wi-Fi 等の機器を利用する方法が考えられる．AR マーカの利用は，高精度ではあるが，AR マーカを大量に配置する必要があり，外観を損なう恐れがある．また，ビーコン，Wi-Fi 等の機器を利用する方法では，機器が存在しなければ利用できない問題点がある．

そこで，特別な機器を必要とせず，画像処理技術を用いた Visual - SLAM の利用を考えた．SLAM には Visual - SLAM よりも高精度な LiDAR - SLAM ，Depth - SLAM が存在するが，いずれも特殊なセンサ等が必要なため，Visual - SLAM（以下 SLAM ）を利用する．

SLAM は環境マップの作成と自己位置推定の2つの機能が備わっている。環境マップの作成では検出した特徴点の座標を保持し、自己位置推定では検出している特徴点の座標と環境マップに含まれる座標が類似するかを判定する。自己位置推定の際，類似するならば同一の場所であると判定する。

## 実行環境

| ツール・パッケージ名 | バージョン |
|----|----|
| Unity | 2020.3.34f1|
| Xcode |  |
| AR Foundation |  |
| ARKit XR Plugin |  |
| NavMesh Extension |  |
| macOS |  |
| iPadOS | 13.1.1 |


## その他
* Xcode で build 出来ないとき（原因は端末のOSのバージョンが対応外だった）
    1. まず，Xcode の Window タブから Device and Simulators を開き，エラーが出ているか確認する．
    2. If you are certain that Xcode supports development on this device, try disconnecing and reconnecting the deviceが出ている場合，選択し，Detailを開くとより詳細なメッセージを確認することが出来る．そこに英語でバージョン対応外の様な文言があれば，以下の対処で解決する可能性あり．
    3. FinderでApplication内のXcodeを右クリックし，パッケージの内容を表示を選択．
    4. Contents/Developer/Platforms/iPhoneOS.platform/DeviceSupport内を確認し，自身の端末のバージョンが存在するか確認する．なかった場合は下記を進める，ある場合は別の対処が必要．
    5. [ここ](https://github.com/iGhibli/iOS-DeviceSupport/tree/master/DeviceSupport)からデバイスのOSに対応するZipファイルをダウンロードする．
    6. ダウンロード後，解凍し，Contents/Developer/Platforms/iPhoneOS.platform/DeviceSupport内に移動させる．
    7. これで今回はビルド出来る様になった．
