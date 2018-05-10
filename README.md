BridgeUI-综合性UGUI框架
=====================

----基于Unity3d及UGUI,结合编辑器扩展之节点图制作而成的利于编辑,接口简洁的人性化界面框架.

- **节点编辑器取自AssetBundleGraph**
- **分离界面自身属性及界面关联属性**
- **分离属性的设置及预制体的制作**
- **支持双定义*层级* 即基本层级类型和int型层级**
- **支持打开动画及关闭动画的编辑器状态指定**
- **将游戏自身逻辑完全独立于界面创建和关联的逻辑**
- **支持同父级面板互斥不同显功能**
- **支持编辑器模式快速打开面板及批量保存功能**
- **支持xLua文本各种加载方式**
- **支持mvvm模式，可动态绑定viewModel**
- **支持代码生成与更新，解析与重写**
- **支持unity5.3.4及以上版本

-------------------
## 接口

### 1.任意脚本中打开一个面板
```
var handle = UIFacade.Instence.Open(PanelName:string);
```
### 2.在面板中打开面板为子面板
```
var handle = selfFacade.Open(PanelName:string, data:object);
```
### 3.强行关闭对应名称的面板
```
UIFacade.Instence.Close(PanelName:string);
```
### 4.强行隐藏对应的面板
```
UIFacade.Instence.Hide(PanelName:string);
```
### 5.注册界面打开事件
```
IUIHandle RegistCreate(UnityAction<IPanelBase> onCreate);
```
### 6.注册界面关闭事件
```
IUIHandle RegistClose(UnityAction<IPanelBase> onClose);
```
### 7.注册界面信息回调
```
IUIHandle RegistCallBack(UnityAction<IPanelBase, object> onCallBack);
```
### 8.定向发送信息
```
IUIHandle Send(object data);
```
----------
## 图形化
### 1.利用线来表示界面与界面之间的关系
![null](Pics/6.png)
### 2.将节点信息记录独立于Prefab
![null](Pics/5.png)
### 3.快速展开编辑和快速保存
![null](Pics/1.png)
### 4.自定义加载菜单
![null](Pics/2.png)
### 5.快速编辑预制体代码
![null](Pics/3.png)
### 6.定义界面关联与打开时的状态
![null](Pics/4.png)

## 项目引用
*** AssetBundleTools (可选)
[AssetBundleTools](https://github.com/zouhunter/unity-assetbundle-tools) 
（导入后添加"AssetBundleTools宏定义"）
*** ICSharpCode.NRefactory (代码生成选)
[ICSharpCode.NRefactory.CSharp](https://github.com/zouhunter/cs-codetree-.net3.5)
*** NodeGraph （必选）
[NodeGraph](https://github.com/zouhunter/unity-nodegraph-frame)
*** XLua (可选)
[XLua](https://github.com/Tencent/xLua)
（导入后添加"XLua宏定义"）