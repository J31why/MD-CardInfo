# MD-CardInf

net6.0 WPF开发  游戏王大师决斗（Yu-Gi-Oh! Master Duel）卡查器

非独立版本需要.net6.0环境

读取内存获取卡片ID, 通过查询专门的数据库显示卡片效果。感谢ygopro的数据库，前人栽树，后人乘凉。

只是读内存应该不会被封号。

已知且暂时无法解决的问题: 

卡包界面显示的是内存中最近载入的卡片（也就是当前卡片的前一张或后一张）。

[作者主页](https://www.acfun.cn/u/353448)   [下载地址](https://github.com/J31why/MD-CardInfo/releases)

# 功能：
## 1.卡片信息

点击游戏中的卡片可显示本地数据库中的卡片信息，**如果点击卡片没有反应，请尝试管理员模式运行软件**。

双击卡名可以复制双击的卡片名称。

## 2.卡组编辑

载入卡组文件：可以加载读取ygopro卡组文件（.ydk）到软件中，MD中未出现的卡片将不会读取。

读取游戏卡组：需要游戏进入卡组编辑模式中才可以读取。

保存当前卡组：保存当前软件中的卡组为ygopro卡组文件（.ydk）。

写入卡组到游戏（**此功能会修改游戏内存**）：请先把当前编辑的卡组用任意卡片填充到与软件加载的卡组数量一致（错误的话软件会提示），然后点击写入卡组到游戏，写入成功后回到游戏保存卡组即可，再次点击编辑卡组就可以看到写入的卡组了。

# 设置：
窗口置顶/显示日文卡名/显示英文卡名/字体大小/退出时自动保存窗口宽度和高度


# 图片
![image](https://github.com/J31why/MD-CardInfo/blob/master/MD-CardInfo/image3.png?raw=true)

![image](https://github.com/J31why/MD-CardInfo/blob/master/MD-CardInfo/image4.png?raw=true)

![image](https://github.com/J31why/MD-CardInfo/blob/master/MD-CardInfo/image2.png?raw=true)

![image](https://github.com/J31why/MD-CardInfo/blob/master/MD-CardInfo/image1.png?raw=true)
