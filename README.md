# ConfigLoader
 unity导表工具，支持Json或者Protobuf

 json支持数据集的的嵌套
 Protobuf不支持数据集合的嵌套，但是压缩文件更小

 从Releases下载configLoader.unitypackage 导入unity项目
 https://github.com/duyutong/ConfigLoader/releases/download/v1.0.0/configLoader.unitypackage

 工具入口：unity顶部编辑窗口Tools->Excel
 
 第一次使用需要配置文件路径
 <img width="600" height="181" alt="QQ_1767679887374" src="https://github.com/user-attachments/assets/bb8c055c-c981-4d05-9c98-fd79486d0cee" />

 之后根据按钮文字提示可进行操作，其中josn格式可以直接refreshAll;binary需要先生成Csharp文件
 <img width="221" height="15" alt="QQ_1767680117864" src="https://github.com/user-attachments/assets/d70920e0-a02c-41ca-bba5-aa902de60313" />

使用时参考如下代码：
<img width="848" height="58" alt="QQ_1767680245428" src="https://github.com/user-attachments/assets/0f179918-bedd-437f-a51d-a691c084c82d" />

