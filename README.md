# ConfigLoader
 unity导表工具，支持Json或者Protobuf

 json支持数据集的的嵌套
 Protobuf不支持数据集合的嵌套，但是压缩文件更小

 从Releases下载configLoader.unitypackage 导入unity项目


 工具入口：unity顶部编辑窗口Tools->Excel


 
 第一次使用需要配置文件路径
 
 <img width="600" height="181" alt="QQ_1767679887374" src="https://github.com/user-attachments/assets/bb8c055c-c981-4d05-9c98-fd79486d0cee" />



 之后根据按钮文字提示可进行操作，其中
 
 josn格式可以直接refreshAll;
 
 binary需要先生成Csharp文件


使用时参考如下代码：

<img width="650" height="55" alt="QQ_1767698576700" src="https://github.com/user-attachments/assets/fe77a343-4d2e-4c39-b907-0753d3780441" />



