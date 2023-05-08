# ArcDock
## 单据打印工具
---
![](https://img.shields.io/badge/version-1.0-green)
![](https://img.shields.io/badge/.NET%20Framework-4.7.2-blue)
### 概述
欢迎使用单据生成工具，本程序可以根据模板生成样式统一的单页图文媒体，您可以将其保存为图片文件、网页或者直接将其打印。
### 典型使用场景
1. 单页填写打印

   数据可由手动、文本解析、CLI获取。

   此种方式适合打印定制化的**票据、凭条、优惠券、入场券、页签、瓶签、腕带、名片、徽章等**。

2. 循环打印

   打印多张相同内容的媒体，数据可由手动、文本解析、CLI获取。

   当`单页填写打印`需要**多张副本**时，可以使用该方式打印。

3. 文件数据导入为对象（JSON）

   **数据存储在Excel中（仅支持.xlsx文件）**，将文件全部数据导入模板，得到**单张**包含多个数据的图文媒体。

   此种填写方式适合打印定制化的**展示板、名单、宣传栏等**。

4. 文件数据导入为文本（批量打印）

   **数据存储在Excel中（仅支持.xlsx文件）**，每次将文件中的一行数据导入模板，得到**多张**包含单条数据的图文媒体。

   此种填写方式适合批量打印给定内容，通常为**团体**的凭条、入场券等。

5. 混合

   您可以将JSON保存在Excel中，通过批量打印该Excel得到上述第三条与第四条的叠加效果。

   此种填写方式适合打印定制化的**数据图表、幻灯片、简单手册等**。

**详细内容参见[使用文档](https://github.com/yimig/ArcDock/blob/master/ArcDock/help/index.md)**

---

![主页](https://raw.githubusercontent.com/yimig/ArcDock/master/ArcDock/help/assets/image-20230428144611943.png)