# ToTour：基于RESTful风格的电商后端API

## Overview | 项目简述
基于.Net 6.0开发的企业级电商后端API，打造符合RESTful L3成熟度的API，并通过JWT实现身份的认证与授权。并使用Pstman生成高标准高质量的API文档。

## Requirements | 开发环境
- Visual Studio 2022
- .Net 6.0
- EF Core 6.0
- MySQL-Alicloud / SQL Server

## Database configuration | 数据库路径配置
在```./appsettings.json```配置文件中的```appSettings```，设置已提供的数据库```totour```的连接路径。
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",

  "DbContext": {
    "MySQLConnectionString": "Server=yourAdress ; Database=totour; Uid=*; Pwd=*"
  },
  "Authentication": {
    "SecretKey": "privatekeyoftheserver",
    "Issuer": "totour.com",
    "Audience": "totour.com"
  }
}
```

## Core technology | 核心技术
1. 项目采用RESTFul的架构风格设计项目逻辑；

2. 使用AutoMapper进行不同对象之间的自动映射；

3. 使用JWT实现无状态单点登录，并通过设计加密、动态过期时间等手段保证安全性质；

4. 基于RBAC模型管理用户认证与授权；

5. 设计模块化实现分页数据处理、数据排序、数据塑性；

6. 依照HATEOA理念增强 RESTful API的互动性和可发现性；

7. 数据库设计上采用读写分离，引入Redis进行数据缓存，在业务层面使用异步处理各种Http请求，保证API的高并发性；

8. 项目部署到Aurze云，并发布高标准的RESTful API文档。
