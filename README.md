# api.lcwebsite.cn

LC 的网站的全新的 API！

## 搭建一个同样的 API
请保证在部署设备上已安装 `ASP.NET Core 运行时 8.0` 或 `.NET SDK 8.0`（若要使用 IIS 部署请安装**托管捆绑包**，其中包含 IIS 支持）
- Windows
	1. 在[最新的发行版](https://github.com/lc6464/api.lcwebsite.cn/releases/latest)中下载 Windows x64 版本
	2. 解压文件
	3. 运行 `API.exe` 或用 `dotnet cli` 运行 `API.dll`，也可以部署到 IIS（建议）
	4. 在浏览器中访问 [localhost:5000](http://localhost:5000/Hello "localhost:5000")（若要更改端口号通过命令行参数、环境变量等方式更改，IIS 部署则在`绑定`中设定）
- Linux
	1. 在[最新的发行版](https://github.com/lc6464/api.lcwebsite.cn/releases/latest)中下载 Linux x64 版本
	2. 解压文件
	3. 运行 `API` 或用 `dotnet cli` 运行 `API.dll`
	4. 在浏览器中访问 [localhost:5000](http://localhost:5000/Hello "localhost:5000")（若要更改端口号通过命令行参数、环境变量等方式更改）
- macOS
	1. 参考下方教程自行编译

### 自行编译
1. 克隆项目到本地
2. 如有需要，可修改 `Program.cs` 中的 Cors 设置（本项目采用 [Apache License 2.0](https://github.com/lc6464/api.lcwebsite.cn/blob/main/LICENSE.txt)）
3. 使用 `Visual Studio 2022` 或 `dotnet cli` 发布项目
4. 参考上方教程部署