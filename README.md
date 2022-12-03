# goblin.ui.fontcreator
游戏开发中，总有一些**默认字体无法满足**需求的地方。需要美术输出图片来**制作艺术字**来满足需求。
如果想要实现艺术字的制作，一般会采用 **BMFont 导出至 Unity**，或者使用 **TextMesh Pro 配合 TexturePacker** 来处理。
甚至，使用 TextMesh Pro 还需要导入。
所以，以上两种方案都比较麻烦，无法做到一站式，傻瓜式输出表达内容。

**[goblin.ui.fontcreator](https://github.com/wantdabo/goblin.ui.fontcreator) 可以傻瓜式的处理这个问题，一站式。**

### Chinese 
- 1.右键，创建 GFont（Create/Create New GFont）
- 2.选中创建出来的 GFont
  - a.设置 FontName
  - b.设置 Padding
  - c.拖拽映射好 sprite 与 char 的关系。char 使用（ASCII）
- 3.点击生成按钮（generate font）

*注意 texture 需要开启 read/write

好了，开始你的愉快开发咯！

### English
- 1.create/create new gfont
- 2.select gfont
  - a.set font name
  - b.settings your padding.
  - c.mapping sprite with char(ascii)
- 3.click generate font

*tips texture need support read/write

now, enjoy your develop.

![step_7.png](/images/step_7.png)
# 效果图

<details>

## *step 1
![step_1.png](/images/step_1.png)
## *step 2
![step_2.png](/images/step_2.png)
## *step 3
![step_3.png](/images/step_3.png)
## *step 4
![step_4.png](/images/step_4.png)
## *step 5
![step_5.png](/images/step_5.png)
## *step 
![step_6.png](/images/step_6.png)

</details>

