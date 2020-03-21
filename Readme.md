# 框架介绍

该框架主要是可以快速封装第三方的库到本地项目中，而且不会对代码有过多倾入性，通过Module来各自封装第三方库，实现高类聚，低耦合。支持模块话的开发，目前框架使用的是 .net core 3.1来设计的，并提供了一些简单封装的module并运用到demo中，可以很好的运行。

## 框架设计思路

---

### 由来

首先源于wpf的插件式开发，使用MEF来加载程序集，定义一套公用接口，然后插件去实现公用接口，可以在加载程序集的时候把每个实现都能读取出来，来达到继承到应用程序中，后来简略看了下abp的设计，也根据其思路设计了一个相对简单许多的框架。

### 设计
定义一个IModule的抽象层，他依赖微软自带的DI系统，然后每个模块需要继承这个接口，就能注册对应的服务到Ioc中，来供引用方调用。通过DepandsOnAttribute来实现模块之间的依赖，先将依赖模块加载到Ioc中，然后才加载该模块，所以使用的时候一定要分清依赖关系。

#### 使用
首先需要在启动项目定义一个IModule的实现类，然后内部程序会依次加载依赖的模块，然后可以通过依赖注入的方式使用依赖模块的抽象服务。

## 模块

---
- ConfigurationModule
- LoggingModule
- DapperModule
- CachingModlue
- EntityFrameworkModule
- MvcModule

该项目主要实现了几个常用的Module

### ConfigurationModule
可以自定义一些配置，通过读取配置文件映射的方式获取配置的实体

#### LoggingMoule
主要依赖SeriLog的事件日志系统，能更详细的记录一些信息，他依赖于ConfigurationModule,需要在配置文件中读取配置然后就能自动的对日志事件进行按配置的规则来输出对应的日志。还提供了一个中间件，主要打印系统的异常，耗时，请求参数（application/json）等。日志的打印场景也提供Console,File,Mongo的支持。

### DapperModule
可以注册多个数据库的连接，通过Name的不同，可以获取不同的数据库连接，方便多数据库操作

### EntityFrameworkModule
可以注册多个DbContext,依赖LoggingModule,可嵌合EFCore的日志输出中去，进行日志打印，动态获取DbContext也是在易用性和性能中做了相应的取舍，自测过，性能还行

### MvcModule
封装aspnetcore.mvc的简单操作，实现实体属性校验，json格式的基本封装，主要是让模块化的使用得到一种表达，而不是应用程序直接使用mvc框架，而是通过模块加载的方式来使用

## 结语
模块化的使用可以提高代码的易读性和维护成本，让代码更为简便。

作为一个 .net core初学者，也想通过不断的学习，来分享自己一些好的想法。


