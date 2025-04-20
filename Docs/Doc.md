# 工具
1. 版本控制:
	- Git
2. 数据存储:
	- EF框架. (PS: 数据库选择Sqlite, 而不是MySql, MySql太麻烦了, 而且也没必要. Sqlite简单够用.)
3. GUI: 有待讨论
	- UI框架: Avalonia. (PS: 我觉得WinForm可能太古老了, 如果要把界面做好看一点, 选一个现代化一点的UI框架可能好一点. avalonia我前面写winform的作业的时候,去试了下,并且跑了起来.)
	- 模式: MVVM. (PS: avalonia默认支持)
4. 测试:==有待讨论==
	- xUnit. (PS: MSTest或者NUnit也行, 都大差不差. 我觉得xUnit流行一点可能)
5. AI部分:==有待讨论==
	- 未知 (PS: 目前我的知识盲区.)
# 项目结构
==有待讨论==
```
DesktopAssistant (解决方案)
├── Core(项目)
│   ├── AI
│   ├── Table (课表)
│   │   └── TableService.cs
│   └── TODO
│       └── TodoService.cs
├── Data(项目)
│   ├── Models
│   │   ├── Table
│   │   │   ├── Lesson.cs
│   │   │   ├── Table.cs
│   │   │   └── ...
│   │   └── TODO
│   │       ├── TodoItem.cs
│   │       └── ...
│   ├── Migrations (EF框架的东西)
│   ├── DbContext.cs
│   └── DA.db (Sqlite数据库文件)
├── GUI(项目)
│   ├── View
│   │   ├── TableView.axaml
│   │   ├── TodoView.axaml
│   │   └── ...
│   ├── ViewModel
│   │   ├── TableViewModel.cs
│   │   ├── TodoViewModel.cs
│   │   └── ...
│   └── ...
├── Test(项目)
│   ├── TableTest.cs
│   ├── TodoTest.cs
│   └── ...
├── Docs(文件夹)
│   └── ...
├── README.md
└── ...
```
PS:
1. AI部分未知, 但大概率不可能是跑本地, 应该是调API, 不太会需要本地存储,所以Data那块没有写AI方面的文件的例子
2. 以课表功能为例, 一般是先在Data的models里把几个基本的类,比如课程类,课表类啥的写出来, 然后在DbContext里添上数据库要用到的表, 然后在Core的service里面, 用DbContext去访问数据库, 增删改查数据. Test和GUI去用models和service.
# 分工
==有待讨论==
四个人, 可能的分工如下:
1. 组长, 创建项目仓库和main分支, 把项目的基本结构, 哪几个项目, 哪几个文件夹给创建出来, 然后上传.
2. 第一周可能的分工:
	- 一个人去研究怎么把AI整进来. 最好是自己新建一个小demo项目, 自己试一试, 看能不能实现发一个string内容,然后ai返回一个string内容. 如果有空余时间, 去试试能不能让ai返回的内容不是纯文本, 而是markdown之类的富文本. 然后如果成功的话, 可以在群里发一下成果, 或者下次课见面的时候展示一下demo. 如果有困难的话,也可以及时说.
	  然后把AI弄进项目里的时候, 如果工作量太大, 还可以再进一步去分工
	- 一个人写课表这块, 最好是新建一个table分支, 这样会方便组长后面来整合. 然后在里面写. 只需要写models, Dbcontext, service. 测试的话如果有空就写一下, 没空就算了. 测试这个东西, ai生成比较好, 自己写太麻烦了. 
	- 一个人写TODO这块, 同上.
	- 一个人负责GUI, 数据部分因为别人正在做, 所以写不了. 先把静态的界面, 控件啥之类的先弄一下. 然后就是要去尝试怎么美化. 可以在自己前面课上做过的作业里去试.
3. 后续分工...