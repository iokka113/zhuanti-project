# 破碎時空

[ENGLISH VERSION](/README_ENG.md)

## 概要

![CoverArt.png](/Docs/CoverArt.png "CoverArt.png")

> **本專案已停止開發**

《破碎時空》是一款像素風的 2D Rougelike 角色扮演遊戲。  
玩家需要探索這個地城來尋找出口，  
棲息於地城中的怪物和陷阱將會阻礙你的行動，  
保持小心謹慎才能逃離地城。

## 文檔

* [遊戲企劃書](/Docs/Proposal.pdf)
* [專題成果報告書](/Docs/ProjectReport.pdf)

## 獎項和提名

* 2022年放視大賞 PC遊戲組 入圍

## 下載

點擊 [此處](https://github.com/iokka113/zhuanti-project/releases/latest) 下載適用於 Windows x86-64 的最新遊戲版本

## 開發心得

本作品為 2D 單機 RPG 遊戲。使用 C# .NET 及 Unity 2D 引擎開發。

整體遊戲的流程使用事件觸發（函數指標）來驅動。

最大的設計重點在於怪物的人工智慧（有限狀態機）：  
把每個怪物的狀態設計成不同的物件，使用工廠方法生成。藉由狀態機內的狀態改變，模擬自動化怪物智慧。

且為了有效地運用資源，實作了物件池模式：  
每個可被物件池回收的物件，都可使用共同的介面來抽象，並且被物件池靜態方法統一管理。

而遊戲中的每個管理器（角色數值，關卡管理，UI 管理等等），則使用單例基底來實現：  
藉由繼承相同的泛型基底類別，確保每個管理器在進程中只會有單一實例，避免程序衝突。

充分地利用物件導向的特性（封裝、繼承、多型），完成有系統地遊戲開發。

## 授權和鳴謝

所有公開源代碼（即 C#）均受作者 [Iokka Lim](https://github.com/iokka113) 版權保護，並根據 [MIT 授權條款](/LICENSE)發布許可。

所有非代碼資產（例如美術圖像、音效、字型、動態鏈結庫、標記式語言檔案），無論是否被構建於二進位文件中，均受其原作者的版權保護。未經授權，不可進行逆向工程、反編譯或反組譯，也不可複製、修改、發布、再授權或以其他方式侵害原作者的版權。

### 使用的第三方素材

* 角色圖片
  * 名稱 2D Retro Heroes - SPUM Premium Addon Pack
  * 作者 [soonsoon](https://assetstore.unity.com/publishers/4419)
* 怪物圖片
  * 名稱 Pixel Mobs
  * 作者 [Henry Software](https://assetstore.unity.com/publishers/9216)
* 道具圖片
  * 名稱 2D Pixel Item Asset Pack
  * 作者 [Startled Pixels](https://assetstore.unity.com/publishers/31653)
* 場景圖片
  * 名稱 2D Pixel Top-Down Dungeon Tileset
  * 作者 [Matthias Stuetzer](https://assetstore.unity.com/publishers/23590)
* 特效素材
  * 作者 [ぴぽや](http://blog.pipoya.net/)
* 音效素材
  * 作者 [小森　平](https://taira-komori.jpn.org/freesoundtw.html)
* 使用字型
  * 名稱 源界明朝 & 装甲明朝
  * 作者 [FLOP DESIGN](https://flopdesign.booth.pm/)

感謝上述所有繪師及製作者
