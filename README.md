# ConfigLoader

一个 **Unity 导表工具**，支持将 Excel 数据导出为 **Json** 或 **Protobuf(Binary)** 格式，并在 Unity 中加载使用。

---

## ✨ 功能特性

- 支持 **Json / Protobuf(Binary)** 两种数据格式  
- Json：
  - 支持 **数据集合嵌套**
  - 便于调试与查看
- Protobuf(Binary)：
  - **不支持数据集合嵌套**
  - 文件体积更小，适合正式环境

---

## 📦 安装方式

1. 前往 **Releases**
2. 下载 `configLoader.unitypackage`
3. 导入到 Unity 项目中

---

## 🚀 工具入口

Unity 顶部菜单：Tools -> Excel


---

## ⚙️ 初次使用配置

第一次使用需要配置 **Excel 文件路径**。

<img width="600" height="181" alt="Config Path" src="https://github.com/user-attachments/assets/bb8c055c-c981-4d05-9c98-fd79486d0cee" />

---

## 🧭 使用流程说明

配置完成后，根据按钮文字提示即可完成导表操作：

- **Json 格式**
  - 可直接点击 `Refresh All`
- **Binary(Protobuf) 格式**
  - 需要 **先生成 C# 数据结构文件**
  - 再进行导表

---

## 📊 Excel 表格格式规范（重要）

Excel 表需严格按照以下格式编写：

| 行号 | 内容说明 |
| ---- | -------- |
| 第 1 行 | 变量名 |
| 第 2 行 | 变量类型 |
| 第 3 行 | 字段注释（会自动生成到 C# 代码中） |
| 第 4 行及以后 | 具体数据内容 |

### ⚠️ 注意事项

- **强烈建议第一列固定为 `ID`**
- 如果不设置 ID 列：
  - 生成的数据表将以 **数组下标（从 0 开始）** 作为索引
  - 不利于配置的维护与引用

---

## 💻 使用示例

加载配置表参考如下代码：

<img width="650" height="55" alt="Code Example" src="https://github.com/user-attachments/assets/fe77a343-4d2e-4c39-b907-0753d3780441" />

---

## 📌 说明

- 本工具适用于 Unity 项目中的配置表管理
- 可根据项目需要选择 Json 或 Protobuf 格式
- 推荐：
  - **开发阶段使用 Json**
  - **发布阶段使用 Binary**



